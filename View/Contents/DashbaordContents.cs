using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server_HTML_Handler.View.Contents
{
    public partial class DashboardContents : UserControl
    {
        private const string ApacheServiceName = "Apache2.4";
        private const string MySqlServiceName = "mysql";

        public DashboardContents()
        {
            InitializeComponent();

            dgvRoutes.CurrentCellDirtyStateChanged += (s, e) => {
                if (dgvRoutes.IsCurrentCellDirty)
                {
                    dgvRoutes.CommitEdit(DataGridViewDataErrorContexts.Commit);
                }
            };
        }

        // ==========================================
        // Load & Async Initializations
        // ==========================================
        private async void DashboardContents_Load(object sender, EventArgs e)
        {
            // 1. Initial UI setup
            string host = string.IsNullOrEmpty(Properties.Settings.Default.IP) ? GetLocalIpAddress() : Properties.Settings.Default.IP;
            string portStr = string.IsNullOrEmpty(Properties.Settings.Default.Port) ? "80" : Properties.Settings.Default.Port;

            txtIP.Text = host;
            txtServerPort.Text = portStr;
            lblStatusValue.Text = "CHECKING SERVICE...";

            // 2. Fetch server status asynchronously
            await CheckApacheStatusAsync();

            // 3. Populate tabs
            LoadEnvironmentSummary();
            PopulateRoutesGrid();
            PopulateConnectedClientsGrid();
            LoadDatabaseAndMetricsTab();

            // 4. Start auto-refresh timer for live telemetry
            timerTelemetry.Start();
        }

        private string GetLocalIpAddress()
        {
            try
            {
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (ni.OperationalStatus == OperationalStatus.Up && ni.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                    {
                        foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                        {
                            if (ip.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                return ip.Address.ToString();
                            }
                        }
                    }
                }
            }
            catch { }
            return "localhost";
        }

        private async Task CheckApacheStatusAsync()
        {
            bool isRunning = false;
            bool serviceExists = false;

            await Task.Run(() =>
            {
                try
                {
                    if (Process.GetProcessesByName("httpd").Length > 0)
                    {
                        isRunning = true;
                        serviceExists = true;
                    }
                }
                catch { }

                if (!isRunning)
                {
                    string[] candidateNames = { ApacheServiceName, "Apache2.4", "apache", "Apache2", "xamppapache" };
                    foreach (string name in candidateNames.Distinct())
                    {
                        try
                        {
                            using (ServiceController sc = new ServiceController(name))
                            {
                                sc.Refresh();
                                if (sc.Status == ServiceControllerStatus.Running)
                                {
                                    isRunning = true;
                                    serviceExists = true;
                                    break;
                                }
                                else if (sc.Status == ServiceControllerStatus.Stopped || sc.Status == ServiceControllerStatus.Paused)
                                {
                                    serviceExists = true;
                                }
                            }
                        }
                        catch { }
                    }
                }
            });

            if (isRunning)
            {
                txtIP.Enabled = false;
                txtServerPort.Enabled = false;

                lblStatusValue.Text = "RUNNING (Apache XAMPP)";
                lblStatusValue.BackColor = Color.FromArgb(192, 255, 192);
                lblStatusValue.ForeColor = Color.FromArgb(0, 128, 0);

                AppendConsole("[System] Detected XAMPP Apache is currently running.");
            }
            else
            {
                txtIP.Enabled = true;
                txtServerPort.Enabled = true;

                lblStatusValue.Text = serviceExists ? "STOPPED (Apache XAMPP)" : "STOPPED (Service Not Found)";
                lblStatusValue.BackColor = Color.FromArgb(255, 192, 192);
                lblStatusValue.ForeColor = Color.FromArgb(192, 0, 0);
            }
        }

        private void btnRefreshAll_Click(object sender, EventArgs e)
        {
            _ = CheckApacheStatusAsync();
            LoadEnvironmentSummary();
            PopulateRoutesGrid();
            PopulateConnectedClientsGrid();
            LoadDatabaseAndMetricsTab();
        }

        // ==========================================
        // Server Management (Start / Stop)
        // ==========================================
        private void btnStartServer_Click(object sender, EventArgs e)
        {
            string host = txtIP.Text.Trim();
            if (string.IsNullOrEmpty(host)) host = "localhost";

            string port = txtServerPort.Text.Trim();
            if (string.IsNullOrEmpty(port)) port = "80";

            Properties.Settings.Default.IP = host;
            Properties.Settings.Default.Port = port;
            Properties.Settings.Default.Save();

            AppendConsole("[System] Starting XAMPP Apache service...");

            try
            {
                if (Helper.ServerBackupHelper.StartApacheService(out string error, ApacheServiceName))
                {
                    txtIP.Enabled = false;
                    txtServerPort.Enabled = false;

                    lblStatusValue.Text = "RUNNING (Apache XAMPP)";
                    lblStatusValue.BackColor = Color.FromArgb(192, 255, 192);
                    lblStatusValue.ForeColor = Color.FromArgb(0, 128, 0);

                    AppendConsole("[System] XAMPP Apache started successfully.");
                }
                else
                {
                    AppendConsole($"[Error] Failed to start Apache: {error}");
                }
            }
            catch (Exception ex)
            {
                AppendConsole($"[Error] Unexpected error: {ex.Message}");
            }
        }

        private void btnStopServer_Click(object sender, EventArgs e)
        {
            AppendConsole("[System] Stopping XAMPP Apache service...");

            try
            {
                if (Helper.ServerBackupHelper.StopApacheService(out string error))
                {
                    AppendConsole("[System] XAMPP Apache stopped successfully.");
                }
                else
                {
                    AppendConsole($"[Error] Failed to stop Apache: {error}");
                }
            }
            catch (Exception ex)
            {
                AppendConsole($"[Error] Unexpected error: {ex.Message}");
            }
            finally
            {
                txtIP.Enabled = true;
                txtServerPort.Enabled = true;

                lblStatusValue.Text = "STOPPED (Apache XAMPP)";
                lblStatusValue.BackColor = Color.FromArgb(255, 192, 192);
                lblStatusValue.ForeColor = Color.FromArgb(192, 0, 0);
            }
        }

        // ==========================================
        // Environment Summary & Data Helpers
        // ==========================================
        private void LoadEnvironmentSummary()
        {
            Task.Run(() =>
            {
                string phpVersion = Helper.ServerBackupHelper.GetPhpVersion();
                string dbState = Helper.ServerBackupHelper.GetMySqlStatus(MySqlServiceName);
                string htmlPath = Properties.Settings.Default.HTMLPath;
                string diskSpaceStr = Helper.ServerBackupHelper.GetDirectorySizeFormatted(htmlPath);

                this.Invoke(new Action(() =>
                {
                    lblPhpVersion.Text = $"PHP Version: {phpVersion}";
                    lblDbState.Text = $"Database: {dbState}";
                    lblDiskSpace.Text = $"Disk Space Utilized: {diskSpaceStr}";
                }));
            });
        }

        private void PopulateRoutesGrid()
        {
            string rootPath = Properties.Settings.Default.HTMLPath;
            if (string.IsNullOrEmpty(rootPath) || !Directory.Exists(rootPath))
            {
                rootPath = GetFirstExistingFolder(new[] { @"C:\xampp\htdocs", @"D:\xampp\htdocs", @"E:\xampp\htdocs" });
            }

            if (string.IsNullOrEmpty(rootPath) || !Directory.Exists(rootPath))
            {
                AppendConsole("[Error] Cannot populate routes: Root path does not exist.");
                return;
            }

            dgvRoutes.Rows.Clear();

            try
            {
                List<Models.RouteInfo> routes = Helper.ServerBackupHelper.GetRoutes(rootPath);

                foreach (var route in routes)
                {
                    // Read current access restriction state from the target folder's .htaccess
                    string currentVisibility = GetRouteVisibility(route.PhysicalLocation);

                    dgvRoutes.Rows.Add(
                        route.RouteName,
                        route.TargetFile,
                        route.UrlPath,
                        route.PhysicalLocation,
                        route.StatusAction ?? "Online",
                        "Open",
                        currentVisibility // <--- Added Visibility Column State
                    );
                }

                AppendConsole($"[System] Loaded {dgvRoutes.Rows.Count} route(s) into grid.");
            }
            catch (Exception ex)
            {
                AppendConsole($"[Error] Failed to load routes: {ex.Message}");
            }
        }

        private void dgvRoutes_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            string colName = dgvRoutes.Columns[e.ColumnIndex].Name;
            var row = dgvRoutes.Rows[e.RowIndex];

            // 1. OPEN BUTTON CLICK
            if (colName.Equals("colActionOpen", StringComparison.OrdinalIgnoreCase))
            {
                string host = string.IsNullOrWhiteSpace(txtIP.Text) ? "localhost" : txtIP.Text.Trim();
                string port = txtServerPort.Text.Trim();
                string portSuffix = (port == "80" || string.IsNullOrEmpty(port)) ? "" : $":{port}";

                string urlPath = row.Cells[2]?.Value?.ToString() ?? $"/{row.Cells[0].Value}/";
                string fullUrl = $"http://{host}{portSuffix}{urlPath}";

                try
                {
                    Process.Start(new ProcessStartInfo { FileName = fullUrl, UseShellExecute = true });
                    AppendConsole($"[System] Launched website in browser: {fullUrl}");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to open website: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            // 2. STATUS BUTTON CLICK (Online -> Maintenance -> Offline)
            else if (colName.Equals("colActionStatus", StringComparison.OrdinalIgnoreCase))
            {
                string physicalPath = row.Cells[3]?.Value?.ToString();
                string currentStatus = row.Cells[e.ColumnIndex].Value?.ToString() ?? "Online";

                string nextStatus = currentStatus switch
                {
                    "Online" => "Maintenance",
                    "Maintenance" => "Offline",
                    "Offline" => "Online",
                    _ => "Online"
                };

                if (ApplyRouteStatus(physicalPath, nextStatus))
                {
                    row.Cells[e.ColumnIndex].Value = nextStatus;
                    dgvRoutes.UpdateCellValue(e.ColumnIndex, e.RowIndex);
                    AppendConsole($"[System] Route '{row.Cells[0].Value}' state changed to: {nextStatus}");
                }
            }
            // 3. VISIBILITY BUTTON CLICK (Public -> Hidden -> Local Only)
            else if (colName.Equals("colActionVisibility", StringComparison.OrdinalIgnoreCase) || colName.Equals("Visibility", StringComparison.OrdinalIgnoreCase))
            {
                string physicalPath = row.Cells[3]?.Value?.ToString();
                string currentVisibility = row.Cells[e.ColumnIndex].Value?.ToString() ?? "Public";

                string nextVisibility = currentVisibility switch
                {
                    "Public" => "Hidden",
                    "Hidden" => "Local Only",
                    "Local Only" => "Public",
                    _ => "Public"
                };

                if (ApplyRouteVisibility(physicalPath, nextVisibility))
                {
                    row.Cells[e.ColumnIndex].Value = nextVisibility;
                    dgvRoutes.UpdateCellValue(e.ColumnIndex, e.RowIndex);
                    AppendConsole($"[System] Route '{row.Cells[0].Value}' visibility set to: {nextVisibility}");
                }
            }
        }
        private string GetRouteVisibility(string physicalPath)
        {
            if (string.IsNullOrEmpty(physicalPath) || !Directory.Exists(physicalPath)) return "Public";

            string htaccessPath = Path.Combine(physicalPath, ".htaccess");
            if (!File.Exists(htaccessPath)) return "Public";

            try
            {
                string content = File.ReadAllText(htaccessPath);
                if (content.Contains("Require all denied")) return "Hidden";
                if (content.Contains("Require local") || content.Contains("Require ip 127.0.0.1")) return "Local Only";
            }
            catch { }

            return "Public";
        }

        private bool ApplyRouteVisibility(string physicalPath, string newVisibility)
        {
            if (string.IsNullOrEmpty(physicalPath) || !Directory.Exists(physicalPath))
            {
                MessageBox.Show("Invalid route physical location.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            string htaccessPath = Path.Combine(physicalPath, ".htaccess");

            try
            {
                string content = File.Exists(htaccessPath) ? File.ReadAllText(htaccessPath) : "";

                // Strip existing visibility blocks
                content = Regex.Replace(content, @"# --- VISIBILITY BLOCK START ---[\s\S]*?# --- VISIBILITY BLOCK END ---", "").Trim();

                string visibilityBlock = "";
                switch (newVisibility)
                {
                    case "Hidden":
                        visibilityBlock = "\n# --- VISIBILITY BLOCK START ---\nRequire all denied\n# --- VISIBILITY BLOCK END ---\n";
                        break;
                    case "Local Only":
                        visibilityBlock = "\n# --- VISIBILITY BLOCK START ---\nRequire local\n# --- VISIBILITY BLOCK END ---\n";
                        break;
                    case "Public":
                    default:
                        // No restrictive directives needed for public view
                        break;
                }

                File.WriteAllText(htaccessPath, (content + visibilityBlock).Trim());
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to apply visibility setting: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private bool ApplyRouteStatus(string physicalPath, string newStatus)
        {
            if (Helper.ServerBackupHelper.SetRouteStatus(physicalPath, newStatus, out string errorMessage))
            {
                return true;
            }

            MessageBox.Show($"Failed to update .htaccess: {errorMessage}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }

        // ==========================================
        // Client Monitoring & Logs
        // ==========================================
        private void PopulateConnectedClientsGrid()
        {
            string logPath = GetFirstExistingFile(new[] {
                @"C:\xampp\apache\logs\access.log",
                @"D:\xampp\apache\logs\access.log",
                @"E:\xampp\apache\logs\access.log"
            });

            if (string.IsNullOrEmpty(logPath)) return;

            dgvClients.Rows.Clear();

            try
            {
                var clients = Helper.ServerBackupHelper.GetConnectedClients(logPath);
                foreach (var client in clients)
                {
                    dgvClients.Rows.Add(client.IP, client.Hostname, client.ConnectedSince, client.Status, client.UserAgent);
                }
            }
            catch (Exception ex)
            {
                AppendConsole($"[Error] Reading access log: {ex.Message}");
            }
        }

        private void LoadDatabaseAndMetricsTab()
        {
            Task.Run(() =>
            {
                var metrics = Helper.ServerBackupHelper.GetSystemMetrics();

                this.Invoke(new Action(() =>
                {
                    lblDbPoolStatus.Text = metrics.DbPoolStatus;
                    lblCpuLoad.Text = metrics.CpuLoad;
                    lblRamUtil.Text = metrics.RamUtil;
                }));
            });

            RefreshFullLogs();
        }

        private void RefreshFullLogs()
        {
            string logPath = GetFirstExistingFile(new[] {
                @"C:\xampp\apache\logs\error.log",
                @"C:\xampp\apache\logs\access.log",
                @"D:\xampp\apache\logs\error.log"
            });

            if (string.IsNullOrEmpty(logPath)) return;

            string filterText = txtLogSearch != null ? txtLogSearch.Text.Trim() : "";
            string logContent = Helper.ServerBackupHelper.ReadAndFilterLogs(logPath, filterText);

            if (txtFullLogs != null)
            {
                txtFullLogs.Text = logContent;
                txtFullLogs.SelectionStart = txtFullLogs.Text.Length;
                txtFullLogs.ScrollToCaret();
            }
        }

        private void TimerTelemetry_Tick(object sender, EventArgs e)
        {
            if (tabControlMain.SelectedTab == tabPageDatabaseLogs)
            {
                Task.Run(() =>
                {
                    var metrics = Helper.ServerBackupHelper.GetSystemMetrics();
                    this.Invoke(new Action(() =>
                    {
                        lblDbPoolStatus.Text = metrics.DbPoolStatus;
                        lblCpuLoad.Text = metrics.CpuLoad;
                        lblRamUtil.Text = metrics.RamUtil;
                    }));
                });

                if (chkAutoRefreshLogs.Checked)
                {
                    RefreshFullLogs();
                }
            }
        }

        // ==========================================
        // Quick Action Buttons
        // ==========================================
        private void btnOpenBrowser_Click(object sender, EventArgs e)
        {
            try
            {
                string host = string.IsNullOrWhiteSpace(txtIP.Text) ? "localhost" : txtIP.Text.Trim();
                string port = txtServerPort.Text.Trim();
                string portSuffix = (port == "80" || string.IsNullOrEmpty(port)) ? "" : $":{port}";

                string url = $"http://{host}{portSuffix}/";
                Process.Start(new ProcessStartInfo { FileName = url, UseShellExecute = true });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to open browser: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            string htmlPath = Properties.Settings.Default.HTMLPath;
            if (!string.IsNullOrEmpty(htmlPath) && Directory.Exists(htmlPath))
            {
                Process.Start(new ProcessStartInfo { FileName = htmlPath, UseShellExecute = true, Verb = "open" });
            }
            else
            {
                MessageBox.Show("Shared website path is not configured or does not exist.", "Path Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void btnOpenIni_Click(object sender, EventArgs e) => OpenConfigFile(@"apache\php\php.ini", "php.ini");
        private void btnHTTPD_Click(object sender, EventArgs e) => OpenConfigFile(@"apache\conf\httpd.conf", "httpd.conf");

        private void OpenConfigFile(string relativePath, string fileName)
        {
            string path = GetFirstExistingFile(new[] {
                Path.Combine(@"C:\xampp", relativePath),
                Path.Combine(@"D:\xampp", relativePath)
            });

            if (string.IsNullOrEmpty(path))
            {
                using (OpenFileDialog ofd = new OpenFileDialog { Title = $"Locate {fileName}", Filter = "Config Files|*.*" })
                {
                    if (ofd.ShowDialog() == DialogResult.OK) path = ofd.FileName;
                    else return;
                }
            }

            try { Process.Start(new ProcessStartInfo { FileName = path, UseShellExecute = true }); }
            catch (Exception ex) { MessageBox.Show($"Could not open file: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnDBbase_Click(object sender, EventArgs e)
        {
            string host = string.IsNullOrWhiteSpace(txtIP.Text) ? "localhost" : txtIP.Text.Trim();
            Process.Start(new ProcessStartInfo { FileName = $"http://{host}/phpmyadmin/", UseShellExecute = true });
        }

        private void btnSaveConfig_Click(object sender, EventArgs e)
        {
            string htmlPath = Properties.Settings.Default.HTMLPath;
            if (string.IsNullOrEmpty(htmlPath) || !Directory.Exists(htmlPath)) return;

            string htaccessPath = Path.Combine(htmlPath, ".htaccess");
            try
            {
                if (chkMaintenanceMode.Checked)
                {
                    string maintenanceBlock = "\n# --- MAINTENANCE MODE START ---\n# Site in Maintenance\n# --- MAINTENANCE MODE END ---\n";
                    string content = File.Exists(htaccessPath) ? File.ReadAllText(htaccessPath) : "";
                    if (!content.Contains("MAINTENANCE MODE START")) File.WriteAllText(htaccessPath, content + maintenanceBlock);
                    AppendConsole("[System] Maintenance Mode enabled.");
                }
                else
                {
                    if (File.Exists(htaccessPath))
                    {
                        string content = File.ReadAllText(htaccessPath);
                        content = Regex.Replace(content, @"# --- MAINTENANCE MODE START ---[\s\S]*?# --- MAINTENANCE MODE END ---", "");
                        File.WriteAllText(htaccessPath, content.Trim());
                    }
                    AppendConsole("[System] Maintenance Mode disabled.");
                }
            }
            catch (Exception ex) { MessageBox.Show($"Failed to save config: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void btnFilterLogs_Click(object sender, EventArgs e) => RefreshFullLogs();

        private void txtLogSearch_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                RefreshFullLogs();
            }
        }

        private void AppendConsole(string message)
        {
            if (txtConsoleOutput.InvokeRequired)
            {
                txtConsoleOutput.Invoke(new Action<string>(AppendConsole), message);
            }
            else
            {
                txtConsoleOutput.AppendText(Environment.NewLine + message);
            }
        }

        private string GetFirstExistingFile(IEnumerable<string> paths) => paths.FirstOrDefault(File.Exists) ?? "";
        private string GetFirstExistingFolder(IEnumerable<string> paths) => paths.FirstOrDefault(Directory.Exists) ?? "";
    }
}