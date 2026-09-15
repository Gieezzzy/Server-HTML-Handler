namespace Server_HTML_Handler.View.Contents
{
    partial class ServerControlForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControlServer = new System.Windows.Forms.TabControl();
            this.tabDatabaseUsers = new System.Windows.Forms.TabPage();
            this.grpAccountInfo = new System.Windows.Forms.GroupBox();
            this.btnTestDbConnection = new System.Windows.Forms.Button();
            this.chkShowPassword = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dgvDatabaseUsers = new System.Windows.Forms.DataGridView();
            this.lblNewUsername = new System.Windows.Forms.Label();
            this.txtNewUsername = new System.Windows.Forms.TextBox();
            this.lblNewPassword = new System.Windows.Forms.Label();
            this.txtNewPassword = new System.Windows.Forms.TextBox();
            this.lblUserHost = new System.Windows.Forms.Label();
            this.txtUserHost = new System.Windows.Forms.TextBox();
            this.lblTargetDb = new System.Windows.Forms.Label();
            this.txtTargetDb = new System.Windows.Forms.TextBox();
            this.btnCreateUser = new System.Windows.Forms.Button();
            this.chkGlobalCheckAll = new System.Windows.Forms.CheckBox();
            this.grpDataPriv = new System.Windows.Forms.GroupBox();
            this.chkSelect = new System.Windows.Forms.CheckBox();
            this.chkInsert = new System.Windows.Forms.CheckBox();
            this.chkUpdate = new System.Windows.Forms.CheckBox();
            this.chkDelete = new System.Windows.Forms.CheckBox();
            this.chkFile = new System.Windows.Forms.CheckBox();
            this.grpStructurePriv = new System.Windows.Forms.GroupBox();
            this.chkCreate = new System.Windows.Forms.CheckBox();
            this.chkAlter = new System.Windows.Forms.CheckBox();
            this.chkIndex = new System.Windows.Forms.CheckBox();
            this.chkDrop = new System.Windows.Forms.CheckBox();
            this.chkCreateTemp = new System.Windows.Forms.CheckBox();
            this.chkShowView = new System.Windows.Forms.CheckBox();
            this.chkCreateRoutine = new System.Windows.Forms.CheckBox();
            this.chkAlterRoutine = new System.Windows.Forms.CheckBox();
            this.chkExecute = new System.Windows.Forms.CheckBox();
            this.chkCreateView = new System.Windows.Forms.CheckBox();
            this.chkEvent = new System.Windows.Forms.CheckBox();
            this.chkTrigger = new System.Windows.Forms.CheckBox();
            this.grpAdminPriv = new System.Windows.Forms.GroupBox();
            this.chkGrant = new System.Windows.Forms.CheckBox();
            this.chkSuper = new System.Windows.Forms.CheckBox();
            this.chkProcess = new System.Windows.Forms.CheckBox();
            this.chkReload = new System.Windows.Forms.CheckBox();
            this.chkShutdown = new System.Windows.Forms.CheckBox();
            this.chkShowDb = new System.Windows.Forms.CheckBox();
            this.chkLockTables = new System.Windows.Forms.CheckBox();
            this.chkReferences = new System.Windows.Forms.CheckBox();
            this.chkCreateUser = new System.Windows.Forms.CheckBox();
            this.tabHtmlPermissions = new System.Windows.Forms.TabPage();
            this.grpHtmlPermissions = new System.Windows.Forms.GroupBox();
            this.btnOpenHtmlDir = new System.Windows.Forms.Button();
            this.btnchkName = new System.Windows.Forms.Button();
            this.dgvAclPermissions = new System.Windows.Forms.DataGridView();
            this.btnBrowseACLPath = new System.Windows.Forms.Button();
            this.lblWindowsAccount = new System.Windows.Forms.Label();
            this.txtWindowsAccount = new System.Windows.Forms.TextBox();
            this.lblHtmlPath = new System.Windows.Forms.Label();
            this.txtHtmlPath = new System.Windows.Forms.TextBox();
            this.rdoAccessRead = new System.Windows.Forms.RadioButton();
            this.rdoAccessWrite = new System.Windows.Forms.RadioButton();
            this.btnApplyFolderPermissions = new System.Windows.Forms.Button();
            this.grpStorageFolders = new System.Windows.Forms.GroupBox();
            this.btnOpenStorageDir = new System.Windows.Forms.Button();
            this.lblConsumedStorage = new System.Windows.Forms.Label();
            this.lblAvailableStorage = new System.Windows.Forms.Label();
            this.dgvStorageFolders = new System.Windows.Forms.DataGridView();
            this.btnUploadPath = new System.Windows.Forms.Button();
            this.lblStoragePath = new System.Windows.Forms.Label();
            this.txtStoragePath = new System.Windows.Forms.TextBox();
            this.btnCreateUploadStorage = new System.Windows.Forms.Button();
            this.tabBackupsPaths = new System.Windows.Forms.TabPage();
            this.grpBackupServer = new System.Windows.Forms.GroupBox();
            this.btnRestoreDatabase = new System.Windows.Forms.Button();
            this.lblBackupDbName = new System.Windows.Forms.Label();
            this.txtBackupDbName = new System.Windows.Forms.TextBox();
            this.btnBackupDatabase = new System.Windows.Forms.Button();
            this.btnBackupWebFiles = new System.Windows.Forms.Button();
            this.grpServerPathSettings = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.txthtmlNewPath = new System.Windows.Forms.TextBox();
            this.btnUpdateDocumentRoot = new System.Windows.Forms.Button();
            this.lblMaxFileSizeMb = new System.Windows.Forms.Label();
            this.txtMaxFileSizeMb = new System.Windows.Forms.TextBox();
            this.btnSavePhpLimits = new System.Windows.Forms.Button();
            this.tabServiceHealth = new System.Windows.Forms.TabPage();
            this.grpLogsViewer = new System.Windows.Forms.GroupBox();
            this.btnLoadLogs = new System.Windows.Forms.Button();
            this.cmbLogType = new System.Windows.Forms.ComboBox();
            this.txtServerLogs = new System.Windows.Forms.TextBox();
            this.grpServicesControl = new System.Windows.Forms.GroupBox();
            this.btnRestartApache = new System.Windows.Forms.Button();
            this.btnRefreshServiceStatus = new System.Windows.Forms.Button();
            this.lblMysqlStatus = new System.Windows.Forms.Label();
            this.lblApacheStatus = new System.Windows.Forms.Label();
            this.tabControlServer.SuspendLayout();
            this.tabDatabaseUsers.SuspendLayout();
            this.grpAccountInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatabaseUsers)).BeginInit();
            this.grpDataPriv.SuspendLayout();
            this.grpStructurePriv.SuspendLayout();
            this.grpAdminPriv.SuspendLayout();
            this.tabHtmlPermissions.SuspendLayout();
            this.grpHtmlPermissions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAclPermissions)).BeginInit();
            this.grpStorageFolders.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStorageFolders)).BeginInit();
            this.tabBackupsPaths.SuspendLayout();
            this.grpBackupServer.SuspendLayout();
            this.grpServerPathSettings.SuspendLayout();
            this.tabServiceHealth.SuspendLayout();
            this.grpLogsViewer.SuspendLayout();
            this.grpServicesControl.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControlServer
            // 
            this.tabControlServer.Controls.Add(this.tabDatabaseUsers);
            this.tabControlServer.Controls.Add(this.tabHtmlPermissions);
            this.tabControlServer.Controls.Add(this.tabBackupsPaths);
            this.tabControlServer.Controls.Add(this.tabServiceHealth);
            this.tabControlServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlServer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControlServer.Location = new System.Drawing.Point(0, 0);
            this.tabControlServer.Name = "tabControlServer";
            this.tabControlServer.SelectedIndex = 0;
            this.tabControlServer.Size = new System.Drawing.Size(716, 644);
            this.tabControlServer.TabIndex = 0;
            // 
            // tabDatabaseUsers
            // 
            this.tabDatabaseUsers.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.tabDatabaseUsers.Controls.Add(this.grpAccountInfo);
            this.tabDatabaseUsers.Controls.Add(this.chkGlobalCheckAll);
            this.tabDatabaseUsers.Controls.Add(this.grpDataPriv);
            this.tabDatabaseUsers.Controls.Add(this.grpStructurePriv);
            this.tabDatabaseUsers.Controls.Add(this.grpAdminPriv);
            this.tabDatabaseUsers.ForeColor = System.Drawing.Color.White;
            this.tabDatabaseUsers.Location = new System.Drawing.Point(4, 24);
            this.tabDatabaseUsers.Name = "tabDatabaseUsers";
            this.tabDatabaseUsers.Padding = new System.Windows.Forms.Padding(10);
            this.tabDatabaseUsers.Size = new System.Drawing.Size(708, 616);
            this.tabDatabaseUsers.TabIndex = 0;
            this.tabDatabaseUsers.Text = "Database Privileges & Users";
            // 
            // grpAccountInfo
            // 
            this.grpAccountInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAccountInfo.Controls.Add(this.btnTestDbConnection);
            this.grpAccountInfo.Controls.Add(this.chkShowPassword);
            this.grpAccountInfo.Controls.Add(this.label1);
            this.grpAccountInfo.Controls.Add(this.dgvDatabaseUsers);
            this.grpAccountInfo.Controls.Add(this.lblNewUsername);
            this.grpAccountInfo.Controls.Add(this.txtNewUsername);
            this.grpAccountInfo.Controls.Add(this.lblNewPassword);
            this.grpAccountInfo.Controls.Add(this.txtNewPassword);
            this.grpAccountInfo.Controls.Add(this.lblUserHost);
            this.grpAccountInfo.Controls.Add(this.txtUserHost);
            this.grpAccountInfo.Controls.Add(this.lblTargetDb);
            this.grpAccountInfo.Controls.Add(this.txtTargetDb);
            this.grpAccountInfo.Controls.Add(this.btnCreateUser);
            this.grpAccountInfo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpAccountInfo.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.grpAccountInfo.Location = new System.Drawing.Point(13, 13);
            this.grpAccountInfo.Name = "grpAccountInfo";
            this.grpAccountInfo.Size = new System.Drawing.Size(682, 240);
            this.grpAccountInfo.TabIndex = 0;
            this.grpAccountInfo.TabStop = false;
            this.grpAccountInfo.Text = "User Credentials & Target Database";
            // 
            // btnTestDbConnection
            // 
            this.btnTestDbConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnTestDbConnection.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.btnTestDbConnection.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnTestDbConnection.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestDbConnection.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnTestDbConnection.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnTestDbConnection.Location = new System.Drawing.Point(301, 55);
            this.btnTestDbConnection.Name = "btnTestDbConnection";
            this.btnTestDbConnection.Size = new System.Drawing.Size(162, 27);
            this.btnTestDbConnection.TabIndex = 12;
            this.btnTestDbConnection.Text = "Test Connection";
            this.btnTestDbConnection.UseVisualStyleBackColor = false;
            this.btnTestDbConnection.Click += new System.EventHandler(this.btnTestDbConnection_Click);
            // 
            // chkShowPassword
            // 
            this.chkShowPassword.AutoSize = true;
            this.chkShowPassword.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkShowPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkShowPassword.Location = new System.Drawing.Point(403, 26);
            this.chkShowPassword.Name = "chkShowPassword";
            this.chkShowPassword.Size = new System.Drawing.Size(55, 17);
            this.chkShowPassword.TabIndex = 11;
            this.chkShowPassword.Text = "Show";
            this.chkShowPassword.UseVisualStyleBackColor = true;
            this.chkShowPassword.CheckedChanged += new System.EventHandler(this.chkShowPassword_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.label1.Location = new System.Drawing.Point(3, 88);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "List of Users:";
            // 
            // dgvDatabaseUsers
            // 
            this.dgvDatabaseUsers.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvDatabaseUsers.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvDatabaseUsers.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvDatabaseUsers.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvDatabaseUsers.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvDatabaseUsers.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvDatabaseUsers.EnableHeadersVisualStyles = false;
            this.dgvDatabaseUsers.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.dgvDatabaseUsers.Location = new System.Drawing.Point(6, 104);
            this.dgvDatabaseUsers.Name = "dgvDatabaseUsers";
            this.dgvDatabaseUsers.Size = new System.Drawing.Size(670, 131);
            this.dgvDatabaseUsers.TabIndex = 9;
            this.dgvDatabaseUsers.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDatabaseUsers_CellContentClick);
            // 
            // lblNewUsername
            // 
            this.lblNewUsername.AutoSize = true;
            this.lblNewUsername.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewUsername.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblNewUsername.Location = new System.Drawing.Point(15, 27);
            this.lblNewUsername.Name = "lblNewUsername";
            this.lblNewUsername.Size = new System.Drawing.Size(61, 13);
            this.lblNewUsername.TabIndex = 0;
            this.lblNewUsername.Text = "Username:";
            // 
            // txtNewUsername
            // 
            this.txtNewUsername.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.txtNewUsername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNewUsername.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txtNewUsername.ForeColor = System.Drawing.Color.White;
            this.txtNewUsername.Location = new System.Drawing.Point(79, 24);
            this.txtNewUsername.Name = "txtNewUsername";
            this.txtNewUsername.Size = new System.Drawing.Size(150, 22);
            this.txtNewUsername.TabIndex = 1;
            // 
            // lblNewPassword
            // 
            this.lblNewPassword.AutoSize = true;
            this.lblNewPassword.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNewPassword.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblNewPassword.Location = new System.Drawing.Point(235, 27);
            this.lblNewPassword.Name = "lblNewPassword";
            this.lblNewPassword.Size = new System.Drawing.Size(59, 13);
            this.lblNewPassword.TabIndex = 2;
            this.lblNewPassword.Text = "Password:";
            // 
            // txtNewPassword
            // 
            this.txtNewPassword.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.txtNewPassword.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtNewPassword.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txtNewPassword.ForeColor = System.Drawing.Color.White;
            this.txtNewPassword.Location = new System.Drawing.Point(292, 24);
            this.txtNewPassword.Name = "txtNewPassword";
            this.txtNewPassword.PasswordChar = '*';
            this.txtNewPassword.Size = new System.Drawing.Size(105, 22);
            this.txtNewPassword.TabIndex = 3;
            // 
            // lblUserHost
            // 
            this.lblUserHost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblUserHost.AutoSize = true;
            this.lblUserHost.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUserHost.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblUserHost.Location = new System.Drawing.Point(492, 27);
            this.lblUserHost.Name = "lblUserHost";
            this.lblUserHost.Size = new System.Drawing.Size(78, 13);
            this.lblUserHost.TabIndex = 4;
            this.lblUserHost.Text = "Host (% = all):";
            // 
            // txtUserHost
            // 
            this.txtUserHost.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUserHost.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.txtUserHost.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtUserHost.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txtUserHost.ForeColor = System.Drawing.Color.White;
            this.txtUserHost.Location = new System.Drawing.Point(569, 24);
            this.txtUserHost.Name = "txtUserHost";
            this.txtUserHost.Size = new System.Drawing.Size(100, 22);
            this.txtUserHost.TabIndex = 5;
            this.txtUserHost.Text = "%";
            // 
            // lblTargetDb
            // 
            this.lblTargetDb.AutoSize = true;
            this.lblTargetDb.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTargetDb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblTargetDb.Location = new System.Drawing.Point(15, 62);
            this.lblTargetDb.Name = "lblTargetDb";
            this.lblTargetDb.Size = new System.Drawing.Size(106, 13);
            this.lblTargetDb.TabIndex = 6;
            this.lblTargetDb.Text = "Target Database (*):";
            // 
            // txtTargetDb
            // 
            this.txtTargetDb.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.txtTargetDb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtTargetDb.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txtTargetDb.ForeColor = System.Drawing.Color.White;
            this.txtTargetDb.Location = new System.Drawing.Point(128, 59);
            this.txtTargetDb.Name = "txtTargetDb";
            this.txtTargetDb.Size = new System.Drawing.Size(150, 22);
            this.txtTargetDb.TabIndex = 7;
            // 
            // btnCreateUser
            // 
            this.btnCreateUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateUser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.btnCreateUser.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.btnCreateUser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateUser.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnCreateUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.btnCreateUser.Location = new System.Drawing.Point(469, 52);
            this.btnCreateUser.Name = "btnCreateUser";
            this.btnCreateUser.Size = new System.Drawing.Size(200, 32);
            this.btnCreateUser.TabIndex = 8;
            this.btnCreateUser.Text = "Create User & Grant Privileges";
            this.btnCreateUser.UseVisualStyleBackColor = false;
            this.btnCreateUser.Click += new System.EventHandler(this.btnCreateUser_Click);
            // 
            // chkGlobalCheckAll
            // 
            this.chkGlobalCheckAll.AutoSize = true;
            this.chkGlobalCheckAll.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.chkGlobalCheckAll.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.chkGlobalCheckAll.Location = new System.Drawing.Point(19, 259);
            this.chkGlobalCheckAll.Name = "chkGlobalCheckAll";
            this.chkGlobalCheckAll.Size = new System.Drawing.Size(172, 17);
            this.chkGlobalCheckAll.TabIndex = 1;
            this.chkGlobalCheckAll.Text = "Global Privileges (Check All)";
            this.chkGlobalCheckAll.UseVisualStyleBackColor = true;
            this.chkGlobalCheckAll.CheckedChanged += new System.EventHandler(this.chkGlobalCheckAll_CheckedChanged);
            // 
            // grpDataPriv
            // 
            this.grpDataPriv.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpDataPriv.Controls.Add(this.chkSelect);
            this.grpDataPriv.Controls.Add(this.chkInsert);
            this.grpDataPriv.Controls.Add(this.chkUpdate);
            this.grpDataPriv.Controls.Add(this.chkDelete);
            this.grpDataPriv.Controls.Add(this.chkFile);
            this.grpDataPriv.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpDataPriv.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.grpDataPriv.Location = new System.Drawing.Point(19, 286);
            this.grpDataPriv.Name = "grpDataPriv";
            this.grpDataPriv.Size = new System.Drawing.Size(160, 320);
            this.grpDataPriv.TabIndex = 2;
            this.grpDataPriv.TabStop = false;
            this.grpDataPriv.Text = "Data";
            // 
            // chkSelect
            // 
            this.chkSelect.AutoSize = true;
            this.chkSelect.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkSelect.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkSelect.Location = new System.Drawing.Point(15, 25);
            this.chkSelect.Name = "chkSelect";
            this.chkSelect.Size = new System.Drawing.Size(61, 17);
            this.chkSelect.TabIndex = 0;
            this.chkSelect.Text = "SELECT";
            this.chkSelect.UseVisualStyleBackColor = true;
            // 
            // chkInsert
            // 
            this.chkInsert.AutoSize = true;
            this.chkInsert.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkInsert.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkInsert.Location = new System.Drawing.Point(15, 50);
            this.chkInsert.Name = "chkInsert";
            this.chkInsert.Size = new System.Drawing.Size(61, 17);
            this.chkInsert.TabIndex = 1;
            this.chkInsert.Text = "INSERT";
            this.chkInsert.UseVisualStyleBackColor = true;
            // 
            // chkUpdate
            // 
            this.chkUpdate.AutoSize = true;
            this.chkUpdate.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkUpdate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkUpdate.Location = new System.Drawing.Point(15, 75);
            this.chkUpdate.Name = "chkUpdate";
            this.chkUpdate.Size = new System.Drawing.Size(65, 17);
            this.chkUpdate.TabIndex = 2;
            this.chkUpdate.Text = "UPDATE";
            this.chkUpdate.UseVisualStyleBackColor = true;
            // 
            // chkDelete
            // 
            this.chkDelete.AutoSize = true;
            this.chkDelete.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkDelete.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkDelete.Location = new System.Drawing.Point(15, 100);
            this.chkDelete.Name = "chkDelete";
            this.chkDelete.Size = new System.Drawing.Size(62, 17);
            this.chkDelete.TabIndex = 3;
            this.chkDelete.Text = "DELETE";
            this.chkDelete.UseVisualStyleBackColor = true;
            // 
            // chkFile
            // 
            this.chkFile.AutoSize = true;
            this.chkFile.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkFile.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkFile.Location = new System.Drawing.Point(15, 125);
            this.chkFile.Name = "chkFile";
            this.chkFile.Size = new System.Drawing.Size(46, 17);
            this.chkFile.TabIndex = 4;
            this.chkFile.Text = "FILE";
            this.chkFile.UseVisualStyleBackColor = true;
            // 
            // grpStructurePriv
            // 
            this.grpStructurePriv.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grpStructurePriv.Controls.Add(this.chkCreate);
            this.grpStructurePriv.Controls.Add(this.chkAlter);
            this.grpStructurePriv.Controls.Add(this.chkIndex);
            this.grpStructurePriv.Controls.Add(this.chkDrop);
            this.grpStructurePriv.Controls.Add(this.chkCreateTemp);
            this.grpStructurePriv.Controls.Add(this.chkShowView);
            this.grpStructurePriv.Controls.Add(this.chkCreateRoutine);
            this.grpStructurePriv.Controls.Add(this.chkAlterRoutine);
            this.grpStructurePriv.Controls.Add(this.chkExecute);
            this.grpStructurePriv.Controls.Add(this.chkCreateView);
            this.grpStructurePriv.Controls.Add(this.chkEvent);
            this.grpStructurePriv.Controls.Add(this.chkTrigger);
            this.grpStructurePriv.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpStructurePriv.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.grpStructurePriv.Location = new System.Drawing.Point(191, 286);
            this.grpStructurePriv.Name = "grpStructurePriv";
            this.grpStructurePriv.Size = new System.Drawing.Size(220, 320);
            this.grpStructurePriv.TabIndex = 3;
            this.grpStructurePriv.TabStop = false;
            this.grpStructurePriv.Text = "Structure";
            // 
            // chkCreate
            // 
            this.chkCreate.AutoSize = true;
            this.chkCreate.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkCreate.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkCreate.Location = new System.Drawing.Point(15, 25);
            this.chkCreate.Name = "chkCreate";
            this.chkCreate.Size = new System.Drawing.Size(63, 17);
            this.chkCreate.TabIndex = 0;
            this.chkCreate.Text = "CREATE";
            this.chkCreate.UseVisualStyleBackColor = true;
            // 
            // chkAlter
            // 
            this.chkAlter.AutoSize = true;
            this.chkAlter.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkAlter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkAlter.Location = new System.Drawing.Point(15, 50);
            this.chkAlter.Name = "chkAlter";
            this.chkAlter.Size = new System.Drawing.Size(55, 17);
            this.chkAlter.TabIndex = 1;
            this.chkAlter.Text = "ALTER";
            this.chkAlter.UseVisualStyleBackColor = true;
            // 
            // chkIndex
            // 
            this.chkIndex.AutoSize = true;
            this.chkIndex.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkIndex.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkIndex.Location = new System.Drawing.Point(15, 75);
            this.chkIndex.Name = "chkIndex";
            this.chkIndex.Size = new System.Drawing.Size(57, 17);
            this.chkIndex.TabIndex = 2;
            this.chkIndex.Text = "INDEX";
            this.chkIndex.UseVisualStyleBackColor = true;
            // 
            // chkDrop
            // 
            this.chkDrop.AutoSize = true;
            this.chkDrop.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkDrop.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkDrop.Location = new System.Drawing.Point(15, 100);
            this.chkDrop.Name = "chkDrop";
            this.chkDrop.Size = new System.Drawing.Size(56, 17);
            this.chkDrop.TabIndex = 3;
            this.chkDrop.Text = "DROP";
            this.chkDrop.UseVisualStyleBackColor = true;
            // 
            // chkCreateTemp
            // 
            this.chkCreateTemp.AutoSize = true;
            this.chkCreateTemp.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkCreateTemp.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkCreateTemp.Location = new System.Drawing.Point(15, 125);
            this.chkCreateTemp.Name = "chkCreateTemp";
            this.chkCreateTemp.Size = new System.Drawing.Size(166, 17);
            this.chkCreateTemp.TabIndex = 4;
            this.chkCreateTemp.Text = "CREATE TEMPORARY TABLES";
            this.chkCreateTemp.UseVisualStyleBackColor = true;
            // 
            // chkShowView
            // 
            this.chkShowView.AutoSize = true;
            this.chkShowView.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkShowView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkShowView.Location = new System.Drawing.Point(15, 150);
            this.chkShowView.Name = "chkShowView";
            this.chkShowView.Size = new System.Drawing.Size(90, 17);
            this.chkShowView.TabIndex = 5;
            this.chkShowView.Text = "SHOW VIEW";
            this.chkShowView.UseVisualStyleBackColor = true;
            // 
            // chkCreateRoutine
            // 
            this.chkCreateRoutine.AutoSize = true;
            this.chkCreateRoutine.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkCreateRoutine.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkCreateRoutine.Location = new System.Drawing.Point(15, 175);
            this.chkCreateRoutine.Name = "chkCreateRoutine";
            this.chkCreateRoutine.Size = new System.Drawing.Size(112, 17);
            this.chkCreateRoutine.TabIndex = 6;
            this.chkCreateRoutine.Text = "CREATE ROUTINE";
            this.chkCreateRoutine.UseVisualStyleBackColor = true;
            // 
            // chkAlterRoutine
            // 
            this.chkAlterRoutine.AutoSize = true;
            this.chkAlterRoutine.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkAlterRoutine.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkAlterRoutine.Location = new System.Drawing.Point(15, 200);
            this.chkAlterRoutine.Name = "chkAlterRoutine";
            this.chkAlterRoutine.Size = new System.Drawing.Size(104, 17);
            this.chkAlterRoutine.TabIndex = 7;
            this.chkAlterRoutine.Text = "ALTER ROUTINE";
            this.chkAlterRoutine.UseVisualStyleBackColor = true;
            // 
            // chkExecute
            // 
            this.chkExecute.AutoSize = true;
            this.chkExecute.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkExecute.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkExecute.Location = new System.Drawing.Point(15, 225);
            this.chkExecute.Name = "chkExecute";
            this.chkExecute.Size = new System.Drawing.Size(70, 17);
            this.chkExecute.TabIndex = 8;
            this.chkExecute.Text = "EXECUTE";
            this.chkExecute.UseVisualStyleBackColor = true;
            // 
            // chkCreateView
            // 
            this.chkCreateView.AutoSize = true;
            this.chkCreateView.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkCreateView.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkCreateView.Location = new System.Drawing.Point(15, 250);
            this.chkCreateView.Name = "chkCreateView";
            this.chkCreateView.Size = new System.Drawing.Size(93, 17);
            this.chkCreateView.TabIndex = 9;
            this.chkCreateView.Text = "CREATE VIEW";
            this.chkCreateView.UseVisualStyleBackColor = true;
            // 
            // chkEvent
            // 
            this.chkEvent.AutoSize = true;
            this.chkEvent.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkEvent.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkEvent.Location = new System.Drawing.Point(15, 275);
            this.chkEvent.Name = "chkEvent";
            this.chkEvent.Size = new System.Drawing.Size(58, 17);
            this.chkEvent.TabIndex = 10;
            this.chkEvent.Text = "EVENT";
            this.chkEvent.UseVisualStyleBackColor = true;
            // 
            // chkTrigger
            // 
            this.chkTrigger.AutoSize = true;
            this.chkTrigger.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkTrigger.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkTrigger.Location = new System.Drawing.Point(15, 298);
            this.chkTrigger.Name = "chkTrigger";
            this.chkTrigger.Size = new System.Drawing.Size(70, 17);
            this.chkTrigger.TabIndex = 11;
            this.chkTrigger.Text = "TRIGGER";
            this.chkTrigger.UseVisualStyleBackColor = true;
            // 
            // grpAdminPriv
            // 
            this.grpAdminPriv.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpAdminPriv.Controls.Add(this.chkGrant);
            this.grpAdminPriv.Controls.Add(this.chkSuper);
            this.grpAdminPriv.Controls.Add(this.chkProcess);
            this.grpAdminPriv.Controls.Add(this.chkReload);
            this.grpAdminPriv.Controls.Add(this.chkShutdown);
            this.grpAdminPriv.Controls.Add(this.chkShowDb);
            this.grpAdminPriv.Controls.Add(this.chkLockTables);
            this.grpAdminPriv.Controls.Add(this.chkReferences);
            this.grpAdminPriv.Controls.Add(this.chkCreateUser);
            this.grpAdminPriv.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpAdminPriv.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.grpAdminPriv.Location = new System.Drawing.Point(423, 286);
            this.grpAdminPriv.Name = "grpAdminPriv";
            this.grpAdminPriv.Size = new System.Drawing.Size(272, 320);
            this.grpAdminPriv.TabIndex = 4;
            this.grpAdminPriv.TabStop = false;
            this.grpAdminPriv.Text = "Administration";
            // 
            // chkGrant
            // 
            this.chkGrant.AutoSize = true;
            this.chkGrant.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkGrant.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkGrant.Location = new System.Drawing.Point(15, 25);
            this.chkGrant.Name = "chkGrant";
            this.chkGrant.Size = new System.Drawing.Size(61, 17);
            this.chkGrant.TabIndex = 0;
            this.chkGrant.Text = "GRANT";
            this.chkGrant.UseVisualStyleBackColor = true;
            // 
            // chkSuper
            // 
            this.chkSuper.AutoSize = true;
            this.chkSuper.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkSuper.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkSuper.Location = new System.Drawing.Point(15, 50);
            this.chkSuper.Name = "chkSuper";
            this.chkSuper.Size = new System.Drawing.Size(59, 17);
            this.chkSuper.TabIndex = 1;
            this.chkSuper.Text = "SUPER";
            this.chkSuper.UseVisualStyleBackColor = true;
            // 
            // chkProcess
            // 
            this.chkProcess.AutoSize = true;
            this.chkProcess.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkProcess.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkProcess.Location = new System.Drawing.Point(15, 75);
            this.chkProcess.Name = "chkProcess";
            this.chkProcess.Size = new System.Drawing.Size(73, 17);
            this.chkProcess.TabIndex = 2;
            this.chkProcess.Text = "PROCESS";
            this.chkProcess.UseVisualStyleBackColor = true;
            // 
            // chkReload
            // 
            this.chkReload.AutoSize = true;
            this.chkReload.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkReload.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkReload.Location = new System.Drawing.Point(15, 100);
            this.chkReload.Name = "chkReload";
            this.chkReload.Size = new System.Drawing.Size(68, 17);
            this.chkReload.TabIndex = 3;
            this.chkReload.Text = "RELOAD";
            this.chkReload.UseVisualStyleBackColor = true;
            // 
            // chkShutdown
            // 
            this.chkShutdown.AutoSize = true;
            this.chkShutdown.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkShutdown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkShutdown.Location = new System.Drawing.Point(15, 125);
            this.chkShutdown.Name = "chkShutdown";
            this.chkShutdown.Size = new System.Drawing.Size(89, 17);
            this.chkShutdown.TabIndex = 4;
            this.chkShutdown.Text = "SHUTDOWN";
            this.chkShutdown.UseVisualStyleBackColor = true;
            // 
            // chkShowDb
            // 
            this.chkShowDb.AutoSize = true;
            this.chkShowDb.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkShowDb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkShowDb.Location = new System.Drawing.Point(15, 150);
            this.chkShowDb.Name = "chkShowDb";
            this.chkShowDb.Size = new System.Drawing.Size(120, 17);
            this.chkShowDb.TabIndex = 5;
            this.chkShowDb.Text = "SHOW DATABASES";
            this.chkShowDb.UseVisualStyleBackColor = true;
            // 
            // chkLockTables
            // 
            this.chkLockTables.AutoSize = true;
            this.chkLockTables.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkLockTables.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkLockTables.Location = new System.Drawing.Point(15, 175);
            this.chkLockTables.Name = "chkLockTables";
            this.chkLockTables.Size = new System.Drawing.Size(91, 17);
            this.chkLockTables.TabIndex = 6;
            this.chkLockTables.Text = "LOCK TABLES";
            this.chkLockTables.UseVisualStyleBackColor = true;
            // 
            // chkReferences
            // 
            this.chkReferences.AutoSize = true;
            this.chkReferences.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkReferences.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkReferences.Location = new System.Drawing.Point(15, 200);
            this.chkReferences.Name = "chkReferences";
            this.chkReferences.Size = new System.Drawing.Size(91, 17);
            this.chkReferences.TabIndex = 7;
            this.chkReferences.Text = "REFERENCES";
            this.chkReferences.UseVisualStyleBackColor = true;
            // 
            // chkCreateUser
            // 
            this.chkCreateUser.AutoSize = true;
            this.chkCreateUser.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkCreateUser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.chkCreateUser.Location = new System.Drawing.Point(15, 225);
            this.chkCreateUser.Name = "chkCreateUser";
            this.chkCreateUser.Size = new System.Drawing.Size(93, 17);
            this.chkCreateUser.TabIndex = 8;
            this.chkCreateUser.Text = "CREATE USER";
            this.chkCreateUser.UseVisualStyleBackColor = true;
            // 
            // tabHtmlPermissions
            // 
            this.tabHtmlPermissions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.tabHtmlPermissions.Controls.Add(this.grpHtmlPermissions);
            this.tabHtmlPermissions.Controls.Add(this.grpStorageFolders);
            this.tabHtmlPermissions.ForeColor = System.Drawing.Color.White;
            this.tabHtmlPermissions.Location = new System.Drawing.Point(4, 24);
            this.tabHtmlPermissions.Name = "tabHtmlPermissions";
            this.tabHtmlPermissions.Padding = new System.Windows.Forms.Padding(10);
            this.tabHtmlPermissions.Size = new System.Drawing.Size(708, 616);
            this.tabHtmlPermissions.TabIndex = 1;
            this.tabHtmlPermissions.Text = "HTML Path & Developer Access";
            // 
            // grpHtmlPermissions
            // 
            this.grpHtmlPermissions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpHtmlPermissions.Controls.Add(this.btnOpenHtmlDir);
            this.grpHtmlPermissions.Controls.Add(this.btnchkName);
            this.grpHtmlPermissions.Controls.Add(this.dgvAclPermissions);
            this.grpHtmlPermissions.Controls.Add(this.btnBrowseACLPath);
            this.grpHtmlPermissions.Controls.Add(this.lblWindowsAccount);
            this.grpHtmlPermissions.Controls.Add(this.txtWindowsAccount);
            this.grpHtmlPermissions.Controls.Add(this.lblHtmlPath);
            this.grpHtmlPermissions.Controls.Add(this.txtHtmlPath);
            this.grpHtmlPermissions.Controls.Add(this.rdoAccessRead);
            this.grpHtmlPermissions.Controls.Add(this.rdoAccessWrite);
            this.grpHtmlPermissions.Controls.Add(this.btnApplyFolderPermissions);
            this.grpHtmlPermissions.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpHtmlPermissions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.grpHtmlPermissions.Location = new System.Drawing.Point(3, 13);
            this.grpHtmlPermissions.Name = "grpHtmlPermissions";
            this.grpHtmlPermissions.Size = new System.Drawing.Size(702, 361);
            this.grpHtmlPermissions.TabIndex = 0;
            this.grpHtmlPermissions.TabStop = false;
            this.grpHtmlPermissions.Text = "HTML Directory Developer Access (Windows ACL)";
            // 
            // btnOpenHtmlDir
            // 
            this.btnOpenHtmlDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenHtmlDir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.btnOpenHtmlDir.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnOpenHtmlDir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenHtmlDir.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnOpenHtmlDir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnOpenHtmlDir.Location = new System.Drawing.Point(450, 56);
            this.btnOpenHtmlDir.Name = "btnOpenHtmlDir";
            this.btnOpenHtmlDir.Size = new System.Drawing.Size(93, 30);
            this.btnOpenHtmlDir.TabIndex = 10;
            this.btnOpenHtmlDir.Text = "Open Dir";
            this.btnOpenHtmlDir.UseVisualStyleBackColor = false;
            this.btnOpenHtmlDir.Click += new System.EventHandler(this.btnOpenHtmlDir_Click);
            // 
            // btnchkName
            // 
            this.btnchkName.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnchkName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.btnchkName.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnchkName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnchkName.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnchkName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnchkName.Location = new System.Drawing.Point(552, 21);
            this.btnchkName.Name = "btnchkName";
            this.btnchkName.Size = new System.Drawing.Size(132, 30);
            this.btnchkName.TabIndex = 9;
            this.btnchkName.Text = "Check Name";
            this.btnchkName.UseVisualStyleBackColor = false;
            this.btnchkName.Click += new System.EventHandler(this.btnchkName_Click);
            // 
            // dgvAclPermissions
            // 
            this.dgvAclPermissions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvAclPermissions.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvAclPermissions.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvAclPermissions.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvAclPermissions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvAclPermissions.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvAclPermissions.EnableHeadersVisualStyles = false;
            this.dgvAclPermissions.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.dgvAclPermissions.Location = new System.Drawing.Point(6, 136);
            this.dgvAclPermissions.Name = "dgvAclPermissions";
            this.dgvAclPermissions.Size = new System.Drawing.Size(678, 215);
            this.dgvAclPermissions.TabIndex = 8;
            this.dgvAclPermissions.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvAclPermissions_CellValueChanged);
            // 
            // btnBrowseACLPath
            // 
            this.btnBrowseACLPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBrowseACLPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.btnBrowseACLPath.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnBrowseACLPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBrowseACLPath.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnBrowseACLPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnBrowseACLPath.Location = new System.Drawing.Point(552, 56);
            this.btnBrowseACLPath.Name = "btnBrowseACLPath";
            this.btnBrowseACLPath.Size = new System.Drawing.Size(132, 30);
            this.btnBrowseACLPath.TabIndex = 7;
            this.btnBrowseACLPath.Text = "Browse ACL Path";
            this.btnBrowseACLPath.UseVisualStyleBackColor = false;
            this.btnBrowseACLPath.Click += new System.EventHandler(this.btnBrowseACLPath_Click);
            // 
            // lblWindowsAccount
            // 
            this.lblWindowsAccount.AutoSize = true;
            this.lblWindowsAccount.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWindowsAccount.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblWindowsAccount.Location = new System.Drawing.Point(15, 30);
            this.lblWindowsAccount.Name = "lblWindowsAccount";
            this.lblWindowsAccount.Size = new System.Drawing.Size(122, 13);
            this.lblWindowsAccount.TabIndex = 0;
            this.lblWindowsAccount.Text = "Windows User/Group:";
            // 
            // txtWindowsAccount
            // 
            this.txtWindowsAccount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWindowsAccount.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.txtWindowsAccount.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtWindowsAccount.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txtWindowsAccount.ForeColor = System.Drawing.Color.White;
            this.txtWindowsAccount.Location = new System.Drawing.Point(147, 27);
            this.txtWindowsAccount.Name = "txtWindowsAccount";
            this.txtWindowsAccount.Size = new System.Drawing.Size(280, 22);
            this.txtWindowsAccount.TabIndex = 1;
            this.txtWindowsAccount.Text = "Everyone";
            // 
            // lblHtmlPath
            // 
            this.lblHtmlPath.AutoSize = true;
            this.lblHtmlPath.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHtmlPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblHtmlPath.Location = new System.Drawing.Point(15, 65);
            this.lblHtmlPath.Name = "lblHtmlPath";
            this.lblHtmlPath.Size = new System.Drawing.Size(126, 13);
            this.lblHtmlPath.TabIndex = 2;
            this.lblHtmlPath.Text = "Web Root / HTML Path:";
            // 
            // txtHtmlPath
            // 
            this.txtHtmlPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtHtmlPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.txtHtmlPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtHtmlPath.Enabled = false;
            this.txtHtmlPath.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txtHtmlPath.ForeColor = System.Drawing.Color.White;
            this.txtHtmlPath.Location = new System.Drawing.Point(147, 62);
            this.txtHtmlPath.Name = "txtHtmlPath";
            this.txtHtmlPath.Size = new System.Drawing.Size(280, 22);
            this.txtHtmlPath.TabIndex = 3;
            this.txtHtmlPath.Text = "C:\\xampp\\htdocs";
            // 
            // rdoAccessRead
            // 
            this.rdoAccessRead.AutoSize = true;
            this.rdoAccessRead.Checked = true;
            this.rdoAccessRead.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.rdoAccessRead.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.rdoAccessRead.Location = new System.Drawing.Point(147, 100);
            this.rdoAccessRead.Name = "rdoAccessRead";
            this.rdoAccessRead.Size = new System.Drawing.Size(126, 17);
            this.rdoAccessRead.TabIndex = 4;
            this.rdoAccessRead.TabStop = true;
            this.rdoAccessRead.Text = "Read-Only (Default)";
            this.rdoAccessRead.UseVisualStyleBackColor = true;
            // 
            // rdoAccessWrite
            // 
            this.rdoAccessWrite.AutoSize = true;
            this.rdoAccessWrite.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.rdoAccessWrite.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.rdoAccessWrite.Location = new System.Drawing.Point(280, 100);
            this.rdoAccessWrite.Name = "rdoAccessWrite";
            this.rdoAccessWrite.Size = new System.Drawing.Size(147, 17);
            this.rdoAccessWrite.TabIndex = 5;
            this.rdoAccessWrite.Text = "Read/Write (Full Access)";
            this.rdoAccessWrite.UseVisualStyleBackColor = true;
            // 
            // btnApplyFolderPermissions
            // 
            this.btnApplyFolderPermissions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApplyFolderPermissions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.btnApplyFolderPermissions.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.btnApplyFolderPermissions.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnApplyFolderPermissions.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnApplyFolderPermissions.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.btnApplyFolderPermissions.Location = new System.Drawing.Point(552, 100);
            this.btnApplyFolderPermissions.Name = "btnApplyFolderPermissions";
            this.btnApplyFolderPermissions.Size = new System.Drawing.Size(132, 30);
            this.btnApplyFolderPermissions.TabIndex = 6;
            this.btnApplyFolderPermissions.Text = "Apply ACL Permissions";
            this.btnApplyFolderPermissions.UseVisualStyleBackColor = false;
            this.btnApplyFolderPermissions.Click += new System.EventHandler(this.btnApplyFolderPermissions_Click);
            // 
            // grpStorageFolders
            // 
            this.grpStorageFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpStorageFolders.Controls.Add(this.btnOpenStorageDir);
            this.grpStorageFolders.Controls.Add(this.lblConsumedStorage);
            this.grpStorageFolders.Controls.Add(this.lblAvailableStorage);
            this.grpStorageFolders.Controls.Add(this.dgvStorageFolders);
            this.grpStorageFolders.Controls.Add(this.btnUploadPath);
            this.grpStorageFolders.Controls.Add(this.lblStoragePath);
            this.grpStorageFolders.Controls.Add(this.txtStoragePath);
            this.grpStorageFolders.Controls.Add(this.btnCreateUploadStorage);
            this.grpStorageFolders.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpStorageFolders.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.grpStorageFolders.Location = new System.Drawing.Point(3, 380);
            this.grpStorageFolders.Name = "grpStorageFolders";
            this.grpStorageFolders.Size = new System.Drawing.Size(702, 230);
            this.grpStorageFolders.TabIndex = 1;
            this.grpStorageFolders.TabStop = false;
            this.grpStorageFolders.Text = "File Uploads & External Storage Management";
            // 
            // btnOpenStorageDir
            // 
            this.btnOpenStorageDir.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOpenStorageDir.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.btnOpenStorageDir.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnOpenStorageDir.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenStorageDir.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnOpenStorageDir.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnOpenStorageDir.Location = new System.Drawing.Point(450, 14);
            this.btnOpenStorageDir.Name = "btnOpenStorageDir";
            this.btnOpenStorageDir.Size = new System.Drawing.Size(93, 30);
            this.btnOpenStorageDir.TabIndex = 12;
            this.btnOpenStorageDir.Text = "Open Dir";
            this.btnOpenStorageDir.UseVisualStyleBackColor = false;
            this.btnOpenStorageDir.Click += new System.EventHandler(this.btnOpenStorageDir_Click);
            // 
            // lblConsumedStorage
            // 
            this.lblConsumedStorage.AutoSize = true;
            this.lblConsumedStorage.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblConsumedStorage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.lblConsumedStorage.Location = new System.Drawing.Point(252, 44);
            this.lblConsumedStorage.Name = "lblConsumedStorage";
            this.lblConsumedStorage.Size = new System.Drawing.Size(15, 13);
            this.lblConsumedStorage.TabIndex = 11;
            this.lblConsumedStorage.Text = "--";
            // 
            // lblAvailableStorage
            // 
            this.lblAvailableStorage.AutoSize = true;
            this.lblAvailableStorage.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.lblAvailableStorage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.lblAvailableStorage.Location = new System.Drawing.Point(32, 44);
            this.lblAvailableStorage.Name = "lblAvailableStorage";
            this.lblAvailableStorage.Size = new System.Drawing.Size(15, 13);
            this.lblAvailableStorage.TabIndex = 10;
            this.lblAvailableStorage.Text = "--";
            // 
            // dgvStorageFolders
            // 
            this.dgvStorageFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvStorageFolders.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvStorageFolders.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.dgvStorageFolders.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvStorageFolders.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            dataGridViewCellStyle5.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle5.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(122)))), ((int)(((byte)(204)))));
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvStorageFolders.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvStorageFolders.EnableHeadersVisualStyles = false;
            this.dgvStorageFolders.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.dgvStorageFolders.Location = new System.Drawing.Point(6, 80);
            this.dgvStorageFolders.Name = "dgvStorageFolders";
            this.dgvStorageFolders.Size = new System.Drawing.Size(678, 145);
            this.dgvStorageFolders.TabIndex = 9;
            this.dgvStorageFolders.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvStorageFolders_CellContentClick);
            // 
            // btnUploadPath
            // 
            this.btnUploadPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUploadPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.btnUploadPath.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnUploadPath.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUploadPath.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnUploadPath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnUploadPath.Location = new System.Drawing.Point(552, 14);
            this.btnUploadPath.Name = "btnUploadPath";
            this.btnUploadPath.Size = new System.Drawing.Size(132, 30);
            this.btnUploadPath.TabIndex = 8;
            this.btnUploadPath.Text = "Browse Upload Path";
            this.btnUploadPath.UseVisualStyleBackColor = false;
            this.btnUploadPath.Click += new System.EventHandler(this.btnUploadPath_Click);
            // 
            // lblStoragePath
            // 
            this.lblStoragePath.AutoSize = true;
            this.lblStoragePath.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStoragePath.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblStoragePath.Location = new System.Drawing.Point(32, 22);
            this.lblStoragePath.Name = "lblStoragePath";
            this.lblStoragePath.Size = new System.Drawing.Size(117, 13);
            this.lblStoragePath.TabIndex = 0;
            this.lblStoragePath.Text = "Upload Storage Path:";
            // 
            // txtStoragePath
            // 
            this.txtStoragePath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtStoragePath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.txtStoragePath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStoragePath.Enabled = false;
            this.txtStoragePath.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txtStoragePath.ForeColor = System.Drawing.Color.White;
            this.txtStoragePath.Location = new System.Drawing.Point(147, 19);
            this.txtStoragePath.Name = "txtStoragePath";
            this.txtStoragePath.Size = new System.Drawing.Size(280, 22);
            this.txtStoragePath.TabIndex = 1;
            this.txtStoragePath.Text = "C:\\xampp\\htdocs\\uploads";
            // 
            // btnCreateUploadStorage
            // 
            this.btnCreateUploadStorage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCreateUploadStorage.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.btnCreateUploadStorage.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.btnCreateUploadStorage.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreateUploadStorage.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnCreateUploadStorage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.btnCreateUploadStorage.Location = new System.Drawing.Point(552, 44);
            this.btnCreateUploadStorage.Name = "btnCreateUploadStorage";
            this.btnCreateUploadStorage.Size = new System.Drawing.Size(132, 30);
            this.btnCreateUploadStorage.TabIndex = 2;
            this.btnCreateUploadStorage.Text = "Create Storage Folder";
            this.btnCreateUploadStorage.UseVisualStyleBackColor = false;
            this.btnCreateUploadStorage.Click += new System.EventHandler(this.btnCreateUploadStorage_Click);
            // 
            // tabBackupsPaths
            // 
            this.tabBackupsPaths.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.tabBackupsPaths.Controls.Add(this.grpBackupServer);
            this.tabBackupsPaths.Controls.Add(this.grpServerPathSettings);
            this.tabBackupsPaths.ForeColor = System.Drawing.Color.White;
            this.tabBackupsPaths.Location = new System.Drawing.Point(4, 24);
            this.tabBackupsPaths.Name = "tabBackupsPaths";
            this.tabBackupsPaths.Padding = new System.Windows.Forms.Padding(10);
            this.tabBackupsPaths.Size = new System.Drawing.Size(708, 616);
            this.tabBackupsPaths.TabIndex = 2;
            this.tabBackupsPaths.Text = "Backups, File Size & Path Transfer";
            // 
            // grpBackupServer
            // 
            this.grpBackupServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpBackupServer.Controls.Add(this.btnRestoreDatabase);
            this.grpBackupServer.Controls.Add(this.lblBackupDbName);
            this.grpBackupServer.Controls.Add(this.txtBackupDbName);
            this.grpBackupServer.Controls.Add(this.btnBackupDatabase);
            this.grpBackupServer.Controls.Add(this.btnBackupWebFiles);
            this.grpBackupServer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpBackupServer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.grpBackupServer.Location = new System.Drawing.Point(13, 13);
            this.grpBackupServer.Name = "grpBackupServer";
            this.grpBackupServer.Size = new System.Drawing.Size(682, 120);
            this.grpBackupServer.TabIndex = 0;
            this.grpBackupServer.TabStop = false;
            this.grpBackupServer.Text = "Server & Database Backup Management";
            // 
            // btnRestoreDatabase
            // 
            this.btnRestoreDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRestoreDatabase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.btnRestoreDatabase.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(179)))), ((int)(((byte)(0)))));
            this.btnRestoreDatabase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestoreDatabase.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnRestoreDatabase.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(179)))), ((int)(((byte)(0)))));
            this.btnRestoreDatabase.Location = new System.Drawing.Point(544, 22);
            this.btnRestoreDatabase.Name = "btnRestoreDatabase";
            this.btnRestoreDatabase.Size = new System.Drawing.Size(125, 30);
            this.btnRestoreDatabase.TabIndex = 4;
            this.btnRestoreDatabase.Text = "Restore SQL";
            this.btnRestoreDatabase.UseVisualStyleBackColor = false;
            this.btnRestoreDatabase.Click += new System.EventHandler(this.btnRestoreDatabase_Click);
            // 
            // lblBackupDbName
            // 
            this.lblBackupDbName.AutoSize = true;
            this.lblBackupDbName.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblBackupDbName.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblBackupDbName.Location = new System.Drawing.Point(15, 30);
            this.lblBackupDbName.Name = "lblBackupDbName";
            this.lblBackupDbName.Size = new System.Drawing.Size(123, 13);
            this.lblBackupDbName.TabIndex = 0;
            this.lblBackupDbName.Text = "Database (Blank = All):";
            // 
            // txtBackupDbName
            // 
            this.txtBackupDbName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBackupDbName.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.txtBackupDbName.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtBackupDbName.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txtBackupDbName.ForeColor = System.Drawing.Color.White;
            this.txtBackupDbName.Location = new System.Drawing.Point(147, 27);
            this.txtBackupDbName.Name = "txtBackupDbName";
            this.txtBackupDbName.Size = new System.Drawing.Size(195, 22);
            this.txtBackupDbName.TabIndex = 1;
            // 
            // btnBackupDatabase
            // 
            this.btnBackupDatabase.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBackupDatabase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.btnBackupDatabase.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.btnBackupDatabase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackupDatabase.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnBackupDatabase.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.btnBackupDatabase.Location = new System.Drawing.Point(355, 22);
            this.btnBackupDatabase.Name = "btnBackupDatabase";
            this.btnBackupDatabase.Size = new System.Drawing.Size(180, 30);
            this.btnBackupDatabase.TabIndex = 2;
            this.btnBackupDatabase.Text = "Backup Database (.SQL)";
            this.btnBackupDatabase.UseVisualStyleBackColor = false;
            this.btnBackupDatabase.Click += new System.EventHandler(this.btnBackupDatabase_Click);
            // 
            // btnBackupWebFiles
            // 
            this.btnBackupWebFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnBackupWebFiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.btnBackupWebFiles.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.btnBackupWebFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBackupWebFiles.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnBackupWebFiles.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.btnBackupWebFiles.Location = new System.Drawing.Point(355, 58);
            this.btnBackupWebFiles.Name = "btnBackupWebFiles";
            this.btnBackupWebFiles.Size = new System.Drawing.Size(180, 30);
            this.btnBackupWebFiles.TabIndex = 3;
            this.btnBackupWebFiles.Text = "Backup Web Files (.ZIP)";
            this.btnBackupWebFiles.UseVisualStyleBackColor = false;
            this.btnBackupWebFiles.Click += new System.EventHandler(this.btnBackupWebFiles_Click);
            // 
            // grpServerPathSettings
            // 
            this.grpServerPathSettings.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpServerPathSettings.Controls.Add(this.label2);
            this.grpServerPathSettings.Controls.Add(this.button1);
            this.grpServerPathSettings.Controls.Add(this.txthtmlNewPath);
            this.grpServerPathSettings.Controls.Add(this.btnUpdateDocumentRoot);
            this.grpServerPathSettings.Controls.Add(this.lblMaxFileSizeMb);
            this.grpServerPathSettings.Controls.Add(this.txtMaxFileSizeMb);
            this.grpServerPathSettings.Controls.Add(this.btnSavePhpLimits);
            this.grpServerPathSettings.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpServerPathSettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.grpServerPathSettings.Location = new System.Drawing.Point(13, 150);
            this.grpServerPathSettings.Name = "grpServerPathSettings";
            this.grpServerPathSettings.Size = new System.Drawing.Size(682, 130);
            this.grpServerPathSettings.TabIndex = 1;
            this.grpServerPathSettings.TabStop = false;
            this.grpServerPathSettings.Text = "HTML Path Transfer & PHP File Limits";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.label2.Location = new System.Drawing.Point(15, 25);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Deployment Path:";
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.button1.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.button1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.button1.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.button1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.button1.Location = new System.Drawing.Point(260, 35);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 30);
            this.button1.TabIndex = 8;
            this.button1.Text = "Browse Path";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txthtmlNewPath
            // 
            this.txthtmlNewPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txthtmlNewPath.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.txthtmlNewPath.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txthtmlNewPath.Enabled = false;
            this.txthtmlNewPath.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txthtmlNewPath.ForeColor = System.Drawing.Color.White;
            this.txthtmlNewPath.Location = new System.Drawing.Point(18, 41);
            this.txthtmlNewPath.Name = "txthtmlNewPath";
            this.txthtmlNewPath.Size = new System.Drawing.Size(223, 22);
            this.txthtmlNewPath.TabIndex = 4;
            // 
            // btnUpdateDocumentRoot
            // 
            this.btnUpdateDocumentRoot.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdateDocumentRoot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.btnUpdateDocumentRoot.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.btnUpdateDocumentRoot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdateDocumentRoot.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnUpdateDocumentRoot.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.btnUpdateDocumentRoot.Location = new System.Drawing.Point(419, 34);
            this.btnUpdateDocumentRoot.Name = "btnUpdateDocumentRoot";
            this.btnUpdateDocumentRoot.Size = new System.Drawing.Size(250, 32);
            this.btnUpdateDocumentRoot.TabIndex = 0;
            this.btnUpdateDocumentRoot.Text = "Transfer Server Path (Update httpd.conf)";
            this.btnUpdateDocumentRoot.UseVisualStyleBackColor = false;
            this.btnUpdateDocumentRoot.Click += new System.EventHandler(this.btnUpdateDocumentRoot_Click);
            // 
            // lblMaxFileSizeMb
            // 
            this.lblMaxFileSizeMb.AutoSize = true;
            this.lblMaxFileSizeMb.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMaxFileSizeMb.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblMaxFileSizeMb.Location = new System.Drawing.Point(15, 80);
            this.lblMaxFileSizeMb.Name = "lblMaxFileSizeMb";
            this.lblMaxFileSizeMb.Size = new System.Drawing.Size(144, 13);
            this.lblMaxFileSizeMb.TabIndex = 1;
            this.lblMaxFileSizeMb.Text = "PHP Max Upload Size (MB):";
            // 
            // txtMaxFileSizeMb
            // 
            this.txtMaxFileSizeMb.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.txtMaxFileSizeMb.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtMaxFileSizeMb.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txtMaxFileSizeMb.ForeColor = System.Drawing.Color.White;
            this.txtMaxFileSizeMb.Location = new System.Drawing.Point(161, 77);
            this.txtMaxFileSizeMb.Name = "txtMaxFileSizeMb";
            this.txtMaxFileSizeMb.Size = new System.Drawing.Size(80, 22);
            this.txtMaxFileSizeMb.TabIndex = 2;
            this.txtMaxFileSizeMb.Text = "64";
            // 
            // btnSavePhpLimits
            // 
            this.btnSavePhpLimits.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSavePhpLimits.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.btnSavePhpLimits.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.btnSavePhpLimits.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSavePhpLimits.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnSavePhpLimits.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.btnSavePhpLimits.Location = new System.Drawing.Point(260, 69);
            this.btnSavePhpLimits.Name = "btnSavePhpLimits";
            this.btnSavePhpLimits.Size = new System.Drawing.Size(150, 28);
            this.btnSavePhpLimits.TabIndex = 3;
            this.btnSavePhpLimits.Text = "Save PHP File Limits";
            this.btnSavePhpLimits.UseVisualStyleBackColor = false;
            this.btnSavePhpLimits.Click += new System.EventHandler(this.btnSavePhpLimits_Click);
            // 
            // tabServiceHealth
            // 
            this.tabServiceHealth.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.tabServiceHealth.Controls.Add(this.grpLogsViewer);
            this.tabServiceHealth.Controls.Add(this.grpServicesControl);
            this.tabServiceHealth.ForeColor = System.Drawing.Color.White;
            this.tabServiceHealth.Location = new System.Drawing.Point(4, 24);
            this.tabServiceHealth.Name = "tabServiceHealth";
            this.tabServiceHealth.Padding = new System.Windows.Forms.Padding(10);
            this.tabServiceHealth.Size = new System.Drawing.Size(708, 616);
            this.tabServiceHealth.TabIndex = 3;
            this.tabServiceHealth.Text = "Server Health & Logs";
            // 
            // grpLogsViewer
            // 
            this.grpLogsViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpLogsViewer.Controls.Add(this.btnLoadLogs);
            this.grpLogsViewer.Controls.Add(this.cmbLogType);
            this.grpLogsViewer.Controls.Add(this.txtServerLogs);
            this.grpLogsViewer.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpLogsViewer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.grpLogsViewer.Location = new System.Drawing.Point(13, 110);
            this.grpLogsViewer.Name = "grpLogsViewer";
            this.grpLogsViewer.Size = new System.Drawing.Size(682, 495);
            this.grpLogsViewer.TabIndex = 1;
            this.grpLogsViewer.TabStop = false;
            this.grpLogsViewer.Text = "Server Live Logs Viewer";
            // 
            // btnLoadLogs
            // 
            this.btnLoadLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoadLogs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.btnLoadLogs.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnLoadLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLoadLogs.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnLoadLogs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnLoadLogs.Location = new System.Drawing.Point(535, 23);
            this.btnLoadLogs.Name = "btnLoadLogs";
            this.btnLoadLogs.Size = new System.Drawing.Size(130, 25);
            this.btnLoadLogs.TabIndex = 2;
            this.btnLoadLogs.Text = "Load / Refresh Logs";
            this.btnLoadLogs.UseVisualStyleBackColor = false;
            this.btnLoadLogs.Click += new System.EventHandler(this.btnLoadLogs_Click);
            // 
            // cmbLogType
            // 
            this.cmbLogType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(42)))), ((int)(((byte)(42)))), ((int)(((byte)(42)))));
            this.cmbLogType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLogType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbLogType.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.cmbLogType.ForeColor = System.Drawing.Color.White;
            this.cmbLogType.FormattingEnabled = true;
            this.cmbLogType.Items.AddRange(new object[] {
            "Apache Error Log",
            "Apache Access Log",
            "PHP Error Log"});
            this.cmbLogType.Location = new System.Drawing.Point(15, 25);
            this.cmbLogType.Name = "cmbLogType";
            this.cmbLogType.Size = new System.Drawing.Size(200, 21);
            this.cmbLogType.TabIndex = 1;
            // 
            // txtServerLogs
            // 
            this.txtServerLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServerLogs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(15)))), ((int)(((byte)(15)))));
            this.txtServerLogs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtServerLogs.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtServerLogs.ForeColor = System.Drawing.Color.Lime;
            this.txtServerLogs.Location = new System.Drawing.Point(15, 58);
            this.txtServerLogs.Multiline = true;
            this.txtServerLogs.Name = "txtServerLogs";
            this.txtServerLogs.ReadOnly = true;
            this.txtServerLogs.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtServerLogs.Size = new System.Drawing.Size(650, 420);
            this.txtServerLogs.TabIndex = 0;
            // 
            // grpServicesControl
            // 
            this.grpServicesControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpServicesControl.Controls.Add(this.btnRestartApache);
            this.grpServicesControl.Controls.Add(this.btnRefreshServiceStatus);
            this.grpServicesControl.Controls.Add(this.lblMysqlStatus);
            this.grpServicesControl.Controls.Add(this.lblApacheStatus);
            this.grpServicesControl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpServicesControl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.grpServicesControl.Location = new System.Drawing.Point(13, 13);
            this.grpServicesControl.Name = "grpServicesControl";
            this.grpServicesControl.Size = new System.Drawing.Size(682, 85);
            this.grpServicesControl.TabIndex = 0;
            this.grpServicesControl.TabStop = false;
            this.grpServicesControl.Text = "Process Monitor & Control";
            // 
            // btnRestartApache
            // 
            this.btnRestartApache.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRestartApache.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.btnRestartApache.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(179)))), ((int)(((byte)(0)))));
            this.btnRestartApache.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRestartApache.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnRestartApache.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(179)))), ((int)(((byte)(0)))));
            this.btnRestartApache.Location = new System.Drawing.Point(535, 27);
            this.btnRestartApache.Name = "btnRestartApache";
            this.btnRestartApache.Size = new System.Drawing.Size(130, 30);
            this.btnRestartApache.TabIndex = 3;
            this.btnRestartApache.Text = "Restart Apache";
            this.btnRestartApache.UseVisualStyleBackColor = false;
            this.btnRestartApache.Click += new System.EventHandler(this.btnRestartApache_Click);
            // 
            // btnRefreshServiceStatus
            // 
            this.btnRefreshServiceStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshServiceStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.btnRefreshServiceStatus.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnRefreshServiceStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshServiceStatus.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Bold);
            this.btnRefreshServiceStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnRefreshServiceStatus.Location = new System.Drawing.Point(388, 27);
            this.btnRefreshServiceStatus.Name = "btnRefreshServiceStatus";
            this.btnRefreshServiceStatus.Size = new System.Drawing.Size(130, 30);
            this.btnRefreshServiceStatus.TabIndex = 2;
            this.btnRefreshServiceStatus.Text = "Check Services";
            this.btnRefreshServiceStatus.UseVisualStyleBackColor = false;
            this.btnRefreshServiceStatus.Click += new System.EventHandler(this.btnRefreshServiceStatus_Click);
            // 
            // lblMysqlStatus
            // 
            this.lblMysqlStatus.AutoSize = true;
            this.lblMysqlStatus.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMysqlStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.lblMysqlStatus.Location = new System.Drawing.Point(180, 33);
            this.lblMysqlStatus.Name = "lblMysqlStatus";
            this.lblMysqlStatus.Size = new System.Drawing.Size(127, 17);
            this.lblMysqlStatus.TabIndex = 1;
            this.lblMysqlStatus.Text = "MySQL: Checking...";
            // 
            // lblApacheStatus
            // 
            this.lblApacheStatus.AutoSize = true;
            this.lblApacheStatus.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblApacheStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.lblApacheStatus.Location = new System.Drawing.Point(18, 33);
            this.lblApacheStatus.Name = "lblApacheStatus";
            this.lblApacheStatus.Size = new System.Drawing.Size(129, 17);
            this.lblApacheStatus.TabIndex = 0;
            this.lblApacheStatus.Text = "Apache: Checking...";
            // 
            // ServerControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(25)))), ((int)(((byte)(25)))));
            this.Controls.Add(this.tabControlServer);
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "ServerControlForm";
            this.Size = new System.Drawing.Size(716, 644);
            this.tabControlServer.ResumeLayout(false);
            this.tabDatabaseUsers.ResumeLayout(false);
            this.tabDatabaseUsers.PerformLayout();
            this.grpAccountInfo.ResumeLayout(false);
            this.grpAccountInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvDatabaseUsers)).EndInit();
            this.grpDataPriv.ResumeLayout(false);
            this.grpDataPriv.PerformLayout();
            this.grpStructurePriv.ResumeLayout(false);
            this.grpStructurePriv.PerformLayout();
            this.grpAdminPriv.ResumeLayout(false);
            this.grpAdminPriv.PerformLayout();
            this.tabHtmlPermissions.ResumeLayout(false);
            this.grpHtmlPermissions.ResumeLayout(false);
            this.grpHtmlPermissions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvAclPermissions)).EndInit();
            this.grpStorageFolders.ResumeLayout(false);
            this.grpStorageFolders.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStorageFolders)).EndInit();
            this.tabBackupsPaths.ResumeLayout(false);
            this.grpBackupServer.ResumeLayout(false);
            this.grpBackupServer.PerformLayout();
            this.grpServerPathSettings.ResumeLayout(false);
            this.grpServerPathSettings.PerformLayout();
            this.tabServiceHealth.ResumeLayout(false);
            this.grpLogsViewer.ResumeLayout(false);
            this.grpLogsViewer.PerformLayout();
            this.grpServicesControl.ResumeLayout(false);
            this.grpServicesControl.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControlServer;
        private System.Windows.Forms.TabPage tabDatabaseUsers;
        private System.Windows.Forms.TabPage tabHtmlPermissions;
        private System.Windows.Forms.TabPage tabBackupsPaths;
        private System.Windows.Forms.TabPage tabServiceHealth;

        // Tab 1 Controls
        private System.Windows.Forms.GroupBox grpAccountInfo;
        private System.Windows.Forms.Label lblNewUsername;
        private System.Windows.Forms.TextBox txtNewUsername;
        private System.Windows.Forms.Label lblNewPassword;
        private System.Windows.Forms.TextBox txtNewPassword;
        private System.Windows.Forms.Label lblUserHost;
        private System.Windows.Forms.TextBox txtUserHost;
        private System.Windows.Forms.Label lblTargetDb;
        private System.Windows.Forms.TextBox txtTargetDb;
        private System.Windows.Forms.Button btnCreateUser;
        private System.Windows.Forms.CheckBox chkShowPassword;
        private System.Windows.Forms.Button btnTestDbConnection;

        private System.Windows.Forms.CheckBox chkGlobalCheckAll;
        private System.Windows.Forms.GroupBox grpDataPriv;
        private System.Windows.Forms.CheckBox chkSelect;
        private System.Windows.Forms.CheckBox chkInsert;
        private System.Windows.Forms.CheckBox chkUpdate;
        private System.Windows.Forms.CheckBox chkDelete;
        private System.Windows.Forms.CheckBox chkFile;

        private System.Windows.Forms.GroupBox grpStructurePriv;
        private System.Windows.Forms.CheckBox chkCreate;
        private System.Windows.Forms.CheckBox chkAlter;
        private System.Windows.Forms.CheckBox chkIndex;
        private System.Windows.Forms.CheckBox chkDrop;
        private System.Windows.Forms.CheckBox chkCreateTemp;
        private System.Windows.Forms.CheckBox chkShowView;
        private System.Windows.Forms.CheckBox chkCreateRoutine;
        private System.Windows.Forms.CheckBox chkAlterRoutine;
        private System.Windows.Forms.CheckBox chkExecute;
        private System.Windows.Forms.CheckBox chkCreateView;
        private System.Windows.Forms.CheckBox chkEvent;
        private System.Windows.Forms.CheckBox chkTrigger;

        private System.Windows.Forms.GroupBox grpAdminPriv;
        private System.Windows.Forms.CheckBox chkGrant;
        private System.Windows.Forms.CheckBox chkSuper;
        private System.Windows.Forms.CheckBox chkProcess;
        private System.Windows.Forms.CheckBox chkReload;
        private System.Windows.Forms.CheckBox chkShutdown;
        private System.Windows.Forms.CheckBox chkShowDb;
        private System.Windows.Forms.CheckBox chkLockTables;
        private System.Windows.Forms.CheckBox chkReferences;
        private System.Windows.Forms.CheckBox chkCreateUser;

        // Tab 2 Controls
        private System.Windows.Forms.GroupBox grpHtmlPermissions;
        private System.Windows.Forms.Label lblWindowsAccount;
        private System.Windows.Forms.TextBox txtWindowsAccount;
        private System.Windows.Forms.Label lblHtmlPath;
        private System.Windows.Forms.TextBox txtHtmlPath;
        private System.Windows.Forms.RadioButton rdoAccessRead;
        private System.Windows.Forms.RadioButton rdoAccessWrite;
        private System.Windows.Forms.Button btnApplyFolderPermissions;
        private System.Windows.Forms.Button btnOpenHtmlDir;

        private System.Windows.Forms.GroupBox grpStorageFolders;
        private System.Windows.Forms.Label lblStoragePath;
        private System.Windows.Forms.TextBox txtStoragePath;
        private System.Windows.Forms.Button btnCreateUploadStorage;
        private System.Windows.Forms.Button btnOpenStorageDir;

        // Tab 3 Controls
        private System.Windows.Forms.GroupBox grpBackupServer;
        private System.Windows.Forms.Label lblBackupDbName;
        private System.Windows.Forms.TextBox txtBackupDbName;
        private System.Windows.Forms.Button btnBackupDatabase;
        private System.Windows.Forms.Button btnBackupWebFiles;
        private System.Windows.Forms.Button btnRestoreDatabase;

        private System.Windows.Forms.GroupBox grpServerPathSettings;
        private System.Windows.Forms.Button btnUpdateDocumentRoot;
        private System.Windows.Forms.Label lblMaxFileSizeMb;
        private System.Windows.Forms.TextBox txtMaxFileSizeMb;
        private System.Windows.Forms.Button btnSavePhpLimits;
        private System.Windows.Forms.Button btnBrowseACLPath;
        private System.Windows.Forms.Button btnUploadPath;
        private System.Windows.Forms.DataGridView dgvAclPermissions;
        private System.Windows.Forms.DataGridView dgvStorageFolders;
        private System.Windows.Forms.Label lblAvailableStorage;
        private System.Windows.Forms.Label lblConsumedStorage;
        private System.Windows.Forms.DataGridView dgvDatabaseUsers;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnchkName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txthtmlNewPath;

        // Tab 4 Controls
        private System.Windows.Forms.GroupBox grpServicesControl;
        private System.Windows.Forms.Label lblApacheStatus;
        private System.Windows.Forms.Label lblMysqlStatus;
        private System.Windows.Forms.Button btnRefreshServiceStatus;
        private System.Windows.Forms.Button btnRestartApache;
        private System.Windows.Forms.GroupBox grpLogsViewer;
        private System.Windows.Forms.TextBox txtServerLogs;
        private System.Windows.Forms.ComboBox cmbLogType;
        private System.Windows.Forms.Button btnLoadLogs;
    }
}