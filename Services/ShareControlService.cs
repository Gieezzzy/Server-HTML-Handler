using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server_HTML_Handler.Services
{
    public sealed class SmbClientInfo
    {
        public string SessionId { get; set; }
        public string Ip { get; set; }
        public string User { get; set; }
        public int OpenFiles { get; set; }
        public long ConnectedSeconds { get; set; }
        public long IdleSeconds { get; set; }
        /// <summary>Connected to the monitored share, or has files open inside the monitored folder.</summary>
        public bool OnFolder { get; set; }
        public string ConnectedText => TimeSpan.FromSeconds(Math.Max(0, ConnectedSeconds)).ToString(ConnectedSeconds >= 86400 ? @"d\.hh\:mm\:ss" : @"hh\:mm\:ss");
        public string IdleText => TimeSpan.FromSeconds(Math.Max(0, IdleSeconds)).ToString(IdleSeconds >= 86400 ? @"d\.hh\:mm\:ss" : @"hh\:mm\:ss");
    }

    public sealed class SmbOpenInfo
    {
        public string FileId { get; set; }
        public string SessionId { get; set; }
        public string Path { get; set; }
        public string Ip { get; set; }
        public string User { get; set; }
    }

    public sealed class ShareSnapshot
    {
        public List<SmbClientInfo> Clients { get; set; } = new List<SmbClientInfo>();
        public List<SmbOpenInfo> Opens { get; set; } = new List<SmbOpenInfo>();
    }

    public sealed class ShareAclEntry
    {
        public string Account { get; set; }
        public string Type { get; set; }
        public string Right { get; set; }
    }

    public sealed class NtfsAclEntry
    {
        public string Identity { get; set; }
        public string Rights { get; set; }
        public string Type { get; set; }
        public bool Inherited { get; set; }
    }

    public sealed class AccessRequestInfo
    {
        public DateTime Time { get; set; }
        /// <summary>"Logon failed" (no/invalid account on this PC) or "Access denied" (account known, no permission on the folder).</summary>
        public string Kind { get; set; }
        public string Ip { get; set; }
        public string User { get; set; }
        public string Target { get; set; }
        public string Detail { get; set; }
        public int Count { get; set; }
        public string Key => (Kind + "|" + Ip + "|" + User + "|" + Target).ToLowerInvariant();
    }

    /// <summary>
    /// Controls who can reach the hosting folder (HTMLPath) over the network: lists the PCs connected to it, disconnects ("kicks")
    /// and blocks them, reports PCs that were refused, and grants Read / Write / Full access. Uses the Windows SMB cmdlets through
    /// PowerShell, which need Administrator rights.
    /// </summary>
    public sealed class ShareControlService
    {
        private static readonly Lazy<ShareControlService> _instance = new Lazy<ShareControlService>(() => new ShareControlService());
        public static ShareControlService Instance => _instance.Value;

        public string FolderPath { get; set; } = @"E:\html";
        public string ShareName { get; private set; } = "";
        public int RequestLookbackMinutes { get; set; } = 120;
        public int PollSeconds { get; set; } = 20;

        public event Action<string> OnLog;
        /// <summary>Raised once for each new refused connection (not for ones already dismissed).</summary>
        public event Action<AccessRequestInfo> OnNewAccessRequest;

        private readonly object _lock = new object();
        private readonly Dictionary<string, AccessRequestInfo> _pending = new Dictionary<string, AccessRequestInfo>();
        private readonly HashSet<string> _dismissed = new HashSet<string>();
        private readonly HashSet<string> _announced = new HashSet<string>();
        private Timer _timer;
        private int _busy;

        private ShareControlService()
        {
            LoadDismissed();
        }

        // ------------------------------------------------------------------ elevation / PowerShell

        public static bool IsElevated()
        {
            try
            {
                using (var id = WindowsIdentity.GetCurrent())
                    return new WindowsPrincipal(id).IsInRole(WindowsBuiltInRole.Administrator);
            }
            catch { return false; }
        }

        /// <summary>Starts a second copy of this program as Administrator (UAC prompt). The caller should then exit.</summary>
        public static bool RestartElevated()
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Process.GetCurrentProcess().MainModule.FileName,
                    UseShellExecute = true,
                    Verb = "runas"
                });
                return true;
            }
            catch
            {
                return false;   // UAC prompt declined
            }
        }

        private static string Q(string s) => "'" + (s ?? "").Replace("'", "''") + "'";

        /// <summary>Runs PowerShell (no profile, no window) and returns its output. Throws with the error text on failure.</summary>
        public static async Task<string> RunPsAsync(string body, int timeoutSeconds = 60)
        {
            string script = "$ErrorActionPreference='Stop'; $ProgressPreference='SilentlyContinue'; $WarningPreference='SilentlyContinue'; " +
                            "[Console]::OutputEncoding=[System.Text.Encoding]::UTF8; " +
                            // plain-text errors (PowerShell would otherwise answer in CLIXML)
                            "try { " + body + " } catch { [Console]::Error.WriteLine($_.Exception.Message); exit 1 }";
            string encoded = Convert.ToBase64String(Encoding.Unicode.GetBytes(script));

            var psi = new ProcessStartInfo
            {
                FileName = "powershell.exe",
                Arguments = "-NoProfile -NonInteractive -ExecutionPolicy Bypass -EncodedCommand " + encoded,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8
            };

            using (var p = Process.Start(psi))
            {
                Task<string> outTask = p.StandardOutput.ReadToEndAsync();
                Task<string> errTask = p.StandardError.ReadToEndAsync();
                bool exited = await Task.Run(() => p.WaitForExit(timeoutSeconds * 1000));
                if (!exited)
                {
                    try { p.Kill(); } catch { }
                    throw new TimeoutException("PowerShell did not finish in " + timeoutSeconds + " s.");
                }
                string output = (await outTask).Trim();
                string error = (await errTask).Trim();
                if (p.ExitCode != 0)
                {
                    string msg = error.Length > 0 ? error : output;
                    var first = msg.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(l => !l.StartsWith("At line") && !l.StartsWith("+"));
                    throw new InvalidOperationException(string.IsNullOrWhiteSpace(first) ? "PowerShell failed (exit code " + p.ExitCode + ")." : first.Trim());
                }
                return output;
            }
        }

        private static JToken ParseJson(string json)
        {
            return string.IsNullOrWhiteSpace(json) ? JValue.CreateNull() : JToken.Parse(json);
        }

        private static IEnumerable<JToken> AsList(JToken t)
        {
            if (t == null || t.Type == JTokenType.Null) return Enumerable.Empty<JToken>();
            return t.Type == JTokenType.Array ? t.Children() : new[] { t };
        }

        // ------------------------------------------------------------------ input validation (values end up inside PowerShell)

        private static readonly Regex AccountRegex = new Regex(@"^[A-Za-z0-9 _.\-\\@$]{1,100}$", RegexOptions.Compiled);
        public static bool IsSafeAccount(string a) => !string.IsNullOrWhiteSpace(a) && AccountRegex.IsMatch(a.Trim());

        public static bool TryParseIpv4(string text, out string ip)
        {
            ip = null;
            if (IPAddress.TryParse((text ?? "").Trim(), out IPAddress a) && a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                ip = a.ToString();
                return true;
            }
            return false;
        }

        /// <summary>"192.168.33.0/24" or a single IPv4; returns the normalised text.</summary>
        public static bool TryParseCidr(string text, out string cidr)
        {
            cidr = null;
            string t = (text ?? "").Trim();
            int slash = t.IndexOf('/');
            string ipPart = slash >= 0 ? t.Substring(0, slash) : t;
            if (!TryParseIpv4(ipPart, out string ip)) return false;
            if (slash < 0) { cidr = ip; return true; }
            if (!int.TryParse(t.Substring(slash + 1), out int bits) || bits < 8 || bits > 32) return false;
            cidr = ip + "/" + bits;
            return true;
        }

        // ------------------------------------------------------------------ share + live connections

        /// <summary>Finds the SMB share that exposes FolderPath (the share whose folder is FolderPath or a parent of it).</summary>
        public async Task<string> FindShareAsync()
        {
            string json = await RunPsAsync(
                "$p=" + Q(FolderPath.TrimEnd('\\')) + "; " +
                "@(Get-SmbShare | Where-Object { -not $_.Special -and $_.Path } | Where-Object { $p -ieq $_.Path.TrimEnd('\\') -or $p.StartsWith($_.Path.TrimEnd('\\') + '\\', [StringComparison]::OrdinalIgnoreCase) } | " +
                "Sort-Object { $_.Path.Length } -Descending | Select-Object -First 1 Name,Path) | ConvertTo-Json -Compress");
            var first = AsList(ParseJson(json)).FirstOrDefault();
            ShareName = first == null ? "" : (string)first["Name"];
            return ShareName;
        }

        public async Task<ShareSnapshot> GetSnapshotAsync(bool showAllSessions)
        {
            string root = FolderPath.TrimEnd('\\');
            string json = await RunPsAsync(
                "$root=" + Q(root) + "; $share=" + Q(ShareName) + "; " +
                "$sessions=@(Get-SmbSession | ForEach-Object { [pscustomobject]@{ id=[string]$_.SessionId; ip=[string]$_.ClientComputerName; user=[string]$_.ClientUserName; opens=[int]$_.NumOpens; exists=[long]$_.SecondsExists; idle=[long]$_.SecondsIdle } }); " +
                "$opens=@(Get-SmbOpenFile | Where-Object { $_.Path -like ($root + '*') } | ForEach-Object { [pscustomobject]@{ file=[string]$_.FileId; id=[string]$_.SessionId; path=[string]$_.Path; ip=[string]$_.ClientComputerName; user=[string]$_.ClientUserName } }); " +
                "$conns=@(); try { $conns=@(Get-CimInstance Win32_ServerConnection | Where-Object { $_.ShareName -eq $share } | ForEach-Object { [pscustomobject]@{ pc=[string]$_.ComputerName; user=[string]$_.UserName } }) } catch {}; " +
                "[pscustomobject]@{ sessions=$sessions; opens=$opens; conns=$conns } | ConvertTo-Json -Compress -Depth 4");

            JToken root0 = ParseJson(json);
            var snap = new ShareSnapshot();
            var opens = AsList(root0["opens"]).ToList();
            var conns = AsList(root0["conns"]).ToList();

            foreach (var o in opens)
                snap.Opens.Add(new SmbOpenInfo { FileId = (string)o["file"], SessionId = (string)o["id"], Path = (string)o["path"], Ip = (string)o["ip"], User = (string)o["user"] });

            foreach (var s in AsList(root0["sessions"]))
            {
                string id = (string)s["id"], ip = (string)s["ip"], user = (string)s["user"];
                int inFolder = opens.Count(o => (string)o["id"] == id);
                string userShort = (user ?? "").Split('\\').Last();
                // The share-connection list names the client PC (often by NetBIOS name, not IP), so match on the account name.
                bool onShare = conns.Any(c => string.Equals(((string)c["user"] ?? "").Split('\\').Last(), userShort, StringComparison.OrdinalIgnoreCase));
                bool relevant = inFolder > 0 || onShare;
                if (!relevant && !showAllSessions) continue;
                snap.Clients.Add(new SmbClientInfo
                {
                    SessionId = id, Ip = ip, User = user, OpenFiles = inFolder,
                    ConnectedSeconds = (long?)s["exists"] ?? 0, IdleSeconds = (long?)s["idle"] ?? 0, OnFolder = relevant
                });
            }
            snap.Clients = snap.Clients.OrderBy(c => c.Ip).ThenBy(c => c.User).ToList();
            return snap;
        }

        // ------------------------------------------------------------------ kick / block

        /// <summary>Immediately closes every SMB session and open file of a client PC.</summary>
        public async Task KickAsync(string ip, bool alsoBlock)
        {
            if (!TryParseIpv4(ip, out string safe)) throw new ArgumentException("Not a valid IPv4 address: " + ip);
            if (alsoBlock) await BlockIpAsync(safe);
            await RunPsAsync(
                "$ip=" + Q(safe) + "; " +
                "Get-SmbOpenFile | Where-Object { $_.ClientComputerName -eq $ip } | Close-SmbOpenFile -Force; " +
                "Get-SmbSession | Where-Object { $_.ClientComputerName -eq $ip } | Close-SmbSession -Force");
            Log($"Kicked {safe}{(alsoBlock ? " and blocked it" : "")}.");
        }

        public async Task KickEveryoneAsync()
        {
            await RunPsAsync("Get-SmbOpenFile | Where-Object { $_.Path -like (" + Q(FolderPath.TrimEnd('\\')) + " + '*') } | Close-SmbOpenFile -Force; " +
                             "$ids=@(Get-SmbOpenFile | Select-Object -ExpandProperty SessionId -Unique); " +
                             "Get-SmbSession | Where-Object { $_.NumOpens -gt 0 -or $ids -contains $_.SessionId } | Close-SmbSession -Force");
            Log("Closed all sessions that use the folder.");
        }

        public async Task CloseOpenFileAsync(string fileId)
        {
            if (!Regex.IsMatch(fileId ?? "", @"^\d{1,20}$")) throw new ArgumentException("Invalid file id.");
            await RunPsAsync("Get-SmbOpenFile -FileId " + fileId + " | Close-SmbOpenFile -Force");
        }

        private const string BlockGroup = "HTMLHandler Blocks";
        private const string AllowGroup = "HTMLHandler Access";

        public async Task BlockIpAsync(string ip)
        {
            if (!TryParseIpv4(ip, out string safe)) throw new ArgumentException("Not a valid IPv4 address: " + ip);
            await RunPsAsync(
                "$n='HTMLHandler Block ' + " + Q(safe) + "; " +
                "if (-not (Get-NetFirewallRule -DisplayName $n -ErrorAction SilentlyContinue)) { " +
                "New-NetFirewallRule -DisplayName $n -Group " + Q(BlockGroup) + " -Direction Inbound -Action Block -Protocol TCP -LocalPort 445,139 -RemoteAddress " + Q(safe) + " -Profile Any | Out-Null }");
            Log($"Blocked {safe} from the file share (firewall).");
        }

        public async Task UnblockIpAsync(string ip)
        {
            if (!TryParseIpv4(ip, out string safe)) throw new ArgumentException("Not a valid IPv4 address: " + ip);
            await RunPsAsync("Get-NetFirewallRule -DisplayName ('HTMLHandler Block ' + " + Q(safe) + ") -ErrorAction SilentlyContinue | Remove-NetFirewallRule");
            Log($"Unblocked {safe}.");
        }

        public async Task<List<string>> GetBlockedIpsAsync()
        {
            string json = await RunPsAsync(
                "@(Get-NetFirewallRule -Group " + Q(BlockGroup) + " -ErrorAction SilentlyContinue | ForEach-Object { ($_ | Get-NetFirewallAddressFilter).RemoteAddress }) | ConvertTo-Json -Compress");
            return AsList(ParseJson(json)).Select(t => (string)t).Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().OrderBy(x => x).ToList();
        }

        /// <summary>Opens the firewall for a whole subnet (SMB 445 + ping), e.g. a second subnet such as 192.168.33.0/24.</summary>
        public async Task AllowSubnetAsync(string cidr)
        {
            if (!TryParseCidr(cidr, out string safe)) throw new ArgumentException("Use an IPv4 address or subnet like 192.168.33.0/24.");
            await RunPsAsync(
                "$c=" + Q(safe) + "; " +
                "if (-not (Get-NetFirewallRule -DisplayName ('HTMLHandler Allow SMB from ' + $c) -ErrorAction SilentlyContinue)) { " +
                "New-NetFirewallRule -DisplayName ('HTMLHandler Allow SMB from ' + $c) -Group " + Q(AllowGroup) + " -Direction Inbound -Action Allow -Protocol TCP -LocalPort 445 -RemoteAddress $c -Profile Any | Out-Null }; " +
                "if (-not (Get-NetFirewallRule -DisplayName ('HTMLHandler Allow ping from ' + $c) -ErrorAction SilentlyContinue)) { " +
                "New-NetFirewallRule -DisplayName ('HTMLHandler Allow ping from ' + $c) -Group " + Q(AllowGroup) + " -Direction Inbound -Action Allow -Protocol ICMPv4 -IcmpType 8 -RemoteAddress $c -Profile Any | Out-Null }");
            Log($"Firewall now allows file sharing and ping from {safe}.");
        }

        /// <summary>Reachability test from this PC to a client (ping + SMB port). Does not need Administrator.</summary>
        public async Task<string> TestConnectionAsync(string ip)
        {
            if (!TryParseIpv4(ip, out string safe)) throw new ArgumentException("Not a valid IPv4 address: " + ip);
            return await RunPsAsync(
                "$ip=" + Q(safe) + "; " +
                "'ping: ' + [bool](Test-Connection $ip -Count 2 -Quiet -ErrorAction SilentlyContinue); " +
                "'TCP 445 (file sharing): ' + [bool](Test-NetConnection $ip -Port 445 -WarningAction SilentlyContinue -InformationLevel Quiet); " +
                "'TCP 80 (web): ' + [bool](Test-NetConnection $ip -Port 80 -WarningAction SilentlyContinue -InformationLevel Quiet)", 90);
        }

        // ------------------------------------------------------------------ permissions

        private static string RightFor(string level)
        {
            switch ((level ?? "").ToLowerInvariant())
            {
                case "read": return "Read";
                case "write": return "Change";
                case "full": return "Full";
                default: throw new ArgumentException("Level must be Read, Write or Full.");
            }
        }

        private static string NtfsFor(string level)
        {
            switch ((level ?? "").ToLowerInvariant())
            {
                case "read": return "RX";
                case "write": return "M";
                case "full": return "F";
                default: throw new ArgumentException("Level must be Read, Write or Full.");
            }
        }

        public async Task<List<ShareAclEntry>> GetShareAccessAsync()
        {
            if (string.IsNullOrEmpty(ShareName)) return new List<ShareAclEntry>();
            string json = await RunPsAsync("@(Get-SmbShareAccess -Name " + Q(ShareName) + " | ForEach-Object { [pscustomobject]@{ a=[string]$_.AccountName; t=[string]$_.AccessControlType; r=[string]$_.AccessRight } }) | ConvertTo-Json -Compress");
            return AsList(ParseJson(json)).Select(t => new ShareAclEntry { Account = (string)t["a"], Type = (string)t["t"], Right = (string)t["r"] }).ToList();
        }

        public async Task<List<NtfsAclEntry>> GetNtfsAccessAsync()
        {
            string json = await RunPsAsync("@((Get-Acl -LiteralPath " + Q(FolderPath) + ").Access | ForEach-Object { [pscustomobject]@{ i=[string]$_.IdentityReference.Value; r=[string]$_.FileSystemRights; t=[string]$_.AccessControlType; h=[bool]$_.IsInherited } }) | ConvertTo-Json -Compress");
            return AsList(ParseJson(json)).Select(t => new NtfsAclEntry { Identity = (string)t["i"], Rights = (string)t["r"], Type = (string)t["t"], Inherited = (bool?)t["h"] ?? false }).ToList();
        }

        /// <summary>Gives an account Read / Write / Full access to the share and to the folder itself.</summary>
        public async Task GrantAsync(string account, string level)
        {
            if (!IsSafeAccount(account)) throw new ArgumentException("That account name has characters that are not allowed.");
            account = account.Trim();
            string ntfs = NtfsFor(level), share = RightFor(level);
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(ShareName))
                sb.Append("Grant-SmbShareAccess -Name " + Q(ShareName) + " -AccountName " + Q(account) + " -AccessRight " + share + " -Force | Out-Null; ");
            sb.Append("$g=" + Q(account) + " + ':(OI)(CI)" + ntfs + "'; $out = & icacls.exe " + Q(FolderPath) + " /grant $g 2>&1; if ($LASTEXITCODE -ne 0) { throw ($out -join ' ') }");
            await RunPsAsync(sb.ToString(), 180);
            Log($"Granted {level} access to {account}.");
        }

        public async Task RevokeAsync(string account)
        {
            if (!IsSafeAccount(account)) throw new ArgumentException("That account name has characters that are not allowed.");
            account = account.Trim();
            var sb = new StringBuilder();
            if (!string.IsNullOrEmpty(ShareName))
                sb.Append("Revoke-SmbShareAccess -Name " + Q(ShareName) + " -AccountName " + Q(account) + " -Force -ErrorAction SilentlyContinue | Out-Null; ");
            sb.Append("$out = & icacls.exe " + Q(FolderPath) + " /remove " + Q(account) + " 2>&1; if ($LASTEXITCODE -ne 0) { throw ($out -join ' ') }");
            await RunPsAsync(sb.ToString(), 180);
            Log($"Removed access of {account}.");
        }

        // ------------------------------------------------------------------ refused connections

        private const string FileShareAuditGuid = "{0CCE9224-69AE-11D9-BED3-505054503030}";

        /// <summary>True when failed share-access checks are written to the Security log (needed to see "access denied").</summary>
        public async Task<bool> IsAuditEnabledAsync()
        {
            string text = await RunPsAsync("& auditpol.exe /get /subcategory:" + Q(FileShareAuditGuid) + " | Out-String");
            return Regex.IsMatch(text, @"Failure", RegexOptions.IgnoreCase);
        }

        public async Task EnableAuditAsync()
        {
            await RunPsAsync("$o = & auditpol.exe /set /subcategory:" + Q(FileShareAuditGuid) + " /failure:enable 2>&1; if ($LASTEXITCODE -ne 0) { throw ($o -join ' ') }");
            Log("Auditing of failed file-share access is now on.");
        }

        /// <summary>Refused connections of the last N minutes: wrong/unknown account (event 4625) and no permission on the share (event 5145).</summary>
        public async Task<List<AccessRequestInfo>> GetAccessRequestsAsync(int minutes)
        {
            string json = await RunPsAsync(
                "$since=(Get-Date).AddMinutes(-" + Math.Max(1, minutes) + "); $share=" + Q(ShareName) + "; $out=@(); " +
                "function Read-Ev($e){ $x=[xml]$e.ToXml(); $d=@{}; foreach($n in $x.Event.EventData.Data){ $d[$n.Name]=$n.'#text' }; $d } " +
                "try { foreach($e in (Get-WinEvent -FilterHashtable @{LogName='Security';Id=4625;StartTime=$since} -MaxEvents 300 -ErrorAction Stop)) { $d=Read-Ev $e; " +
                "  if ($d.LogonType -eq '3' -and $d.IpAddress -and $d.IpAddress -ne '-' -and $d.TargetUserName -and $d.TargetUserName -ne '-') { " +
                "    $out += [pscustomobject]@{ t=$e.TimeCreated.ToString('s'); k='Logon failed'; ip=$d.IpAddress; u=($d.TargetDomainName + '\\' + $d.TargetUserName); target=''; detail=('status ' + $d.SubStatus) } } } } catch {} " +
                "try { foreach($e in (Get-WinEvent -FilterHashtable @{LogName='Security';Id=5145;Keywords=4503599627370496;StartTime=$since} -MaxEvents 300 -ErrorAction Stop)) { $d=Read-Ev $e; " +
                "  if ((-not $share) -or $d.ShareName -like ('*\\' + $share)) { " +
                "    $out += [pscustomobject]@{ t=$e.TimeCreated.ToString('s'); k='Access denied'; ip=$d.IpAddress; u=($d.SubjectDomainName + '\\' + $d.SubjectUserName); target=($d.ShareName + '\\' + $d.RelativeTargetName); detail=('access mask ' + $d.AccessMask) } } } } catch {} " +
                "$out | ConvertTo-Json -Compress -Depth 3", 90);

            var groups = new Dictionary<string, AccessRequestInfo>();
            foreach (var t in AsList(ParseJson(json)))
            {
                string ip = ((string)t["ip"] ?? "").Replace("::ffff:", "");
                if (string.IsNullOrWhiteSpace(ip) || ip == "-" || ip == "::1" || ip == "127.0.0.1") continue;
                var r = new AccessRequestInfo
                {
                    Time = DateTime.TryParse((string)t["t"], out var dt) ? dt : DateTime.Now,
                    Kind = (string)t["k"], Ip = ip, User = (string)t["u"], Target = (string)t["target"] ?? "", Detail = (string)t["detail"] ?? "", Count = 1
                };
                if (r.Kind == "Access denied" && (r.User ?? "").TrimEnd('$').EndsWith("\\", StringComparison.Ordinal)) continue;
                if (groups.TryGetValue(r.Key, out var existing))
                {
                    existing.Count++;
                    if (r.Time > existing.Time) existing.Time = r.Time;
                }
                else groups[r.Key] = r;
            }
            return groups.Values.OrderByDescending(x => x.Time).ToList();
        }

        // ------------------------------------------------------------------ background watcher (keeps notifying while another tab is open)

        public IReadOnlyList<AccessRequestInfo> PendingRequests
        {
            get { lock (_lock) return _pending.Values.OrderByDescending(x => x.Time).ToList(); }
        }

        public void StartMonitor()
        {
            if (_timer != null) return;
            _timer = new Timer(async _ => await PollAsync(), null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(Math.Max(10, PollSeconds)));
        }

        public void StopMonitor()
        {
            _timer?.Dispose();
            _timer = null;
        }

        private async Task PollAsync()
        {
            if (!IsElevated()) return;                                // the security log is only readable as Administrator
            if (Interlocked.Exchange(ref _busy, 1) == 1) return;
            try
            {
                if (string.IsNullOrEmpty(ShareName)) await FindShareAsync();
                var found = await GetAccessRequestsAsync(RequestLookbackMinutes);
                var fresh = new List<AccessRequestInfo>();
                lock (_lock)
                {
                    foreach (var r in found)
                    {
                        if (_dismissed.Contains(r.Key)) continue;
                        _pending[r.Key] = r;
                        if (_announced.Add(r.Key)) fresh.Add(r);
                    }
                }
                foreach (var r in fresh)
                {
                    try { OnNewAccessRequest?.Invoke(r); } catch { }
                }
            }
            catch (Exception ex)
            {
                Log("Share monitor: " + ex.Message);
            }
            finally
            {
                Interlocked.Exchange(ref _busy, 0);
            }
        }

        public void Dismiss(AccessRequestInfo r)
        {
            lock (_lock)
            {
                _dismissed.Add(r.Key);
                _pending.Remove(r.Key);
            }
            SaveDismissed();
        }

        private static string DismissedPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ServerHTMLHandler", "share-dismissed.json");

        private void LoadDismissed()
        {
            try
            {
                if (File.Exists(DismissedPath))
                    foreach (string k in JsonConvert.DeserializeObject<List<string>>(File.ReadAllText(DismissedPath)) ?? new List<string>())
                        _dismissed.Add(k);
            }
            catch { }
        }

        private void SaveDismissed()
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(DismissedPath));
                List<string> copy;
                lock (_lock) copy = _dismissed.Take(2000).ToList();
                File.WriteAllText(DismissedPath, JsonConvert.SerializeObject(copy));
            }
            catch { }
        }

        private void Log(string msg)
        {
            try { OnLog?.Invoke($"[{DateTime.Now:HH:mm:ss}] [Share] {msg}"); } catch { }
        }
    }
}
