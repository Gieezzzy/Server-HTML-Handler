using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server_HTML_Handler.View.Contents
{
    public partial class ServerControlUserControl : UserControl
    {
        private string GetDbConnectionString(string dbName = "")
        {
            string baseConn = "Server=localhost;Uid=master;Pwd=M@$t3r;";
            return string.IsNullOrEmpty(dbName) ? baseConn : $"{baseConn}Database={dbName};";
        }

        public ServerControlUserControl()
        {
            InitializeComponent();
            if (comboDvwaDiff.Items.Count > 0) comboDvwaDiff.SelectedIndex = 0;
        }

        private void ServerControlUserControl_Load(object sender, EventArgs e)
        {
            RefreshAllStatus();
        }

        private void BtnRefreshHealth_Click(object sender, EventArgs e)
        {
            RefreshAllStatus();
        }

        // ==========================================
        // Real-Time Health Checking Engine
        // ==========================================
        private async void RefreshAllStatus()
        {
            lblDvwaStatus.Text = "Status: Checking..."; lblDvwaStatus.ForeColor = Color.Gray;
            lblJuiceStatus.Text = "Status: Checking..."; lblJuiceStatus.ForeColor = Color.Gray;
            lblGoatStatus.Text = "Status: Checking..."; lblGoatStatus.ForeColor = Color.Gray;
            lblMutillidaeStatus.Text = "Status: Checking..."; lblMutillidaeStatus.ForeColor = Color.Gray;
            lblBwappStatus.Text = "Status: Checking..."; lblBwappStatus.ForeColor = Color.Gray;

            Task<string> dvwaLevelTask = GetDvwaSecurityLevelAsync();

            await Task.WhenAll(
                CheckEndpointAsync("http://localhost/dvwa/", lblDvwaStatus),
                CheckEndpointAsync("http://localhost:3000/", lblJuiceStatus),
                CheckEndpointAsync("http://localhost:8080/WebGoat", lblGoatStatus),
                CheckEndpointAsync("http://localhost/mutillidae/", lblMutillidaeStatus),
                CheckEndpointAsync("http://localhost/bWAPP/", lblBwappStatus),
                dvwaLevelTask
            );

            // Sync dropdown with active DVWA level
            string currentLevel = await dvwaLevelTask;
            SetDropdownSelectedValue(comboDvwaDiff, currentLevel);
        }

        private async Task<string> GetDvwaSecurityLevelAsync()
        {
            try
            {
                using (var conn = new MySqlConnection(GetDbConnectionString("dvwa")))
                {
                    await conn.OpenAsync();
                    string sql = "SELECT security FROM dvwa_security WHERE id = 1 LIMIT 1;";
                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        var result = await cmd.ExecuteScalarAsync();
                        if (result != null && result != DBNull.Value)
                        {
                            return result.ToString().ToLower();
                        }
                    }
                }
            }
            catch
            {
                // Fallback default if database table isn't initialized yet
            }
            return "low";
        }

        private void SetDropdownSelectedValue(ComboBox comboBox, string value)
        {
            if (this.IsDisposed || comboBox == null) return;

            for (int i = 0; i < comboBox.Items.Count; i++)
            {
                if (comboBox.Items[i].ToString().Equals(value, StringComparison.OrdinalIgnoreCase))
                {
                    comboBox.SelectedIndex = i;
                    break;
                }
            }
        }
        private async Task CheckEndpointAsync(string url, Label targetLabel)
        {
            bool isOnline = await Task.Run(() =>
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
                    request.Timeout = 2000;
                    request.Method = "HEAD";
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        return response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Redirect;
                    }
                }
                catch { return false; }
            });

            if (this.IsDisposed) return;

            this.Invoke(new Action(() =>
            {
                if (isOnline)
                {
                    targetLabel.Text = "Status: 🟢 ONLINE";
                    targetLabel.ForeColor = Color.LimeGreen;
                }
                else
                {
                    targetLabel.Text = "Status: 🔴 OFFLINE";
                    targetLabel.ForeColor = Color.IndianRed;
                }
            }));
        }

        // ==========================================
        // Event Handlers
        // ==========================================
        private void BtnDvwaLaunch_Click(object sender, EventArgs e) => OpenUrl("http://localhost/dvwa/");
        private void BtnDvwaReset_Click(object sender, EventArgs e) => ResetPhpDatabase("dvwa");
        private void BtnDvwaSetDiff_Click(object sender, EventArgs e) => SetDvwaDifficulty(comboDvwaDiff.SelectedItem?.ToString() ?? "low");

        private void BtnJuiceLaunch_Click(object sender, EventArgs e) => OpenUrl("http://localhost:3000/");
        private void BtnJuiceStart_Click(object sender, EventArgs e) => ExecuteCmdAsync("docker rm -f lab_juiceshop && docker run -d --name lab_juiceshop -p 3000:3000 bkimminich/juice-shop");
        private void BtnJuiceStop_Click(object sender, EventArgs e) => ExecuteCmdAsync("docker stop lab_juiceshop");

        private void BtnGoatLaunch_Click(object sender, EventArgs e) => OpenUrl("http://localhost:8080/WebGoat");
        private void BtnGoatStart_Click(object sender, EventArgs e) => ExecuteCmdAsync("docker rm -f lab_webgoat && docker run -d --name lab_webgoat -p 8080:8080 webgoat/webgoat");
        private void BtnGoatStop_Click(object sender, EventArgs e) => ExecuteCmdAsync("docker stop lab_webgoat");

        private void BtnMutillidaeLaunch_Click(object sender, EventArgs e) => OpenUrl("http://localhost/mutillidae/");
        private void BtnMutillidaeReset_Click(object sender, EventArgs e) => ResetPhpDatabase("mutillidae");

        private void BtnBwappLaunch_Click(object sender, EventArgs e) => OpenUrl("http://localhost/bWAPP/");
        private void BtnBwappReset_Click(object sender, EventArgs e) => ResetPhpDatabase("bWAPP");

        // ==========================================
        // Helpers
        // ==========================================
        private void OpenUrl(string url)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = url,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to open browser: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ResetPhpDatabase(string appName)
        {
            try
            {
                using (var conn = new MySqlConnection(GetDbConnectionString()))
                {
                    conn.Open();
                    string dbName = appName.ToLower() == "bwapp" ? "bWAPP" : appName;
                    using (var cmd = new MySqlCommand($"DROP DATABASE IF EXISTS `{dbName}`; CREATE DATABASE `{dbName}`;", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                MessageBox.Show($"{appName.ToUpper()} Database purged and recreated.", "Database Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshAllStatus();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database Reset Error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void SetDvwaDifficulty(string level)
        {
            string selectedLevel = level.ToLower();

            try
            {
                // 1. Ensure table exists and update database level
                using (var conn = new MySqlConnection(GetDbConnectionString("dvwa")))
                {
                    await conn.OpenAsync();
                    string sql = @"
                CREATE TABLE IF NOT EXISTS `dvwa_security` (
                    `id` INT PRIMARY KEY,
                    `security` VARCHAR(20) NOT NULL
                );
                INSERT INTO `dvwa_security` (`id`, `security`) 
                VALUES (1, @level) 
                ON DUPLICATE KEY UPDATE `security` = @level;";

                    using (var cmd = new MySqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@level", selectedLevel);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }

                // 2. Submit HTTP POST request to DVWA security page to update session state
                using (var handler = new System.Net.Http.HttpClientHandler { UseCookies = true })
                using (var client = new System.Net.Http.HttpClient(handler))
                {
                    var formValues = new Dictionary<string, string>
            {
                { "security", selectedLevel },
                { "seclev_submit", "Submit" }
            };

                    var content = new System.Net.Http.FormUrlEncodedContent(formValues);
                    await client.PostAsync("http://localhost/dvwa/security.php", content);
                }

                MessageBox.Show($"DVWA Security Level set to: {selectedLevel.ToUpper()}", "Security Update", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to update difficulty: " + ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void ExecuteCmdAsync(string command)
        {
            await Task.Run(() =>
            {
                try
                {
                    ProcessStartInfo psi = new ProcessStartInfo("cmd.exe", $"/c {command}")
                    {
                        CreateNoWindow = true,
                        UseShellExecute = false
                    };
                    using (Process p = Process.Start(psi))
                    {
                        p?.WaitForExit();
                    }

                    this.Invoke(new Action(() =>
                    {
                        MessageBox.Show($"Executed:\n{command}", "Status", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RefreshAllStatus();
                    }));
                }
                catch (Exception ex)
                {
                    this.Invoke(new Action(() => MessageBox.Show($"Failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)));
                }
            });
        }
    }
}