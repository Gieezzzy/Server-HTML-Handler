using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Server_HTML_Handler.Services
{
    public class KonginReadingItem
    {
        public long Id { get; set; }
        public int DeviceId { get; set; }
        public string Location { get; set; }
        public DateTime Timestamp { get; set; }
        public double TemperatureCelsius { get; set; }
        public double HumidityPercent { get; set; }

        public const double TempStandard = 23.0;
        public const double TempTolerance = 2.0;
        public const double TempMin = TempStandard - TempTolerance; // 21.0
        public const double TempMax = TempStandard + TempTolerance; // 25.0

        public const double HumStandard = 55.0;
        public const double HumTolerance = 10.0;
        public const double HumMin = HumStandard - HumTolerance; // 45.0
        public const double HumMax = HumStandard + HumTolerance; // 65.0

        public bool IsTempTooLow => TemperatureCelsius < TempMin;
        public bool IsTempTooHigh => TemperatureCelsius > TempMax;
        public bool IsTempAlert => IsTempTooLow || IsTempTooHigh;

        public bool IsHumTooLow => HumidityPercent < HumMin;
        public bool IsHumTooHigh => HumidityPercent > HumMax;
        public bool IsHumAlert => IsHumTooLow || IsHumTooHigh;

        public bool IsAlert => IsTempAlert || IsHumAlert;

        public string DisplayTemp => $"{TemperatureCelsius:F1}°C";
        public string DisplayHum => $"{HumidityPercent:F1}%";
        public string DisplayTime => Timestamp.ToString("yyyy-MM-dd HH:mm:ss");

        public string AlertDescription
        {
            get
            {
                var issues = new List<string>();
                if (IsTempTooLow) issues.Add($"Temperature too low ({TemperatureCelsius:F1}°C < {TempMin:F1}°C)");
                else if (IsTempTooHigh) issues.Add($"Temperature too high ({TemperatureCelsius:F1}°C > {TempMax:F1}°C)");

                if (IsHumTooLow) issues.Add($"Humidity too low ({HumidityPercent:F1}% < {HumMin:F0}%)");
                else if (IsHumTooHigh) issues.Add($"Humidity too high ({HumidityPercent:F1}% > {HumMax:F0}%)");

                return issues.Count > 0 ? string.Join(", ", issues) : "Normal";
            }
        }
    }

    public class KonginConfigState
    {
        public int AlertIntervalMinutes { get; set; } = 60;
    }

    /// <summary>
    /// Service monitoring Kongin datalogger readings on dtp-qclab03\sqlexpress.
    /// Checks standards: Temp 23 ± 2°C (21-25°C), Humidity 55 ± 10% (45-65%).
    /// Sends out-of-spec alert notifications via Synology & Android push at a custom interval (default 60 min).
    /// </summary>
    public class KonginTrackerService
    {
        private static readonly Lazy<KonginTrackerService> _instance =
            new Lazy<KonginTrackerService>(() => new KonginTrackerService());

        public static KonginTrackerService Instance => _instance.Value;

        public string SqlConnectionString { get; set; } =
            @"Server=dtp-qclab03\sqlexpress;Database=datalogger;User Id=processaudit_user;Password=processaudit1234*;TrustServerCertificate=True;Connect Timeout=5;";

        public const string CategoryEnvironmentAlert = "ENVIRONMENT_ALERT";

        /// <summary>
        /// Custom alert dispatch interval in minutes (default 60 minutes).
        /// </summary>
        public int AlertIntervalMinutes { get; set; } = 60;

        public KonginReadingItem LatestReading { get; private set; }
        public DateTime LastCheckedTime { get; private set; } = DateTime.MinValue;
        public DateTime LastAlertDispatchedTime { get; private set; } = DateTime.MinValue;

        public event Action<KonginReadingItem> OnReadingUpdated;
        public event Action<string> OnLog;

        private System.Threading.Timer _timer;
        private bool _isChecking = false;

        private static string ConfigFilePath =>
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ServerHTMLHandler", "kongin-config.json");

        public KonginTrackerService()
        {
            LoadConfig();
            // Auto start background check every 60 seconds
            _timer = new System.Threading.Timer(TimerCallback, null, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(60));
        }

        public void LoadConfig()
        {
            try
            {
                if (File.Exists(ConfigFilePath))
                {
                    string json = File.ReadAllText(ConfigFilePath);
                    var state = JsonConvert.DeserializeObject<KonginConfigState>(json);
                    if (state != null && state.AlertIntervalMinutes > 0)
                    {
                        AlertIntervalMinutes = state.AlertIntervalMinutes;
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"LoadConfig error: {ex.Message}");
            }
        }

        public void SaveConfig()
        {
            try
            {
                var state = new KonginConfigState { AlertIntervalMinutes = AlertIntervalMinutes };
                string dir = Path.GetDirectoryName(ConfigFilePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(ConfigFilePath, JsonConvert.SerializeObject(state, Formatting.Indented));
                Log($"Configuration saved: AlertInterval = {AlertIntervalMinutes} minutes");
            }
            catch (Exception ex)
            {
                Log($"SaveConfig error: {ex.Message}");
            }
        }

        public void Log(string message)
        {
            string entry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [KonginTracker] {message}";
            try
            {
                OnLog?.Invoke(entry);
                NotificationDispatcherService.Instance.Log(entry);
            }
            catch { }
        }

        private async void TimerCallback(object state)
        {
            if (_isChecking) return;
            _isChecking = true;
            try
            {
                await CheckLatestReadingAndAlertAsync();
            }
            catch (Exception ex)
            {
                Log($"Poll error: {ex.Message}");
            }
            finally
            {
                _isChecking = false;
            }
        }

        public async Task<KonginReadingItem> CheckLatestReadingAndAlertAsync()
        {
            KonginReadingItem item = null;
            try
            {
                using (var conn = new SqlConnection(SqlConnectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand(@"
                        SELECT TOP 1 id, device_id, ISNULL(station_id, 'QC Lab'), [timestamp], temperature_c, humidity_pct
                        FROM dbo.readings WITH (NOLOCK)
                        ORDER BY [timestamp] DESC", conn))
                    {
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                item = new KonginReadingItem
                                {
                                    Id = reader.GetInt64(0),
                                    DeviceId = reader.GetInt32(1),
                                    Location = reader.GetString(2),
                                    Timestamp = reader.GetDateTime(3),
                                    TemperatureCelsius = reader.GetDouble(4),
                                    HumidityPercent = reader.GetDouble(5)
                                };
                            }
                        }
                    }
                }

                if (item != null)
                {
                    LatestReading = item;
                    LastCheckedTime = DateTime.Now;
                    try { OnReadingUpdated?.Invoke(item); } catch { }

                    // Evaluate alert condition: Temp 23+-2 (21-25), Humidity 55+-10 (45-65)
                    if (item.IsAlert)
                    {
                        int interval = AlertIntervalMinutes > 0 ? AlertIntervalMinutes : 60;

                        // Freshness check: if reading timestamp has lapsed beyond the interval or hour (e.g. from 12 o'clock but now 1 PM), do not send stale alert
                        TimeSpan readingAge = DateTime.Now - item.Timestamp;
                        if (readingAge < TimeSpan.Zero) readingAge = TimeSpan.Zero;

                        if (readingAge.TotalMinutes > interval)
                        {
                            Log($"Skipping stale alert for reading #{item.Id} from {item.DisplayTime} (age {readingAge.TotalMinutes:F1} min > {interval} min interval).");
                        }
                        else if (DateTime.Now - LastAlertDispatchedTime >= TimeSpan.FromMinutes(interval))
                        {
                            await DispatchEnvironmentAlertAsync(item);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"CheckLatestReading failed: {ex.Message}");
            }
            return item;
        }

        public async Task<List<KonginReadingItem>> GetRecentReadingsAsync(int limit = 100)
        {
            var list = new List<KonginReadingItem>();
            try
            {
                using (var conn = new SqlConnection(SqlConnectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = new SqlCommand(@"
                        SELECT TOP (@lim) id, device_id, ISNULL(station_id, 'QC Lab'), [timestamp], temperature_c, humidity_pct
                        FROM dbo.readings WITH (NOLOCK)
                        ORDER BY [timestamp] DESC", conn))
                    {
                        cmd.Parameters.AddWithValue("@lim", limit);
                        using (var reader = await cmd.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                list.Add(new KonginReadingItem
                                {
                                    Id = reader.GetInt64(0),
                                    DeviceId = reader.GetInt32(1),
                                    Location = reader.GetString(2),
                                    Timestamp = reader.GetDateTime(3),
                                    TemperatureCelsius = reader.GetDouble(4),
                                    HumidityPercent = reader.GetDouble(5)
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"GetRecentReadings failed: {ex.Message}");
            }
            return list;
        }

        public async Task DispatchEnvironmentAlertAsync(KonginReadingItem item)
        {
            if (item == null) return;
            LastAlertDispatchedTime = DateTime.Now;

            string title = $"⚠️ Environment Alert: {item.AlertDescription}";
            string details = $"[ENVIRONMENT ALERT - QC LAB]\n" +
                             $"Timestamp: {item.DisplayTime}\n" +
                             $"Location: {item.Location} (Device #{item.DeviceId})\n" +
                             $"Current Temperature: {item.DisplayTemp} (Standard: 23.0 ± 2.0°C)\n" +
                             $"Current Humidity: {item.DisplayHum} (Standard: 55 ± 10%)\n" +
                             $"Status: {item.AlertDescription}\n" +
                             $"⚠️ Immediate environmental conditioning required! Alert repeats every {AlertIntervalMinutes} minute(s) while out of range.";

            Log($"Dispatching environment alert: {item.AlertDescription}");

            await NotificationDispatcherService.Instance.DispatchNotificationAsync(
                "Environment Alert",
                "HIGH",
                title,
                CategoryEnvironmentAlert,
                "Environment Alert",
                details,
                "KONGIN_TRACKER"
            );
        }
    }
}
