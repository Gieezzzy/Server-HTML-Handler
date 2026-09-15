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
using static Server_HTML_Handler.Helper.ServerBackupHelper;

namespace Server_HTML_Handler.View.Contents
{
    public partial class ServerControlForm : UserControl
    {
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
                    MessageBox.Show("Database connection failed. Please check MySQL server status.", "Connection Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
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
            bool isApacheRunning = await Task.Run(() => Process.GetProcessesByName("httpd").Length > 0);
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
    }
}