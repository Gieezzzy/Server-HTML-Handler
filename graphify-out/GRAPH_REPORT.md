# Graph Report - Server HTML Handler  (2026-10-24)

## Corpus Check
- 66 files · ~87,292 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 13 file(s) not represented in the graph (top: .resx 6, (none) 2, .config 2)

## Summary
- 1148 nodes · 2850 edges · 54 communities (27 shown, 27 thin omitted)
- Extraction: 96% EXTRACTED · 4% INFERRED · 0% AMBIGUOUS · INFERRED: 101 edges (avg confidence: 0.87)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `61f25db8`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- ShareControlService
- NotificationHubUserControl
- system
- ConsoleUserControl
- CallSignalingService
- CodeEditorUserControl
- Attribute
- ServerControlUserControl
- KonginReadingItem
- DeploymentStagingService
- ApacheWatchdogService
- DashboardPanel
- ServerControlForm
- Settings
- DatabaseCredentialsDialog
- SmbClientInfo
- CreateCi4ProjectDialog
- NotificationDispatcherService
- CallServerUserControl
- MaterialPalletQualityItem
- List
- ServerBackupHelper
- AppLoginStore
- ProjectViewUserControl
- LoginForm
- ICM Record: Server HTML Handler
- DashboardContents
- ConnectedClientInfo
- .Main
- ShareMonitorUserControl
- NtfsAclEntry
- RouteInfo
- SystemMetricsInfo
- Task
- README.md
- Server HTML Handler
- AclPermissionRule
- Helper
- .GetDirectorySize
- Label
- NotificationCategoryInfo
- FolderAclEntry
- AccessRequestInfo
- StockInventoryItem
- InboundSummaryItem
- ColorExpiryItem
- .dgvStorageFolders_CellContentClick
- Phase 120 - Notification hub, Apache watchdog, Folder Access, shared login (2026-10-24)
- ShareAclEntry

## God Nodes (most connected - your core abstractions)
1. `NotificationDispatcherService` - 86 edges
2. `ConsoleUserControl` - 85 edges
3. `ServerControlForm` - 79 edges
4. `NotificationHubUserControl` - 67 edges
5. `ShareMonitorUserControl` - 59 edges
6. `ShareControlService` - 52 edges
7. `DashboardContents` - 47 edges
8. `DashboardPanel` - 46 edges
9. `CodeEditorUserControl` - 43 edges
10. `ServerControlUserControl` - 37 edges

## Surprising Connections (you probably didn't know these)
- `Phase 120 - Notification hub, Apache watchdog, Folder Access, shared login (2026-10-24)` --references--> `ServerBackupHelper`  [INFERRED]
  _docs/ICM_CHANGELOG.md → Helper/Helper.cs
- `Phase 120 - Notification hub, Apache watchdog, Folder Access, shared login (2026-10-24)` --references--> `ShareControlService`  [INFERRED]
  _docs/ICM_CHANGELOG.md → Services/ShareControlService.cs
- `Phase 120 - Notification hub, Apache watchdog, Folder Access, shared login (2026-10-24)` --references--> `ShareMonitorUserControl`  [INFERRED]
  _docs/ICM_CHANGELOG.md → View/Contents/ShareMonitorUserControl.cs
- `KonginTrackerUserControl` --references--> `KonginTrackerService`  [EXTRACTED]
  View/Contents/KonginTrackerUserControl.cs → Services/KonginTrackerService.cs
- `NotificationHubUserControl` --references--> `StockInventoryItem`  [EXTRACTED]
  View/Contents/NotificationHubUserControl.Designer.cs → Services/NotificationDispatcherService.cs

## Import Cycles
- None detected.

## Communities (54 total, 27 thin omitted)

### Community 0 - "ShareControlService"
Cohesion: 0.13
Nodes (8): ShareControlService, DismissedPath, FolderPath, Instance, PendingRequests, PollSeconds, RequestLookbackMinutes, ShareName

### Community 2 - "system"
Cohesion: 0.08
Nodes (19): Server_HTML_Handler.Database, Server_HTML_Handler.View.Panel, Server_HTML_Handler.Routes, Server_HTML_Handler, Server_HTML_Handler.View.Dialogs, Server_HTML_Handler.Services, Server_HTML_Handler.Controller, Server_HTML_Handler.Models (+11 more)

### Community 4 - "CallSignalingService"
Cohesion: 0.07
Nodes (21): CallSignalingService.cs - map (current lines), CallSignalingService, ActiveUsers, IsRunning, Port, PendingSignal, QueuedPush, NotificationLogItem (+13 more)

### Community 6 - "Attribute"
Cohesion: 0.10
Nodes (19): System.Diagnostics.CodeAnalysis, AllowNullAttribute, DisallowNullAttribute, DoesNotReturnAttribute, DoesNotReturnIfAttribute, ParameterValue, MaybeNullAttribute, MaybeNullWhenAttribute (+11 more)

### Community 8 - "KonginReadingItem"
Cohesion: 0.07
Nodes (28): KonginConfigState, AlertIntervalMinutes, KonginReadingItem, AlertDescription, DeviceId, DisplayHum, DisplayTemp, DisplayTime (+20 more)

### Community 9 - "DeploymentStagingService"
Cohesion: 0.10
Nodes (11): dbBase, TestLabHelper, HostingPath, DbUserOverview, GlobalPrivileges, GrantPrivilege, HasPassword, HostName (+3 more)

### Community 10 - "ApacheWatchdogService"
Cohesion: 0.14
Nodes (9): ApacheWatchdogService, ApacheRoot, CheckIntervalSeconds, HealthUrl, HttpdExe, Instance, LastStatus, MaxRestartsPerHour (+1 more)

### Community 13 - "Settings"
Cohesion: 0.10
Nodes (16): Server_HTML_Handler.Properties, Resources, Culture, ResourceManager, Settings, ConenctionString, DbHost, DBmaster (+8 more)

### Community 15 - "SmbClientInfo"
Cohesion: 0.10
Nodes (19): ShareSnapshot, Clients, Opens, SmbClientInfo, ConnectedSeconds, ConnectedText, IdleSeconds, IdleText (+11 more)

### Community 16 - "CreateCi4ProjectDialog"
Cohesion: 0.17
Nodes (3): CreateCi4ProjectDialog, CreatedProjectPath, CreatedProjectUrl

### Community 17 - "NotificationDispatcherService"
Cohesion: 0.07
Nodes (23): NotificationDispatcherService, ColorExpiredGraceDays, ColorNoticeLeadDays, DarkBrightnessCutoff, DarkColorLifeMonths, HistoryFilePath, Instance, IsWatcherRunning (+15 more)

### Community 19 - "MaterialPalletQualityItem"
Cohesion: 0.07
Nodes (28): MaterialPalletQualityItem, BatchTrackingNo, C10, ChemicalsDisplay, Compowder, CreatedAt, D05, DisplayCreatedAt (+20 more)

### Community 22 - "AppLoginStore"
Cohesion: 0.18
Nodes (6): AppLoginStore, FilePath, RememberPath, Record, PasswordHash, User

### Community 23 - "ProjectViewUserControl"
Cohesion: 0.09
Nodes (8): ClientConnectedEventArgs, ClientId, IpAddress, IsRegistered, Port, ProjectViewUserControl, DynamicHostingPath, PromptDialog

### Community 25 - "ICM Record: Server HTML Handler"
Cohesion: 0.20
Nodes (8): Deployment notes, ICM Record: Server HTML Handler, Phase 102 - Notification categories (2026-10-22), Phase 108 - Signaling hardening for weak signal (2026-10-23), Phase 110 - LAN push fallback (2026-10-23), Message types, Rules, Server HTML Handler - Codebase Index

### Community 27 - "ConnectedClientInfo"
Cohesion: 0.22
Nodes (6): ConnectedClientInfo, ConnectedSince, Hostname, IP, Status, UserAgent

### Community 30 - "NtfsAclEntry"
Cohesion: 0.40
Nodes (5): NtfsAclEntry, Identity, Inherited, Rights, Type

### Community 31 - "RouteInfo"
Cohesion: 0.12
Nodes (14): RouteInfo, Category, DisplayName, PhysicalLocation, RequiresAdmin, RouteKey, RouteName, StatusAction (+6 more)

### Community 32 - "SystemMetricsInfo"
Cohesion: 0.25
Nodes (4): SystemMetricsInfo, CpuLoad, DbPoolStatus, RamUtil

### Community 39 - "AclPermissionRule"
Cohesion: 0.33
Nodes (4): AclPermissionRule, AccessLevel, AccessType, AccountName

### Community 43 - "NotificationCategoryInfo"
Cohesion: 0.40
Nodes (5): NotificationCategoryInfo, DefaultPriority, Description, Label, NotifType

### Community 44 - "FolderAclEntry"
Cohesion: 0.50
Nodes (4): FolderAclEntry, AccessLevel, AccessType, AccountName

### Community 45 - "AccessRequestInfo"
Cohesion: 0.16
Nodes (9): AccessRequestInfo, Count, Detail, Ip, Key, Kind, Target, Time (+1 more)

### Community 46 - "StockInventoryItem"
Cohesion: 0.12
Nodes (16): LowStockItem, StockInventoryItem, BaselineStock, Code, CurrentStock, DisplayBaseline, DisplayCurrent, DisplayPct (+8 more)

### Community 47 - "InboundSummaryItem"
Cohesion: 0.13
Nodes (15): InboundSummaryItem, BatchTrackingNo, Category, Code, ContainerNo, CreatedBy, DisplayDate, DisplayQty (+7 more)

### Community 48 - "ColorExpiryItem"
Cohesion: 0.14
Nodes (14): ColorExpiryItem, Basis, ColorCode, ColorName, DaysLeft, EnteredDate, ExpiryDate, FormulaId (+6 more)

### Community 53 - "ShareAclEntry"
Cohesion: 0.50
Nodes (4): ShareAclEntry, Account, Right, Type

## Knowledge Gaps
- **253 isolated node(s):** `User`, `PasswordHash`, `FilePath`, `RememberPath`, `ConnectionString` (+248 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 372 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **27 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `NotificationDispatcherService` connect `NotificationDispatcherService` to `Task`, `system`, `NotificationHubUserControl`, `CallSignalingService`, `ApacheWatchdogService`, `NotificationCategoryInfo`, `Label`, `StockInventoryItem`, `ColorExpiryItem`, `MaterialPalletQualityItem`, `List`?**
  _High betweenness centrality (0.144) - this node is a cross-community bridge._
- **Are the 3 inferred relationships involving `ConsoleUserControl` (e.g. with `.btnConsole_Click()` and `.btnDatabases_Click()`) actually correct?**
  _`ConsoleUserControl` has 3 INFERRED edges - model-reasoned connections that need verification._
- **What connects `User`, `PasswordHash`, `FilePath` to the rest of the system?**
  _253 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `ShareControlService` be split into smaller, more focused modules?**
  _Cohesion score 0.13333333333333333 - nodes in this community are weakly interconnected._
- **Why does `NotificationHubUserControl` connect `NotificationHubUserControl` to `Task`, `system`, `Label`, `DashboardPanel`, `StockInventoryItem`, `InboundSummaryItem`, `NotificationDispatcherService`, `CallServerUserControl`, `MaterialPalletQualityItem`, `List`, `LoginForm`, `DashboardContents`, `ShareMonitorUserControl`, `RouteInfo`?**
  _High betweenness centrality (0.120) - this node is a cross-community bridge._
- **Should `NotificationHubUserControl` be split into smaller, more focused modules?**
  _Cohesion score 0.11428571428571428 - nodes in this community are weakly interconnected._
- **Why does `ConsoleUserControl` connect `ConsoleUserControl` to `system`, `CallSignalingService`, `DeploymentStagingService`, `Label`, `DashboardPanel`, `CallServerUserControl`, `List`, `LoginForm`, `RouteInfo`?**
  _High betweenness centrality (0.112) - this node is a cross-community bridge._