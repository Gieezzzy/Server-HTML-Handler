# ICM Record: Server HTML Handler

[[OBSIDIAN_CODEBASE_INDEX]] | Android record: `C:\Users\AUDIT STAFF\AndroidStudioProjects\RMS\_docs\ICM_CHANGELOG.md`

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

## Phase 121 - Notification Type 'ALL' Global Broadcast Recipients (2026-10-24)
- **Backup:** _code_backups/v121_notification_type_all_recipients/ (NotificationDispatcherService.cs, NotificationHubUserControl.cs, send-notification.php)
- **Requirement:** Add an ALL option in Notification Type under the Recipients tab so any Synology ID assigned to ALL automatically receives every system notification.
- **Database (	bl_NotificationCategory):** Ensured row NotifType = 'ALL' exists in dbRMSAndroid.dbo.tbl_NotificationCategory with default priority HIGH.
- **C# Dispatcher (NotificationDispatcherService.cs):**
  - Added TypeAll = "ALL" constant.
  - Placed All Notifications (Global) at the very top of KnownCategories.
  - Upgraded GetRecipientsAsync(notifType): When querying recipients for any specific category, it performs a SQL union query (WHERE NotifType = @t OR NotifType = 'ALL') to combine category-specific recipients with global ALL recipients.
- **PHP Sender (D:\html\send-notification.php):**
  - Upgraded recipient resolution: SELECT ToChatSynolofyID FROM dbo.tbl_NotificationCategory WHERE UPPER(...) = ? OR UPPER(LTRIM(RTRIM(NotifType))) = 'ALL'.
  - Merges and deduplicates IDs so every Synology ID listed under ALL receives the broadcast.
- **UI (NotificationHubUserControl.cs):**
  - Displays All Notifications (Global) as the first item in the Notification type dropdown.
  - Editing and saving ALL updates 	bl_NotificationCategory and clears the cache for immediate live application.
- **Verification:** Both Debug and Release builds compiled cleanly with 0 errors / 0 warnings. PHP syntax validated. Graphify updated and published to D:\html\graphify.html.
