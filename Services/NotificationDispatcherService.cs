using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server_HTML_Handler.Services
{
    public class NotificationLogItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Category { get; set; }
        public string Priority { get; set; }
        public string Title { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Details { get; set; }
        public bool SynologySent { get; set; }
        public bool FcmSent { get; set; }
        public bool WebSocketSent { get; set; }
        public string DeliveryStatus { get; set; }
        public string TargetSystem { get; set; } = "RMS";
        public string DisplayTime => Timestamp.ToString("HH:mm:ss");
    }

    public class StockInventoryItem
    {
        public string Type { get; set; } // Material Stock, Pigment Stock, Chemical Stock
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal CurrentStock { get; set; }
        public decimal BaselineStock { get; set; }
        public decimal SafetyStock { get; set; }
        public double Percentage { get; set; }
        public string Unit { get; set; }
        public bool IsLowStock => Percentage <= 30.0;
        public string Status
        {
            get
            {
                if (Percentage <= 15.0) return "CRITICAL (<=15%)";
                if (Percentage <= 30.0) return "LOW (<=30%)";
                if (Percentage <= 40.0) return "WARNING (<=40%)";
                return "NORMAL";
            }
        }
        public string DisplayCurrent => $"{CurrentStock:N2} {Unit}";
        public string DisplayBaseline => $"{BaselineStock:N2} {Unit}";
        public string DisplaySafety => SafetyStock > 0 ? $"{SafetyStock:N2} {Unit}" : $"{(BaselineStock * 0.3m):N2} {Unit}";
        public string DisplayPct => $"{Percentage:F1}%";
    }

    public class LowStockItem : StockInventoryItem
    {
    }

    public class InboundSummaryItem
    {
        public int InboundId { get; set; }
        public string Type { get; set; } // Material, Pigment, Chemical
        public DateTime? InboundDate { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal Quantity { get; set; }
        public string Unit { get; set; }
        public string BatchTrackingNo { get; set; }
        public string ContainerNo { get; set; }
        public string CreatedBy { get; set; }
        public string FifoNotice { get; set; } = "FIFO REQUIRED";

        public string DisplayDate => InboundDate.HasValue ? InboundDate.Value.ToString("yyyy-MM-dd HH:mm") : "N/A";
        public string DisplayQty => $"{Quantity:N2} {Unit}";
    }

    public class MaterialPalletQualityItem
    {
        public int PalletId { get; set; }
        public string BatchTrackingNo { get; set; }
        public string MaterialCode { get; set; }
        public string MaterialName { get; set; }
        public decimal? TS2 { get; set; }
        public decimal? TC90 { get; set; }
        public string E02 { get; set; }
        public string E09 { get; set; }
        public string D05 { get; set; }
        public string E09_1 { get; set; }
        public string C10 { get; set; }
        public string E06 { get; set; }
        public string Compowder { get; set; }
        public string E01 { get; set; }
        public DateTime? CreatedAt { get; set; }
        public double HoursLapsed => CreatedAt.HasValue ? (DateTime.Now - CreatedAt.Value).TotalHours : 0;

        public bool HasTs2 => TS2.HasValue && TS2.Value > 0;
        public bool HasTc90 => TC90.HasValue && TC90.Value > 0;

        public bool HasAnyChemical
        {
            get
            {
                return !string.IsNullOrWhiteSpace(E02) ||
                       !string.IsNullOrWhiteSpace(E09) ||
                       !string.IsNullOrWhiteSpace(D05) ||
                       !string.IsNullOrWhiteSpace(E09_1) ||
                       !string.IsNullOrWhiteSpace(C10) ||
                       !string.IsNullOrWhiteSpace(E06) ||
                       !string.IsNullOrWhiteSpace(Compowder) ||
                       !string.IsNullOrWhiteSpace(E01);
            }
        }

        public string ChemicalsDisplay
        {
            get
            {
                var list = new List<string>();
                if (!string.IsNullOrWhiteSpace(E02)) list.Add($"E02:{E02.Trim()}");
                if (!string.IsNullOrWhiteSpace(E09)) list.Add($"E09:{E09.Trim()}");
                if (!string.IsNullOrWhiteSpace(D05)) list.Add($"D05:{D05.Trim()}");
                if (!string.IsNullOrWhiteSpace(E09_1)) list.Add($"E09_1:{E09_1.Trim()}");
                if (!string.IsNullOrWhiteSpace(C10)) list.Add($"C10:{C10.Trim()}");
                if (!string.IsNullOrWhiteSpace(E06)) list.Add($"E06:{E06.Trim()}");
                if (!string.IsNullOrWhiteSpace(Compowder)) list.Add($"Comp:{Compowder.Trim()}");
                if (!string.IsNullOrWhiteSpace(E01)) list.Add($"E01:{E01.Trim()}");

                return list.Count > 0 ? string.Join(", ", list) : "EMPTY";
            }
        }

        public bool Is48HrLapsed => HoursLapsed >= 48.0 && (!HasTs2 || !HasTc90 || !HasAnyChemical);

        public string TestingStatus
        {
            get
            {
                if (HasTs2 && HasTc90 && HasAnyChemical) return "COMPLETE";
                if (HoursLapsed >= 48.0) return "OVERDUE (48h+ LAPSED)";
                return $"PENDING ({(Math.Max(0, 48.0 - HoursLapsed)):F0}h left)";
            }
        }

        public string MissingSummary
        {
            get
            {
                var missing = new List<string>();
                if (!HasTs2) missing.Add("TS2");
                if (!HasTc90) missing.Add("TC90");
                if (!HasAnyChemical) missing.Add("Chemicals");
                return missing.Count > 0 ? string.Join(", ", missing) : "None (Complete)";
            }
        }

        public string DisplayTs2 => HasTs2 ? $"{TS2.Value:N2}" : "EMPTY";
        public string DisplayTc90 => HasTc90 ? $"{TC90.Value:N2}" : "EMPTY";
        public string DisplayCreatedAt => CreatedAt.HasValue ? CreatedAt.Value.ToString("yyyy-MM-dd HH:mm") : "N/A";
        public string DisplayHoursLapsed => $"{HoursLapsed:F1}h";
    }

    /// <summary>An approved colour of the RDMS colour library (dbRubber.tbl_NewColorFormulaHeader) that is about to expire or just expired.</summary>
    public class ColorExpiryItem
    {
        public int FormulaId { get; set; }
        public string ColorCode { get; set; }
        public string ColorName { get; set; }
        public string Hex { get; set; }
        public bool IsDark { get; set; }
        /// <summary>How the dark/light decision was made: the Hex, the colour name in the palette, or an assumption.</summary>
        public string Basis { get; set; }
        public DateTime EnteredDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int LifeMonths { get; set; }

        public int DaysLeft => (int)Math.Floor((ExpiryDate.Date - DateTime.Today).TotalDays);
        public bool IsExpired => DaysLeft < 0;
        public string Tone => IsDark ? "dark" : "light";
        public string WhenText => IsExpired ? $"EXPIRED {-DaysLeft} day(s) ago" : (DaysLeft == 0 ? "expires TODAY" : $"in {DaysLeft} day(s)");
    }

    public class NotificationDispatcherService
    {
        private static readonly Lazy<NotificationDispatcherService> _instance =
            new Lazy<NotificationDispatcherService>(() => new NotificationDispatcherService());

        public static NotificationDispatcherService Instance => _instance.Value;

        // Settings / Connection defaults
        public string SqlConnectionString { get; set; } =
            @"Server=192.168.32.45\sqlexpress;Database=dbRubber;User Id=processaudit_user;Password=processaudit1234*;TrustServerCertificate=True;Connect Timeout=5;";

        // Messaging database that holds tbl_NotificationCategory (who receives which notification type).
        public string RmsConnectionString { get; set; } =
            @"Server=192.168.32.45\sqlexpress;Database=dbRMSAndroid;User Id=processaudit_user;Password=processaudit1234*;TrustServerCertificate=True;Connect Timeout=5;";

        public string NotificationPhpUrl { get; set; } = "http://192.168.32.43/send-notification.php";

        public string SynologyBotUrlRms { get; set; } =
            "https://192.168.32.130:5001/webapi/entry.cgi?api=SYNO.Chat.External&method=chatbot&version=2&token=%220GmgVlE4ZZ9h9uCoGt7zSlszyCNXKtLH1bJnFs2RN9XbaN9HwHK6rnsUn5AHtNQo%22";

        public string SynologyBotUrlNexus { get; set; } =
            "https://192.168.32.130:5001/webapi/entry.cgi?api=SYNO.Chat.External&method=chatbot&version=2&token=%22cEiE4qHlTZI8GSboOTS9LOTcYJGq4bgZaAC51vd4lUUXedHUobUAV8nZa3qMPfFn%22";

        public string SynologyBotUrl
        {
            get => SynologyBotUrlRms;
            set => SynologyBotUrlRms = value;
        }

        public double StockThresholdPct { get; set; } = 30.0;
        public int PollIntervalSeconds { get; set; } = 15;

        // ---- Delivery schedule (anti-flood) ----
        // Automatic notifications are only delivered between SendWindowStart and SendWindowEnd (local time) and at most once
        // per category per 24-hour cycle. A cycle starts at SendWindowStart (07:00): anything detected outside the window waits
        // for 07:00. Each category is sent as ONE digest, not one message per item.
        public TimeSpan SendWindowStart { get; set; } = new TimeSpan(7, 0, 0);
        public TimeSpan SendWindowEnd { get; set; } = new TimeSpan(17, 0, 0);
        public int MaxItemsPerDigest { get; set; } = 12;
        public int RetryMinutesAfterFailure { get; set; } = 10;
        public int MaxAttemptsPerCycle { get; set; } = 3;

        // ---- Colour expiration rules ----
        // A library colour is valid from its EnteredDate for DarkColorLifeMonths (dark colours) or LightColorLifeMonths (light colours).
        // Light or dark comes from the colour's Hex; when Hex is empty, from the colour name in the palette (e.g. "maroon" = dark).
        public int DarkColorLifeMonths { get; set; } = 6;
        public int LightColorLifeMonths { get; set; } = 2;
        /// <summary>A colour is announced this many days before it expires...</summary>
        public int ColorNoticeLeadDays { get; set; } = 14;
        /// <summary>...and keeps being listed (once a day) until it has been expired this many days. Older ones are only counted.</summary>
        public int ColorExpiredGraceDays { get; set; } = 30;
        /// <summary>Neither Hex nor a known colour name: true = treat as dark (6 months), false = as light (2 months, the cautious choice).</summary>
        public bool UnknownColorTreatedAsDark { get; set; } = false;
        /// <summary>Perceived brightness (0 = black, 1 = white) below this is a dark colour.</summary>
        public double DarkBrightnessCutoff { get; set; } = 0.5;

        /// <summary>
        /// Earliest time of day (inside the send window) at which each digest of a type may go out. The number of entries is
        /// how many digests that type may send per 24h cycle. Types not listed send once per cycle.
        /// Low stock: two per day, from 07:00 and from 13:00.
        /// </summary>
        public Dictionary<string, TimeSpan[]> SendSlots { get; } = new Dictionary<string, TimeSpan[]>(StringComparer.OrdinalIgnoreCase)
        {
            ["LOW_STOCK"] = new[] { new TimeSpan(7, 0, 0), new TimeSpan(13, 0, 0) },
            ["MATERIAL_INBOUND"] = new[] { new TimeSpan(9, 30, 0), new TimeSpan(14, 30, 0) },
            ["PIGMENT_INBOUND"] = new[] { new TimeSpan(9, 30, 0), new TimeSpan(14, 30, 0) },
            ["CHEMICAL_INBOUND"] = new[] { new TimeSpan(9, 30, 0), new TimeSpan(14, 30, 0) },
            ["COLOR_EXPIRY"] = new[] { new TimeSpan(10, 0, 0), new TimeSpan(15, 0, 0) },
        };
        public bool IsWatcherRunning { get; private set; } = false;

        // Event delegates
        public event Action<NotificationLogItem> OnNotificationDispatched;
        public event Action<string> OnLog;
        public event Action<bool> OnWatcherStateChanged;
        public event Action<List<StockInventoryItem>> OnLowStockUpdated;
        public event Action<List<StockInventoryItem>> OnStockInventoryUpdated;
        public event Action<List<MaterialPalletQualityItem>> OnPalletQualityUpdated;

        // In-memory data store
        private readonly List<NotificationLogItem> _notificationHistory = new List<NotificationLogItem>();
        // Latest scan results; the scheduler turns them into at most one digest per category per cycle.
        private readonly object _scheduleLock = new object();
        private List<StockInventoryItem> _currentLowStock = new List<StockInventoryItem>();
        private List<MaterialPalletQualityItem> _currentOverdue = new List<MaterialPalletQualityItem>();
        private List<ColorExpiryItem> _currentColorExpiry = new List<ColorExpiryItem>();
        private int _colorOlderExpired;
        private DateTime _lastColorScan = DateTime.MinValue;
        private NotificationScheduleState _state;
        private readonly Dictionary<string, DateTime> _nextTryAfter = new Dictionary<string, DateTime>();
        private readonly Dictionary<string, int> _attempts = new Dictionary<string, int>();

        // Watermark IDs for Inbound tables
        private int _lastMaterialInboundId = -1;
        private int _lastPigmentInboundId = -1;
        private int _lastChemicalInboundId = -1;

        private System.Threading.Timer _timer;
        private bool _isScanning = false;
        private DateTime _lastStockCheckTime = DateTime.MinValue;

        private NotificationDispatcherService()
        {
            // Allow self-signed Synology HTTPS certificates
            ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            LoadHistory();
        }

        // What was sent survives a restart, so the Hub still shows the notification that went out when the application started.
        private static string HistoryFilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ServerHTMLHandler", "notification-history.json");

        private readonly List<string> _logLines = new List<string>();

        private void LoadHistory()
        {
            try
            {
                if (!File.Exists(HistoryFilePath)) return;
                var saved = JsonConvert.DeserializeObject<List<NotificationLogItem>>(File.ReadAllText(HistoryFilePath));
                if (saved == null) return;
                lock (_notificationHistory) { _notificationHistory.AddRange(saved.OrderByDescending(x => x.Timestamp).Take(200)); }
            }
            catch { /* unreadable history: start with an empty feed */ }
        }

        private void SaveHistory()
        {
            try
            {
                List<NotificationLogItem> copy;
                lock (_notificationHistory) { copy = _notificationHistory.ToList(); }
                Directory.CreateDirectory(Path.GetDirectoryName(HistoryFilePath));
                File.WriteAllText(HistoryFilePath, JsonConvert.SerializeObject(copy, Formatting.Indented));
            }
            catch { /* history is a convenience: never let it break a notification */ }
        }

        /// <summary>The service's own log lines (scheduler decisions, delivery results), oldest first.</summary>
        public IReadOnlyList<string> GetLogLines()
        {
            lock (_logLines) { return _logLines.ToList(); }
        }

        public IReadOnlyList<NotificationLogItem> GetHistory()
        {
            lock (_notificationHistory)
            {
                return _notificationHistory.ToList();
            }
        }

        public void ClearHistory()
        {
            lock (_notificationHistory)
            {
                _notificationHistory.Clear();
            }
            SaveHistory();
        }

        public void StartWatcher()
        {
            if (IsWatcherRunning) return;

            IsWatcherRunning = true;
            Log("Real-Time Notification Watcher started.");
            Raise(OnWatcherStateChanged, true);

            // Initialize watermarks asynchronously
            Task.Run(async () =>
            {
                await InitializeInboundWatermarksAsync();
                await ScanInboundsAsync();
                await CheckLowStockLevelsAsync();
                await ScanLapsedPalletQualityAsync();
                await ScanColorExpiryAsync();
            });

            _timer = new System.Threading.Timer(TimerCallback, null, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(Math.Max(5, PollIntervalSeconds)));
        }

        public void StopWatcher()
        {
            if (!IsWatcherRunning) return;

            IsWatcherRunning = false;
            _timer?.Dispose();
            _timer = null;
            Log("Real-Time Notification Watcher stopped.");
            Raise(OnWatcherStateChanged, false);
        }

        private async void TimerCallback(object state)
        {
            if (!IsWatcherRunning || _isScanning) return;
            _isScanning = true;

            try
            {
                await ScanInboundsAsync();
                await ScanLapsedPalletQualityAsync();

                // Check low stock levels periodically (e.g. every 60 seconds)
                if ((DateTime.Now - _lastStockCheckTime).TotalSeconds >= 60)
                {
                    _lastStockCheckTime = DateTime.Now;
                    await CheckLowStockLevelsAsync();
                }

                // The colour library changes rarely: re-read it every 30 minutes.
                if ((DateTime.Now - _lastColorScan).TotalMinutes >= 30)
                {
                    await ScanColorExpiryAsync();
                }

                // Check Kongin environment readings
                await KonginTrackerService.Instance.CheckLatestReadingAndAlertAsync();

                await ProcessScheduleAsync();
            }
            catch (Exception ex)
            {
                Log($"Watcher poll note: {ex.Message}");
            }
            finally
            {
                _isScanning = false;
            }
        }

        private async Task InitializeInboundWatermarksAsync()
        {
            try
            {
                var st = GetState();
                if (st.WmMaterial >= 0 && st.WmPigment >= 0 && st.WmChemical >= 0)
                {
                    // Resume where we stopped so inbounds received while the handler was off are still announced.
                    _lastMaterialInboundId = st.WmMaterial;
                    _lastPigmentInboundId = st.WmPigment;
                    _lastChemicalInboundId = st.WmChemical;
                    Log($"Inbound watermarks restored -> Material: {_lastMaterialInboundId}, Pigment: {_lastPigmentInboundId}, Chemical: {_lastChemicalInboundId}");
                    return;
                }

                using (var conn = new SqlConnection(SqlConnectionString))
                {
                    await conn.OpenAsync();

                    using (var cmd = new SqlCommand("SELECT ISNULL(MAX(MaterialInboundID), 0) FROM dbo.tbl_log_MaterialInbound WITH (NOLOCK)", conn))
                    {
                        var res = await cmd.ExecuteScalarAsync();
                        _lastMaterialInboundId = Convert.ToInt32(res);
                    }

                    using (var cmd = new SqlCommand("SELECT ISNULL(MAX(PigmentInboundID), 0) FROM dbo.tbl_log_PigmentInbound WITH (NOLOCK)", conn))
                    {
                        var res = await cmd.ExecuteScalarAsync();
                        _lastPigmentInboundId = Convert.ToInt32(res);
                    }

                    using (var cmd = new SqlCommand("SELECT ISNULL(MAX(ChemicalInboundID), 0) FROM dbo.tbl_log_ChemicalInbound WITH (NOLOCK)", conn))
                    {
                        var res = await cmd.ExecuteScalarAsync();
                        _lastChemicalInboundId = Convert.ToInt32(res);
                    }

                    Log($"Inbound watermarks initialized -> Material: {_lastMaterialInboundId}, Pigment: {_lastPigmentInboundId}, Chemical: {_lastChemicalInboundId}");
                    PersistWatermarks();
                }
            }
            catch (Exception ex)
            {
                Log($"Failed to initialize inbounds watermark: {ex.Message}");
            }
        }

        public async Task TriggerManualScanAsync()
        {
            if (_lastMaterialInboundId < 0)
            {
                await InitializeInboundWatermarksAsync();
            }
            await ScanInboundsAsync();
            await CheckLowStockLevelsAsync();
            await ScanLapsedPalletQualityAsync();
            await ScanColorExpiryAsync();
            await KonginTrackerService.Instance.CheckLatestReadingAndAlertAsync();
            await ProcessScheduleAsync(); // still honours the send window and the once-per-cycle rule
        }

        #region Inbound Watcher Logic

        private async Task ScanInboundsAsync()
        {
            if (_lastMaterialInboundId < 0) return;

            try
            {
                using (var conn = new SqlConnection(SqlConnectionString))
                {
                    await conn.OpenAsync();

                    // 1. Scan tbl_log_MaterialInbound
                    string sqlMat = @"
                        SELECT MaterialInboundID, BatchTrackingNo, MaterialInboundDate, MaterialType, StockType, 
                               MaterialCode, MaterialName, Category, MaterialQty, ContainerNo, CreatedBy
                        FROM dbo.tbl_log_MaterialInbound WITH (NOLOCK)
                        WHERE MaterialInboundID > @lastId
                        ORDER BY MaterialInboundID ASC";

                    using (var cmd = new SqlCommand(sqlMat, conn))
                    {
                        cmd.Parameters.AddWithValue("@lastId", _lastMaterialInboundId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int inbId = reader.GetInt32(0);
                                string batch = reader.IsDBNull(1) ? "N/A" : reader.GetString(1);
                                DateTime? inbDate = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2);
                                string matType = reader.IsDBNull(3) ? (reader.IsDBNull(4) ? "Rubber" : reader.GetString(4)) : reader.GetString(3);
                                string code = reader.IsDBNull(5) ? "N/A" : reader.GetString(5);
                                string name = reader.IsDBNull(6) ? code : reader.GetString(6);
                                string cat = reader.IsDBNull(7) ? "Standard" : reader.GetString(7);
                                decimal qty = reader.IsDBNull(8) ? 0 : reader.GetDecimal(8);
                                string container = reader.IsDBNull(9) ? "N/A" : reader.GetString(9);
                                string createdBy = reader.IsDBNull(10) ? "System" : reader.GetString(10);

                                if (inbId > _lastMaterialInboundId) _lastMaterialInboundId = inbId;

                                string dateStr = inbDate.HasValue ? inbDate.Value.ToString("yyyy-MM-dd HH:mm") : DateTime.Now.ToString("yyyy-MM-dd HH:mm");

                                QueueInbound("MATERIAL_INBOUND", $"\u2022 {dateStr} | {code} {name} ({matType}/{cat}) | {qty:N2} KG/KL | Batch {batch} | Container {container} | by {createdBy}");
                            }
                        }
                    }

                    // 2. Scan tbl_log_PigmentInbound
                    string sqlPig = @"
                        SELECT PigmentInboundID, BatchTrackingNo, fld_PNInboundDate, fld_PigmentName, 
                               fld_PNConcentration, fld_PNQty, ContainerNo, CreatedBy, fld_PigmentCode
                        FROM dbo.tbl_log_PigmentInbound WITH (NOLOCK)
                        WHERE PigmentInboundID > @lastId
                        ORDER BY PigmentInboundID ASC";

                    using (var cmd = new SqlCommand(sqlPig, conn))
                    {
                        cmd.Parameters.AddWithValue("@lastId", _lastPigmentInboundId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int inbId = reader.GetInt32(0);
                                string batch = reader.IsDBNull(1) ? "N/A" : reader.GetString(1);
                                DateTime? inbDate = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2);
                                string name = reader.IsDBNull(3) ? "Pigment" : reader.GetString(3);
                                string conc = reader.IsDBNull(4) ? "100%" : reader.GetString(4);
                                decimal qty = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5);
                                string container = reader.IsDBNull(6) ? "N/A" : reader.GetString(6);
                                string createdBy = reader.IsDBNull(7) ? "System" : reader.GetString(7);
                                string code = reader.IsDBNull(8) ? name : reader.GetString(8);

                                if (inbId > _lastPigmentInboundId) _lastPigmentInboundId = inbId;

                                string dateStr = inbDate.HasValue ? inbDate.Value.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd");

                                QueueInbound("PIGMENT_INBOUND", $"\u2022 {dateStr} | {code} {name} ({conc}) | {qty:N2} g/KG | Batch {batch} | Container {container} | by {createdBy}");
                            }
                        }
                    }

                    // 3. Scan tbl_log_ChemicalInbound
                    string sqlChem = @"
                        SELECT ChemicalInboundID, BatchTrackingNo, fld_ChemInboundDate, fld_ChemCode, 
                               fld_ChemName, fld_ChemQty, ContainerNo, CreatedBy
                        FROM dbo.tbl_log_ChemicalInbound WITH (NOLOCK)
                        WHERE ChemicalInboundID > @lastId
                        ORDER BY ChemicalInboundID ASC";

                    using (var cmd = new SqlCommand(sqlChem, conn))
                    {
                        cmd.Parameters.AddWithValue("@lastId", _lastChemicalInboundId);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                int inbId = reader.GetInt32(0);
                                string batch = reader.IsDBNull(1) ? "N/A" : reader.GetString(1);
                                DateTime? inbDate = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2);
                                string code = reader.IsDBNull(3) ? "N/A" : reader.GetString(3);
                                string name = reader.IsDBNull(4) ? code : reader.GetString(4);
                                decimal qty = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5);
                                string container = reader.IsDBNull(6) ? "N/A" : reader.GetString(6);
                                string createdBy = reader.IsDBNull(7) ? "System" : reader.GetString(7);

                                if (inbId > _lastChemicalInboundId) _lastChemicalInboundId = inbId;

                                string dateStr = inbDate.HasValue ? inbDate.Value.ToString("yyyy-MM-dd") : DateTime.Now.ToString("yyyy-MM-dd");

                                QueueInbound("CHEMICAL_INBOUND", $"\u2022 {dateStr} | {code} {name} | {qty:N2} KG | Batch {batch} | Container {container} | by {createdBy}");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"ScanInbounds Error: {ex.Message}");
            }
            finally
            {
                PersistWatermarks();
            }
        }

        #endregion

        #region Stock Inventory & Low Stock (<= 30%) Monitoring Logic

        public async Task<List<StockInventoryItem>> GetAllStockInventoryAsync()
        {
            var list = new List<StockInventoryItem>();

            try
            {
                using (var conn = new SqlConnection(SqlConnectionString))
                {
                    await conn.OpenAsync();

                    // A. Material / Rubber Stock (tbl_log_MaterialMasterStock)
                    string sqlMatStock = @"
                        SELECT MaterialCode, MaterialName, CurrentStocks, 
                               COALESCE(NULLIF(CurrentBaselineQty, 0), NULLIF(TotalStock, 0), CurrentStocks, 1000.0) AS Baseline,
                               ISNULL(StockSafetyKg, 0) AS SafetyStock
                        FROM dbo.tbl_log_MaterialMasterStock WITH (NOLOCK)
                        WHERE CurrentStocks IS NOT NULL";

                    using (var cmd = new SqlCommand(sqlMatStock, conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string code = reader.IsDBNull(0) ? "" : reader.GetString(0).Trim();
                            string name = reader.IsDBNull(1) ? code : reader.GetString(1).Trim();
                            decimal cur = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2);
                            decimal baseStock = reader.IsDBNull(3) ? 1000 : reader.GetDecimal(3);
                            decimal safety = reader.IsDBNull(4) ? 0 : Convert.ToDecimal(reader.GetValue(4));

                            if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(name)) continue;
                            if (baseStock <= 0) baseStock = Math.Max(cur, 1000m);

                            double pct = (double)(cur / baseStock) * 100.0;
                            list.Add(new StockInventoryItem
                            {
                                Type = "Material Stock",
                                Code = code,
                                Name = name,
                                CurrentStock = cur,
                                BaselineStock = baseStock,
                                SafetyStock = safety > 0 ? safety : Math.Round(baseStock * 0.3m, 2),
                                Percentage = pct,
                                Unit = "KG"
                            });
                        }
                    }

                    // B. Pigment Stock (tbl_log_PigmentMasterStock)
                    string sqlPigStock = @"
                        SELECT PigmentCode, PigmentName, CurrentStock, 
                               COALESCE(NULLIF(TotalStock, 0), CurrentStock, 1000.0) AS TotalStock,
                               ISNULL(SafetyStock, 0) AS SafetyStock
                        FROM dbo.tbl_log_PigmentMasterStock WITH (NOLOCK)
                        WHERE CurrentStock IS NOT NULL";

                    using (var cmd = new SqlCommand(sqlPigStock, conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string code = reader.IsDBNull(0) ? "" : reader.GetString(0).Trim();
                            string name = reader.IsDBNull(1) ? code : reader.GetString(1).Trim();
                            decimal cur = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2);
                            decimal tot = reader.IsDBNull(3) ? 1000 : reader.GetDecimal(3);
                            decimal safety = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4);

                            if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(name)) continue;
                            if (tot <= 0) tot = Math.Max(cur, 1000m);

                            double pct = (double)(cur / tot) * 100.0;
                            list.Add(new StockInventoryItem
                            {
                                Type = "Pigment Stock",
                                Code = code,
                                Name = name,
                                CurrentStock = cur,
                                BaselineStock = tot,
                                SafetyStock = safety > 0 ? safety : Math.Round(tot * 0.3m, 2),
                                Percentage = pct,
                                Unit = "g"
                            });
                        }
                    }

                    // C. Chemical Stock (tbl_log_ChemicalMasterStock)
                    string sqlChemStock = @"
                        SELECT ChemicalCode, ChemicalName, CurrentStock, 
                               COALESCE(NULLIF(TotalStock, 0), CurrentStock, 500.0) AS TotalStock,
                               ISNULL(SafetyStock, 0) AS SafetyStock
                        FROM dbo.tbl_log_ChemicalMasterStock WITH (NOLOCK)
                        WHERE CurrentStock IS NOT NULL";

                    using (var cmd = new SqlCommand(sqlChemStock, conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            string code = reader.IsDBNull(0) ? "" : reader.GetString(0).Trim();
                            string name = reader.IsDBNull(1) ? code : reader.GetString(1).Trim();
                            decimal cur = reader.IsDBNull(2) ? 0 : reader.GetDecimal(2);
                            decimal tot = reader.IsDBNull(3) ? 500 : reader.GetDecimal(3);
                            decimal safety = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4);

                            if (string.IsNullOrEmpty(code) && string.IsNullOrEmpty(name)) continue;
                            if (tot <= 0) tot = Math.Max(cur, 500m);

                            double pct = (double)(cur / tot) * 100.0;
                            list.Add(new StockInventoryItem
                            {
                                Type = "Chemical Stock",
                                Code = code,
                                Name = name,
                                CurrentStock = cur,
                                BaselineStock = tot,
                                SafetyStock = safety > 0 ? safety : Math.Round(tot * 0.3m, 2),
                                Percentage = pct,
                                Unit = "KG"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"GetAllStockInventory Error: {ex.Message}");
            }

            return list.OrderBy(x => x.Percentage).ThenBy(x => x.Type).ThenBy(x => x.Name).ToList();
        }

        public async Task<List<StockInventoryItem>> CheckLowStockLevelsAsync()
        {
            var allItems = await GetAllStockInventoryAsync();
            var lowStockList = allItems.Where(x => x.Percentage <= StockThresholdPct).ToList();

            try
            {
                // Delivery is handled by ProcessScheduleAsync (one digest per cycle inside the send window).
                lock (_scheduleLock) { _currentLowStock = lowStockList; }

                Raise(OnStockInventoryUpdated, allItems);
                Raise(OnLowStockUpdated, lowStockList);
            }
            catch (Exception ex)
            {
                Log($"CheckLowStockLevels Error: {ex.Message}");
            }

            return lowStockList;
        }

        #endregion

        #region Dispatcher & Multi-Channel Delivery

        public async Task<NotificationLogItem> DispatchNotificationAsync(
            string category, string priority, string title, string code, string name, string details, string actionLink = "STOCK_ALERT", string targetSystem = "RMS")
        {
            if (string.Equals(category, "Environment Alert", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(code, TypeEnvironmentAlert, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(actionLink, "KONGIN_TRACKER", StringComparison.OrdinalIgnoreCase))
            {
                targetSystem = "NEXUS";
            }

            var logItem = new NotificationLogItem
            {
                Category = category,
                Priority = priority,
                Title = title,
                ItemCode = code,
                ItemName = name,
                Details = details,
                TargetSystem = targetSystem
            };

            // Who receives this type of notification comes from dbRMSAndroid.tbl_NotificationCategory.
            // No row / no recipients = nobody is notified (never a broadcast to every user).
            string notifType = CategoryToType(category);
            List<string> recipients = await GetRecipientsAsync(notifType);
            if (recipients.Count == 0)
            {
                Log($"[{category}] [{targetSystem}] No recipients configured for '{notifType}' in tbl_NotificationCategory - phone/Synology delivery skipped.");
            }

            // 1. Channel A: Send to Firebase & Synology via send-notification.php
            if (recipients.Count > 0)
            try
            {
                using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(30) })
                {
                    string targetBotUrl = string.Equals(targetSystem, "NEXUS", StringComparison.OrdinalIgnoreCase)
                        ? SynologyBotUrlNexus
                        : SynologyBotUrlRms;

                    var postContent = new FormUrlEncodedContent(new[]
                    {
                        new KeyValuePair<string, string>("synology_ids", string.Join(";", recipients)),
                        new KeyValuePair<string, string>("category", category),
                        new KeyValuePair<string, string>("priority_level", priority),
                        new KeyValuePair<string, string>("details", details),
                        new KeyValuePair<string, string>("username", "System"),
                        new KeyValuePair<string, string>("initiator_name", targetSystem == "NEXUS" ? "Kongin Environmental Tracker" : "Server Manager"),
                        new KeyValuePair<string, string>("action_link", actionLink),
                        new KeyValuePair<string, string>("target_system", targetSystem),
                        new KeyValuePair<string, string>("synology_bot_url", targetBotUrl)
                    });

                    var response = await client.PostAsync(NotificationPhpUrl, postContent);
                    if (response.IsSuccessStatusCode)
                    {
                        string body = await response.Content.ReadAsStringAsync();
                        logItem.FcmSent = true;
                        logItem.SynologySent = true;
                    }
                    else
                    {
                        string errBody = await response.Content.ReadAsStringAsync();
                        Log($"PHP / FCM Dispatch HTTP {(int)response.StatusCode}: {errBody}");
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"PHP / FCM Dispatch note: {ex.Message}");
            }

            // 2. Channel B: Direct Synology Webhook Fallback if PHP failed (still only to the configured recipients)
            string directBotUrl = string.Equals(targetSystem, "NEXUS", StringComparison.OrdinalIgnoreCase)
                ? SynologyBotUrlNexus
                : SynologyBotUrlRms;

            if (recipients.Count > 0 && !logItem.SynologySent && !string.IsNullOrWhiteSpace(directBotUrl))
            {
                try
                {
                    using (var client = new HttpClient { Timeout = TimeSpan.FromSeconds(6) })
                    {
                        bool anyOk = false;
                        foreach (string rid in recipients)
                        {
                            object uid = int.TryParse(rid, out int numericId) ? (object)numericId : rid;
                            string jsonPayload = JsonConvert.SerializeObject(new { text = details, user_ids = new[] { uid } });
                            var formContent = new FormUrlEncodedContent(new[]
                            {
                                new KeyValuePair<string, string>("payload", jsonPayload)
                            });

                            var resp = await client.PostAsync(directBotUrl, formContent);
                            if (resp.IsSuccessStatusCode) anyOk = true;
                        }

                        if (anyOk) logItem.SynologySent = true;
                    }
                }
                catch (Exception ex)
                {
                    Log($"Synology Direct Webhook note: {ex.Message}");
                }
            }

            // 3. Channel C: Local WebSocket Call & Chat Signaling Broadcast (ws://0.0.0.0:9000)
            try
            {
                var wsPayload = new JObject
                {
                    ["type"] = "SYSTEM_NOTIFICATION",
                    ["system"] = targetSystem,
                    ["category"] = category,
                    ["priority"] = priority,
                    ["title"] = title,
                    ["details"] = details,
                    ["timestamp"] = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
                };

                int clientCount = CallSignalingService.Broadcast(wsPayload.ToString(Formatting.None));
                logItem.WebSocketSent = true;
                if (clientCount > 0)
                {
                    Log($"Broadcasted notification to {clientCount} WebSocket client(s).");
                }
            }
            catch (Exception ex)
            {
                Log($"WebSocket Broadcast note: {ex.Message}");
            }

            // Set final status text
            var delivered = new List<string>();
            if (logItem.SynologySent) delivered.Add("Synology");
            if (logItem.FcmSent) delivered.Add("FCM");
            if (logItem.WebSocketSent) delivered.Add("WebSocket/Chat");

            logItem.DeliveryStatus = delivered.Count > 0
                ? $"Delivered ({string.Join(", ", delivered)})"
                : (recipients.Count == 0 ? "Skipped (no recipients in tbl_NotificationCategory)" : "Delivery Failed");

            lock (_notificationHistory)
            {
                _notificationHistory.Insert(0, logItem);
                if (_notificationHistory.Count > 200) _notificationHistory.RemoveAt(_notificationHistory.Count - 1);
            }
            SaveHistory();

            Log($"[{category}] {title} -> {logItem.DeliveryStatus}");
            Raise(OnNotificationDispatched, logItem);

            return logItem;
        }

        #endregion

        #region Helper Fetch for Recent Inbounds Display

        public async Task<List<InboundSummaryItem>> GetRecentInboundsAsync(int top = 50)
        {
            var list = new List<InboundSummaryItem>();

            try
            {
                using (var conn = new SqlConnection(SqlConnectionString))
                {
                    await conn.OpenAsync();

                    // Material
                    string sqlMat = $@"
                        SELECT TOP {top} MaterialInboundID, MaterialInboundDate, MaterialCode, MaterialName, 
                               Category, MaterialQty, BatchTrackingNo, ContainerNo, CreatedBy
                        FROM dbo.tbl_log_MaterialInbound WITH (NOLOCK)
                        ORDER BY MaterialInboundID DESC";

                    using (var cmd = new SqlCommand(sqlMat, conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new InboundSummaryItem
                            {
                                InboundId = reader.GetInt32(0),
                                Type = "Material Inbound",
                                InboundDate = reader.IsDBNull(1) ? (DateTime?)null : Convert.ToDateTime(reader.GetValue(1)),
                                Code = reader.IsDBNull(2) ? "N/A" : reader.GetString(2).Trim(),
                                Name = reader.IsDBNull(3) ? "N/A" : reader.GetString(3).Trim(),
                                Category = reader.IsDBNull(4) ? "Standard" : reader.GetString(4).Trim(),
                                Quantity = reader.IsDBNull(5) ? 0 : reader.GetDecimal(5),
                                Unit = "KG/KL",
                                BatchTrackingNo = reader.IsDBNull(6) ? "N/A" : reader.GetString(6).Trim(),
                                ContainerNo = reader.IsDBNull(7) ? "N/A" : reader.GetString(7).Trim(),
                                CreatedBy = reader.IsDBNull(8) ? "System" : reader.GetString(8).Trim(),
                                FifoNotice = "FIFO REQUIRED"
                            });
                        }
                    }

                    // Pigment
                    string sqlPig = $@"
                        SELECT TOP {top} PigmentInboundID, fld_PNInboundDate, fld_PigmentCode, fld_PigmentName, 
                               fld_PNQty, BatchTrackingNo, ContainerNo, CreatedBy
                        FROM dbo.tbl_log_PigmentInbound WITH (NOLOCK)
                        ORDER BY PigmentInboundID DESC";

                    using (var cmd = new SqlCommand(sqlPig, conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new InboundSummaryItem
                            {
                                InboundId = reader.GetInt32(0),
                                Type = "Pigment Inbound",
                                InboundDate = reader.IsDBNull(1) ? (DateTime?)null : Convert.ToDateTime(reader.GetValue(1)),
                                Code = reader.IsDBNull(2) ? "N/A" : reader.GetString(2).Trim(),
                                Name = reader.IsDBNull(3) ? "N/A" : reader.GetString(3).Trim(),
                                Category = "Pigment",
                                Quantity = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4),
                                Unit = "g/KG",
                                BatchTrackingNo = reader.IsDBNull(5) ? "N/A" : reader.GetString(5).Trim(),
                                ContainerNo = reader.IsDBNull(6) ? "N/A" : reader.GetString(6).Trim(),
                                CreatedBy = reader.IsDBNull(7) ? "System" : reader.GetString(7).Trim(),
                                FifoNotice = "FIFO REQUIRED"
                            });
                        }
                    }

                    // Chemical
                    string sqlChem = $@"
                        SELECT TOP {top} ChemicalInboundID, fld_ChemInboundDate, fld_ChemCode, fld_ChemName, 
                               fld_ChemQty, BatchTrackingNo, ContainerNo, CreatedBy
                        FROM dbo.tbl_log_ChemicalInbound WITH (NOLOCK)
                        ORDER BY ChemicalInboundID DESC";

                    using (var cmd = new SqlCommand(sqlChem, conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new InboundSummaryItem
                            {
                                InboundId = reader.GetInt32(0),
                                Type = "Chemical Inbound",
                                InboundDate = reader.IsDBNull(1) ? (DateTime?)null : Convert.ToDateTime(reader.GetValue(1)),
                                Code = reader.IsDBNull(2) ? "N/A" : reader.GetString(2).Trim(),
                                Name = reader.IsDBNull(3) ? "N/A" : reader.GetString(3).Trim(),
                                Category = "Chemical",
                                Quantity = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4),
                                Unit = "KG",
                                BatchTrackingNo = reader.IsDBNull(5) ? "N/A" : reader.GetString(5).Trim(),
                                ContainerNo = reader.IsDBNull(6) ? "N/A" : reader.GetString(6).Trim(),
                                CreatedBy = reader.IsDBNull(7) ? "System" : reader.GetString(7).Trim(),
                                FifoNotice = "FIFO REQUIRED"
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"GetRecentInbounds Error: {ex.Message}");
            }

            return list.OrderByDescending(x => x.InboundDate ?? DateTime.MinValue).ThenByDescending(x => x.InboundId).ToList();
        }

        #endregion

        #region Material Pallet Lab Testing (TS2 / TC90 / Chemicals 48hr Lapse)

        public async Task<List<MaterialPalletQualityItem>> GetMaterialPalletsAsync(int top = 150)
        {
            var list = new List<MaterialPalletQualityItem>();

            try
            {
                using (var conn = new SqlConnection(SqlConnectionString))
                {
                    await conn.OpenAsync();

                    string sql = $@"
                        SELECT TOP {top} PalletID, BatchtrackingNo, MaterialCode, MaterialName, TS2, TC90,
                               E02, E09, D05, E09_1, C10, E06, Compowder, E01, CreatedAt
                        FROM dbo.tbl_log_MaterialInboundPallet WITH (NOLOCK)
                        ORDER BY PalletID DESC";

                    using (var cmd = new SqlCommand(sql, conn))
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            list.Add(new MaterialPalletQualityItem
                            {
                                PalletId = reader.GetInt32(0),
                                BatchTrackingNo = reader.IsDBNull(1) ? "N/A" : reader.GetString(1).Trim(),
                                MaterialCode = reader.IsDBNull(2) ? "N/A" : reader.GetString(2).Trim(),
                                MaterialName = reader.IsDBNull(3) ? "N/A" : reader.GetString(3).Trim(),
                                TS2 = reader.IsDBNull(4) ? (decimal?)null : reader.GetDecimal(4),
                                TC90 = reader.IsDBNull(5) ? (decimal?)null : reader.GetDecimal(5),
                                E02 = reader.IsDBNull(6) ? null : reader.GetString(6).Trim(),
                                E09 = reader.IsDBNull(7) ? null : reader.GetString(7).Trim(),
                                D05 = reader.IsDBNull(8) ? null : reader.GetString(8).Trim(),
                                E09_1 = reader.IsDBNull(9) ? null : reader.GetString(9).Trim(),
                                C10 = reader.IsDBNull(10) ? null : reader.GetString(10).Trim(),
                                E06 = reader.IsDBNull(11) ? null : reader.GetString(11).Trim(),
                                Compowder = reader.IsDBNull(12) ? null : reader.GetString(12).Trim(),
                                E01 = reader.IsDBNull(13) ? null : reader.GetString(13).Trim(),
                                CreatedAt = reader.IsDBNull(14) ? (DateTime?)null : Convert.ToDateTime(reader.GetValue(14))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"GetMaterialPallets Error: {ex.Message}");
            }

            return list.OrderByDescending(x => x.Is48HrLapsed).ThenByDescending(x => x.CreatedAt ?? DateTime.MinValue).ToList();
        }

        public async Task<List<MaterialPalletQualityItem>> ScanLapsedPalletQualityAsync()
        {
            var allPallets = await GetMaterialPalletsAsync(150);
            var overduePallets = allPallets.Where(x => x.Is48HrLapsed).ToList();

            try
            {
                lock (_scheduleLock) { _currentOverdue = overduePallets; }

                Raise(OnPalletQualityUpdated, allPallets);
            }
            catch (Exception ex)
            {
                Log($"ScanLapsedPalletQuality Error: {ex.Message}");
            }

            return overduePallets;
        }

        #endregion

        #region Delivery Schedule (24h cycle from 07:00, send window 07:00-17:00, one digest per category)

        private const string TypeMaterialInbound = "MATERIAL_INBOUND";
        private const string TypePigmentInbound = "PIGMENT_INBOUND";
        private const string TypeChemicalInbound = "CHEMICAL_INBOUND";
        private const string TypeLowStock = "LOW_STOCK";
        private const string TypeLabQuality = "LAB_QUALITY_48H";
        private const string TypeColorExpiry = "COLOR_EXPIRY";
                public const string TypeAll = "ALL";
        public const string TypeEnvironmentAlert = "ENVIRONMENT_ALERT";

        /// <summary>Notification types the watcher sends automatically (rows of tbl_NotificationCategory.NotifType).</summary>
        public static readonly string[] ScheduledTypes = { TypeMaterialInbound, TypePigmentInbound, TypeChemicalInbound, TypeLowStock, TypeLabQuality, TypeColorExpiry };

        /// <summary>Maps the display category sent to tbl_Notifications / the phones to its tbl_NotificationCategory.NotifType.</summary>
        public static string CategoryToType(string category)
        {
            switch ((category ?? "").Trim().ToLowerInvariant())
            {
                case "material inbound": return TypeMaterialInbound;
                case "pigment inbound": return TypePigmentInbound;
                case "chemical inbound": return TypeChemicalInbound;
                case "low stock warning": return TypeLowStock;
                case "lab quality 48hr lapse": return TypeLabQuality;
                case "color expiration": return TypeColorExpiry;
                                case "environment alert": return TypeEnvironmentAlert;
                case "system test": return "SYSTEM_TEST";
                default: return (category ?? "").Trim().ToUpperInvariant().Replace(' ', '_').Replace('-', '_');
            }
        }

        public bool IsInSendWindow(DateTime now)
        {
            TimeSpan t = now.TimeOfDay;
            return t >= SendWindowStart && t < SendWindowEnd;
        }

        /// <summary>Id of the 24-hour cycle that contains <paramref name="now"/>; cycles roll over at SendWindowStart (07:00).</summary>
        public string CurrentCycleId(DateTime now)
        {
            DateTime start = now.Date + SendWindowStart;
            if (now < start) start = start.AddDays(-1);
            return start.ToString("yyyy-MM-dd");
        }

        private sealed class NotificationScheduleState
        {
            public int WmMaterial = -1, WmPigment = -1, WmChemical = -1;
            public Dictionary<string, string> LastCycle = new Dictionary<string, string>();   // legacy: type -> cycle already fully sent
            public Dictionary<string, string> SentCycle = new Dictionary<string, string>();   // type -> cycle of SentCount
            public Dictionary<string, int> SentCount = new Dictionary<string, int>();         // digests already sent in that cycle
            public Dictionary<string, List<string>> PendingInbound = new Dictionary<string, List<string>>();
        }

        private static string StatePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ServerHTMLHandler", "notification-state.json");

        private NotificationScheduleState GetState()
        {
            lock (_scheduleLock)
            {
                if (_state != null) return _state;
                try
                {
                    if (File.Exists(StatePath))
                        _state = JsonConvert.DeserializeObject<NotificationScheduleState>(File.ReadAllText(StatePath));
                }
                catch (Exception ex)
                {
                    Log($"Notification state could not be read, starting fresh: {ex.Message}");
                }
                if (_state == null) _state = new NotificationScheduleState();
                if (_state.LastCycle == null) _state.LastCycle = new Dictionary<string, string>();
                if (_state.SentCycle == null) _state.SentCycle = new Dictionary<string, string>();
                if (_state.SentCount == null) _state.SentCount = new Dictionary<string, int>();
                if (_state.PendingInbound == null) _state.PendingInbound = new Dictionary<string, List<string>>();
                return _state;
            }
        }

        private void SaveState()
        {
            lock (_scheduleLock)
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(StatePath));
                    string tmp = StatePath + ".tmp";
                    File.WriteAllText(tmp, JsonConvert.SerializeObject(_state ?? new NotificationScheduleState(), Formatting.Indented));
                    if (File.Exists(StatePath)) File.Delete(StatePath);
                    File.Move(tmp, StatePath);
                }
                catch (Exception ex)
                {
                    Log($"Notification state could not be saved: {ex.Message}");
                }
            }
        }

        private void PersistWatermarks()
        {
            if (_lastMaterialInboundId < 0) return;
            var st = GetState();
            lock (_scheduleLock)
            {
                st.WmMaterial = _lastMaterialInboundId;
                st.WmPigment = _lastPigmentInboundId;
                st.WmChemical = _lastChemicalInboundId;
            }
            SaveState();
        }

        private void QueueInbound(string notifType, string line)
        {
            var st = GetState();
            lock (_scheduleLock)
            {
                if (!st.PendingInbound.TryGetValue(notifType, out var list))
                {
                    list = new List<string>();
                    st.PendingInbound[notifType] = list;
                }
                if (list.Count < 500) list.Add(line);
            }
        }

        // ---- who receives what: dbRMSAndroid.tbl_NotificationCategory (NotifType, ToChatSynolofyID "id;id;id") ----
        private readonly Dictionary<string, KeyValuePair<DateTime, List<string>>> _recipientCache =
            new Dictionary<string, KeyValuePair<DateTime, List<string>>>(StringComparer.OrdinalIgnoreCase);

        public async Task<List<string>> GetRecipientsAsync(string notifType)
        {
            lock (_scheduleLock)
            {
                if (_recipientCache.TryGetValue(notifType, out var hit) && (DateTime.Now - hit.Key).TotalMinutes < 5)
                    return new List<string>(hit.Value);
            }

            var result = new List<string>();
            try
            {
                using (var conn = new SqlConnection(RmsConnectionString))
                {
                    await conn.OpenAsync();
                    // If asking for a specific RMS category, include global ALL recipients automatically.
                    // Environment Alert is strictly for NEXUS and must NOT inherit RMS ALL recipients!
                    string query;
                    if (string.Equals(notifType, TypeAll, StringComparison.OrdinalIgnoreCase) ||
                        string.Equals(notifType, TypeEnvironmentAlert, StringComparison.OrdinalIgnoreCase))
                    {
                        query = "SELECT ToChatSynolofyID FROM dbo.tbl_NotificationCategory WHERE NotifType = @t";
                    }
                    else
                    {
                        query = "SELECT ToChatSynolofyID FROM dbo.tbl_NotificationCategory WHERE NotifType = @t OR NotifType = 'ALL'";
                    }
                    using (var cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@t", notifType);
                        using (var rd = await cmd.ExecuteReaderAsync())
                        {
                            while (await rd.ReadAsync())
                            {
                                string raw = rd.IsDBNull(0) ? "" : rd.GetString(0);
                                foreach (string part in raw.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                                {
                                    string id = part.Trim();
                                    if (id.Length > 0 && !result.Contains(id)) result.Add(id);
                                }
                            }
                        }
                    }
                }

                lock (_scheduleLock) { _recipientCache[notifType] = new KeyValuePair<DateTime, List<string>>(DateTime.Now, result); }
            }
            catch (Exception ex)
            {
                Log($"Could not read recipients for '{notifType}': {ex.Message}");
                lock (_scheduleLock)
                {
                    // Keep serving the last known list for a while rather than failing open or closed on a blip.
                    if (_recipientCache.TryGetValue(notifType, out var old)) return new List<string>(old.Value);
                }
            }

            return result;
        }

        // ---- recipient management (used by the Notification Hub "Recipients" tab) ----

        public sealed class NotificationCategoryInfo
        {
            public string NotifType { get; set; }
            public string Label { get; set; }
            public string Description { get; set; }
            public string DefaultPriority { get; set; } = "HIGH";
        }

        /// <summary>The notification types this handler sends, with a readable label for the dropdown.</summary>
        public static readonly NotificationCategoryInfo[] KnownCategories =
        {
            new NotificationCategoryInfo { NotifType = TypeAll, Label = "All Notifications (Global)", Description = "Global broadcast recipients. Users listed here receive every notification type sent by the system (Low Stock, Lab Quality, Inbounds, Color Expiry, and Custom Alerts)." },
            new NotificationCategoryInfo { NotifType = TypeMaterialInbound, Label = "Material Inbound (FIFO)", Description = "New rubber/material inbounds. One digest per day." },
            new NotificationCategoryInfo { NotifType = TypePigmentInbound, Label = "Pigment Inbound (FIFO)", Description = "New pigment inbounds. One digest per day." },
            new NotificationCategoryInfo { NotifType = TypeChemicalInbound, Label = "Chemical Inbound (FIFO)", Description = "New chemical inbounds. One digest per day." },
            new NotificationCategoryInfo { NotifType = TypeLowStock, Label = "Low Stock Warning", Description = "Items at or below the stock threshold. Two digests per day (from 07:00 and from 13:00, only while 07:00-17:00) while unresolved." },
            new NotificationCategoryInfo { NotifType = TypeLabQuality, Label = "Lab Quality 48hr Lapse", Description = "Pallets with TS2 / TC90 / chemical results still missing after 48 hours. One digest per day while unresolved." },
            new NotificationCategoryInfo { NotifType = TypeColorExpiry, Label = "Color Expiration", Description = "Approved library colors valid from their EnteredDate for 6 months (dark) or 2 months (light, by Hex / color name). Listed from 14 days before expiry. One digest per day." },
            new NotificationCategoryInfo { NotifType = "SYSTEM_TEST", Label = "System Test (Send test button)", Description = "Only used by the Send Test button. Sent immediately.", DefaultPriority = "NORMAL" },
        };

        /// <summary>Every NotifType currently stored in tbl_NotificationCategory (oldest first).</summary>
        public async Task<List<string>> GetStoredCategoryTypesAsync()
        {
            var list = new List<string>();
            using (var conn = new SqlConnection(RmsConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand("SELECT NotifType FROM dbo.tbl_NotificationCategory ORDER BY NotifID", conn))
                using (var rd = await cmd.ExecuteReaderAsync())
                {
                    while (await rd.ReadAsync())
                        if (!rd.IsDBNull(0)) list.Add(rd.GetString(0).Trim());
                }
            }
            return list;
        }

        /// <summary>Current recipients straight from the database (bypasses the 5-minute cache).</summary>
        public async Task<List<string>> LoadRecipientsFreshAsync(string notifType)
        {
            lock (_scheduleLock) { _recipientCache.Remove(notifType); }
            using (var conn = new SqlConnection(RmsConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand("SELECT TOP 1 ToChatSynolofyID FROM dbo.tbl_NotificationCategory WHERE NotifType = @t", conn))
                {
                    cmd.Parameters.AddWithValue("@t", notifType);
                    var ids = new List<string>();
                    foreach (string part in (Convert.ToString(await cmd.ExecuteScalarAsync()) ?? "").Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        string id = part.Trim();
                        if (id.Length > 0 && !ids.Contains(id)) ids.Add(id);
                    }
                    return ids;
                }
            }
        }

        /// <summary>Splits pasted text ("214; 426, 500") into Synology IDs. Valid IDs are 1-10 digits.</summary>
        public static void ParseSynologyIds(string raw, out List<string> valid, out List<string> invalid)
        {
            valid = new List<string>();
            invalid = new List<string>();
            foreach (string part in (raw ?? "").Split(new[] { ';', ',', ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
            {
                string id = part.Trim();
                bool ok = id.Length >= 1 && id.Length <= 10 && id.All(ch => ch >= '0' && ch <= '9');
                if (ok) { if (!valid.Contains(id)) valid.Add(id); }
                else if (!invalid.Contains(id)) invalid.Add(id);
            }
        }

        /// <summary>Synology ID -> display name from dbRMSAndroid.dbo.users (IDs without a match are omitted).</summary>
        public async Task<Dictionary<string, string>> ResolveNamesAsync(IEnumerable<string> ids)
        {
            var result = new Dictionary<string, string>();
            var idList = ids.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct().ToList();
            if (idList.Count == 0) return result;

            using (var conn = new SqlConnection(RmsConnectionString))
            {
                await conn.OpenAsync();
                var names = idList.Select((_, i) => "@p" + i).ToList();
                using (var cmd = new SqlCommand($"SELECT synologyID, ISNULL(Name, UserName) FROM dbo.users WHERE synologyID IN ({string.Join(",", names)})", conn))
                {
                    for (int i = 0; i < idList.Count; i++) cmd.Parameters.AddWithValue(names[i], idList[i]);
                    using (var rd = await cmd.ExecuteReaderAsync())
                    {
                        while (await rd.ReadAsync())
                            if (!rd.IsDBNull(0)) result[rd.GetString(0).Trim()] = rd.IsDBNull(1) ? "" : rd.GetString(1);
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Saves the recipient list of one notification type into tbl_NotificationCategory.ToChatSynolofyID ("id;id;id"):
        /// updates the row, or adds it when the type does not exist yet. Returns the stored string.
        /// </summary>
        public async Task<string> SaveRecipientsAsync(string notifType, IEnumerable<string> ids, string priority = "HIGH")
        {
            notifType = (notifType ?? "").Trim();
            if (notifType.Length == 0 || notifType.Length > 50) throw new ArgumentException("Notification type is required (max 50 characters).");

            ParseSynologyIds(string.Join(";", ids ?? new string[0]), out var valid, out var invalid);
            if (invalid.Count > 0) throw new ArgumentException("Invalid Synology ID(s): " + string.Join(", ", invalid) + ". IDs are numbers.");
            if (valid.Count > 200) throw new ArgumentException("Too many recipients (max 200).");
            string joined = string.Join(";", valid);

            using (var conn = new SqlConnection(RmsConnectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new SqlCommand(
                    @"UPDATE dbo.tbl_NotificationCategory SET ToChatSynolofyID = @ids, UpdatedAt = GETDATE() WHERE NotifType = @t;
                      IF @@ROWCOUNT = 0
                          INSERT INTO dbo.tbl_NotificationCategory (NotifType, ToChatSynolofyID, PriorityLevel) VALUES (@t, @ids, @p);", conn))
                {
                    cmd.Parameters.AddWithValue("@t", notifType);
                    cmd.Parameters.AddWithValue("@ids", joined);
                    cmd.Parameters.AddWithValue("@p", string.IsNullOrWhiteSpace(priority) ? "HIGH" : priority);
                    await cmd.ExecuteNonQueryAsync();
                }
            }

            lock (_scheduleLock) { _recipientCache.Remove(notifType); } // next dispatch uses the new list immediately
            Log($"Recipients for '{notifType}' saved: {(joined.Length == 0 ? "(nobody)" : joined)}");
            return joined;
        }

        // ---- colour expiration scan ----

        // Representative colour of common colour names, so a colour without a Hex can still be placed on the dark/light scale.
        private static readonly Dictionary<string, string> PaletteHex = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["black"] = "#000000", ["charcoal"] = "#36454F", ["graphite"] = "#383428", ["gray"] = "#8C8C8C", ["grey"] = "#8C8C8C", ["silver"] = "#C0C0C0",
            ["white"] = "#FFFFFF", ["ivory"] = "#FFFFF0", ["cream"] = "#FFFDD0", ["egret"] = "#EDEAE0", ["birch"] = "#CABDAC", ["turtledove"] = "#D8D2C2",
            ["beige"] = "#F5F5DC", ["sand"] = "#C2B280", ["khaki"] = "#C3B091", ["taupe"] = "#8B7D6B", ["incense"] = "#B09A7D", ["tan"] = "#D2B48C",
            ["brown"] = "#6F4E37", ["chocolate"] = "#4B2E1E", ["coffee"] = "#4B3621", ["cocoa"] = "#3D1C02", ["gum"] = "#7B4F28", ["mahogany"] = "#4E1B0E",
            ["maroon"] = "#800000", ["burgundy"] = "#800020", ["wine"] = "#722F37", ["port"] = "#3B0A19", ["chili"] = "#94383D", ["red"] = "#FF0000",
            ["crimson"] = "#DC143C", ["scarlet"] = "#FF2400", ["tomato"] = "#FF6347", ["rust"] = "#B7410E", ["orange"] = "#FFA500", ["coral"] = "#FF7F50",
            ["salmon"] = "#FA8072", ["peach"] = "#FFDAB9", ["apricot"] = "#FBCEB1", ["autumn"] = "#B5651D", ["yellow"] = "#FFFF00", ["gold"] = "#FFD700",
            ["mustard"] = "#FFDB58", ["lemon"] = "#FFF44F", ["lime"] = "#BFFF00", ["green"] = "#3A9D23", ["olive"] = "#808000", ["pine"] = "#01796F",
            ["ivy"] = "#2E6B3E", ["emerald"] = "#046307", ["forest"] = "#228B22", ["balsam"] = "#5E7F5E", ["willow"] = "#9BAE7D", ["mint"] = "#98FF98",
            ["teal"] = "#008080", ["cyan"] = "#00FFFF", ["aqua"] = "#7FDBFF", ["turquoise"] = "#40E0D0", ["sky"] = "#87CEEB", ["azure"] = "#007FFF",
            ["blue"] = "#0000FF", ["cobalt"] = "#0047AB", ["navy"] = "#000080", ["indigo"] = "#4B0082", ["denim"] = "#1560BD", ["royal"] = "#4169E1",
            ["purple"] = "#800080", ["violet"] = "#8A2BE2", ["plum"] = "#8E4585", ["orchid"] = "#DA70D6", ["lavender"] = "#B57EDC", ["lilac"] = "#C8A2C8",
            ["magenta"] = "#FF00FF", ["fuchsia"] = "#FF00FF", ["pink"] = "#FFC0CB", ["rose"] = "#FF66CC", ["blush"] = "#F4C2C2", ["carnation"] = "#FFA6C9",
            ["transparent"] = "#FFFFFF", ["clear"] = "#FFFFFF",
        };
        private static readonly HashSet<string> LightWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "light", "pale", "pastel", "soft", "baby", "ice", "icy", "foam", "mist", "powder", "sunny", "fluro", "neon", "optic", "blanc" };
        private static readonly HashSet<string> DarkWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "dark", "deep", "midnight", "dusk", "night" };

        private static double Brightness(string hex6)
        {
            int r = Convert.ToInt32(hex6.Substring(0, 2), 16), g = Convert.ToInt32(hex6.Substring(2, 2), 16), b = Convert.ToInt32(hex6.Substring(4, 2), 16);
            return (0.299 * r + 0.587 * g + 0.114 * b) / 255.0;   // perceived brightness
        }

        /// <summary>Hex -> dark/light. Accepts #RRGGBB, RRGGBB, #RGB and "transparent"/"white" (light). False when it cannot be read.</summary>
        public static bool TryClassifyHex(string hex, double cutoff, out bool isDark)
        {
            isDark = false;
            string h = (hex ?? "").Trim();
            if (h.Length == 0) return false;
            if (h.Equals("transparent", StringComparison.OrdinalIgnoreCase) || h.Equals("white", StringComparison.OrdinalIgnoreCase)) return true;
            if (h.StartsWith("#")) h = h.Substring(1);
            if (h.Length == 3) h = string.Concat(h.Select(ch => new string(ch, 2)));
            if (h.Length != 6 || !h.All(Uri.IsHexDigit)) return false;

            isDark = Brightness(h) < cutoff;
            return true;
        }

        /// <summary>
        /// Light or dark: from the Hex when present; otherwise from the colour name using the palette
        /// ("maroon" -> dark, "coral blush" -> light, "dark X"/"light X" modifiers respected). False when neither tells.
        /// </summary>
        public static bool TryClassifyColor(string hex, string name, double cutoff, out bool isDark, out string basis)
        {
            basis = "";
            if (TryClassifyHex(hex, cutoff, out isDark)) { basis = "Hex " + hex.Trim(); return true; }

            var tokens = System.Text.RegularExpressions.Regex.Matches((name ?? "").ToLowerInvariant(), "[a-z]+").Cast<System.Text.RegularExpressions.Match>().Select(m => m.Value).ToList();
            string baseWord = tokens.LastOrDefault(t => PaletteHex.ContainsKey(t));
            if (tokens.Any(t => DarkWords.Contains(t))) { isDark = true; basis = $"name \"{string.Join(" ", tokens)}\""; return true; }
            if (tokens.Any(t => LightWords.Contains(t))) { isDark = false; basis = $"name \"{string.Join(" ", tokens)}\""; return true; }
            if (baseWord != null)
            {
                isDark = Brightness(PaletteHex[baseWord].TrimStart('#')) < cutoff;
                basis = $"palette: {baseWord}";
                return true;
            }
            isDark = false;
            return false;
        }

        /// <summary>
        /// Approved library colours (newest approved formula of each colour code) whose validity -- EnteredDate + 6 months for dark
        /// colours, + 2 months for light colours -- ends within ColorNoticeLeadDays or ended less than ColorExpiredGraceDays ago.
        /// </summary>
        public async Task<List<ColorExpiryItem>> ScanColorExpiryAsync()
        {
            var due = new List<ColorExpiryItem>();
            int older = 0;
            try
            {
                // The library keeps past versions of a colour. A row is only used when no newer approved row has the same
                // colour code OR the same colour name; the newest one is the basis for the dates.
                var seenCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var seenNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                DateTime today = DateTime.Today;

                using (var conn = new SqlConnection(SqlConnectionString))
                {
                    await conn.OpenAsync();
                    const string sql = @"
                        SELECT NewColorFormulaID, ColorName, ColorCode, Hex, EnteredDate
                        FROM dbo.tbl_NewColorFormulaHeader WITH (NOLOCK)
                        WHERE Status = 'QC Approved' AND (DeleteFlag IS NULL OR DeleteFlag = 0) AND EnteredDate IS NOT NULL
                        ORDER BY EnteredDate DESC, NewColorFormulaID DESC";
                    using (var cmd = new SqlCommand(sql, conn))
                    using (var rd = await cmd.ExecuteReaderAsync())
                    {
                        while (await rd.ReadAsync())
                        {
                            int id = rd.GetInt32(0);
                            string name = rd.IsDBNull(1) ? "" : rd.GetString(1).Trim();
                            string code = rd.IsDBNull(2) ? "" : rd.GetString(2).Trim();
                            string hex = rd.IsDBNull(3) ? "" : rd.GetString(3).Trim();
                            DateTime entered = rd.GetDateTime(4).Date;

                            string nameKey = System.Text.RegularExpressions.Regex.Replace(name.ToLowerInvariant(), @"\s+", " ").Trim();
                            bool usableName = nameKey.Length > 0 && nameKey != "-";            // empty / "-" names say nothing about identity
                            bool usableCode = code.Length > 0 && code != "-";
                            if ((usableCode && seenCodes.Contains(code)) || (usableName && seenNames.Contains(nameKey))) continue;   // a newer row of this colour was already handled
                            if (usableCode) seenCodes.Add(code);
                            if (usableName) seenNames.Add(nameKey);

                            bool known = TryClassifyColor(hex, name, DarkBrightnessCutoff, out bool isDark, out string basis);
                            if (!known)
                            {
                                isDark = UnknownColorTreatedAsDark;
                                basis = "no Hex / unknown name - assumed " + (isDark ? "dark" : "light");
                            }

                            int life = isDark ? DarkColorLifeMonths : LightColorLifeMonths;
                            DateTime expiry = entered.AddMonths(life);
                            int daysLeft = (int)Math.Floor((expiry - today).TotalDays);

                            if (daysLeft > ColorNoticeLeadDays) continue;                  // not due yet
                            if (daysLeft < -ColorExpiredGraceDays) { older++; continue; } // expired long ago: only counted

                            due.Add(new ColorExpiryItem
                            {
                                FormulaId = id, ColorCode = code, ColorName = name, Hex = hex, IsDark = isDark, Basis = basis,
                                EnteredDate = entered, ExpiryDate = expiry, LifeMonths = life
                            });
                        }
                    }
                }

                due = due.OrderBy(x => x.ExpiryDate).ThenBy(x => x.ColorCode).ToList();
                lock (_scheduleLock) { _currentColorExpiry = due; _colorOlderExpired = older; }
                _lastColorScan = DateTime.Now;
            }
            catch (Exception ex)
            {
                Log($"ScanColorExpiry Error: {ex.Message}");
            }
            return due;
        }

        /// <summary>The digest text that would be sent for a type right now (nothing is sent). Null when there is nothing to report.</summary>
        public async Task<string> PreviewDigestAsync(string notifType)
        {
            if (notifType == TypeColorExpiry) await ScanColorExpiryAsync();
            else if (notifType == TypeLowStock) await CheckLowStockLevelsAsync();
            else if (notifType == TypeLabQuality) await ScanLapsedPalletQualityAsync();
            return BuildDigest(notifType, out _, out _, out _, out _);
        }

        private static string Plural(int n, string one, string many) => n == 1 ? one : many;

        private string BuildDigest(string notifType, out string category, out string title, out string actionLink, out int itemCount)
        {
            int max = Math.Max(3, MaxItemsPerDigest);
            var lines = new List<string>();
            string header, footer;
            category = ""; title = ""; actionLink = "STOCK_ALERT"; itemCount = 0;

            if (notifType == TypeLowStock)
            {
                List<StockInventoryItem> items;
                lock (_scheduleLock) { items = new List<StockInventoryItem>(_currentLowStock); }
                itemCount = items.Count;
                if (itemCount == 0) return null;
                category = "Low Stock Warning"; actionLink = "STOCK_MONITOR";
                title = $"Daily Low Stock Summary ({itemCount})";
                header = $"[DAILY LOW STOCK SUMMARY - {itemCount} ITEM(S) AT OR BELOW {StockThresholdPct:F0}%]";
                foreach (var i in items.Take(max))
                    lines.Add($"\u2022 {i.Type} | {i.Code} {i.Name} | {i.CurrentStock:N2} {i.Unit} of {i.BaselineStock:N2} ({i.Percentage:F1}%)");
                footer = "\u26A0\uFE0F Inventory replenishment is required.";
            }
            else if (notifType == TypeColorExpiry)
            {
                List<ColorExpiryItem> items;
                int older;
                lock (_scheduleLock) { items = new List<ColorExpiryItem>(_currentColorExpiry); older = _colorOlderExpired; }
                itemCount = items.Count;
                if (itemCount == 0) return null;
                category = "Color Expiration"; actionLink = "COLOR_EXPIRY";
                title = $"Color Expiration Notice ({itemCount})";
                header = $"[COLOR EXPIRATION - {itemCount} COLOR(S) EXPIRING SOON OR JUST EXPIRED (valid from the entered date: dark {DarkColorLifeMonths} months, light {LightColorLifeMonths} months)]";
                foreach (var i in items.Take(max))
                    lines.Add($"\u2022 {i.ColorCode} {i.ColorName} | {i.Tone} ({i.Basis}) | entered {i.EnteredDate:yyyy-MM-dd} \u2192 expires {i.ExpiryDate:yyyy-MM-dd} ({i.WhenText})");
                footer = (older > 0 ? $"({older} older expired color(s) not listed.)\n" : "") + "\u26A0\uFE0F Please renew or replace these colors before/after they expire.";
            }
            else if (notifType == TypeLabQuality)
            {
                List<MaterialPalletQualityItem> items;
                lock (_scheduleLock) { items = new List<MaterialPalletQualityItem>(_currentOverdue); }
                itemCount = items.Count;
                if (itemCount == 0) return null;
                category = "Lab Quality 48hr Lapse"; actionLink = "PALLET_QUALITY";
                title = $"48h Lab Testing Overdue ({itemCount})";
                header = $"[48-HOUR LAB TESTING OVERDUE - {itemCount} PALLET(S)]";
                foreach (var i in items.Take(max))
                    lines.Add($"\u2022 Pallet {i.PalletId} | Batch {i.BatchTrackingNo} | {i.MaterialName} ({i.MaterialCode}) | {i.DisplayHoursLapsed} lapsed | Missing: {i.MissingSummary}");
                footer = "\u26A0\uFE0F TS2, TC90 and chemical results are still missing. Please enter them.";
            }
            else
            {
                List<string> pending;
                var st = GetState();
                lock (_scheduleLock) { pending = st.PendingInbound.TryGetValue(notifType, out var l) ? new List<string>(l) : new List<string>(); }
                itemCount = pending.Count;
                if (itemCount == 0) return null;
                string kind = notifType == TypeMaterialInbound ? "Material" : notifType == TypePigmentInbound ? "Pigment" : "Chemical";
                category = kind + " Inbound"; actionLink = "INBOUND_FIFO";
                title = $"New {kind} Inbound ({itemCount})";
                header = $"[NEW {kind.ToUpperInvariant()} INBOUND - {itemCount} RECEIVED - FIFO REQUIRED]";
                lines.AddRange(pending.Take(max));
                footer = "\u26A0\uFE0F FIFO NOTICE: First-In, First-Out rotation required!";
            }

            var sb = new StringBuilder();
            sb.AppendLine(header);
            foreach (string l in lines) sb.AppendLine(l);
            if (itemCount > lines.Count) sb.AppendLine($"\u2026 and {itemCount - lines.Count} more {Plural(itemCount - lines.Count, "item", "items")} (see the Notification Hub).");
            sb.Append(footer);
            return sb.ToString();
        }

        private TimeSpan[] SlotsFor(string type)
        {
            return SendSlots.TryGetValue(type, out var slots) && slots.Length > 0 ? slots.OrderBy(x => x).ToArray() : new[] { SendWindowStart };
        }

        /// <summary>Digests of this type already sent in the given cycle.</summary>
        private int SentInCycle(NotificationScheduleState st, string type, string cycle)
        {
            lock (_scheduleLock)
            {
                if (st.SentCycle.TryGetValue(type, out var c) && c == cycle) return st.SentCount.TryGetValue(type, out var n) ? n : 0;
                if (st.LastCycle.TryGetValue(type, out var legacy) && legacy == cycle) return 1; // sent under the old once-per-cycle rule
                return 0;
            }
        }

        /// <summary>Records a send. Slots whose time has already passed are consumed too, so two digests are never sent back to back.</summary>
        private void MarkSent(NotificationScheduleState st, string type, string cycle, DateTime now, bool giveUp = false)
        {
            var slots = SlotsFor(type);
            int before = SentInCycle(st, type, cycle);
            int passed = slots.Count(x => x <= now.TimeOfDay);
            int after = giveUp ? slots.Length : Math.Min(slots.Length, Math.Max(before + 1, passed));
            lock (_scheduleLock)
            {
                st.SentCycle[type] = cycle;
                st.SentCount[type] = after;
                st.LastCycle.Remove(type);
            }
        }

        /// <summary>
        /// Delivers due digests. Rules: only inside the send window; each category at most once per 24h cycle
        /// (cycle starts 07:00); nothing is sent when there is nothing to report; failures are retried slowly.
        /// </summary>
        private async Task ProcessScheduleAsync()
        {
            DateTime now = DateTime.Now;
            if (!IsInSendWindow(now)) return; // outside 07:00-17:00: everything waits (it is kept, not lost)

            string cycle = CurrentCycleId(now);
            var st = GetState();

            foreach (string type in ScheduledTypes)
            {
                var slots = SlotsFor(type);
                int sent = SentInCycle(st, type, cycle);
                if (sent >= slots.Length) continue;                 // this cycle's quota is used up
                if (now.TimeOfDay < slots[sent]) continue;          // next slot has not started yet

                if (_nextTryAfter.TryGetValue(type, out var next) && now < next) continue;

                string details = BuildDigest(type, out string category, out string title, out string actionLink, out int count);
                if (details == null) continue;

                var result = await DispatchNotificationAsync(category, "HIGH", title, type, category, details, actionLink);
                bool delivered = result.FcmSent || result.SynologySent;
                bool noRecipients = result.DeliveryStatus != null && result.DeliveryStatus.StartsWith("Skipped", StringComparison.OrdinalIgnoreCase);

                if (delivered || noRecipients)
                {
                    // "No recipients" counts as handled for the cycle too: it must not be retried every few seconds.
                    lock (_scheduleLock)
                    {
                        if (st.PendingInbound.ContainsKey(type)) st.PendingInbound[type] = new List<string>();
                    }
                    MarkSent(st, type, cycle, now);
                    _attempts.Remove(type);
                    _nextTryAfter.Remove(type);
                    SaveState();
                    int doneNow = SentInCycle(st, type, cycle);
                    string nextInfo = doneNow < slots.Length
                        ? $"next one from {now.Date + SlotsFor(type)[doneNow]:HH:mm}"
                        : $"next cycle starts {now.Date.AddDays(1) + SendWindowStart:yyyy-MM-dd HH:mm}";
                    Log($"[{category}] Digest {doneNow}/{slots.Length} sent for cycle {cycle} ({count} item(s)); {nextInfo}.");
                }
                else
                {
                    int tries = (_attempts.TryGetValue(type, out int t) ? t : 0) + 1;
                    _attempts[type] = tries;
                    if (tries >= Math.Max(1, MaxAttemptsPerCycle))
                    {
                        MarkSent(st, type, cycle, now, giveUp: true); // give up for this cycle, try again tomorrow
                        SaveState();
                        Log($"[{category}] Delivery failed {tries}x - giving up for cycle {cycle}.");
                    }
                    else
                    {
                        _nextTryAfter[type] = now.AddMinutes(Math.Max(1, RetryMinutesAfterFailure));
                        Log($"[{category}] Delivery failed (attempt {tries}) - retrying in {RetryMinutesAfterFailure} min.");
                    }
                }

                await Task.Delay(1500); // never fire several categories back to back
            }
        }

        #endregion

        /// <summary>
        /// Raises an event to every subscriber, isolating failures. A screen that was closed or disposed (the Notification Hub tab,
        /// the dashboard hidden in the tray) must never be able to break a notification that is being sent.
        /// </summary>
        private static void Raise<T>(Action<T> handler, T arg)
        {
            if (handler == null) return;
            foreach (Delegate d in handler.GetInvocationList())
            {
                try { ((Action<T>)d)(arg); }
                catch { /* subscriber is gone or failed: ignore */ }
            }
        }

        public void Log(string msg)
        {
            string line = $"[{DateTime.Now:HH:mm:ss}] {msg}";
            lock (_logLines)
            {
                _logLines.Add(line);
                if (_logLines.Count > 500) _logLines.RemoveRange(0, _logLines.Count - 500);
            }
            Raise(OnLog, line);
        }
    }
}
