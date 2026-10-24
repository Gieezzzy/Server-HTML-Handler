# Graph Report - Server HTML Handler  (2026-10-24)

## Corpus Check
- 62 files · ~80,153 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 13 file(s) not represented in the graph (top: .resx 6, (none) 2, .config 2)

## Summary
- 1070 nodes · 2610 edges · 45 communities (27 shown, 18 thin omitted)
- Extraction: 97% EXTRACTED · 3% INFERRED · 0% AMBIGUOUS · INFERRED: 73 edges (avg confidence: 0.86)
- Token cost: 0 input · 0 output

## Graph Freshness
- Built from commit: `a5f99017`
- Run `git rev-parse HEAD` and compare to check if the graph is stale.
- Run `graphify update .` after code changes (no API cost).

## Community Hubs (Navigation)
- Task
- NotificationHubUserControl
- system
- ConsoleUserControl
- CallSignalingService
- CodeEditorUserControl
- Attribute
- ServerControlUserControl
- EventArgs
- DeploymentStagingService
- ApacheWatchdogService
- DashboardPanel
- ServerControlForm
- Settings
- RouteInfo
- SmbClientInfo
- CreateCi4ProjectDialog
- NotificationDispatcherService
- CallServerUserControl
- MaterialPalletQualityItem
- ServerBackupHelper
- AppLoginStore
- NotificationLogItem
- InboundSummaryItem
- ColorExpiryItem
- DashboardContents
- ConnectedClientInfo
- .Main
- ClientConnectedEventArgs
- NtfsAclEntry
- SystemMetricsInfo
- .GetState
- README.md
- Server HTML Handler
- AclPermissionRule
- Helper
- .GetDirectorySize
- FolderSizeDetail
- NotificationCategoryInfo
- FolderAclEntry

## God Nodes (most connected - your core abstractions)
1. `ConsoleUserControl` - 85 edges
2. `NotificationDispatcherService` - 79 edges
3. `ServerControlForm` - 79 edges
4. `NotificationHubUserControl` - 64 edges
5. `ShareMonitorUserControl` - 58 edges
6. `ShareControlService` - 51 edges
7. `DashboardContents` - 47 edges
8. `DashboardPanel` - 45 edges
9. `CodeEditorUserControl` - 43 edges
10. `ServerControlUserControl` - 37 edges

## Surprising Connections (you probably didn't know these)
- `NotificationHubUserControl` --references--> `InboundSummaryItem`  [EXTRACTED]
  View/Contents/NotificationHubUserControl.Designer.cs → Services/NotificationDispatcherService.cs
- `NotificationHubUserControl` --references--> `MaterialPalletQualityItem`  [EXTRACTED]
  View/Contents/NotificationHubUserControl.Designer.cs → Services/NotificationDispatcherService.cs
- `NotificationHubUserControl` --references--> `NotificationDispatcherService`  [EXTRACTED]
  View/Contents/NotificationHubUserControl.Designer.cs → Services/NotificationDispatcherService.cs
- `ShareMonitorUserControl` --references--> `SmbClientInfo`  [EXTRACTED]
  View/Contents/ShareMonitorUserControl.cs → Services/ShareControlService.cs
- `RouteConfig` --references--> `RouteInfo`  [EXTRACTED]
  Routes/RouteConfig.cs → Model/RouteInfo.cs

## Import Cycles
- None detected.

## Communities (45 total, 18 thin omitted)

### Community 0 - "Task"
Cohesion: 0.07
Nodes (18): AccessRequestInfo, Count, Detail, Ip, Key, Kind, Target, Time (+10 more)

### Community 1 - "NotificationHubUserControl"
Cohesion: 0.06
Nodes (18): LowStockItem, StockInventoryItem, BaselineStock, Code, CurrentStock, DisplayBaseline, DisplayCurrent, DisplayPct (+10 more)

### Community 2 - "system"
Cohesion: 0.09
Nodes (14): Server_HTML_Handler.Database, Server_HTML_Handler.Routes, Server_HTML_Handler, Server_HTML_Handler.View.Dialogs, Server_HTML_Handler.Services, Server_HTML_Handler.Controller, Server_HTML_Handler.Models, Server_HTML_Handler.View (+6 more)

### Community 4 - "CallSignalingService"
Cohesion: 0.09
Nodes (16): Deployment notes, ICM Record: Server HTML Handler, Phase 102 - Notification categories (2026-10-22), Phase 108 - Signaling hardening for weak signal (2026-10-23), Phase 110 - LAN push fallback (2026-10-23), CallSignalingService.cs - map (current lines), Main files, Message types (+8 more)

### Community 6 - "Attribute"
Cohesion: 0.10
Nodes (19): System.Diagnostics.CodeAnalysis, AllowNullAttribute, DisallowNullAttribute, DoesNotReturnAttribute, DoesNotReturnIfAttribute, ParameterValue, MaybeNullAttribute, MaybeNullWhenAttribute (+11 more)

### Community 8 - "EventArgs"
Cohesion: 0.10
Nodes (3): ProjectViewUserControl, DynamicHostingPath, PromptDialog

### Community 9 - "DeploymentStagingService"
Cohesion: 0.09
Nodes (12): dbBase, AesEncryptionHelper, TestLabHelper, HostingPath, DbUserOverview, GlobalPrivileges, GrantPrivilege, HasPassword (+4 more)

### Community 10 - "ApacheWatchdogService"
Cohesion: 0.14
Nodes (9): ApacheWatchdogService, ApacheRoot, CheckIntervalSeconds, HealthUrl, HttpdExe, Instance, LastStatus, MaxRestartsPerHour (+1 more)

### Community 12 - "ServerControlForm"
Cohesion: 0.05
Nodes (5): ControllerService, ServerControlForm, NetworkItem, DisplayName, Path

### Community 13 - "Settings"
Cohesion: 0.10
Nodes (16): Server_HTML_Handler.Properties, Resources, Culture, ResourceManager, Settings, ConenctionString, DbHost, DBmaster (+8 more)

### Community 14 - "RouteInfo"
Cohesion: 0.12
Nodes (14): RouteInfo, Category, DisplayName, PhysicalLocation, RequiresAdmin, RouteKey, RouteName, StatusAction (+6 more)

### Community 15 - "SmbClientInfo"
Cohesion: 0.11
Nodes (19): ShareSnapshot, Clients, Opens, SmbClientInfo, ConnectedSeconds, ConnectedText, IdleSeconds, IdleText (+11 more)

### Community 16 - "CreateCi4ProjectDialog"
Cohesion: 0.07
Nodes (6): Form1, CreateCi4ProjectDialog, CreatedProjectPath, CreatedProjectUrl, DatabaseCredentialsDialog, LoginForm

### Community 17 - "NotificationDispatcherService"
Cohesion: 0.07
Nodes (22): NotificationDispatcherService, ColorExpiredGraceDays, ColorNoticeLeadDays, DarkBrightnessCutoff, DarkColorLifeMonths, Instance, IsWatcherRunning, LightColorLifeMonths (+14 more)

### Community 19 - "MaterialPalletQualityItem"
Cohesion: 0.07
Nodes (28): MaterialPalletQualityItem, BatchTrackingNo, C10, ChemicalsDisplay, Compowder, CreatedAt, D05, DisplayCreatedAt (+20 more)

### Community 22 - "AppLoginStore"
Cohesion: 0.27
Nodes (5): AppLoginStore, FilePath, Record, PasswordHash, User

### Community 23 - "NotificationLogItem"
Cohesion: 0.13
Nodes (14): NotificationLogItem, Category, DeliveryStatus, Details, DisplayTime, FcmSent, Id, ItemCode (+6 more)

### Community 24 - "InboundSummaryItem"
Cohesion: 0.13
Nodes (15): InboundSummaryItem, BatchTrackingNo, Category, Code, ContainerNo, CreatedBy, DisplayDate, DisplayQty (+7 more)

### Community 25 - "ColorExpiryItem"
Cohesion: 0.14
Nodes (14): ColorExpiryItem, Basis, ColorCode, ColorName, DaysLeft, EnteredDate, ExpiryDate, FormulaId (+6 more)

### Community 27 - "ConnectedClientInfo"
Cohesion: 0.22
Nodes (6): ConnectedClientInfo, ConnectedSince, Hostname, IP, Status, UserAgent

### Community 29 - "ClientConnectedEventArgs"
Cohesion: 0.40
Nodes (5): ClientConnectedEventArgs, ClientId, IpAddress, IsRegistered, Port

### Community 30 - "NtfsAclEntry"
Cohesion: 0.40
Nodes (5): NtfsAclEntry, Identity, Inherited, Rights, Type

### Community 32 - "SystemMetricsInfo"
Cohesion: 0.25
Nodes (4): SystemMetricsInfo, CpuLoad, DbPoolStatus, RamUtil

### Community 39 - "AclPermissionRule"
Cohesion: 0.33
Nodes (4): AclPermissionRule, AccessLevel, AccessType, AccountName

### Community 42 - "FolderSizeDetail"
Cohesion: 0.40
Nodes (5): FolderSizeDetail, FolderName, FormattedSize, FullPath, SizeBytes

### Community 43 - "NotificationCategoryInfo"
Cohesion: 0.40
Nodes (5): NotificationCategoryInfo, DefaultPriority, Description, Label, NotifType

### Community 44 - "FolderAclEntry"
Cohesion: 0.50
Nodes (4): FolderAclEntry, AccessLevel, AccessType, AccountName

## Knowledge Gaps
- **226 isolated node(s):** `User`, `PasswordHash`, `FilePath`, `ConnectionString`, `IP` (+221 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 343 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **18 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **Why does `NotificationHubUserControl` connect `NotificationHubUserControl` to `Task`, `system`, `ServerControlUserControl`, `DashboardPanel`, `RouteInfo`, `CreateCi4ProjectDialog`, `NotificationDispatcherService`, `MaterialPalletQualityItem`, `List`, `InboundSummaryItem`, `DashboardContents`?**
  _High betweenness centrality (0.136) - this node is a cross-community bridge._
- **Are the 3 inferred relationships involving `ConsoleUserControl` (e.g. with `.btnConsole_Click()` and `.btnDatabases_Click()`) actually correct?**
  _`ConsoleUserControl` has 3 INFERRED edges - model-reasoned connections that need verification._
- **What connects `User`, `PasswordHash`, `FilePath` to the rest of the system?**
  _226 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `Task` be split into smaller, more focused modules?**
  _Cohesion score 0.07087628865979381 - nodes in this community are weakly interconnected._
- **Why does `ConsoleUserControl` connect `ConsoleUserControl` to `system`, `CallSignalingService`, `ServerControlUserControl`, `DeploymentStagingService`, `DashboardPanel`, `RouteInfo`, `CreateCi4ProjectDialog`, `CallServerUserControl`, `List`, `DateTime`?**
  _High betweenness centrality (0.112) - this node is a cross-community bridge._
- **Should `NotificationHubUserControl` be split into smaller, more focused modules?**
  _Cohesion score 0.056265984654731455 - nodes in this community are weakly interconnected._
- **Why does `ShareMonitorUserControl` connect `Task` to `NotificationHubUserControl`, `system`, `ServerControlUserControl`, `DeploymentStagingService`, `DashboardPanel`, `RouteInfo`, `SmbClientInfo`, `CreateCi4ProjectDialog`, `CallServerUserControl`, `List`, `DashboardContents`?**
  _High betweenness centrality (0.103) - this node is a cross-community bridge._