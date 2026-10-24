using Server_HTML_Handler.Controller;
using Server_HTML_Handler.Helper;
using Server_HTML_Handler.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;


using System.Security.AccessControl;
using System.Security.Principal;
using System.Text.RegularExpressions;


using static Server_HTML_Handler.Helper.ServerBackupHelper;

namespace Server_HTML_Handler.View.Contents
{
    public partial class ServerControlForm : UserControl
    {
        private CheckedListBox clbNetworkItems;
        private DataGridView dgvAccessRights;
        public ServerControlForm()
        {
            InitializeComponent();

            // Enable auto-scrolling & flexible docking to fit parent container
            this.AutoScroll = true;
            this.Dock = DockStyle.Fill;

            this.Load += ServerControlForm_Load;

            // Wire immediate dropdown commit & error handling for DataGridViews
            dgvAclPermissions.CurrentCellDirtyStateChanged += DgvAclPermissions_CurrentCellDirtyStateChanged;
            dgvAclPermissions.DataError += DgvAclPermissions_DataError;
        }

        private void SetupAccessPanel()
        {
            // Create the DataGridView dynamically to fill your panelAccess
            dgvAccessRights = new DataGridView();
            dgvAccessRights.Dock = DockStyle.Fill;
            dgvAccessRights.AllowUserToAddRows = false;
            dgvAccessRights.AllowUserToDeleteRows = false;
            dgvAccessRights.RowHeadersVisible = false;
            dgvAccessRights.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvAccessRights.BackgroundColor = Color.FromArgb(42, 42, 42);
            dgvAccessRights.ForeColor = Color.Black; // Standard text color for rows
            dgvAccessRights.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // 1. Add Checkbox Column
            DataGridViewCheckBoxColumn chkCol = new DataGridViewCheckBoxColumn();
            chkCol.Name = "Select";
            chkCol.HeaderText = "Select";
            chkCol.Width = 50;
            dgvAccessRights.Columns.Add(chkCol);

            // 2. Add Info Columns
            dgvAccessRights.Columns.Add("Identity", "User / IP Address");
            dgvAccessRights.Columns.Add("Type", "Platform / Type");
            dgvAccessRights.Columns.Add("Permissions", "Permissions");

            // 3. Add hidden column to store raw rule data (helps with deleting later)
            dgvAccessRights.Columns.Add("RawObject", "RawObject");
            dgvAccessRights.Columns["RawObject"].Visible = false;

            // Add it to your designer panel
            panelAccess.Controls.Add(dgvAccessRights);
        }

        private void LoadAccessRights()
        {
            if (dgvAccessRights == null) return;
            dgvAccessRights.Rows.Clear();

            string path = Properties.Settings.Default.HTMLPath;
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return;

            // --- 1. LOAD WINDOWS NTFS USERS ---
            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(path);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();
                AuthorizationRuleCollection rules = dSecurity.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));

                foreach (FileSystemAccessRule rule in rules)
                {
                    string permissions = "Read-Only";
                    if (rule.FileSystemRights.HasFlag(FileSystemRights.Write) ||
                        rule.FileSystemRights.HasFlag(FileSystemRights.Modify) ||
                        rule.FileSystemRights.HasFlag(FileSystemRights.FullControl))
                    {
                        permissions = rule.AccessControlType == AccessControlType.Deny ? "Write (DENIED)" : "Read/Write";
                    }

                    string identity = rule.IdentityReference.Value;
                    string type = rule.IsInherited ? "Windows (Inherited)" : "Windows (Explicit)";

                    dgvAccessRights.Rows.Add(false, identity, type, permissions, rule);
                }
            }
            catch { /* Ignore if it fails to read OS permissions */ }

            // --- 2. LOAD WEB IPs (.htaccess) ---
            try
            {
                string htaccessPath = Path.Combine(path, ".htaccess");
                if (File.Exists(htaccessPath))
                {
                    string content = File.ReadAllText(htaccessPath);
                    Match blockMatch = Regex.Match(content, @"# --- IP WRITE BLOCK START ---([\s\S]*?)# --- IP WRITE BLOCK END ---");

                    if (blockMatch.Success)
                    {
                        MatchCollection ipMatches = Regex.Matches(blockMatch.Value, @"Require ip ([\d\.]+)");
                        foreach (Match m in ipMatches)
                        {
                            dgvAccessRights.Rows.Add(false, m.Groups[1].Value, "Web IP (.htaccess)", "Read/Write (POST/PUT)", m.Groups[1].Value);
                        }
                    }
                }
            }
            catch { /* Ignore if no htaccess found */ }
        }

        private void SetupFolderFilesPanel()
        {
            // 1. Create the checkbox list and dock it fully inside your Panel
            clbNetworkItems = new CheckedListBox();
            clbNetworkItems.Dock = DockStyle.Fill;
            clbNetworkItems.CheckOnClick = true;
            clbNetworkItems.BackColor = Color.FromArgb(42, 42, 42); // Match your dark theme
            clbNetworkItems.ForeColor = Color.White;
            clbNetworkItems.BorderStyle = BorderStyle.None;
            clbNetworkItems.Font = new Font("Segoe UI", 9F, FontStyle.Regular);

            // 2. Add it to the panel you created in the designer
            panelFolderFiles.Controls.Add(clbNetworkItems);
        }

        private void LoadNetworkFilesAndFolders()
        {
            if (clbNetworkItems == null) return;

            clbNetworkItems.Items.Clear();
            string rootPath = Properties.Settings.Default.HTMLPath;
            txtNetPath.Text = rootPath;

            if (string.IsNullOrEmpty(rootPath) || !Directory.Exists(rootPath)) return;

            try
            {
                // 1. Load all Folders
                foreach (string dirPath in Directory.GetDirectories(rootPath))
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                    // Check if it's already hidden, if so, pre-check the box
                    bool isHidden = (dirInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;

                    clbNetworkItems.Items.Add(new NetworkItem
                    {
                        Path = dirPath,
                        DisplayName = "📁 " + dirInfo.Name
                    }, isHidden);
                }

                // 2. Load all Files
                foreach (string filePath in Directory.GetFiles(rootPath))
                {
                    FileInfo fileInfo = new FileInfo(filePath);
                    bool isHidden = (fileInfo.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden;

                    clbNetworkItems.Items.Add(new NetworkItem
                    {
                        Path = filePath,
                        DisplayName = "📄 " + fileInfo.Name
                    }, isHidden);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load files from HTMLPath: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void ServerControlForm_Load(object sender, EventArgs e)
        {
            try
            {
                SetLoadingState(true);

                LoadSettings();
                LoadPhpLimits();
                SetupAclGrid();
                SetupStorageGrid();
                SetupDbUsersGrid();

                RefreshDatabaseUsers();
                RefreshStorageDetails();
                LoadAclPermissions();
                SetupFolderFilesPanel();
                LoadNetworkFilesAndFolders();
                SetupAccessPanel();
                LoadAccessRights();
                await RefreshServerServicesStatusAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading Server Control view: {ex.Message}", "Load Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        public void LoadAclPermissions()
        {
            string targetPath = txtHtmlPath.Text.Trim();

            if (dgvAclPermissions.Columns.Count == 0)
            {
                SetupAclGrid();
            }

            if (string.IsNullOrWhiteSpace(targetPath) || !Directory.Exists(targetPath))
            {
                dgvAclPermissions.DataSource = null;
                return;
            }

            List<AclPermissionRule> permissions = GetFolderAclRules(targetPath);
            dgvAclPermissions.DataSource = null;
            dgvAclPermissions.DataSource = permissions;
        }

        public void LoadSettings()
        {
            txtHtmlPath.Text = Properties.Settings.Default.HTMLPath ?? @"C:\xampp\htdocs";
            txthtmlNewPath.Text = Properties.Settings.Default.HTMLPath ?? @"C:\xampp\htdocs";
            txtStoragePath.Text = Properties.Settings.Default.StoragePath ?? @"C:\xampp\htdocs\uploads";
        }

        public void SaveSettings()
        {
            Properties.Settings.Default.HTMLPath = txtHtmlPath.Text.Trim();
            Properties.Settings.Default.StoragePath = txtStoragePath.Text.Trim();
            Properties.Settings.Default.Save();
        }

        private void SetLoadingState(bool isLoading)
        {
            this.UseWaitCursor = isLoading;
            this.Cursor = isLoading ? Cursors.WaitCursor : Cursors.Default;
        }

        #region Tab 1: Database Privileges, Users & Connection

        private void chkGlobalCheckAll_CheckedChanged(object sender, EventArgs e)
        {
            bool isChecked = chkGlobalCheckAll.Checked;

            // Data Privileges
            chkSelect.Checked = isChecked;
            chkInsert.Checked = isChecked;
            chkUpdate.Checked = isChecked;
            chkDelete.Checked = isChecked;
            chkFile.Checked = isChecked;

            // Structure Privileges
            chkCreate.Checked = isChecked;
            chkAlter.Checked = isChecked;
            chkIndex.Checked = isChecked;
            chkDrop.Checked = isChecked;
            chkCreateTemp.Checked = isChecked;
            chkShowView.Checked = isChecked;
            chkCreateRoutine.Checked = isChecked;
            chkAlterRoutine.Checked = isChecked;
            chkExecute.Checked = isChecked;
            chkCreateView.Checked = isChecked;
            chkEvent.Checked = isChecked;
            chkTrigger.Checked = isChecked;

            // Administration Privileges
            chkGrant.Checked = isChecked;
            chkSuper.Checked = isChecked;
            chkProcess.Checked = isChecked;
            chkReload.Checked = isChecked;
            chkShutdown.Checked = isChecked;
            chkShowDb.Checked = isChecked;
            chkLockTables.Checked = isChecked;
            chkReferences.Checked = isChecked;
            chkCreateUser.Checked = isChecked;
        }

        private void chkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtNewPassword.PasswordChar = chkShowPassword.Checked ? '\0' : '*';
        }

        private void btnTestDbConnection_Click(object sender, EventArgs e)
        {
            try
            {
                bool isConnected = Database.dbBase.TestDatabaseConnection();
                if (isConnected)
                {
                    MessageBox.Show("Database connection test succeeded!", "Connection Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    DialogResult prompt = MessageBox.Show(
                        "Database connection failed. Access may be denied or MySQL is unreachable.\n\nWould you like to review or update your MySQL connection credentials now?",
                        "Connection Failed",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (prompt == DialogResult.Yes)
                    {
                        using (var dlg = new View.Dialogs.DatabaseCredentialsDialog())
                        {
                            if (dlg.ShowDialog(this) == DialogResult.OK)
                            {
                                RefreshDatabaseUsers();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if (IsDatabaseAuthError(ex))
                {
                    DialogResult prompt = MessageBox.Show(
                        $"Database authentication error:\n{ex.Message}\n\nWould you like to enter updated credentials now?",
                        "Database Authentication Failed",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (prompt == DialogResult.Yes)
                    {
                        using (var dlg = new View.Dialogs.DatabaseCredentialsDialog())
                        {
                            if (dlg.ShowDialog(this) == DialogResult.OK)
                            {
                                RefreshDatabaseUsers();
                                return;
                            }
                        }
                    }
                }

                MessageBox.Show($"Connection test error: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCreateUser_Click(object sender, EventArgs e)
        {
            string username = txtNewUsername.Text.Trim();
            string password = txtNewPassword.Text.Trim();
            string host = string.IsNullOrWhiteSpace(txtUserHost.Text) ? "%" : txtUserHost.Text.Trim();
            string targetDb = string.IsNullOrWhiteSpace(txtTargetDb.Text) ? "*" : txtTargetDb.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Please enter a username.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<string> selectedPrivileges = GetSelectedPrivileges();

            if (selectedPrivileges.Count == 0 && !chkGrant.Checked)
            {
                MessageBox.Show("Please select at least one privilege to grant.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                bool success = Helper.Helper.CreateUserAndGrantPrivileges(username, password, host, targetDb, selectedPrivileges);
                if (success)
                {
                    MessageBox.Show($"User '{username}'@'{host}' created and privileges granted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    ClearUserCreationForm();
                    RefreshDatabaseUsers();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create database user: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<string> GetSelectedPrivileges()
        {
            List<string> privileges = new List<string>();

            // Data
            if (chkSelect.Checked) privileges.Add("SELECT");
            if (chkInsert.Checked) privileges.Add("INSERT");
            if (chkUpdate.Checked) privileges.Add("UPDATE");
            if (chkDelete.Checked) privileges.Add("DELETE");
            if (chkFile.Checked) privileges.Add("FILE");

            // Structure
            if (chkCreate.Checked) privileges.Add("CREATE");
            if (chkAlter.Checked) privileges.Add("ALTER");
            if (chkIndex.Checked) privileges.Add("INDEX");
            if (chkDrop.Checked) privileges.Add("DROP");
            if (chkCreateTemp.Checked) privileges.Add("CREATE TEMPORARY TABLES");
            if (chkShowView.Checked) privileges.Add("SHOW VIEW");
            if (chkCreateRoutine.Checked) privileges.Add("CREATE ROUTINE");
            if (chkAlterRoutine.Checked) privileges.Add("ALTER ROUTINE");
            if (chkExecute.Checked) privileges.Add("EXECUTE");
            if (chkCreateView.Checked) privileges.Add("CREATE VIEW");
            if (chkEvent.Checked) privileges.Add("EVENT");
            if (chkTrigger.Checked) privileges.Add("TRIGGER");

            // Administration
            if (chkSuper.Checked) privileges.Add("SUPER");
            if (chkProcess.Checked) privileges.Add("PROCESS");
            if (chkReload.Checked) privileges.Add("RELOAD");
            if (chkShutdown.Checked) privileges.Add("SHUTDOWN");
            if (chkShowDb.Checked) privileges.Add("SHOW DATABASES");
            if (chkLockTables.Checked) privileges.Add("LOCK TABLES");
            if (chkReferences.Checked) privileges.Add("REFERENCES");
            if (chkCreateUser.Checked) privileges.Add("CREATE USER");

            return privileges;
        }

        private void ClearUserCreationForm()
        {
            txtNewUsername.Clear();
            txtNewPassword.Clear();
            txtUserHost.Text = "%";
            txtTargetDb.Clear();
            chkGlobalCheckAll.Checked = false;
        }

        #endregion

        #region Tab 2: HTML Path, ACL & Upload Storage

        private void btnApplyFolderPermissions_Click(object sender, EventArgs e)
        {
            string folderPath = txtHtmlPath.Text.Trim();
            string accountName = txtWindowsAccount.Text.Trim();
            bool isWriteAccess = rdoAccessWrite.Checked;

            if (string.IsNullOrEmpty(folderPath) || !Directory.Exists(folderPath))
            {
                MessageBox.Show("Please specify a valid HTML directory path.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(accountName))
            {
                MessageBox.Show("Please specify a Windows user or group account.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Helper.Helper.SetFolderPermissions(folderPath, accountName, isWriteAccess);
                SaveSettings();
                LoadAclPermissions();

                MessageBox.Show($"ACL permissions successfully updated for '{accountName}' on path:\n{folderPath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to apply folder permissions: {ex.Message}", "ACL Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCreateUploadStorage_Click(object sender, EventArgs e)
        {
            string storagePath = txtStoragePath.Text.Trim();

            if (string.IsNullOrEmpty(storagePath))
            {
                MessageBox.Show("Please specify a storage folder path.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Helper.Helper.CreateStorageFolderWithWriteAccess(storagePath);
                SaveSettings();
                RefreshStorageDetails();

                MessageBox.Show($"Storage directory created and write permissions configured at:\n{storagePath}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to create storage directory: {ex.Message}", "Directory Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnOpenHtmlDir_Click(object sender, EventArgs e)
        {
            OpenFolderInExplorer(txtHtmlPath.Text.Trim());
        }

        private void btnOpenStorageDir_Click(object sender, EventArgs e)
        {
            OpenFolderInExplorer(txtStoragePath.Text.Trim());
        }

        private void OpenFolderInExplorer(string path)
        {
            if (Directory.Exists(path))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = path,
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            else
            {
                MessageBox.Show($"Directory does not exist:\n{path}", "Directory Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        #endregion

        #region Tab 3: Backups, Database Restore & PHP Limits

        private async void btnBackupDatabase_Click(object sender, EventArgs e)
        {
            string dbName = txtBackupDbName.Text.Trim();

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "SQL Files (*.sql)|*.sql";
                saveFileDialog.FileName = string.IsNullOrEmpty(dbName) ? "full_backup.sql" : $"{dbName}_backup.sql";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        SetLoadingState(true);
                        string targetFile = saveFileDialog.FileName;

                        await Task.Run(() => Helper.ServerBackupHelper.ExportDatabaseDump(dbName, targetFile));

                        MessageBox.Show("Database backup generated successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Database backup failed: {ex.Message}", "Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        SetLoadingState(false);
                    }
                }
            }
        }

        private async void btnRestoreDatabase_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "SQL Files (*.sql)|*.sql";
                openFileDialog.Title = "Select SQL Dump File to Restore";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string targetDb = txtBackupDbName.Text.Trim();
                    DialogResult confirm = MessageBox.Show(
                        $"Are you sure you want to restore the database dump into '{(string.IsNullOrEmpty(targetDb) ? "Default Database" : targetDb)}'?\nThis operation may overwrite existing tables and data.",
                        "Confirm Restore",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (confirm == DialogResult.Yes)
                    {
                        try
                        {
                            SetLoadingState(true);
                            string sqlFilePath = openFileDialog.FileName;

                            await Task.Run(() => Helper.ServerBackupHelper.ImportDatabaseDump(targetDb, sqlFilePath));

                            MessageBox.Show("Database restored successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            RefreshDatabaseUsers();
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Database restore failed: {ex.Message}", "Restore Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            SetLoadingState(false);
                        }
                    }
                }
            }
        }

        private async void btnBackupWebFiles_Click(object sender, EventArgs e)
        {
            string webPath = txtHtmlPath.Text.Trim();

            if (!Directory.Exists(webPath))
            {
                MessageBox.Show("Web folder path does not exist.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "Zip Archive (*.zip)|*.zip";
                saveFileDialog.FileName = $"htdocs_backup_{DateTime.Now:yyyyMMdd_HHmmss}.zip";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        SetLoadingState(true);
                        string zipPath = saveFileDialog.FileName;

                        await Task.Run(() => Helper.ServerBackupHelper.CompressFolderToZip(webPath, zipPath));

                        MessageBox.Show("Web files archived successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Web directory backup failed: {ex.Message}", "Backup Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally
                    {
                        SetLoadingState(false);
                    }
                }
            }
        }

        private void btnUpdateDocumentRoot_Click(object sender, EventArgs e)
        {
            string newPath = txthtmlNewPath.Text.Trim();

            if (string.IsNullOrEmpty(newPath) || !Directory.Exists(newPath))
            {
                MessageBox.Show("Please select a valid, existing directory path for the web root.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Helper.ServerBackupHelper.UpdateApacheDocumentRoot(newPath);

                txthtmlNewPath.Text = newPath;
                txtHtmlPath.Text = newPath;

                SaveSettings();
                LoadAclPermissions();

                MessageBox.Show($"Apache DocumentRoot path updated successfully to:\n{newPath}\n\nPlease restart Apache via Server Health tab or XAMPP Control Panel for changes to take effect.", "Config Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update Apache configuration: {ex.Message}", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSavePhpLimits_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtMaxFileSizeMb.Text.Trim(), out int maxMb) || maxMb <= 0)
            {
                MessageBox.Show("Please enter a valid positive numeric value for MB limit.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Helper.ServerBackupHelper.UpdatePhpUploadLimits(maxMb);
                MessageBox.Show($"PHP limits updated to {maxMb}MB. Restart Apache for changes to take effect.", "PHP Config Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update php.ini settings: {ex.Message}", "Configuration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadPhpLimits()
        {
            try
            {
                int currentLimit = Helper.ServerBackupHelper.GetCurrentPhpUploadLimit();
                txtMaxFileSizeMb.Text = currentLimit.ToString();
            }
            catch
            {
                txtMaxFileSizeMb.Text = "64";
            }
        }

        #endregion

        #region Tab 4: Server Health & Live Logs (New Feature)

        private async Task RefreshServerServicesStatusAsync()
        {
            bool isApacheRunning = await Task.Run(() => Helper.ServerBackupHelper.IsXamppApacheRunning());
            bool isMysqlRunning = await Task.Run(() => Process.GetProcessesByName("mysqld").Length > 0);

            lblApacheStatus.Text = isApacheRunning ? "Apache: RUNNING" : "Apache: STOPPED";
            lblApacheStatus.ForeColor = isApacheRunning ? Color.Green : Color.Red;

            lblMysqlStatus.Text = isMysqlRunning ? "MySQL: RUNNING" : "MySQL: STOPPED";
            lblMysqlStatus.ForeColor = isMysqlRunning ? Color.Green : Color.Red;
        }

        private async void btnRefreshServiceStatus_Click(object sender, EventArgs e)
        {
            await RefreshServerServicesStatusAsync();
        }

        private async void btnRestartApache_Click(object sender, EventArgs e)
        {
            try
            {
                SetLoadingState(true);
                await Task.Run(() => Helper.ServerBackupHelper.RestartApacheService());
                await RefreshServerServicesStatusAsync();
                MessageBox.Show("Apache service restart triggered successfully.", "Service Action", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to restart Apache: {ex.Message}", "Service Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private async void btnLoadLogs_Click(object sender, EventArgs e)
        {
            try
            {
                SetLoadingState(true);
                string selectedLog = cmbLogType.SelectedItem?.ToString() ?? "Apache Error Log";
                string logContent = await Task.Run(() => Helper.ServerBackupHelper.ReadServerLogFile(selectedLog));

                txtServerLogs.Text = string.IsNullOrWhiteSpace(logContent) ? "[Log File empty or not found]" : logContent;
                txtServerLogs.SelectionStart = txtServerLogs.Text.Length;
                txtServerLogs.ScrollToCaret();
            }
            catch (Exception ex)
            {
                txtServerLogs.Text = $"Error reading log file: {ex.Message}";
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        #endregion

        #region Helpers & UI Setup

        private void btnBrowseACLPath_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select HTML Directory";

                if (folderDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    txtHtmlPath.Text = folderDialog.SelectedPath;
                    SaveSettings();
                    RefreshAclGrid(folderDialog.SelectedPath);
                }
            }
        }

        private void btnUploadPath_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select Storage Directory";

                if (folderDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    txtStoragePath.Text = folderDialog.SelectedPath;
                    SaveSettings();
                    RefreshStorageDetails();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var folderDialog = new FolderBrowserDialog())
            {
                folderDialog.Description = "Select HTML Deployment Directory";

                if (folderDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    txthtmlNewPath.Text = folderDialog.SelectedPath;
                    SaveSettings();
                }
            }
        }

        private void btnchkName_Click(object sender, EventArgs e)
        {
            string inputAccount = txtWindowsAccount.Text.Trim();

            if (string.IsNullOrEmpty(inputAccount))
            {
                MessageBox.Show("Please enter a Windows user or group name to check.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (Helper.Helper.DoesWindowsAccountExist(inputAccount, out string resolvedName))
            {
                txtWindowsAccount.Text = resolvedName;
                MessageBox.Show($"Account verified successfully!\n\nResolved Name: {resolvedName}", "Name Check Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"Account '{inputAccount}' was not found on the local computer or domain network.\n\nPlease verify username/group spelling.", "Account Not Found", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SetupAclGrid()
        {
            dgvAclPermissions.Columns.Clear();
            dgvAclPermissions.AutoGenerateColumns = false;
            dgvAclPermissions.RowHeadersVisible = false;
            dgvAclPermissions.AllowUserToAddRows = false;
            dgvAclPermissions.AllowUserToDeleteRows = false;
            dgvAclPermissions.AllowUserToResizeRows = false;
            dgvAclPermissions.ReadOnly = false;
            dgvAclPermissions.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvAclPermissions.EnableHeadersVisualStyles = false;

            Font baseFont = new Font("Segoe UI", 9F, FontStyle.Regular);
            Font headerFont = new Font("Segoe UI", 9F, FontStyle.Bold);

            dgvAclPermissions.Font = baseFont;
            dgvAclPermissions.RowTemplate.Height = 24;
            dgvAclPermissions.ColumnHeadersHeight = 30;

            dgvAclPermissions.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvAclPermissions.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = headerFont,
                BackColor = Color.FromArgb(20, 20, 20),
                ForeColor = Color.FromArgb(0, 229, 255),
                SelectionBackColor = Color.FromArgb(20, 20, 20),
                SelectionForeColor = Color.FromArgb(0, 229, 255)
            };

            var colAccount = new DataGridViewTextBoxColumn
            {
                Name = "colAccount",
                HeaderText = "User / Group",
                DataPropertyName = "AccountName",
                ReadOnly = true,
                FillWeight = 50,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft, Padding = new Padding(8, 0, 0, 0) }
            };

            var colAccessType = new DataGridViewTextBoxColumn
            {
                Name = "colAccessType",
                HeaderText = "Type",
                DataPropertyName = "AccessType",
                ReadOnly = true,
                FillWeight = 25,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };

            var comboColumn = new DataGridViewComboBoxColumn
            {
                Name = "colAccessLevel",
                HeaderText = "Access Level",
                DataPropertyName = "AccessLevel",
                FillWeight = 25,
                ReadOnly = false,
                FlatStyle = FlatStyle.Flat,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            comboColumn.Items.AddRange("Read-Only", "Read/Write");

            dgvAclPermissions.Columns.Add(colAccount);
            dgvAclPermissions.Columns.Add(colAccessType);
            dgvAclPermissions.Columns.Add(comboColumn);
        }
        private void SetupStorageGrid()
        {
            dgvStorageFolders.Columns.Clear();
            dgvStorageFolders.AutoGenerateColumns = false;
            dgvStorageFolders.RowHeadersVisible = false;
            dgvStorageFolders.AllowUserToAddRows = false;
            dgvStorageFolders.AllowUserToDeleteRows = false;
            dgvStorageFolders.AllowUserToResizeRows = false;
            dgvStorageFolders.ReadOnly = false;
            dgvStorageFolders.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            dgvStorageFolders.EnableHeadersVisualStyles = false;

            Font baseFont = new Font("Segoe UI", 9F, FontStyle.Regular);
            Font headerFont = new Font("Segoe UI", 9F, FontStyle.Bold);

            dgvStorageFolders.Font = baseFont;
            dgvStorageFolders.RowTemplate.Height = 26;
            dgvStorageFolders.ColumnHeadersHeight = 30;

            dgvStorageFolders.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvStorageFolders.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = headerFont,
                BackColor = Color.FromArgb(20, 20, 20),
                ForeColor = Color.FromArgb(0, 229, 255),
                SelectionBackColor = Color.FromArgb(20, 20, 20),
                SelectionForeColor = Color.FromArgb(0, 229, 255)
            };

            dgvStorageFolders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFolderName",
                HeaderText = "Folder Name",
                DataPropertyName = "FolderName",
                ReadOnly = true,
                FillWeight = 40,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleLeft, Padding = new Padding(8, 0, 0, 0) }
            });

            dgvStorageFolders.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colFormattedSize",
                HeaderText = "Size",
                DataPropertyName = "FormattedSize",
                ReadOnly = true,
                FillWeight = 20,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvStorageFolders.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "colBackupBtn",
                HeaderText = "Backup",
                Text = "Backup (.zip)",
                UseColumnTextForButtonValue = true,
                FillWeight = 20,
                FlatStyle = FlatStyle.Standard
            });

            dgvStorageFolders.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "colTransferBtn",
                HeaderText = "Transfer",
                Text = "Transfer Folder",
                UseColumnTextForButtonValue = true,
                FillWeight = 20,
                FlatStyle = FlatStyle.Standard
            });
        }
        private void SetupDbUsersGrid()
        {
            dgvDatabaseUsers.Columns.Clear();
            dgvDatabaseUsers.AutoGenerateColumns = false;
            dgvDatabaseUsers.RowHeadersVisible = false;
            dgvDatabaseUsers.AllowUserToAddRows = false;
            dgvDatabaseUsers.ReadOnly = false;
            dgvDatabaseUsers.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // Required so Windows visual styles don't override custom header colors
            dgvDatabaseUsers.EnableHeadersVisualStyles = false;

            Font headerFont = new Font("Segoe UI", 9F, FontStyle.Bold);
            dgvDatabaseUsers.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                Alignment = DataGridViewContentAlignment.MiddleCenter,
                Font = headerFont,
                BackColor = Color.FromArgb(20, 20, 20),
                ForeColor = Color.FromArgb(0, 229, 255),
                SelectionBackColor = Color.FromArgb(20, 20, 20),
                SelectionForeColor = Color.FromArgb(0, 229, 255)
            };

            dgvDatabaseUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colUserName",
                HeaderText = "User name",
                DataPropertyName = "UserName",
                ReadOnly = true,
                FillWeight = 25
            });

            dgvDatabaseUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colHostName",
                HeaderText = "Host name",
                DataPropertyName = "HostName",
                ReadOnly = true,
                FillWeight = 20
            });

            dgvDatabaseUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colHasPassword",
                HeaderText = "Password",
                DataPropertyName = "HasPassword",
                ReadOnly = true,
                FillWeight = 15,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvDatabaseUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colGlobalPrivs",
                HeaderText = "Global privileges",
                DataPropertyName = "GlobalPrivileges",
                ReadOnly = true,
                FillWeight = 20
            });

            dgvDatabaseUsers.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colGrantPriv",
                HeaderText = "Grant",
                DataPropertyName = "GrantPrivilege",
                ReadOnly = true,
                FillWeight = 10,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            });

            dgvDatabaseUsers.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "colEditPrivsBtn",
                HeaderText = "Action",
                Text = "Edit privileges",
                UseColumnTextForButtonValue = true,
                FillWeight = 20,
                FlatStyle = FlatStyle.Standard
            });

            dgvDatabaseUsers.Columns.Add(new DataGridViewButtonColumn
            {
                Name = "colDropUserBtn",
                HeaderText = "",
                Text = "Drop User",
                UseColumnTextForButtonValue = true,
                FillWeight = 15,
                FlatStyle = FlatStyle.Standard
            });
        }
        public void RefreshStorageDetails()
        {
            string targetPath = txtStoragePath.Text.Trim();

            StorageMetricsHelper.GetDriveAndDirectoryMetrics(targetPath, out string available, out string consumed);
            lblAvailableStorage.Text = $"Available Drive Space: {available}";
            lblConsumedStorage.Text = $"Total Consumed: {consumed}";

            if (dgvStorageFolders.Columns.Count == 0)
            {
                SetupStorageGrid();
            }

            var folderList = StorageMetricsHelper.GetSubfolderSizesSorted(targetPath);
            dgvStorageFolders.DataSource = folderList;
        }

        public void RefreshAclGrid(string folderPath)
        {
            if (dgvAclPermissions.Columns.Count == 0)
            {
                SetupAclGrid();
            }

            var aclList = Helper.ServerBackupHelper.GetFolderAclRules(folderPath);
            dgvAclPermissions.DataSource = aclList;
        }

                private bool IsDatabaseAuthError(Exception ex)
        {
            if (ex == null) return false;
            string msg = ex.Message ?? string.Empty;
            return msg.IndexOf("Access denied", StringComparison.OrdinalIgnoreCase) >= 0
                || msg.IndexOf("Authentication", StringComparison.OrdinalIgnoreCase) >= 0
                || msg.IndexOf("using password", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        private void btnChangeAdminCredentials_Click(object sender, EventArgs e)
        {
            using (var dlg = new View.Dialogs.DatabaseCredentialsDialog())
            {
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    RefreshDatabaseUsers();
                }
            }
        }

        public void RefreshDatabaseUsers()
        {
            try
            {
                if (dgvDatabaseUsers.Columns.Count == 0)
                {
                    SetupDbUsersGrid();
                }

                var users = Database.dbBase.GetDatabaseUserAccounts();
                dgvDatabaseUsers.DataSource = null;
                dgvDatabaseUsers.DataSource = users;
            }
            catch (Exception ex)
            {
                if (IsDatabaseAuthError(ex))
                {
                    DialogResult prompt = MessageBox.Show(
                        $"Database authentication failed:\n{ex.Message}\n\nWould you like to enter updated MySQL administrator credentials now?",
                        "MySQL Authentication Failed",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Warning);

                    if (prompt == DialogResult.Yes)
                    {
                        using (var dlg = new View.Dialogs.DatabaseCredentialsDialog("Please enter your new MySQL username and password to reconnect."))
                        {
                            if (dlg.ShowDialog(this) == DialogResult.OK)
                            {
                                RefreshDatabaseUsers();
                                return;
                            }
                        }
                    }
                }

                MessageBox.Show($"Failed to load database users: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region Event Handlers & DataGridView Actions

        private async void dgvStorageFolders_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var selectedItem = dgvStorageFolders.Rows[e.RowIndex].DataBoundItem as FolderSizeDetail;
            if (selectedItem == null || !Directory.Exists(selectedItem.FullPath))
            {
                MessageBox.Show("Folder no longer exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string columnName = dgvStorageFolders.Columns[e.ColumnIndex].Name;

            if (columnName == "colBackupBtn")
            {
                using (var saveFileDialog = new SaveFileDialog())
                {
                    saveFileDialog.Filter = "Zip Archive (*.zip)|*.zip";
                    saveFileDialog.FileName = $"{selectedItem.FolderName}_Backup_{DateTime.Now:yyyyMMdd}.zip";

                    if (saveFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            SetLoadingState(true);
                            string zipPath = saveFileDialog.FileName;

                            await Task.Run(() =>
                            {
                                if (File.Exists(zipPath)) File.Delete(zipPath);
                                ZipFile.CreateFromDirectory(selectedItem.FullPath, zipPath, CompressionLevel.Optimal, false);
                            });

                            MessageBox.Show($"Folder backup created successfully:\n{zipPath}", "Backup Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Backup failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        finally
                        {
                            SetLoadingState(false);
                        }
                    }
                }
            }
            else if (columnName == "colTransferBtn")
            {
                using (var folderDialog = new FolderBrowserDialog())
                {
                    folderDialog.Description = $"Select destination directory to move '{selectedItem.FolderName}'";

                    if (folderDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                    {
                        string destinationFolder = Path.Combine(folderDialog.SelectedPath, selectedItem.FolderName);

                        DialogResult confirm = MessageBox.Show(
                            $"Are you sure you want to transfer this folder to:\n{destinationFolder}?",
                            "Confirm Transfer",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);

                        if (confirm == DialogResult.Yes)
                        {
                            try
                            {
                                SetLoadingState(true);

                                await Task.Run(() => MoveFolderSafe(selectedItem.FullPath, destinationFolder));

                                MessageBox.Show("Folder transferred successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                RefreshStorageDetails();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Transfer failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                            finally
                            {
                                SetLoadingState(false);
                            }
                        }
                    }
                }
            }
        }

        private void btnResetUserPassword_Click(object sender, EventArgs e)
        {
            if (dgvDatabaseUsers.CurrentRow == null || dgvDatabaseUsers.CurrentRow.Index < 0)
            {
                MessageBox.Show("Please select a user from the 'List of Users' table first.", "No User Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedUser = dgvDatabaseUsers.CurrentRow.DataBoundItem as DbUserOverview;
            if (selectedUser == null)
            {
                MessageBox.Show("Unable to determine the selected user.", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            PromptAndResetUserPassword(selectedUser.UserName, selectedUser.HostName);
        }

        private void PromptAndResetUserPassword(string username, string host)
        {
            using (Form prompt = new Form())
            {
                prompt.Width = 380;
                prompt.Height = 190;
                prompt.FormBorderStyle = FormBorderStyle.FixedDialog;
                prompt.Text = "Reset User Password";
                prompt.StartPosition = FormStartPosition.CenterParent;
                prompt.MaximizeBox = false;
                prompt.MinimizeBox = false;
                prompt.BackColor = Color.FromArgb(40, 40, 40);
                prompt.ForeColor = Color.White;

                Label lblText = new Label()
                {
                    Left = 20,
                    Top = 15,
                    Width = 325,
                    Text = $"Enter new password for '{username}'@'{host}':",
                    ForeColor = Color.FromArgb(0, 229, 255),
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold)
                };

                TextBox txtBox = new TextBox()
                {
                    Left = 20,
                    Top = 45,
                    Width = 325,
                    UseSystemPasswordChar = true,
                    BackColor = Color.FromArgb(25, 25, 25),
                    ForeColor = Color.White,
                    BorderStyle = BorderStyle.FixedSingle,
                    Font = new Font("Segoe UI", 9F)
                };

                CheckBox chkShow = new CheckBox()
                {
                    Left = 20,
                    Top = 75,
                    Width = 150,
                    Text = "Show Password",
                    ForeColor = Color.LightGray,
                    Font = new Font("Segoe UI", 8.25F)
                };
                chkShow.CheckedChanged += (s, ev) => { txtBox.UseSystemPasswordChar = !chkShow.Checked; };

                Button btnConfirm = new Button()
                {
                    Text = "Reset Password",
                    Left = 195,
                    Width = 150,
                    Top = 105,
                    DialogResult = DialogResult.OK,
                    BackColor = Color.FromArgb(230, 126, 34),
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Cursor = Cursors.Hand,
                    Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold)
                };
                btnConfirm.FlatAppearance.BorderSize = 0;

                Button btnCancel = new Button()
                {
                    Text = "Cancel",
                    Left = 110,
                    Width = 75,
                    Top = 105,
                    DialogResult = DialogResult.Cancel,
                    BackColor = Color.FromArgb(60, 60, 60),
                    ForeColor = Color.LightGray,
                    FlatStyle = FlatStyle.Flat
                };
                btnCancel.FlatAppearance.BorderSize = 0;

                prompt.Controls.Add(lblText);
                prompt.Controls.Add(txtBox);
                prompt.Controls.Add(chkShow);
                prompt.Controls.Add(btnConfirm);
                prompt.Controls.Add(btnCancel);
                prompt.AcceptButton = btnConfirm;
                prompt.CancelButton = btnCancel;

                if (prompt.ShowDialog(this) == DialogResult.OK)
                {
                    string newPass = txtBox.Text;
                    try
                    {
                        Database.dbBase.ResetDatabaseUserPassword(username, host, newPass);
                        string currentAdmin = Properties.Settings.Default.MasterUser;
                        if (string.Equals(username, currentAdmin, StringComparison.OrdinalIgnoreCase))
                        {
                            DialogResult syncResult = MessageBox.Show(
                                $"Password for '{username}'@'{host}' has been successfully reset in MySQL!\n\nThis account matches the application's active administrative connection user ('{currentAdmin}').\n\nWould you like to automatically update the application's saved connection credentials with this new password?",
                                "Sync Admin Credentials",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Question);

                            if (syncResult == DialogResult.Yes)
                            {
                                Properties.Settings.Default.DBmaster = AesEncryptionHelper.Encrypt(newPass);
                                Properties.Settings.Default.Save();
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Password for '{username}'@'{host}' has been successfully reset!", "Password Reset", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }

                        RefreshDatabaseUsers();
                    }
                    catch (Exception ex)
                    {
                        if (IsDatabaseAuthError(ex))
                        {
                            DialogResult authPrompt = MessageBox.Show(
                                $"Authentication error resetting password:\n{ex.Message}\n\nWould you like to enter updated MySQL connection credentials now?",
                                "Database Authentication Failed",
                                MessageBoxButtons.YesNo,
                                MessageBoxIcon.Warning);

                            if (authPrompt == DialogResult.Yes)
                            {
                                using (var dlg = new View.Dialogs.DatabaseCredentialsDialog())
                                {
                                    if (dlg.ShowDialog(this) == DialogResult.OK)
                                    {
                                        RefreshDatabaseUsers();
                                        return;
                                    }
                                }
                            }
                        }

                        MessageBox.Show($"Failed to reset password: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void dgvDatabaseUsers_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var selectedUser = dgvDatabaseUsers.Rows[e.RowIndex].DataBoundItem as DbUserOverview;
            if (selectedUser == null) return;

            string columnName = dgvDatabaseUsers.Columns[e.ColumnIndex].Name;

            if (columnName == "colEditPrivsBtn")
            {
                txtNewUsername.Text = selectedUser.UserName;
                txtUserHost.Text = selectedUser.HostName;
                MessageBox.Show($"User '{selectedUser.UserName}'@'{selectedUser.HostName}' selected. Adjust privilege checkboxes below and click 'Create User / Grant Privileges' to apply changes.", "Edit User Privileges", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else if (columnName == "colDropUserBtn")
            {
                DialogResult confirm = MessageBox.Show(
                    $"Are you sure you want to drop user '{selectedUser.UserName}'@'{selectedUser.HostName}'?\nThis will revoke all active access privileges.",
                    "Confirm Drop User",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                        Database.dbBase.DropDatabaseUser(selectedUser.UserName, selectedUser.HostName);
                        MessageBox.Show($"User '{selectedUser.UserName}'@'{selectedUser.HostName}' dropped successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        RefreshDatabaseUsers();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Failed to drop user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void DgvAclPermissions_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvAclPermissions.IsCurrentCellDirty && dgvAclPermissions.CurrentCell is DataGridViewComboBoxCell)
            {
                dgvAclPermissions.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        private void DgvAclPermissions_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.ThrowException = false;
        }

        private void dgvAclPermissions_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            if (dgvAclPermissions.Columns[e.ColumnIndex].Name == "colAccessLevel")
            {
                var row = dgvAclPermissions.Rows[e.RowIndex];
                var item = row.DataBoundItem as AclPermissionRule;

                if (item == null) return;

                string targetPath = txtHtmlPath.Text.Trim();
                string newAccessLevel = row.Cells["colAccessLevel"].Value?.ToString();

                if (string.IsNullOrWhiteSpace(targetPath) || !Directory.Exists(targetPath))
                {
                    MessageBox.Show("Invalid target directory path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                bool success = ControllerService.UpdateWindowsNtfsAcl(targetPath, item.AccountName, newAccessLevel);

                if (success)
                {
                    item.AccessLevel = newAccessLevel;
                }
                else
                {
                    row.Cells["colAccessLevel"].Value = item.AccessLevel;
                }
            }
        }

        private void MoveFolderSafe(string sourceDir, string destinationDir)
        {
            if (Path.GetPathRoot(sourceDir).Equals(Path.GetPathRoot(destinationDir), StringComparison.OrdinalIgnoreCase))
            {
                Directory.Move(sourceDir, destinationDir);
            }
            else
            {
                CopyDirectoryRecursive(sourceDir, destinationDir);
                Directory.Delete(sourceDir, true);
            }
        }

        private void CopyDirectoryRecursive(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, destFile, true);
            }

            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
                CopyDirectoryRecursive(subDir, destSubDir);
            }
        }

        #endregion


        // =========================================================================
        // NETWORK SECURITY & ACCESS METHODS
        // =========================================================================

        // 1. Set Windows Directory / Network Share to "Everyone = Read Only"

        private bool IsAdministrator()
        {
            System.Security.Principal.WindowsIdentity identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.WindowsPrincipal principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }

        private void btnApplyReadOnly_Click(object sender, EventArgs e)
        {
            // --- 1. UAC ELEVATION CHECK ---
            if (!IsAdministrator())
            {
                DialogResult result = MessageBox.Show(
                    "Modifying folder permissions requires Administrative privileges.\n\nWould you like to restart the application as Administrator now?",
                    "Elevation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    ProcessStartInfo proc = new ProcessStartInfo();
                    proc.UseShellExecute = true;
                    proc.WorkingDirectory = Environment.CurrentDirectory;
                    proc.FileName = Application.ExecutablePath;
                    proc.Verb = "runas";

                    try
                    {
                        Process.Start(proc);
                        Application.Exit();
                    }
                    catch
                    {
                        MessageBox.Show("Administrative elevation was canceled.", "Canceled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
                return;
            }

            // --- 2. START PERMISSION LOGIC ---
            string rootPath = Properties.Settings.Default.HTMLPath;

            if (string.IsNullOrEmpty(rootPath) || !Directory.Exists(rootPath))
            {
                MessageBox.Show("The HTML Path is not set or does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Setup Identities
                SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                SecurityIdentifier systemSid = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
                SecurityIdentifier adminSid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
                string currentUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

                List<string> targetFolders = new List<string>();

                if (clbNetworkItems != null && clbNetworkItems.Items.Count > 0)
                {
                    for (int i = 0; i < clbNetworkItems.Items.Count; i++)
                    {
                        if (clbNetworkItems.GetItemChecked(i))
                        {
                            NetworkItem item = (NetworkItem)clbNetworkItems.Items[i];
                            if (Directory.Exists(item.Path)) targetFolders.Add(item.Path);
                        }
                    }
                }

                if (targetFolders.Count == 0) targetFolders.Add(rootPath);

                int processedCount = 0;

                // --- 3. PROCESS EACH TARGET ---
                foreach (string targetPath in targetFolders)
                {
                    // A. Configure CI4 Uploads Folder First
                    string ci4WritablePath = Path.Combine(targetPath, "writable");
                    string ci4UploadsPath = Path.Combine(ci4WritablePath, "uploads");

                    if (!Directory.Exists(ci4WritablePath)) Directory.CreateDirectory(ci4WritablePath);
                    if (!Directory.Exists(ci4UploadsPath)) Directory.CreateDirectory(ci4UploadsPath);

                    DirectoryInfo uploadInfo = new DirectoryInfo(ci4UploadsPath);
                    DirectorySecurity uploadSecurity = uploadInfo.GetAccessControl();

                    foreach (FileSystemAccessRule rule in uploadSecurity.GetAccessRules(true, false, typeof(SecurityIdentifier)))
                    {
                        if (rule.AccessControlType == AccessControlType.Deny) uploadSecurity.RemoveAccessRule(rule);
                    }

                    uploadSecurity.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.Modify | FileSystemRights.Write, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                    uploadInfo.SetAccessControl(uploadSecurity);

                    // B. Configure Target Folder (NO DENY RULES, EXPLICIT ALLOW ONLY)
                    DirectoryInfo targetInfo = new DirectoryInfo(targetPath);
                    DirectorySecurity targetSecurity = targetInfo.GetAccessControl();

                    foreach (FileSystemAccessRule rule in targetSecurity.GetAccessRules(true, false, typeof(SecurityIdentifier)))
                    {
                        if (rule.AccessControlType == AccessControlType.Deny) targetSecurity.RemoveAccessRule(rule);
                    }

                    // Grant FULL CONTROL to Your PC User, SYSTEM (Apache), and Administrators
                    targetSecurity.AddAccessRule(new FileSystemAccessRule(currentUser, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                    targetSecurity.AddAccessRule(new FileSystemAccessRule(systemSid, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
                    targetSecurity.AddAccessRule(new FileSystemAccessRule(adminSid, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));

                    // Grant EVERYONE Read & Execute so normal network PCs and web visitors can view files
                    targetSecurity.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.ReadAndExecute, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));

                    targetInfo.SetAccessControl(targetSecurity);
                    processedCount++;
                }

                // --- 4. APPLY NETWORK SHARE (SMB) WITH EXACT FOLDER NAME ---
                DirectoryInfo rootInfo = new DirectoryInfo(rootPath);
                string shareName = rootInfo.Name; // This forces the share to be named exactly "html"

                // Delete the old _WebShare (ignore error if it doesn't exist), then create the new share.
                // /grant:Everyone,FULL removes the network bottleneck so developers can paste files (NTFS will still protect it).
                string netCommand = $"/c net share {shareName}_WebShare /delete 2>nul & net share {shareName}=\"{rootPath}\" /grant:Everyone,FULL";

                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.Arguments = netCommand;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.Start();
                    process.WaitForExit();
                }

                string targetMsg = (targetFolders.Count == 1 && targetFolders[0] == rootPath)
                    ? "The Entire Root Directory"
                    : $"{processedCount} Selected Folders";

                MessageBox.Show($"Permissions Applied Successfully!\n\n" +
                                $"- Network Share is now exactly '{shareName}'.\n" +
                                $"- Your PC: FULL ACCESS Restored.\n" +
                                $"- Other PCs: READ-ONLY (Use the HTML Access Tab to add Developers).\n" +
                                $"- 'writable/uploads' ensured and set to Writable.",
                                "Security Applied", MessageBoxButtons.OK, MessageBoxIcon.Information);

                LoadAccessRights();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to apply NTFS permissions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnApplyIP_Click(object sender, EventArgs e)
        {
            string rootPath = Properties.Settings.Default.HTMLPath;
            string ipAddr = txtWriteIP.Text.Trim();

            if (string.IsNullOrEmpty(rootPath) || !Directory.Exists(rootPath) || string.IsNullOrEmpty(ipAddr))
            {
                MessageBox.Show("Please provide a valid path and IP Address.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                List<string> targetFolders = new List<string>();

                // 1. Check if specific folders are selected in the UI Panel
                if (clbNetworkItems != null && clbNetworkItems.Items.Count > 0)
                {
                    for (int i = 0; i < clbNetworkItems.Items.Count; i++)
                    {
                        if (clbNetworkItems.GetItemChecked(i))
                        {
                            NetworkItem item = (NetworkItem)clbNetworkItems.Items[i];
                            if (Directory.Exists(item.Path)) targetFolders.Add(item.Path);
                        }
                    }
                }

                // 2. Target root if nothing is selected
                if (targetFolders.Count == 0) targetFolders.Add(rootPath);

                int processedCount = 0;

                // 3. Apply the .htaccess IP rule to EACH targeted folder
                foreach (string targetPath in targetFolders)
                {
                    string htaccessPath = Path.Combine(targetPath, ".htaccess");
                    string content = File.Exists(htaccessPath) ? File.ReadAllText(htaccessPath) : "";

                    // Extract existing IPs from our custom block
                    List<string> allowedIPs = new List<string>();
                    Match blockMatch = Regex.Match(content, @"# --- IP WRITE BLOCK START ---([\s\S]*?)# --- IP WRITE BLOCK END ---");

                    if (blockMatch.Success)
                    {
                        MatchCollection ipMatches = Regex.Matches(blockMatch.Value, @"Require ip ([\d\.]+)");
                        foreach (Match m in ipMatches) allowedIPs.Add(m.Groups[1].Value);

                        // Remove the old block
                        content = content.Replace(blockMatch.Value, "").Trim();
                    }

                    // Add the new IP if it isn't already in the list
                    if (!allowedIPs.Contains(ipAddr))
                    {
                        allowedIPs.Add(ipAddr);
                    }

                    // Rebuild the Apache Rules Block
                    StringBuilder newBlock = new StringBuilder();
                    newBlock.AppendLine("\n# --- IP WRITE BLOCK START ---");
                    newBlock.AppendLine("<Limit POST PUT DELETE>");
                    foreach (string ip in allowedIPs)
                    {
                        newBlock.AppendLine($"    Require ip {ip}");
                    }
                    newBlock.AppendLine("</Limit>");
                    newBlock.AppendLine("<Limit GET OPTIONS>");
                    newBlock.AppendLine("    Require all granted"); // Everyone else can still READ
                    newBlock.AppendLine("</Limit>");
                    newBlock.AppendLine("# --- IP WRITE BLOCK END ---");

                    // Ensure a clean merge with existing content
                    string finalContent = content.Length > 0 ? content + "\n\n" + newBlock.ToString().Trim() : newBlock.ToString().Trim();

                    File.WriteAllText(htaccessPath, finalContent);
                    processedCount++;
                }

                string targetMsg = (targetFolders.Count == 1 && targetFolders[0] == rootPath)
                    ? "The Entire Root Directory"
                    : $"{processedCount} Selected Folders";

                MessageBox.Show($"Web access updated.\n\nIP '{ipAddr}' was ADDED to the Write permission list for:\n- {targetMsg}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the table!
                LoadAccessRights();
                txtWriteIP.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to apply IP: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // 3. Hide specific file or folder in Windows File Explorer
        private void btnHide_Click(object sender, EventArgs e)
        {
            if (clbNetworkItems == null || clbNetworkItems.Items.Count == 0) return;

            int changedCount = 0;

            try
            {
                // Loop through every item in the CheckBox list in the panel
                for (int i = 0; i < clbNetworkItems.Items.Count; i++)
                {
                    NetworkItem item = (NetworkItem)clbNetworkItems.Items[i];

                    // If checked = Should be Hidden. If unchecked = Should be Visible.
                    bool shouldBeHidden = clbNetworkItems.GetItemChecked(i);

                    FileAttributes attributes = File.GetAttributes(item.Path);
                    bool isCurrentlyHidden = (attributes & FileAttributes.Hidden) == FileAttributes.Hidden;

                    // Apply "Super Hidden" status if checked (Hidden + System)
                    if (shouldBeHidden && !isCurrentlyHidden)
                    {
                        File.SetAttributes(item.Path, attributes | FileAttributes.Hidden | FileAttributes.System);
                        changedCount++;
                    }
                    // Remove "Super Hidden" status if unchecked
                    else if (!shouldBeHidden && isCurrentlyHidden)
                    {
                        File.SetAttributes(item.Path, attributes & ~FileAttributes.Hidden & ~FileAttributes.System);
                        changedCount++;
                    }
                }

                MessageBox.Show($"Visibility updated successfully. {changedCount} items changed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the list to ensure the UI matches the actual Windows files
                LoadNetworkFilesAndFolders();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update folder visibility: {ex.Message}\n\nNote: You may need to run this app as Administrator to modify System attributes.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnRemoveAccess_Click(object sender, EventArgs e)
        {
            string path = Properties.Settings.Default.HTMLPath;
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) return;

            bool hasChanges = false;
            bool rewriteHtaccess = false;
            bool ntfsModified = false;

            List<string> remainingIPs = new List<string>();

            DirectoryInfo dInfo = new DirectoryInfo(path);
            DirectorySecurity dSecurity = dInfo.GetAccessControl();

            try
            {
                // 1. Loop through the rows to see what is checked
                foreach (DataGridViewRow row in dgvAccessRights.Rows)
                {
                    bool isChecked = Convert.ToBoolean(row.Cells["Select"].Value);
                    string type = row.Cells["Type"].Value.ToString();
                    string identity = row.Cells["Identity"].Value.ToString();

                    // Handle Web IPs (.htaccess)
                    if (type.Contains("Web IP"))
                    {
                        if (!isChecked)
                            remainingIPs.Add(identity); // Keep this IP
                        else
                            rewriteHtaccess = true; // Delete this IP (by omitting it)
                    }
                    // Handle Windows Users
                    else if (type.Contains("Windows") && isChecked)
                    {
                        if (type.Contains("Inherited"))
                        {
                            MessageBox.Show($"Cannot remove '{identity}'. Inherited Windows permissions must be removed from the Parent folder (C:\\xampp), not this folder.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            FileSystemAccessRule rule = (FileSystemAccessRule)row.Cells["RawObject"].Value;
                            dSecurity.RemoveAccessRule(rule);
                            ntfsModified = true;
                        }
                    }
                }

                // 2. Apply Windows NTFS Deletions
                if (ntfsModified)
                {
                    dInfo.SetAccessControl(dSecurity);
                    hasChanges = true;
                }

                // 3. Apply .htaccess IP Deletions
                if (rewriteHtaccess)
                {
                    string htaccessPath = Path.Combine(path, ".htaccess");
                    if (File.Exists(htaccessPath))
                    {
                        string content = File.ReadAllText(htaccessPath);
                        Match blockMatch = Regex.Match(content, @"# --- IP WRITE BLOCK START ---([\s\S]*?)# --- IP WRITE BLOCK END ---");

                        if (blockMatch.Success)
                        {
                            content = content.Replace(blockMatch.Value, "").Trim();

                            // If we still have IPs remaining, rewrite the block
                            if (remainingIPs.Count > 0)
                            {
                                StringBuilder newBlock = new StringBuilder();
                                newBlock.AppendLine("\n# --- IP WRITE BLOCK START ---");
                                newBlock.AppendLine("<Limit POST PUT DELETE>");
                                foreach (string ip in remainingIPs) newBlock.AppendLine($"    Require ip {ip}");
                                newBlock.AppendLine("</Limit>");
                                newBlock.AppendLine("<Limit GET OPTIONS>");
                                newBlock.AppendLine("    Require all granted");
                                newBlock.AppendLine("</Limit>");
                                newBlock.AppendLine("# --- IP WRITE BLOCK END ---");

                                content = (content + newBlock.ToString()).Trim();
                            }

                            File.WriteAllText(htaccessPath, content);
                            hasChanges = true;
                        }
                    }
                }

                if (hasChanges)
                {
                    MessageBox.Show("Selected access rules have been successfully removed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    LoadAccessRights(); // Refresh UI
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to remove access rules: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnResetPermissions_Click(object sender, EventArgs e)
        {
            string rootPath = Properties.Settings.Default.HTMLPath;

            if (string.IsNullOrEmpty(rootPath) || !Directory.Exists(rootPath))
            {
                MessageBox.Show("The HTML Path is not set or does not exist.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(rootPath);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();

                // 1. Find and Remove ALL "Deny" rules that are locking you out
                AuthorizationRuleCollection rules = dSecurity.GetAccessRules(true, false, typeof(System.Security.Principal.SecurityIdentifier));

                bool removedDeny = false;
                foreach (FileSystemAccessRule rule in rules)
                {
                    if (rule.AccessControlType == AccessControlType.Deny)
                    {
                        dSecurity.RemoveAccessRule(rule);
                        removedDeny = true;
                    }
                }

                // 2. Explicitly Grant Full Control to YOUR Windows Account
                string currentUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;
                dSecurity.AddAccessRule(new FileSystemAccessRule(
                    currentUser,
                    FileSystemRights.FullControl,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                    PropagationFlags.None,
                    AccessControlType.Allow));

                // 3. Apply the fixed security back to the folder
                dInfo.SetAccessControl(dSecurity);

                string msg = removedDeny
                    ? $"Lockout Removed! The 'Deny' rule was stripped.\n\nUser '{currentUser}' has been granted Full Control."
                    : $"User '{currentUser}' has been granted Full Control.";

                MessageBox.Show(msg, "Emergency Reset Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the table to show your restored access
                LoadAccessRights();
            }
            catch (UnauthorizedAccessException)
            {
                // Fallback: If Windows completely blocks the C# app from modifying the folder
                MessageBox.Show("Critical Lockout: Windows blocked this app from resetting the permissions.\n\n" +
                                "To fix this manually:\n" +
                                "1. Open Windows File Explorer and go to C:\\xampp.\n" +
                                "2. Right-click your 'htdocs' folder -> Properties.\n" +
                                "3. Go to the 'Security' tab -> click 'Advanced'.\n" +
                                "4. Find the 'Everyone' entry that says 'Deny' under the Type column.\n" +
                                "5. Click it and select 'Remove', then click Apply.",
                                "Manual Unlock Required", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to reset folder permissions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // --- 1. UAC ELEVATION CHECK ---
            if (!IsAdministrator())
            {
                DialogResult result = MessageBox.Show(
                    "Resetting folder permissions requires Administrative privileges.\n\nWould you like to restart the application as Administrator now?",
                    "Elevation Required", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    ProcessStartInfo proc = new ProcessStartInfo();
                    proc.UseShellExecute = true;
                    proc.WorkingDirectory = Environment.CurrentDirectory;
                    proc.FileName = Application.ExecutablePath;
                    proc.Verb = "runas";
                    try { Process.Start(proc); Application.Exit(); } catch { }
                }
                return;
            }

            // --- 2. CONFIRMATION ---
            DialogResult confirm = MessageBox.Show(
                "This will completely reset the security state.\n\nIt will:\n1. Delete the Network Share.\n2. Remove Web IP Restrictions (.htaccess).\n3. Restore default Windows Folder Permissions.\n\n(No physical files/images will be deleted).\n\nProceed?",
                "Confirm Factory Reset", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (confirm != DialogResult.Yes) return;

            string rootPath = Properties.Settings.Default.HTMLPath;
            if (string.IsNullOrEmpty(rootPath) || !Directory.Exists(rootPath)) return;

            try
            {
                List<string> targetFolders = new List<string>();

                // Check if specific folders are selected
                if (clbNetworkItems != null && clbNetworkItems.Items.Count > 0)
                {
                    for (int i = 0; i < clbNetworkItems.Items.Count; i++)
                    {
                        if (clbNetworkItems.GetItemChecked(i))
                        {
                            NetworkItem item = (NetworkItem)clbNetworkItems.Items[i];
                            if (Directory.Exists(item.Path)) targetFolders.Add(item.Path);
                        }
                    }
                }

                // Target root if nothing is selected
                if (targetFolders.Count == 0) targetFolders.Add(rootPath);

                // Identities we previously added that need to be wiped
                SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                SecurityIdentifier systemSid = new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null);
                SecurityIdentifier adminSid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
                string currentUser = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

                // --- 3. EXECUTE RESET ON TARGETS ---
                foreach (string targetPath in targetFolders)
                {
                    // A. Wipe .htaccess IP Rules
                    string htaccessPath = Path.Combine(targetPath, ".htaccess");
                    if (File.Exists(htaccessPath))
                    {
                        string content = File.ReadAllText(htaccessPath);
                        Match blockMatch = System.Text.RegularExpressions.Regex.Match(content, @"# --- IP WRITE BLOCK START ---([\s\S]*?)# --- IP WRITE BLOCK END ---");
                        if (blockMatch.Success)
                        {
                            content = content.Replace(blockMatch.Value, "").Trim();
                            File.WriteAllText(htaccessPath, content);
                        }
                    }

                    // B. Wipe explicit NTFS rules from the target folder
                    WipeFolderPermissions(targetPath, everyone, systemSid, adminSid, currentUser);

                    // C. Wipe explicit NTFS rules from sub-folders
                    string ci4UploadsPath = Path.Combine(targetPath, "writable", "uploads");
                    if (Directory.Exists(ci4UploadsPath)) WipeFolderPermissions(ci4UploadsPath, everyone, systemSid, adminSid, currentUser);

                    string rmsImageDir = Path.Combine(targetPath, "ImageMessageDirectory");
                    if (Directory.Exists(rmsImageDir)) WipeFolderPermissions(rmsImageDir, everyone, systemSid, adminSid, currentUser);
                }

                // --- 4. DELETE ALL NETWORK SHARES ---
                DirectoryInfo rootInfo = new DirectoryInfo(rootPath);
                string shareName = rootInfo.Name;
                // This silently attempts to delete both the clean name and the old ghost share just in case
                string netCommand = $"/c net share {shareName} /delete 2>nul & net share {shareName}_WebShare /delete 2>nul";

                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "cmd.exe";
                    process.StartInfo.Arguments = netCommand;
                    process.StartInfo.CreateNoWindow = true;
                    process.StartInfo.UseShellExecute = false;
                    process.Start();
                    process.WaitForExit();
                }

                MessageBox.Show("Reset Complete!\n\nAll custom permissions, IP restrictions, and network shares applied by this tool have been removed. The folders are back to their default Windows state.", "Reset Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Refresh the table to reflect the wiped data
                LoadAccessRights();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred during reset: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // --- HELPER METHOD TO STRIP EXPLICIT RULES ---
        private void WipeFolderPermissions(string path, SecurityIdentifier everyone, SecurityIdentifier systemSid, SecurityIdentifier adminSid, string currentUser)
        {
            try
            {
                DirectoryInfo dInfo = new DirectoryInfo(path);
                DirectorySecurity security = dInfo.GetAccessControl();

                // Get all EXPLICIT rules (false means ignore inherited parent rules)
                AuthorizationRuleCollection rules = security.GetAccessRules(true, false, typeof(SecurityIdentifier));

                foreach (FileSystemAccessRule rule in rules)
                {
                    // If the rule belongs to one of the groups we manually altered, delete it
                    if (rule.IdentityReference.Value == everyone.Value ||
                        rule.IdentityReference.Value == systemSid.Value ||
                        rule.IdentityReference.Value == adminSid.Value ||
                        rule.IdentityReference.Value.Equals(currentUser, StringComparison.OrdinalIgnoreCase))
                    {
                        security.RemoveAccessRule(rule);
                    }
                }

                // Turn standard Windows inheritance back on
                security.SetAccessRuleProtection(false, false);
                dInfo.SetAccessControl(security);
            }
            catch { /* Ignore if folder doesn't exist or throws an unexpected error */ }
        }
        private void btnchkName_Click_1(object sender, EventArgs e)
        {

        }
    }
}

// Add this at the bottom of the file
public class NetworkItem
{
    public string Path { get; set; }
    public string DisplayName { get; set; }
    public override string ToString() => DisplayName; // What the CheckBox displays
}