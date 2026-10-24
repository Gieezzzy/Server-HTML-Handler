using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.ServiceProcess;
using System.Threading;
using System.Threading.Tasks;

namespace Server_HTML_Handler.Services
{
    /// <summary>
    /// Keeps the web server (the XAMPP Apache) working while the handler is up, so Apache never has to be stopped and started by hand:
    ///  - on open: starts Apache when the website is not being served; restarts it when it does not answer, or when its configuration
    ///    (httpd.conf, extra\*.conf, php.ini) was changed after Apache started, so the web always runs with the current settings;
    ///  - afterwards: every CheckIntervalSeconds starts it again if it died or stopped answering.
    /// Only the Apache at ApacheRoot is ever stopped or started: other programs that also run an "httpd.exe" (for example the
    /// PostgreSQL PEM web server on port 8080) are recognised by their file path and left alone.
    /// A configuration test (httpd -t) runs before any start, so a broken config is reported instead of looping restarts.
    /// </summary>
    public sealed class ApacheWatchdogService
    {
        private static readonly Lazy<ApacheWatchdogService> _instance = new Lazy<ApacheWatchdogService>(() => new ApacheWatchdogService());
        public static ApacheWatchdogService Instance => _instance.Value;

        public string ApacheRoot { get; set; } = @"C:\xampp\apache";
        /// <summary>Any HTTP answer (even 403/404) counts as healthy; no answer means Apache is hung or not listening.</summary>
        public string HealthUrl { get; set; } = "http://127.0.0.1/";
        public int CheckIntervalSeconds { get; set; } = 60;
        /// <summary>At startup, restart a running Apache whose configuration is newer than the process.</summary>
        public bool RestartWhenConfigIsNewer { get; set; } = true;
        public int MaxRestartsPerHour { get; set; } = 3;

        public string LastStatus { get; private set; } = "Not checked yet";
        public event Action<string> OnLog;

        private Timer _timer;
        private int _busy;
        private readonly List<DateTime> _restarts = new List<DateTime>();

        private ApacheWatchdogService() { }

        private string HttpdExe => Path.Combine(ApacheRoot, "bin", "httpd.exe");

        public void Start()
        {
            if (_timer != null) return;

            Task.Run(async () => await RunGuardedAsync(true));
            int period = Math.Max(15, CheckIntervalSeconds);
            _timer = new Timer(async _ => await RunGuardedAsync(false), null, TimeSpan.FromSeconds(period), TimeSpan.FromSeconds(period));
        }

        public void Stop()
        {
            _timer?.Dispose();
            _timer = null;
        }

        private async Task RunGuardedAsync(bool atStartup)
        {
            if (Interlocked.Exchange(ref _busy, 1) == 1) return;   // never overlap two checks
            try
            {
                await EnsureApacheAsync(atStartup);
            }
            catch (Exception ex)
            {
                LastStatus = "Apache check failed: " + ex.Message;
                Log(LastStatus);
            }
            finally
            {
                Interlocked.Exchange(ref _busy, 0);
            }
        }

        /// <summary>Makes sure the website is served. Safe to call at any time (e.g. from a "Check now" button).</summary>
        public async Task EnsureApacheAsync(bool atStartup)
        {
            if (!File.Exists(HttpdExe))
            {
                LastStatus = $"Apache not found at {HttpdExe}";
                Log(LastStatus);
                return;
            }

            Process[] ours = GetOurApache();
            bool healthy = await IsHealthyAsync();

            if (healthy)
            {
                if (ours.Length > 0 && atStartup && RestartWhenConfigIsNewer && IsConfigNewerThanApache(ours, out string changed))
                {
                    await RestartApacheAsync($"its configuration changed after it started ({changed})");
                    return;
                }

                LastStatus = ours.Length > 0 ? "Apache is running and answering." : "The web server is answering on " + HealthUrl + ".";
                return;
            }

            if (ours.Length > 0)
            {
                await RestartApacheAsync($"it is running but does not answer on {HealthUrl}");
                return;
            }

            int port = PortOfHealthUrl();
            if (IsPortListening(port))
            {
                LastStatus = $"Port {port} is in use by another program that is not serving the website. Apache was not started.";
                Log(LastStatus);
                return;
            }

            await StartApacheAsync("it was not running");
        }

        // ---------------------------------------------------------------- which processes are ours

        /// <summary>The handler's own detection (Helper.ServerBackupHelper): only the XAMPP Apache, never other products' httpd.exe.</summary>
        private Process[] GetOurApache() => Helper.ServerBackupHelper.GetXamppApacheProcesses().ToArray();

        // ---------------------------------------------------------------- health

        private int PortOfHealthUrl()
        {
            try { return new Uri(HealthUrl).Port; } catch { return 80; }
        }

        private static bool IsPortListening(int port)
        {
            try { return IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners().Any(l => l.Port == port); }
            catch { return false; }
        }

        private async Task<bool> IsHealthyAsync()
        {
            for (int attempt = 0; attempt < 2; attempt++)
            {
                try
                {
                    using (var handler = new HttpClientHandler { UseProxy = false, AllowAutoRedirect = false })
                    using (var client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(6) })
                    {
                        using (var resp = await client.GetAsync(HealthUrl, HttpCompletionOption.ResponseHeadersRead))
                        {
                            return true;   // any HTTP status = Apache answered
                        }
                    }
                }
                catch
                {
                    await Task.Delay(1500);
                }
            }
            return false;
        }

        private bool IsConfigNewerThanApache(Process[] procs, out string changedFile)
        {
            changedFile = "";
            try
            {
                // The oldest httpd process is the parent that loaded the configuration.
                DateTime started = procs.Select(p => { try { return p.StartTime; } catch { return DateTime.MaxValue; } }).Min();
                if (started == DateTime.MaxValue) return false;   // start time unreadable: do not guess

                var files = new List<string>();
                string conf = Path.Combine(ApacheRoot, "conf");
                if (Directory.Exists(conf))
                {
                    files.Add(Path.Combine(conf, "httpd.conf"));
                    string extra = Path.Combine(conf, "extra");
                    if (Directory.Exists(extra)) files.AddRange(Directory.GetFiles(extra, "*.conf"));
                }
                files.Add(Path.GetFullPath(Path.Combine(ApacheRoot, "..", "php", "php.ini")));

                foreach (string f in files.Where(File.Exists))
                {
                    if (File.GetLastWriteTime(f) > started.AddSeconds(5))
                    {
                        changedFile = Path.GetFileName(f);
                        return true;
                    }
                }
            }
            catch
            {
                // an unreadable file is not a reason to restart
            }
            return false;
        }

        // ---------------------------------------------------------------- start / restart

        private async Task RestartApacheAsync(string reason)
        {
            lock (_restarts)
            {
                _restarts.RemoveAll(t => (DateTime.Now - t).TotalHours >= 1);
                if (_restarts.Count >= Math.Max(1, MaxRestartsPerHour))
                {
                    LastStatus = $"Apache needs a restart ({reason}) but {_restarts.Count} restarts already happened this hour - not restarting again.";
                    Log(LastStatus);
                    return;
                }
                _restarts.Add(DateTime.Now);
            }

            // Test the configuration first: restarting into a broken config would take the web down.
            string configError = await TestConfigAsync();
            if (configError != null)
            {
                LastStatus = "Apache NOT restarted - configuration error: " + configError;
                Log(LastStatus);
                return;
            }

            Log($"Restarting Apache because {reason}.");
            try
            {
                using (var sc = new ServiceController("Apache2.4"))
                {
                    sc.Refresh();
                    if (sc.Status == ServiceControllerStatus.Running) { sc.Stop(); sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(15)); }
                }
            }
            catch { /* not a service, or not allowed: fall through to ending the process */ }
            foreach (var p in GetOurApache())
            {
                try { p.Kill(); p.WaitForExit(3000); } catch { }
            }
            await Task.Delay(800);
            if (GetOurApache().Length > 0)
            {
                LastStatus = "Apache could not be stopped (it was probably started as Administrator). Stop it with the Stop button of the Dashboard tab (it asks for Administrator), then it will be started again.";
                Log(LastStatus);
                return;
            }
            await LaunchAndWaitAsync("restarted: " + reason);
        }

        private async Task StartApacheAsync(string reason)
        {
            string configError = await TestConfigAsync();
            if (configError != null)
            {
                LastStatus = "Apache NOT started - configuration error: " + configError;
                Log(LastStatus);
                return;
            }

            Log($"Starting Apache because {reason}.");
            await LaunchAndWaitAsync("started: " + reason);
        }

        /// <summary>Starts the XAMPP "Apache2.4" Windows service the way the handler's Start button does, without any prompt. False if that is not possible.</summary>
        private bool TryStartService()
        {
            try
            {
                using (var sc = new ServiceController("Apache2.4"))
                {
                    sc.Refresh();
                    if (sc.Status == ServiceControllerStatus.Running) return true;
                    if (sc.Status != ServiceControllerStatus.Stopped) return false;
                    sc.Start();
                    sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(15));
                    return true;
                }
            }
            catch
            {
                return false;   // not installed, or this user may not start services (needs Administrator)
            }
        }

        private async Task LaunchAndWaitAsync(string what)
        {
            if (TryStartService())
            {
                Log("Apache2.4 service started.");
            }
            else
            {
                try
                {
                    Process.Start(new ProcessStartInfo
                    {
                        FileName = HttpdExe,
                        WorkingDirectory = ApacheRoot,
                        UseShellExecute = false,
                        CreateNoWindow = true,
                        WindowStyle = ProcessWindowStyle.Hidden
                    });
                }
                catch (Exception ex)
                {
                    LastStatus = "Apache could not be launched: " + ex.Message;
                    Log(LastStatus);
                    return;
                }
            }

            for (int i = 0; i < 12; i++)   // up to ~24 s
            {
                await Task.Delay(2000);
                if (await IsHealthyAsync())
                {
                    LastStatus = "Apache " + what;
                    Log(LastStatus + " - answering on " + HealthUrl);
                    return;
                }
            }

            LastStatus = "Apache was launched but is not answering on " + HealthUrl + " (check the Apache error log).";
            Log(LastStatus);
        }

        /// <summary>Runs "httpd -t". Returns null when the configuration is valid, otherwise the error text.</summary>
        public async Task<string> TestConfigAsync()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = HttpdExe,
                    Arguments = "-t",
                    WorkingDirectory = ApacheRoot,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardError = true,
                    RedirectStandardOutput = true
                };
                using (var p = Process.Start(psi))
                {
                    Task<string> err = p.StandardError.ReadToEndAsync();
                    Task<string> outp = p.StandardOutput.ReadToEndAsync();
                    if (!p.WaitForExit(15000))
                    {
                        try { p.Kill(); } catch { }
                        return "httpd -t did not finish";
                    }
                    string text = (await err + " " + await outp).Trim();
                    if (p.ExitCode == 0) return null;
                    return string.IsNullOrWhiteSpace(text) ? "httpd -t failed (exit code " + p.ExitCode + ")" : text.Replace("\r", " ").Replace("\n", " ");
                }
            }
            catch (Exception ex)
            {
                return "could not run httpd -t: " + ex.Message;
            }
        }

        private void Log(string msg)
        {
            string line = $"[{DateTime.Now:HH:mm:ss}] [Apache] {msg}";
            try { OnLog?.Invoke(line); } catch { }
            try
            {
                string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ServerHTMLHandler");
                Directory.CreateDirectory(dir);
                File.AppendAllText(Path.Combine(dir, "apache-watchdog.log"), $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {msg}{Environment.NewLine}");
            }
            catch { }
        }
    }
}
