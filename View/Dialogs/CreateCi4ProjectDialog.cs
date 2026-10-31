using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Server_HTML_Handler.View.Dialogs
{
    public partial class CreateCi4ProjectDialog : Form
    {
        public string CreatedProjectPath { get; private set; }
        public string CreatedProjectUrl { get; private set; }

        public CreateCi4ProjectDialog(string defaultHostingPath = @"E:\html", string defaultIp = "192.168.32.43")
        {
            InitializeComponent();
            txtHostingPath.Text = !string.IsNullOrWhiteSpace(defaultHostingPath) ? defaultHostingPath : @"E:\html";
            txtHostIp.Text = !string.IsNullOrWhiteSpace(defaultIp) ? defaultIp : "localhost";
            comboEnvironment.SelectedIndex = 0; // development
            UpdateCalculatedUrl();
        }

        private void TxtProjectName_TextChanged(object sender, EventArgs e)
        {
            string clean = Regex.Replace(txtProjectName.Text.Trim(), @"[^a-zA-Z0-9_\-]", "");
            if (clean != txtProjectName.Text.Trim())
            {
                txtProjectName.Text = clean;
                txtProjectName.SelectionStart = txtProjectName.Text.Length;
            }

            if (chkAutoDbName.Checked && !string.IsNullOrWhiteSpace(clean))
            {
                txtDbName.Text = clean.Replace("-", "_").ToLower();
            }

            UpdateCalculatedUrl();
        }

        private void TxtHostIp_TextChanged(object sender, EventArgs e)
        {
            UpdateCalculatedUrl();
        }

        private void UpdateCalculatedUrl()
        {
            string projName = txtProjectName.Text.Trim();
            string ip = string.IsNullOrWhiteSpace(txtHostIp.Text.Trim()) ? "localhost" : txtHostIp.Text.Trim();
            if (string.IsNullOrWhiteSpace(projName))
            {
                lblCalculatedUrl.Text = $"http://{ip}/<project-name>/";
            }
            else
            {
                lblCalculatedUrl.Text = $"http://{ip}/{projName}/";
            }
        }

        private async void BtnCreate_Click(object sender, EventArgs e)
        {
            string projName = txtProjectName.Text.Trim();
            string hostingPath = txtHostingPath.Text.Trim();

            if (string.IsNullOrWhiteSpace(projName))
            {
                MessageBox.Show("Please enter a project/folder name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtProjectName.Focus();
                return;
            }

            if (!Directory.Exists(hostingPath))
            {
                MessageBox.Show($"Hosting folder does not exist:\n{hostingPath}", "Folder Not Found", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string targetDir = Path.Combine(hostingPath, projName);
            if (Directory.Exists(targetDir))
            {
                var dr = MessageBox.Show($"Folder already exists at:\n{targetDir}\n\nDo you want to proceed and write to it?", "Directory Exists", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (dr != DialogResult.Yes) return;
            }

            SetBusyState(true);
            txtLog.Clear();

            string dbHost = txtDbHost.Text.Trim();
            string dbPort = txtDbPort.Text.Trim();
            string dbName = txtDbName.Text.Trim();
            string dbUser = txtDbUser.Text.Trim();
            string dbPass = txtDbPass.Text;
            string env = comboEnvironment.SelectedItem?.ToString() ?? "development";
            string baseUrl = lblCalculatedUrl.Text.Trim();
            if (!baseUrl.EndsWith("/")) baseUrl += "/";

            bool success = await Task.Run(() =>
            {
                return RunCi4Setup(hostingPath, projName, targetDir, baseUrl, env, dbHost, dbPort, dbName, dbUser, dbPass, chkCreateDb.Checked);
            });

            SetBusyState(false);

            if (success)
            {
                CreatedProjectPath = targetDir;
                CreatedProjectUrl = baseUrl;
                lblStatus.Text = "Installation Complete! Ready to launch.";
                lblStatus.ForeColor = Color.Green;
                btnOpenBrowser.Visible = true;
                MessageBox.Show($"CodeIgniter 4 Project '{projName}' created successfully!\n\nAccess it at:\n{baseUrl}", "Setup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                lblStatus.Text = "Installation failed. Check log.";
                lblStatus.ForeColor = Color.Red;
            }
        }

        private bool RunCi4Setup(string hostingPath, string projName, string targetDir, string baseUrl, string env,
                                string dbHost, string dbPort, string dbName, string dbUser, string dbPass, bool autoCreateDb)
        {
            Log($"[1/6] Creating CodeIgniter 4 project via Composer into: {targetDir}");
            try
            {
                // Execute composer create-project
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c composer create-project codeigniter4/appstarter \"{projName}\" --no-interaction",
                    WorkingDirectory = hostingPath,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var process = new Process { StartInfo = psi })
                {
                    process.OutputDataReceived += (s, ev) => { if (!string.IsNullOrEmpty(ev.Data)) Log(ev.Data); };
                    process.ErrorDataReceived += (s, ev) => { if (!string.IsNullOrEmpty(ev.Data)) Log($"[ERR] {ev.Data}"); };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();
                    process.WaitForExit();

                    if (process.ExitCode != 0 && !Directory.Exists(Path.Combine(targetDir, "app")))
                    {
                        Log($"[FATAL] Composer project creation returned error code {process.ExitCode}");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Log($"[FATAL] Composer execution failed: {ex.Message}");
                return false;
            }

            Log("[2/6] Configuring CodeIgniter 4 environment (.env file)...");
            try
            {
                string envExample = Path.Combine(targetDir, "env");
                string envTarget = Path.Combine(targetDir, ".env");

                string envContent = "";
                if (File.Exists(envTarget))
                {
                    envContent = File.ReadAllText(envTarget);
                }
                else if (File.Exists(envExample))
                {
                    envContent = File.ReadAllText(envExample);
                }

                // Replace or append settings
                envContent = SetEnvVar(envContent, "CI_ENVIRONMENT", env);
                envContent = SetEnvVar(envContent, "app.baseURL", baseUrl);

                if (!string.IsNullOrWhiteSpace(dbName))
                {
                    envContent = SetEnvVar(envContent, "database.default.hostname", string.IsNullOrWhiteSpace(dbHost) ? "localhost" : dbHost);
                    envContent = SetEnvVar(envContent, "database.default.database", dbName);
                    envContent = SetEnvVar(envContent, "database.default.username", string.IsNullOrWhiteSpace(dbUser) ? "root" : dbUser);
                    envContent = SetEnvVar(envContent, "database.default.password", dbPass);
                    envContent = SetEnvVar(envContent, "database.default.DBDriver", "MySQLi");
                    envContent = SetEnvVar(envContent, "database.default.port", string.IsNullOrWhiteSpace(dbPort) ? "3306" : dbPort);
                }

                File.WriteAllText(envTarget, envContent, Encoding.UTF8);
                Log("[+] .env file created and configured successfully.");
            }
            catch (Exception ex)
            {
                Log($"[!] Warning during .env configuration: {ex.Message}");
            }

            // 3. Create Root .htaccess so users can access http://<ip>/<project>/ directly
            Log("[3/6] Setting up root .htaccess rewrite to /public folder...");
            try
            {
                string rootHtaccess = Path.Combine(targetDir, ".htaccess");
                string htaccessContent = @"Options +FollowSymLinks
<IfModule mod_rewrite.c>
    RewriteEngine On
    RewriteCond %{REQUEST_URI} !^/public/
    RewriteRule ^(.*)$ public/$1 [L]
</IfModule>
";
                File.WriteAllText(rootHtaccess, htaccessContent, Encoding.UTF8);
                Log("[+] Root .htaccess redirect created.");
            }
            catch (Exception ex)
            {
                Log($"[!] Warning creating root .htaccess: {ex.Message}");
            }

            // 4. Create MySQL Database if requested
            if (autoCreateDb && !string.IsNullOrWhiteSpace(dbName))
            {
                Log($"[4/6] Creating MySQL Database '{dbName}'...");
                try
                {
                    string safeHost = string.IsNullOrWhiteSpace(dbHost) ? "127.0.0.1" : dbHost;
                    string safeUser = string.IsNullOrWhiteSpace(dbUser) ? "root" : dbUser;
                    string connStr = $"server={safeHost};port={dbPort};user={safeUser};password={dbPass};database=mysql;";

                    using (var conn = new MySqlConnection(connStr))
                    {
                        conn.Open();
                        string safeDbName = Regex.Replace(dbName, @"[^a-zA-Z0-9_]", "");
                        using (var cmd = new MySqlCommand($"CREATE DATABASE IF NOT EXISTS `{safeDbName}` CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;", conn))
                        {
                            cmd.ExecuteNonQuery();
                        }
                    }
                    Log($"[+] Database '{dbName}' created or already exists.");
                }
                catch (Exception ex)
                {
                    Log($"[!] Database creation failed: {ex.Message}");
                }
            }
            else
            {
                Log("[4/6] Skipping Database creation.");
            }

            // 5. Check and set permissions on writable/
            Log("[5/6] Verifying writable directory permissions...");
            try
            {
                string writablePath = Path.Combine(targetDir, "writable");
                if (Directory.Exists(writablePath))
                {
                    Directory.CreateDirectory(Path.Combine(writablePath, "cache"));
                    Directory.CreateDirectory(Path.Combine(writablePath, "logs"));
                    Directory.CreateDirectory(Path.Combine(writablePath, "session"));
                    Log("[+] Writable subdirectories verified.");
                }
            }
            catch (Exception ex)
            {
                Log($"[!] Warning: {ex.Message}");
            }

            Log("[6/6] CI4 Project Setup Completed Successfully!");
            return true;
        }

        private string SetEnvVar(string content, string key, string value)
        {
            // Matches optional # and key with spaces and =
            string pattern = $@"^(#\s*)?({Regex.Escape(key)}\s*=).*$";
            string newLine = $"{key} = '{value}'";

            if (Regex.IsMatch(content, pattern, RegexOptions.Multiline))
            {
                return Regex.Replace(content, pattern, newLine, RegexOptions.Multiline);
            }
            else
            {
                return content + Environment.NewLine + newLine;
            }
        }

        private void SetBusyState(bool isBusy)
        {
            progressBar.Style = isBusy ? ProgressBarStyle.Marquee : ProgressBarStyle.Blocks;
            btnCreate.Enabled = !isBusy;
            btnCancel.Enabled = !isBusy;
            txtProjectName.Enabled = !isBusy;
            txtHostingPath.Enabled = !isBusy;
            txtDbName.Enabled = !isBusy;
            txtDbUser.Enabled = !isBusy;
            txtDbPass.Enabled = !isBusy;
            lblStatus.Text = isBusy ? "Creating Project... Please wait." : "Ready";
            lblStatus.ForeColor = isBusy ? Color.DarkBlue : Color.DimGray;
        }

        private void Log(string message)
        {
            if (txtLog.InvokeRequired)
            {
                txtLog.Invoke(new Action(() => Log(message)));
                return;
            }
            txtLog.AppendText(message + Environment.NewLine);
        }

        private void BtnOpenBrowser_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(CreatedProjectUrl))
            {
                Process.Start(new ProcessStartInfo { FileName = CreatedProjectUrl, UseShellExecute = true });
            }
        }

        private void BtnBrowseFolder_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.SelectedPath = txtHostingPath.Text;
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    txtHostingPath.Text = fbd.SelectedPath;
                }
            }
        }
    }
}
