---
type: "CodebaseIndex"
tags: [obsidian, graphify, icm, codebase-index, server-html-handler, signaling]
updated: "2026-10-24"
---

# Server HTML Handler - Codebase Index

[[ICM_CHANGELOG]] | Android side: `C:\Users\AUDIT STAFF\AndroidStudioProjects\RMS\_docs\OBSIDIAN_CODEBASE_INDEX.md` and `CALL_AND_NOTIFICATION_GRAPH.md`

WinForms (.NET Framework 4.8) host for the RMS web/PHP services and the **WebSocket signaling server** used by the Android app for LAN calls and the LAN push fallback.

## Main files

| File | Responsibility |
| :--- | :--- |
| `Program.cs`, `Form1.cs` | Application start, main window. |
| `Services/CallSignalingService.cs` (562 lines) | Fleck WebSocket server on `ws://0.0.0.0:9000`: user registration, call signaling, LAN push. |
| `Services/NotificationDispatcherService.cs` (1674 lines) | Digest scheduler (`SendSlots` 241), recipients (`GetRecipientsAsync` 1156, `SaveRecipientsAsync` 1299), colour expiry (`ScanColorExpiryAsync` 1402, `PreviewDigestAsync` 1475), safe events (`Raise` 1658). Used by `View/Contents/NotificationHubUserControl.cs`. |
| `Services/ApacheWatchdogService.cs` (372) | Keeps the XAMPP Apache running; uses `Helper.ServerBackupHelper.IsXamppApacheRunning` (`Helper/Helper.cs` 244). |
| `Services/ShareControlService.cs` (559), `View/Contents/ShareMonitorUserControl.cs` (636) | **Folder Access** tab: SMB sessions, kick/block, Read/Write grants, subnet allow (Administrator). |
| `Helper/AppLoginStore.cs` (76) | Shared sign-in for every build (`%ProgramData%\ServerHTMLHandler\app-login.json`, default user `master`). |
| `Services/DeploymentStagingService.cs` | Deployment staging. |
| `View/Contents/*` | User controls (server control, console, project view, notification hub). |

## CallSignalingService.cs - map (current lines)

| Member | Line | Purpose |
| :--- | :--- | :--- |
| `SweepStaleSockets` | 55 | Every 10 s: closes sockets silent for 40 s, drops expired held signals and expired queued pushes. |
| `FlushPending` | 93 | Delivers signals held for a user who was briefly offline (max age 10 s). |
| `HandlePush` | 132 | Accepts `push` requests, delivers to the target's devices or queues them for `ttl` seconds (max 50 per user), answers `push_result`. |
| `FlushPushQueue` | 188 | Delivers queued pushes when the user registers again. |
| `Start` | 213 | Starts Fleck (NoDelay, restart after listen error), per-socket OnOpen / OnClose / OnMessage / OnPing / OnPong, starts the sweeper. |
| `HandleIncomingMessage` | 308 | Dispatch by `type`. |
| `RemoveUserBySocket` | 476 | Cleans user/socket maps on disconnect. |
| `Broadcast` | 530 | Sends a payload to every connected socket. |

## Message types

| `type` | From | Handling |
| :--- | :--- | :--- |
| `ping` | phone | Replies `pong` (OkHttp also sends protocol pings every 8 s). |
| `register` | phone | Binds `userId` (Synology id) and `username` to the socket (several devices per user); flushes held signals and queued pushes. |
| `offer`, `answer`, `ice_candidate`, `candidate`, `call_accept`, `reject`, `end`, `cancel`, `fallback` | phone | Routed to every device of `targetId` with `senderId` added. Offline target: signal held 10 s; `peer_offline` is sent back only after a 3 s grace period (not for candidate / fallback / end / cancel / reject). `call_accept` also sends `call_answered` to the user's other devices. |
| `push` | PHP only (127.0.0.1, ::1, 192.168.32.43) | `{type:"push", targetId, ttl, title, body, data{...}}` -> `{type:"push", id, title, body, data, sentAt}` to the phone; reply `{type:"push_result", delivered, queued}`. |

## Rules
- Needs a rebuild + restart of Server HTML Handler after changes to `CallSignalingService.cs`.
- PHP side lives in `D:\html` (`fcm-helper.php` -> `lanPushSend` / `lanPushFallback`).
