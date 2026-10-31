# ICM Record: Server HTML Handler

[[OBSIDIAN_CODEBASE_INDEX]] | Android record: `C:\Users\AUDIT STAFF\AndroidStudioProjects\RMS\_docs\ICM_CHANGELOG.md`

## Phase 194 - CLI 'deploy sync-all' Multi-Developer Live Sync (2026-10-31)
- **Backup:** `C:\Users\AUDIT STAFF\AndroidStudioProjects\RMS\_code_backups\v194_deploy_sync_all_cli\`
- **CLI Enhancement:** Added `deploy sync-all` and `deploy sync all` commands to simultaneously synchronize the live site across all registered developer staging workspaces in the selected project.
- **Workflow Protection:** Each developer's `Private/` test directory is refreshed with live code and their `Base/` snapshot is updated (guarded if unmerged files exist in `Update/`).
- **Published:** Bumped ClickOnce `ApplicationRevision` to 28 and published to `E:\Deployed\Web Hosting Handler\`.

## Phase 193 - Unified Tray-First Architecture & Apache Auto-Start on Boot (2026-10-31)
- **Backup:** `C:\Users\AUDIT STAFF\AndroidStudioProjects\RMS\_code_backups\v193_server_handler_tray_first_autostart\`
- **Decoupled System Tray Architecture:** Server HTML Handler now starts directly into the Windows System Notification Tray (`NotifyIcon`) without presenting an interactive login screen or requiring manual clicks to run Apache.
- **Auto-Start on Boot:** Background duties (`ApacheWatchdogService`, `CallSignalingService` on port 9000, `NotificationDispatcherService`) launch immediately on process start. Apache is automatically brought online without user login.
- **Single-Instance IPC:** Mutex prevents duplicate instances. Launching the desktop shortcut while the tray app is running signals the active process via `EventWaitHandle` to bring the Dashboard to the foreground immediately.
- **Apache Windows Service Configuration:** Added `ServerBackupHelper.ConfigureApacheServiceAutoStart` allowing one-click elevation to set `Apache2.4` service start type to `Automatic`, enabling Apache to run on computer power-on even before any Windows user logs in.
- **Published:** Bumped ClickOnce `ApplicationRevision` to 27 and published to `E:\Deployed\Web Hosting Handler\`.

## Phase 120 - Notification hub, Apache watchdog, Folder Access, shared login (2026-10-24)
- **NotificationDispatcherService:** digests only 07:00-17:00, once per 24 h cycle (`SendSlots`: low stock 2/day, colour expiry 1/day); recipients read/written in `dbRMSAndroid.dbo.tbl_NotificationCategory` (`GetRecipientsAsync`, `SaveRecipientsAsync`) and edited in the Hub **Recipients** tab; colour-expiry scan (`ScanColorExpiryAsync`): start = `EnteredDate`, dark +6 months / light +2 months by Hex or palette name, latest row wins per colour code/name, lists items 14 days before to 30 days after expiry; events raised through `Raise<T>` so a closed tab cannot break a send. Keeps running while minimised / in the tray.
- **ApacheWatchdogService:** on open and periodically, makes sure the XAMPP Apache is up (service `Apache2.4`, then `httpd.exe`, config test first). XAMPP-only detection in `Helper.ServerBackupHelper` (`GetXamppApacheProcesses`, `IsXamppApacheRunning`); other `httpd.exe` (e.g. PEMHTTPD on 8080) is ignored.
- **Folder Access** sidebar tab (`ShareMonitorUserControl`, `ShareControlService`): connected SMB users of the HTML share, kick/block, access-request notifications with Read/Write grants, subnet allow. Needs Administrator.
- **AppLoginStore:** one sign-in shared by debug, published and new installs (`%ProgramData%\ServerHTMLHandler\app-login.json`; default user `master`, built-in default hash). A stale file written by the "sync app login" option overrode the default and broke the published login; it was renamed to `app-login.json.bak_2026-10-24`.
- **Incident - corrupted sources:** `Helper.cs`, `DashbaordContents.cs`, `NotificationDispatcherService.cs` and an `obj` tracking file were NUL-filled (caused CS1662 / CS0029 / CS0121 and MSB3541). Restored from backups; Debug and Release build with 0 errors / 0 warnings. D: logged NTFS event 55 on 2026-10-24 07:17 after an unclean shutdown: run `chkdsk D: /scan`. Source backup: `C:\Users\AUDIT STAFF\Documents\ServerHTMLHandler_backup_2026-10-24.zip`.
- **csproj:** Visual Studio dropped Compile entries once; keep `Helper\AppLoginStore.cs`, `Services\ApacheWatchdogService.cs`, `Services\ShareControlService.cs`, `View\Contents\ShareMonitorUserControl.cs`. Reload (do not overwrite) the project, rebuild, republish.
- Not yet tested: Folder Access kick/grant/denied detection and the watchdog from the real window (need Administrator); reaching 192.168.32.43 from 192.168.33.127.

## Phase 108 - Signaling hardening for weak signal (2026-10-23)
- **Backup:** `C:\Users\AUDIT STAFF\AndroidStudioProjects\RMS\_code_backups\v108_call_fallback_and_signaling\CallSignalingService.cs`
- `WebSocketServer`: `RestartAfterListenError = true`, `ListenerSocket.NoDelay = true`.
- Per-socket `OnPing` (answers pong and counts as activity) and `OnPong`; sockets with no traffic for 40 s are closed by a 10 s sweeper (a dead phone is no longer shown as online).
- Signals to a user who is offline for a moment are held up to 10 s and delivered when the user registers again; `peer_offline` is only sent after a 3 s grace period, and only for signals where it makes sense.

## Phase 110 - LAN push fallback (2026-10-23)
- **Backup:** `C:\Users\AUDIT STAFF\AndroidStudioProjects\RMS\_code_backups\v110_lan_push_fallback\CallSignalingService.cs`
- New message type `push` (trusted sources only: 127.0.0.1, ::1, 192.168.32.43). Delivered to the target's connected devices, otherwise queued for `ttl` seconds (max 50 per user, max 24 h) and flushed on `register`. Reply `push_result {delivered, queued}`.
- Used by the PHP senders in `D:\html` (`lanPushFallback`) for users without a Firebase token.

## Phase 102 - Notification categories (2026-10-22)
- No C# change. `D:\html\send-notification.php` now reads `dbRMSAndroid.dbo.tbl_NotificationCategory` to pick recipients (CROSSMIX = 214;426).

## Deployment notes
- After pulling these changes: rebuild **Server HTML Handler** in Visual Studio and restart it; confirm the log shows `WebSocket Server started successfully on ws://0.0.0.0:9000`.
- Verified: file compiles with Roslyn against Fleck 1.2.0 / Newtonsoft.Json; PHP client tested against a mock WebSocket server. Not yet tested against real phones.
