using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fleck;
using Newtonsoft.Json.Linq;

namespace Server_HTML_Handler.Services
{
    public class ClientConnectedEventArgs : EventArgs
    {
        public string ClientId { get; set; }
        public string IpAddress { get; set; }
        public int Port { get; set; }
        public bool IsRegistered { get; set; }
    }

    public static class CallSignalingService
    {
        private static WebSocketServer _server;
        private static readonly ConcurrentDictionary<string, IWebSocketConnection> _activeUsers = new ConcurrentDictionary<string, IWebSocketConnection>();
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<IWebSocketConnection, byte>> _userSockets = new ConcurrentDictionary<string, ConcurrentDictionary<IWebSocketConnection, byte>>(StringComparer.OrdinalIgnoreCase);

        // ---- Weak-signal hardening -------------------------------------------------------------
        // Signals for a user who is offline for a moment (phone reconnecting) are kept for a short time
        // and delivered as soon as that user registers again, instead of failing the call immediately.
        private class PendingSignal
        {
            public string TargetId;
            public string Payload;
            public string Type;
            public DateTime At;
            public volatile bool Delivered;
        }

        private static readonly ConcurrentDictionary<string, ConcurrentQueue<PendingSignal>> _pending =
            new ConcurrentDictionary<string, ConcurrentQueue<PendingSignal>>(StringComparer.OrdinalIgnoreCase);
        private static readonly TimeSpan PendingMaxAge = TimeSpan.FromSeconds(10);
        private static readonly TimeSpan PeerOfflineGrace = TimeSpan.FromSeconds(3);

        // Sockets that stop sending anything (client pings every ~8 s) are closed so a dead phone is not
        // kept as "online" and calls to it are not routed into the void.
        private static readonly ConcurrentDictionary<IWebSocketConnection, DateTime> _lastSeen =
            new ConcurrentDictionary<IWebSocketConnection, DateTime>();
        private static readonly TimeSpan StaleAfter = TimeSpan.FromSeconds(40);
        private static Timer _sweepTimer;

        private static void Touch(IWebSocketConnection socket)
        {
            _lastSeen[socket] = DateTime.UtcNow;
        }

        private static void SweepStaleSockets(object state)
        {
            try
            {
                var now = DateTime.UtcNow;
                foreach (var kv in _lastSeen.ToArray())
                {
                    if (now - kv.Value > StaleAfter)
                    {
                        Log($"[STALE] No traffic from {kv.Key.ConnectionInfo?.ClientIpAddress} for {StaleAfter.TotalSeconds:0}s - closing socket.");
                        DateTime ignored;
                        _lastSeen.TryRemove(kv.Key, out ignored);
                        try { kv.Key.Close(); } catch { }
                    }
                }
                foreach (var kv in _pushQueue.ToArray())
                {
                    QueuedPush head;
                    while (kv.Value.TryPeek(out head) && head.ExpiresUtc < DateTime.UtcNow)
                    {
                        kv.Value.TryDequeue(out head);
                    }
                }
                foreach (var kv in _pending.ToArray())
                {
                    PendingSignal head;
                    while (kv.Value.TryPeek(out head) && now - head.At > PendingMaxAge)
                    {
                        kv.Value.TryDequeue(out head);
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"[ERROR] Sweep failed: {ex.Message}");
            }
        }

        private static void FlushPending(string userId, IWebSocketConnection socket)
        {
            if (string.IsNullOrEmpty(userId)) return;
            ConcurrentQueue<PendingSignal> queue;
            if (!_pending.TryGetValue(userId, out queue)) return;

            int delivered = 0;
            PendingSignal item;
            while (queue.TryDequeue(out item))
            {
                if (DateTime.UtcNow - item.At > PendingMaxAge) continue;
                try
                {
                    socket.Send(item.Payload);
                    item.Delivered = true;
                    delivered++;
                }
                catch { }
            }
            if (delivered > 0) Log($"[PENDING] Delivered {delivered} queued signal(s) to '{userId}' after reconnect.");
        }

        // ---- LAN push (notifications for phones that cannot receive Firebase pushes) ------------------
        // The PHP backend connects from this machine and sends {"type":"push","targetId":..,"ttl":..,"title":..,
        // "body":..,"data":{..}}. Online devices of the target get it at once; otherwise it is held for "ttl"
        // seconds (max 50 per user) and delivered when the user registers again.
        private class QueuedPush
        {
            public string Payload;
            public DateTime ExpiresUtc;
        }

        private static readonly ConcurrentDictionary<string, ConcurrentQueue<QueuedPush>> _pushQueue =
            new ConcurrentDictionary<string, ConcurrentQueue<QueuedPush>>(StringComparer.OrdinalIgnoreCase);
        private const int MaxQueuedPushPerUser = 50;
        private const int MaxPushTtlSeconds = 24 * 3600;
        private static readonly HashSet<string> TrustedPushSources =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "127.0.0.1", "::1", "192.168.32.43" };

        private static void HandlePush(IWebSocketConnection socket, JObject json)
        {
            string targetId = json["targetId"]?.ToString()?.Trim();
            int ttl = json["ttl"] != null ? (int)json["ttl"] : 600;
            if (ttl < 0) ttl = 0;
            if (ttl > MaxPushTtlSeconds) ttl = MaxPushTtlSeconds;

            var outgoing = new JObject
            {
                ["type"] = "push",
                ["id"] = Guid.NewGuid().ToString("N"),
                ["title"] = json["title"]?.ToString() ?? "",
                ["body"] = json["body"]?.ToString() ?? "",
                ["data"] = json["data"] ?? new JObject(),
                ["sentAt"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };
            string payload = outgoing.ToString(Newtonsoft.Json.Formatting.None);

            int delivered = 0;
            if (!string.IsNullOrEmpty(targetId))
            {
                var targets = new List<IWebSocketConnection>();
                ConcurrentDictionary<IWebSocketConnection, byte> sockDict;
                if (_userSockets.TryGetValue(targetId, out sockDict))
                {
                    targets.AddRange(sockDict.Keys.Where(s => s != null && s.IsAvailable));
                }
                foreach (var t in targets)
                {
                    try { t.Send(payload); delivered++; } catch { }
                }
            }

            bool queued = false;
            if (delivered == 0 && ttl > 0 && !string.IsNullOrEmpty(targetId))
            {
                var q = _pushQueue.GetOrAdd(targetId, _ => new ConcurrentQueue<QueuedPush>());
                q.Enqueue(new QueuedPush { Payload = payload, ExpiresUtc = DateTime.UtcNow.AddSeconds(ttl) });
                QueuedPush dropped;
                while (q.Count > MaxQueuedPushPerUser && q.TryDequeue(out dropped)) { }
                queued = true;
            }

            Log($"[PUSH] -> '{targetId}' delivered={delivered} queued={queued} ttl={ttl}s");
            try
            {
                socket.Send(new JObject
                {
                    ["type"] = "push_result",
                    ["delivered"] = delivered,
                    ["queued"] = queued
                }.ToString(Newtonsoft.Json.Formatting.None));
            }
            catch { }
        }

        private static void FlushPushQueue(string userId, IWebSocketConnection socket)
        {
            if (string.IsNullOrEmpty(userId)) return;
            ConcurrentQueue<QueuedPush> q;
            if (!_pushQueue.TryGetValue(userId, out q)) return;

            int delivered = 0;
            QueuedPush item;
            while (q.TryDequeue(out item))
            {
                if (item.ExpiresUtc < DateTime.UtcNow) continue;
                try { socket.Send(item.Payload); delivered++; } catch { }
            }
            if (delivered > 0) Log($"[PUSH] Delivered {delivered} held notification(s) to '{userId}' after reconnect.");
        }

        public static bool IsRunning => _server != null;
        public static int Port { get; private set; } = 9000;

        public static event Action<string> OnLog;
        public static event Action OnUsersUpdated;
        public static event Action<bool> OnServerStateChanged;

        public static IReadOnlyDictionary<string, IWebSocketConnection> ActiveUsers => _activeUsers;

        public static bool Start(out string error, int port = 9000)
        {
            error = string.Empty;
            if (_server != null)
            {
                Log("Server is already running.");
                return true;
            }

            try
            {
                Port = port;
                _server = new WebSocketServer($"ws://0.0.0.0:{Port}");
                _server.RestartAfterListenError = true;
                try { _server.ListenerSocket.NoDelay = true; } catch { }   // send small signaling packets immediately

                _server.Start(socket =>
                {
                    socket.OnOpen = () =>
                    {
                        Touch(socket);
                        Log($"[+] Connection opened from: {socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort}");
                    };

                    socket.OnClose = () =>
                    {
                        Log($"[-] Connection closed: {socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort}");
                        DateTime ignored;
                        _lastSeen.TryRemove(socket, out ignored);
                        RemoveUserBySocket(socket);
                    };

                    socket.OnMessage = message =>
                    {
                        Touch(socket);
                        HandleIncomingMessage(socket, message);
                    };

                    socket.OnPing = data =>
                    {
                        Touch(socket);
                        try { socket.SendPong(data); } catch { }
                    };

                    socket.OnPong = data => Touch(socket);

                    socket.OnError = ex =>
                    {
                        Log($"[!] Socket error ({socket.ConnectionInfo?.ClientIpAddress}): {ex.Message}");
                    };
                });

                if (_sweepTimer == null) _sweepTimer = new Timer(SweepStaleSockets, null, 10000, 10000);
                Log($"WebSocket Server started successfully on ws://0.0.0.0:{Port}");
                OnServerStateChanged?.Invoke(true);
                return true;
            }
            catch (Exception ex)
            {
                _server = null;
                error = ex.Message;
                Log($"[FATAL] Failed to start server on port {port}: {ex.Message}");
                OnServerStateChanged?.Invoke(false);
                return false;
            }
        }

        public static void Stop()
        {
            if (_server != null)
            {
                try
                {
                    _server.Dispose();
                }
                catch (Exception ex)
                {
                    Log($"[!] Error during server shutdown: {ex.Message}");
                }
                finally
                {
                    _server = null;
                    try { if (_sweepTimer != null) { _sweepTimer.Dispose(); _sweepTimer = null; } } catch { }
                    _lastSeen.Clear();
                    _pending.Clear();
                    _pushQueue.Clear();
                    _activeUsers.Clear();
                    _userSockets.Clear();
                    Log("WebSocket Server stopped.");
                    OnUsersUpdated?.Invoke();
                    OnServerStateChanged?.Invoke(false);
                }
            }
        }

        private static void HandleIncomingMessage(IWebSocketConnection socket, string message)
        {
            try
            {
                JObject json = JObject.Parse(message);
                string type = json["type"]?.ToString();

                // 0. Keep-Alive Ping
                if (type == "ping")
                {
                    if (socket.IsAvailable)
                    {
                        socket.Send("{\"type\":\"pong\"}");
                    }
                    return;
                }

                // 1. Register User ID and/or Username
                if (type == "register")
                {
                    string userId = json["userId"]?.ToString()?.Trim();
                    string username = json["username"]?.ToString()?.Trim();

                    if (!string.IsNullOrEmpty(userId))
                    {
                        _activeUsers[userId] = socket;
                        var dict = _userSockets.GetOrAdd(userId, _ => new ConcurrentDictionary<IWebSocketConnection, byte>());
                        dict.TryAdd(socket, 0);
                        Log($"[REGISTER] User ID '{userId}' bound to IP {socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort} (Total devices: {dict.Count})");
                        FlushPending(userId, socket);
                        FlushPushQueue(userId, socket);
                    }
                    if (!string.IsNullOrEmpty(username) && !username.Equals(userId, StringComparison.OrdinalIgnoreCase))
                    {
                        _activeUsers[username] = socket;
                        var dict = _userSockets.GetOrAdd(username, _ => new ConcurrentDictionary<IWebSocketConnection, byte>());
                        dict.TryAdd(socket, 0);
                        Log($"[REGISTER] Username '{username}' bound to IP {socket.ConnectionInfo.ClientIpAddress}:{socket.ConnectionInfo.ClientPort} (Total devices: {dict.Count})");
                        FlushPending(username, socket);
                        FlushPushQueue(username, socket);
                    }
                    OnUsersUpdated?.Invoke();
                }
                // 1b. Push notification requests (only accepted from this machine / the web server)
                else if (type == "push")
                {
                    string source = socket.ConnectionInfo?.ClientIpAddress;
                    if (source != null && TrustedPushSources.Contains(source))
                    {
                        HandlePush(socket, json);
                    }
                    else
                    {
                        Log($"[PUSH] Rejected push request from untrusted source {source}.");
                    }
                }
                // 2. Route signaling payloads
                else if (type == "offer" || type == "answer" || type == "ice_candidate" || type == "candidate"
                      || type == "call_accept" || type == "reject" || type == "end" || type == "cancel" || type == "fallback")
                {
                    string targetId = json["targetId"]?.ToString()?.Trim();
                    string senderId = GetUserIdBySocket(socket);

                    if (string.IsNullOrEmpty(senderId))
                    {
                        senderId = json["senderId"]?.ToString()?.Trim();
                        if (!string.IsNullOrEmpty(senderId))
                        {
                            _activeUsers[senderId] = socket;
                            OnUsersUpdated?.Invoke();
                        }
                    }

                    var targetSockets = new List<IWebSocketConnection>();
                    if (!string.IsNullOrEmpty(targetId) && _userSockets.TryGetValue(targetId, out var sockDict))
                    {
                        targetSockets.AddRange(sockDict.Keys.Where(s => s != null && s.IsAvailable));
                    }
                    if (targetSockets.Count == 0 && !string.IsNullOrEmpty(targetId) && _activeUsers.TryGetValue(targetId, out var fbSock) && fbSock.IsAvailable)
                    {
                        targetSockets.Add(fbSock);
                    }

                    if (targetSockets.Count > 0)
                    {
                        json["senderId"] = senderId;
                        string payload = json.ToString(Newtonsoft.Json.Formatting.None);
                        foreach (var ts in targetSockets)
                        {
                            try { ts.Send(payload); } catch { }
                        }
                        Log($"[ROUTED] {type.ToUpper()} from User '{senderId}' -> Target '{targetId}' ({targetSockets.Count} device(s))");

                        // Multi-device synchronization: If this signal indicates the call was answered on this device,
                        // immediately notify all other devices of the same user to drop the incoming call UI and stop ringing.
                        if (type == "call_accept" || type == "call_answered")
                        {
                            if (!string.IsNullOrEmpty(senderId))
                            {
                                var otherDevices = new List<IWebSocketConnection>();
                                if (_userSockets.TryGetValue(senderId, out var mySockets))
                                {
                                    otherDevices.AddRange(mySockets.Keys.Where(s => s != null && s != socket && s.IsAvailable));
                                }
                                if (otherDevices.Count > 0)
                                {
                                    var dropNotice = new JObject
                                    {
                                        ["type"] = "call_answered",
                                        ["senderId"] = senderId,
                                        ["message"] = "answered_on_other_device"
                                    };
                                    string dropPayload = dropNotice.ToString(Newtonsoft.Json.Formatting.None);
                                    foreach (var otherSock in otherDevices)
                                    {
                                        try { otherSock.Send(dropPayload); } catch { }
                                    }
                                    Log($"[MULTI-DEVICE] Sent call_answered drop notice to {otherDevices.Count} other device(s) of User '{senderId}'");
                                }
                            }
                        }
                    }
                    else
                    {
                        Log($"[HOLD] Target User '{targetId}' is offline right now - holding {type} for up to {PendingMaxAge.TotalSeconds:0}s.");
                        json["senderId"] = senderId;
                        var held = new PendingSignal
                        {
                            TargetId = targetId,
                            Type = type,
                            Payload = json.ToString(Newtonsoft.Json.Formatting.None),
                            At = DateTime.UtcNow
                        };
                        if (!string.IsNullOrEmpty(targetId))
                        {
                            _pending.GetOrAdd(targetId, _ => new ConcurrentQueue<PendingSignal>()).Enqueue(held);
                        }

                        if (type != "ice_candidate" && type != "candidate" && type != "fallback" && type != "end" && type != "cancel" && type != "reject")
                        {
                            // Only tell the caller the peer is offline if it did not come back within the grace period
                            Task.Delay(PeerOfflineGrace).ContinueWith(_ =>
                            {
                                try
                                {
                                    if (!held.Delivered && socket.IsAvailable)
                                    {
                                        var notice = new JObject
                                        {
                                            ["type"] = "peer_offline",
                                            ["senderId"] = targetId
                                        };
                                        socket.Send(notice.ToString(Newtonsoft.Json.Formatting.None));
                                        Log($"[FAILED] Could not route {type} - Target User '{targetId}' stayed offline.");
                                    }
                                }
                                catch { }
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"[ERROR] Failed to process message: {ex.Message}");
            }
        }

        private static void RemoveUserBySocket(IWebSocketConnection socket)
        {
            var emptyKeys = new List<string>();
            foreach (var kvp in _userSockets)
            {
                if (kvp.Value.TryRemove(socket, out _))
                {
                    Log($"[REMOVED] Device disconnected for User/Key '{kvp.Key}'.");
                }
                if (kvp.Value.IsEmpty)
                {
                    emptyKeys.Add(kvp.Key);
                }
            }
            foreach (var k in emptyKeys)
            {
                _userSockets.TryRemove(k, out _);
            }

            var keysToRemove = _activeUsers.Where(x => x.Value == socket).Select(x => x.Key).ToList();
            foreach (var key in keysToRemove)
            {
                if (_activeUsers.TryGetValue(key, out var currentSocket) && currentSocket == socket)
                {
                    if (_userSockets.TryGetValue(key, out var remaining) && remaining.Count > 0)
                    {
                        var nextSocket = remaining.Keys.FirstOrDefault(s => s != null && s.IsAvailable);
                        if (nextSocket != null) _activeUsers[key] = nextSocket;
                        else _activeUsers.TryRemove(key, out _);
                    }
                    else
                    {
                        _activeUsers.TryRemove(key, out _);
                    }
                }
            }
            OnUsersUpdated?.Invoke();
        }

        public static string GetUserIdBySocket(IWebSocketConnection socket)
        {
            foreach (var kvp in _userSockets)
            {
                if (kvp.Value.ContainsKey(socket))
                {
                    if (kvp.Key.All(char.IsDigit)) return kvp.Key;
                }
            }
            var matches = _activeUsers.Where(x => x.Value == socket).Select(x => x.Key).ToList();
            var numericMatch = matches.FirstOrDefault(k => k.All(char.IsDigit));
            if (!string.IsNullOrEmpty(numericMatch)) return numericMatch;
            return matches.FirstOrDefault();
        }

        public static int Broadcast(string jsonPayload)
        {
            if (_server == null) return 0;
            int sent = 0;
            var sockets = new HashSet<IWebSocketConnection>();
            foreach (var dict in _userSockets.Values)
            {
                foreach (var s in dict.Keys) sockets.Add(s);
            }
            foreach (var s in _activeUsers.Values) sockets.Add(s);

            foreach (var client in sockets)
            {
                try
                {
                    if (client.IsAvailable)
                    {
                        client.Send(jsonPayload);
                        sent++;
                    }
                }
                catch { }
            }
            return sent;
        }

        private static void Log(string message)
        {
            string formatted = $"[{DateTime.Now:HH:mm:ss}] {message}";
            OnLog?.Invoke(formatted);
        }
    }
}
