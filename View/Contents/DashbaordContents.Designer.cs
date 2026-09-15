namespace Server_HTML_Handler.View.Contents
{
    partial class DashboardContents
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.tabControlMain = new System.Windows.Forms.TabControl();
            this.tabPageDashboard = new System.Windows.Forms.TabPage();
            this.grpServerToolbar = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtIP = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtServerPort = new System.Windows.Forms.TextBox();
            this.lblStatusTitle = new System.Windows.Forms.Label();
            this.lblStatusValue = new System.Windows.Forms.Label();
            this.btnStartServer = new System.Windows.Forms.Button();
            this.btnStopServer = new System.Windows.Forms.Button();
            this.btnOpenBrowser = new System.Windows.Forms.Button();
            this.btnOpenFolder = new System.Windows.Forms.Button();
            this.btnOpenIni = new System.Windows.Forms.Button();
            this.btnHTTPD = new System.Windows.Forms.Button();
            this.btnDBbase = new System.Windows.Forms.Button();
            this.btnRefreshAll = new System.Windows.Forms.Button();
            this.grpEnvironment = new System.Windows.Forms.GroupBox();
            this.lblPhpVersion = new System.Windows.Forms.Label();
            this.lblDbState = new System.Windows.Forms.Label();
            this.lblDiskSpace = new System.Windows.Forms.Label();
            this.grpHostingControls = new System.Windows.Forms.GroupBox();
            this.chkWebpageVisible = new System.Windows.Forms.CheckBox();
            this.chkMaintenanceMode = new System.Windows.Forms.CheckBox();
            this.btnSaveConfig = new System.Windows.Forms.Button();
            this.lblConsoleTitle = new System.Windows.Forms.Label();
            this.txtConsoleOutput = new System.Windows.Forms.TextBox();
            this.tabPageWebsites = new System.Windows.Forms.TabPage();
            this.dgvRoutes = new System.Windows.Forms.DataGridView();
            this.colRouteName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colTargetController = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUrlPath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPhysicalLocation = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colActionStatus = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colActionOpen = new System.Windows.Forms.DataGridViewButtonColumn();
            this.colActionVisibility = new System.Windows.Forms.DataGridViewButtonColumn();
            this.tabPageClients = new System.Windows.Forms.TabPage();
            this.dgvClients = new System.Windows.Forms.DataGridView();
            this.colClientIp = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colHostname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colConnectedSince = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRequestStatus = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colUserAgent = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tabPageDatabaseLogs = new System.Windows.Forms.TabPage();
            this.grpDbMetrics = new System.Windows.Forms.GroupBox();
            this.lblDbPoolStatus = new System.Windows.Forms.Label();
            this.lblCpuLoad = new System.Windows.Forms.Label();
            this.lblRamUtil = new System.Windows.Forms.Label();
            this.lblSearchLogTitle = new System.Windows.Forms.Label();
            this.txtLogSearch = new System.Windows.Forms.TextBox();
            this.btnFilterLogs = new System.Windows.Forms.Button();
            this.chkAutoRefreshLogs = new System.Windows.Forms.CheckBox();
            this.txtFullLogs = new System.Windows.Forms.TextBox();
            this.timerTelemetry = new System.Windows.Forms.Timer(this.components);

            this.tabControlMain.SuspendLayout();
            this.tabPageDashboard.SuspendLayout();
            this.grpServerToolbar.SuspendLayout();
            this.grpEnvironment.SuspendLayout();
            this.grpHostingControls.SuspendLayout();
            this.tabPageWebsites.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvRoutes)).BeginInit();
            this.tabPageClients.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvClients)).BeginInit();
            this.tabPageDatabaseLogs.SuspendLayout();
            this.grpDbMetrics.SuspendLayout();
            this.SuspendLayout();

            // 
            // tabControlMain
            // 
            this.tabControlMain.Controls.Add(this.tabPageDashboard);
            this.tabControlMain.Controls.Add(this.tabPageWebsites);
            this.tabControlMain.Controls.Add(this.tabPageClients);
            this.tabControlMain.Controls.Add(this.tabPageDatabaseLogs);
            this.tabControlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlMain.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControlMain.Location = new System.Drawing.Point(0, 0);
            this.tabControlMain.Name = "tabControlMain";
            this.tabControlMain.SelectedIndex = 0;
            this.tabControlMain.Size = new System.Drawing.Size(760, 580);
            this.tabControlMain.TabIndex = 0;

            // 
            // tabPageDashboard
            // 
            this.tabPageDashboard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.tabPageDashboard.Controls.Add(this.grpServerToolbar);
            this.tabPageDashboard.Controls.Add(this.grpEnvironment);
            this.tabPageDashboard.Controls.Add(this.grpHostingControls);
            this.tabPageDashboard.Controls.Add(this.lblConsoleTitle);
            this.tabPageDashboard.Controls.Add(this.txtConsoleOutput);
            this.tabPageDashboard.Location = new System.Drawing.Point(4, 24);
            this.tabPageDashboard.Name = "tabPageDashboard";
            this.tabPageDashboard.Padding = new System.Windows.Forms.Padding(12);
            this.tabPageDashboard.Size = new System.Drawing.Size(752, 552);
            this.tabPageDashboard.TabIndex = 0;
            this.tabPageDashboard.Text = "Dashboard";

            // 
            // grpServerToolbar
            // 
            this.grpServerToolbar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpServerToolbar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.grpServerToolbar.Controls.Add(this.label1);
            this.grpServerToolbar.Controls.Add(this.txtIP);
            this.grpServerToolbar.Controls.Add(this.label2);
            this.grpServerToolbar.Controls.Add(this.txtServerPort);
            this.grpServerToolbar.Controls.Add(this.lblStatusTitle);
            this.grpServerToolbar.Controls.Add(this.lblStatusValue);
            this.grpServerToolbar.Controls.Add(this.btnStartServer);
            this.grpServerToolbar.Controls.Add(this.btnStopServer);
            this.grpServerToolbar.Controls.Add(this.btnOpenBrowser);
            this.grpServerToolbar.Controls.Add(this.btnOpenFolder);
            this.grpServerToolbar.Controls.Add(this.btnOpenIni);
            this.grpServerToolbar.Controls.Add(this.btnHTTPD);
            this.grpServerToolbar.Controls.Add(this.btnDBbase);
            this.grpServerToolbar.Controls.Add(this.btnRefreshAll);
            this.grpServerToolbar.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.grpServerToolbar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.grpServerToolbar.Location = new System.Drawing.Point(12, 8);
            this.grpServerToolbar.Name = "grpServerToolbar";
            this.grpServerToolbar.Size = new System.Drawing.Size(728, 98);
            this.grpServerToolbar.TabIndex = 0;
            this.grpServerToolbar.TabStop = false;
            this.grpServerToolbar.Text = "Server Management & Control";

            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.label1.Location = new System.Drawing.Point(14, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP Address";

            // 
            // txtIP
            // 
            this.txtIP.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.txtIP.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtIP.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtIP.ForeColor = System.Drawing.Color.White;
            this.txtIP.Location = new System.Drawing.Point(15, 36);
            this.txtIP.Name = "txtIP";
            this.txtIP.Size = new System.Drawing.Size(160, 23);
            this.txtIP.TabIndex = 1;

            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.label2.Location = new System.Drawing.Point(180, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Port";

            // 
            // txtServerPort
            // 
            this.txtServerPort.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.txtServerPort.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtServerPort.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtServerPort.ForeColor = System.Drawing.Color.White;
            this.txtServerPort.Location = new System.Drawing.Point(181, 36);
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.Size = new System.Drawing.Size(50, 23);
            this.txtServerPort.TabIndex = 3;
            this.txtServerPort.Text = "80";

            // 
            // lblStatusTitle
            // 
            this.lblStatusTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatusTitle.AutoSize = true;
            this.lblStatusTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusTitle.ForeColor = System.Drawing.Color.White;
            this.lblStatusTitle.Location = new System.Drawing.Point(420, 20);
            this.lblStatusTitle.Name = "lblStatusTitle";
            this.lblStatusTitle.Size = new System.Drawing.Size(83, 15);
            this.lblStatusTitle.TabIndex = 4;
            this.lblStatusTitle.Text = "Server Status:";

            // 
            // lblStatusValue
            // 
            this.lblStatusValue.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatusValue.AutoSize = true;
            this.lblStatusValue.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.lblStatusValue.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblStatusValue.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(99)))), ((int)(((byte)(132)))));
            this.lblStatusValue.Location = new System.Drawing.Point(508, 17);
            this.lblStatusValue.Name = "lblStatusValue";
            this.lblStatusValue.Padding = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.lblStatusValue.Size = new System.Drawing.Size(204, 21);
            this.lblStatusValue.TabIndex = 5;
            this.lblStatusValue.Text = "CHECKING SERVICE...";
            this.lblStatusValue.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // 
            // btnStartServer
            // 
            this.btnStartServer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.btnStartServer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStartServer.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.btnStartServer.FlatAppearance.BorderSize = 1;
            this.btnStartServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStartServer.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnStartServer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.btnStartServer.Location = new System.Drawing.Point(15, 63);
            this.btnStartServer.Name = "btnStartServer";
            this.btnStartServer.Size = new System.Drawing.Size(80, 26);
            this.btnStartServer.TabIndex = 6;
            this.btnStartServer.Text = "▶ Start";
            this.btnStartServer.UseVisualStyleBackColor = false;
            this.btnStartServer.Click += new System.EventHandler(this.btnStartServer_Click);

            // 
            // btnStopServer
            // 
            this.btnStopServer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.btnStopServer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnStopServer.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(67)))), ((int)(((byte)(54)))));
            this.btnStopServer.FlatAppearance.BorderSize = 1;
            this.btnStopServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnStopServer.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnStopServer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(82)))), ((int)(((byte)(82)))));
            this.btnStopServer.Location = new System.Drawing.Point(100, 63);
            this.btnStopServer.Name = "btnStopServer";
            this.btnStopServer.Size = new System.Drawing.Size(80, 26);
            this.btnStopServer.TabIndex = 7;
            this.btnStopServer.Text = "⏹ Stop";
            this.btnStopServer.UseVisualStyleBackColor = false;
            this.btnStopServer.Click += new System.EventHandler(this.btnStopServer_Click);

            // 
            // btnOpenBrowser
            // 
            this.btnOpenBrowser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.btnOpenBrowser.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenBrowser.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.btnOpenBrowser.FlatAppearance.BorderSize = 1;
            this.btnOpenBrowser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenBrowser.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnOpenBrowser.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnOpenBrowser.Location = new System.Drawing.Point(186, 63);
            this.btnOpenBrowser.Name = "btnOpenBrowser";
            this.btnOpenBrowser.Size = new System.Drawing.Size(105, 26);
            this.btnOpenBrowser.TabIndex = 8;
            this.btnOpenBrowser.Text = "🌐 Browser";
            this.btnOpenBrowser.UseVisualStyleBackColor = false;
            this.btnOpenBrowser.Click += new System.EventHandler(this.btnOpenBrowser_Click);

            // 
            // btnOpenFolder
            // 
            this.btnOpenFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.btnOpenFolder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenFolder.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.btnOpenFolder.FlatAppearance.BorderSize = 1;
            this.btnOpenFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenFolder.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnOpenFolder.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnOpenFolder.Location = new System.Drawing.Point(296, 63);
            this.btnOpenFolder.Name = "btnOpenFolder";
            this.btnOpenFolder.Size = new System.Drawing.Size(110, 26);
            this.btnOpenFolder.TabIndex = 9;
            this.btnOpenFolder.Text = "📁 Root Folder";
            this.btnOpenFolder.UseVisualStyleBackColor = false;
            this.btnOpenFolder.Click += new System.EventHandler(this.btnOpenFolder_Click);

            // 
            // btnOpenIni
            // 
            this.btnOpenIni.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.btnOpenIni.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenIni.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(160)))), ((int)(((byte)(0)))));
            this.btnOpenIni.FlatAppearance.BorderSize = 1;
            this.btnOpenIni.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenIni.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnOpenIni.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(179)))), ((int)(((byte)(0)))));
            this.btnOpenIni.Location = new System.Drawing.Point(411, 63);
            this.btnOpenIni.Name = "btnOpenIni";
            this.btnOpenIni.Size = new System.Drawing.Size(70, 26);
            this.btnOpenIni.TabIndex = 10;
            this.btnOpenIni.Text = "⚙ php.ini";
            this.btnOpenIni.UseVisualStyleBackColor = false;
            this.btnOpenIni.Click += new System.EventHandler(this.btnOpenIni_Click);

            // 
            // btnHTTPD
            // 
            this.btnHTTPD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.btnHTTPD.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHTTPD.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(200)))), ((int)(((byte)(100)))));
            this.btnHTTPD.FlatAppearance.BorderSize = 1;
            this.btnHTTPD.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHTTPD.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnHTTPD.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(210)))), ((int)(((byte)(110)))));
            this.btnHTTPD.Location = new System.Drawing.Point(486, 63);
            this.btnHTTPD.Name = "btnHTTPD";
            this.btnHTTPD.Size = new System.Drawing.Size(80, 26);
            this.btnHTTPD.TabIndex = 11;
            this.btnHTTPD.Text = "📄 httpd.conf";
            this.btnHTTPD.UseVisualStyleBackColor = false;
            this.btnHTTPD.Click += new System.EventHandler(this.btnHTTPD_Click);

            // 
            // btnDBbase
            // 
            this.btnDBbase.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.btnDBbase.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDBbase.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.btnDBbase.FlatAppearance.BorderSize = 1;
            this.btnDBbase.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDBbase.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnDBbase.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnDBbase.Location = new System.Drawing.Point(571, 63);
            this.btnDBbase.Name = "btnDBbase";
            this.btnDBbase.Size = new System.Drawing.Size(95, 26);
            this.btnDBbase.TabIndex = 12;
            this.btnDBbase.Text = "phpMyAdmin";
            this.btnDBbase.UseVisualStyleBackColor = false;
            this.btnDBbase.Click += new System.EventHandler(this.btnDBbase_Click);

            // 
            // btnRefreshAll
            // 
            this.btnRefreshAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshAll.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.btnRefreshAll.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefreshAll.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.btnRefreshAll.FlatAppearance.BorderSize = 1;
            this.btnRefreshAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshAll.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnRefreshAll.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(230)))), ((int)(((byte)(118)))));
            this.btnRefreshAll.Location = new System.Drawing.Point(671, 63);
            this.btnRefreshAll.Name = "btnRefreshAll";
            this.btnRefreshAll.Size = new System.Drawing.Size(42, 26);
            this.btnRefreshAll.TabIndex = 13;
            this.btnRefreshAll.Text = "🔄";
            this.btnRefreshAll.UseVisualStyleBackColor = false;
            this.btnRefreshAll.Click += new System.EventHandler(this.btnRefreshAll_Click);

            // 
            // grpEnvironment
            // 
            this.grpEnvironment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpEnvironment.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.grpEnvironment.Controls.Add(this.lblPhpVersion);
            this.grpEnvironment.Controls.Add(this.lblDbState);
            this.grpEnvironment.Controls.Add(this.lblDiskSpace);
            this.grpEnvironment.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.grpEnvironment.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.grpEnvironment.Location = new System.Drawing.Point(12, 112);
            this.grpEnvironment.Name = "grpEnvironment";
            this.grpEnvironment.Size = new System.Drawing.Size(728, 52);
            this.grpEnvironment.TabIndex = 1;
            this.grpEnvironment.TabStop = false;
            this.grpEnvironment.Text = "Environment Summary";

            // 
            // lblPhpVersion
            // 
            this.lblPhpVersion.AutoSize = true;
            this.lblPhpVersion.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblPhpVersion.ForeColor = System.Drawing.Color.White;
            this.lblPhpVersion.Location = new System.Drawing.Point(15, 23);
            this.lblPhpVersion.Name = "lblPhpVersion";
            this.lblPhpVersion.Size = new System.Drawing.Size(101, 15);
            this.lblPhpVersion.TabIndex = 0;
            this.lblPhpVersion.Text = "PHP Version: 8.1+";

            // 
            // lblDbState
            // 
            this.lblDbState.AutoSize = true;
            this.lblDbState.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDbState.ForeColor = System.Drawing.Color.White;
            this.lblDbState.Location = new System.Drawing.Point(230, 23);
            this.lblDbState.Name = "lblDbState";
            this.lblDbState.Size = new System.Drawing.Size(171, 15);
            this.lblDbState.TabIndex = 1;
            this.lblDbState.Text = "Database: Connected (MySQL)";

            // 
            // lblDiskSpace
            // 
            this.lblDiskSpace.AutoSize = true;
            this.lblDiskSpace.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDiskSpace.ForeColor = System.Drawing.Color.White;
            this.lblDiskSpace.Location = new System.Drawing.Point(470, 23);
            this.lblDiskSpace.Name = "lblDiskSpace";
            this.lblDiskSpace.Size = new System.Drawing.Size(183, 15);
            this.lblDiskSpace.TabIndex = 2;
            this.lblDiskSpace.Text = "Disk Space Utilized: Calculating...";

            // 
            // grpHostingControls
            // 
            this.grpHostingControls.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpHostingControls.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.grpHostingControls.Controls.Add(this.chkWebpageVisible);
            this.grpHostingControls.Controls.Add(this.chkMaintenanceMode);
            this.grpHostingControls.Controls.Add(this.btnSaveConfig);
            this.grpHostingControls.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.grpHostingControls.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.grpHostingControls.Location = new System.Drawing.Point(12, 170);
            this.grpHostingControls.Name = "grpHostingControls";
            this.grpHostingControls.Size = new System.Drawing.Size(728, 52);
            this.grpHostingControls.TabIndex = 2;
            this.grpHostingControls.TabStop = false;
            this.grpHostingControls.Text = "Webhosting & Visibility Controls";

            // 
            // chkWebpageVisible
            // 
            this.chkWebpageVisible.AutoSize = true;
            this.chkWebpageVisible.Checked = true;
            this.chkWebpageVisible.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWebpageVisible.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.chkWebpageVisible.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.chkWebpageVisible.Location = new System.Drawing.Point(15, 23);
            this.chkWebpageVisible.Name = "chkWebpageVisible";
            this.chkWebpageVisible.Size = new System.Drawing.Size(163, 19);
            this.chkWebpageVisible.TabIndex = 0;
            this.chkWebpageVisible.Text = "Webpage Online (Visible)";
            this.chkWebpageVisible.UseVisualStyleBackColor = true;

            // 
            // chkMaintenanceMode
            // 
            this.chkMaintenanceMode.AutoSize = true;
            this.chkMaintenanceMode.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkMaintenanceMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(160)))), ((int)(((byte)(0)))));
            this.chkMaintenanceMode.Location = new System.Drawing.Point(230, 23);
            this.chkMaintenanceMode.Name = "chkMaintenanceMode";
            this.chkMaintenanceMode.Size = new System.Drawing.Size(168, 19);
            this.chkMaintenanceMode.TabIndex = 1;
            this.chkMaintenanceMode.Text = "Enable Maintenance Mode";
            this.chkMaintenanceMode.UseVisualStyleBackColor = true;

            // 
            // btnSaveConfig
            // 
            this.btnSaveConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.btnSaveConfig.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveConfig.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.btnSaveConfig.FlatAppearance.BorderSize = 1;
            this.btnSaveConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveConfig.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnSaveConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnSaveConfig.Location = new System.Drawing.Point(598, 19);
            this.btnSaveConfig.Name = "btnSaveConfig";
            this.btnSaveConfig.Size = new System.Drawing.Size(115, 25);
            this.btnSaveConfig.TabIndex = 2;
            this.btnSaveConfig.Text = "Update Config";
            this.btnSaveConfig.UseVisualStyleBackColor = false;
            this.btnSaveConfig.Click += new System.EventHandler(this.btnSaveConfig_Click);

            // 
            // lblConsoleTitle
            // 
            this.lblConsoleTitle.AutoSize = true;
            this.lblConsoleTitle.Font = new System.Drawing.Font("Segoe UI Bold", 9F);
            this.lblConsoleTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.lblConsoleTitle.Location = new System.Drawing.Point(12, 230);
            this.lblConsoleTitle.Name = "lblConsoleTitle";
            this.lblConsoleTitle.Size = new System.Drawing.Size(119, 15);
            this.lblConsoleTitle.TabIndex = 3;
            this.lblConsoleTitle.Text = "Live Console Output";

            // 
            // txtConsoleOutput
            // 
            this.txtConsoleOutput.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtConsoleOutput.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.txtConsoleOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtConsoleOutput.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.txtConsoleOutput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(102)))));
            this.txtConsoleOutput.Location = new System.Drawing.Point(12, 250);
            this.txtConsoleOutput.Multiline = true;
            this.txtConsoleOutput.Name = "txtConsoleOutput";
            this.txtConsoleOutput.ReadOnly = true;
            this.txtConsoleOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtConsoleOutput.Size = new System.Drawing.Size(728, 290);
            this.txtConsoleOutput.TabIndex = 4;
            this.txtConsoleOutput.Text = "[System] Initialized Web Management Console...\r\n[Ready] Listening to Apache Service Status...";

            // 
            // tabPageWebsites
            // 
            this.tabPageWebsites.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.tabPageWebsites.Controls.Add(this.dgvRoutes);
            this.tabPageWebsites.Location = new System.Drawing.Point(4, 24);
            this.tabPageWebsites.Name = "tabPageWebsites";
            this.tabPageWebsites.Padding = new System.Windows.Forms.Padding(8);
            this.tabPageWebsites.Size = new System.Drawing.Size(752, 552);
            this.tabPageWebsites.TabIndex = 1;
            this.tabPageWebsites.Text = "Websites & Routing";

            // 
            // dgvRoutes
            // 
            this.dgvRoutes.AllowUserToAddRows = false;
            this.dgvRoutes.AllowUserToDeleteRows = false;
            this.dgvRoutes.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvRoutes.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.dgvRoutes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvRoutes.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvRoutes.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvRoutes.ColumnHeadersHeight = 32;
            this.dgvRoutes.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colRouteName,
            this.colTargetController,
            this.colUrlPath,
            this.colPhysicalLocation,
            this.colActionStatus,
            this.colActionOpen,
            this.colActionVisibility});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvRoutes.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvRoutes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvRoutes.EnableHeadersVisualStyles = false;
            this.dgvRoutes.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.dgvRoutes.Location = new System.Drawing.Point(8, 8);
            this.dgvRoutes.Name = "dgvRoutes";
            this.dgvRoutes.RowHeadersVisible = false;
            this.dgvRoutes.RowTemplate.Height = 28;
            this.dgvRoutes.Size = new System.Drawing.Size(736, 536);
            this.dgvRoutes.TabIndex = 0;
            this.dgvRoutes.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvRoutes_CellContentClick);

            // 
            // colRouteName
            // 
            this.colRouteName.HeaderText = "Route Name";
            this.colRouteName.Name = "colRouteName";
            this.colRouteName.ReadOnly = true;

            // 
            // colTargetController
            // 
            this.colTargetController.HeaderText = "Target Controller/File";
            this.colTargetController.Name = "colTargetController";
            this.colTargetController.ReadOnly = true;

            // 
            // colUrlPath
            // 
            this.colUrlPath.HeaderText = "URL Path";
            this.colUrlPath.Name = "colUrlPath";
            this.colUrlPath.ReadOnly = true;

            // 
            // colPhysicalLocation
            // 
            this.colPhysicalLocation.HeaderText = "Physical File Location";
            this.colPhysicalLocation.Name = "colPhysicalLocation";
            this.colPhysicalLocation.ReadOnly = true;

            // 
            // colActionStatus
            // 
            this.colActionStatus.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.colActionStatus.HeaderText = "Status";
            this.colActionStatus.Name = "colActionStatus";

            // 
            // colActionOpen
            // 
            this.colActionOpen.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.colActionOpen.HeaderText = "Action";
            this.colActionOpen.Name = "colActionOpen";
            this.colActionOpen.Text = "Open";
            this.colActionOpen.UseColumnTextForButtonValue = true;

            // 
            // colActionVisibility
            // 
            this.colActionVisibility.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.colActionVisibility.HeaderText = "Visibility";
            this.colActionVisibility.Name = "colActionVisibility";

            // 
            // tabPageClients
            // 
            this.tabPageClients.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.tabPageClients.Controls.Add(this.dgvClients);
            this.tabPageClients.Location = new System.Drawing.Point(4, 24);
            this.tabPageClients.Name = "tabPageClients";
            this.tabPageClients.Padding = new System.Windows.Forms.Padding(8);
            this.tabPageClients.Size = new System.Drawing.Size(752, 552);
            this.tabPageClients.TabIndex = 2;
            this.tabPageClients.Text = "Connected Clients";

            // 
            // dgvClients
            // 
            this.dgvClients.AllowUserToAddRows = false;
            this.dgvClients.AllowUserToDeleteRows = false;
            this.dgvClients.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvClients.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.dgvClients.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvClients.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvClients.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvClients.ColumnHeadersHeight = 32;
            this.dgvClients.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colClientIp,
            this.colHostname,
            this.colConnectedSince,
            this.colRequestStatus,
            this.colUserAgent});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Segoe UI", 9F);
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvClients.DefaultCellStyle = dataGridViewCellStyle4;
            this.dgvClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvClients.EnableHeadersVisualStyles = false;
            this.dgvClients.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.dgvClients.Location = new System.Drawing.Point(8, 8);
            this.dgvClients.Name = "dgvClients";
            this.dgvClients.ReadOnly = true;
            this.dgvClients.RowHeadersVisible = false;
            this.dgvClients.RowTemplate.Height = 28;
            this.dgvClients.Size = new System.Drawing.Size(736, 536);
            this.dgvClients.TabIndex = 0;

            // 
            // colClientIp
            // 
            this.colClientIp.HeaderText = "Client IP Address";
            this.colClientIp.Name = "colClientIp";
            this.colClientIp.ReadOnly = true;

            // 
            // colHostname
            // 
            this.colHostname.HeaderText = "Hostname / PC Name";
            this.colHostname.Name = "colHostname";
            this.colHostname.ReadOnly = true;

            // 
            // colConnectedSince
            // 
            this.colConnectedSince.HeaderText = "Connected Since";
            this.colConnectedSince.Name = "colConnectedSince";
            this.colConnectedSince.ReadOnly = true;

            // 
            // colRequestStatus
            // 
            this.colRequestStatus.HeaderText = "Request Status";
            this.colRequestStatus.Name = "colRequestStatus";
            this.colRequestStatus.ReadOnly = true;

            // 
            // colUserAgent
            // 
            this.colUserAgent.HeaderText = "User-Agent / Browser";
            this.colUserAgent.Name = "colUserAgent";
            this.colUserAgent.ReadOnly = true;

            // 
            // tabPageDatabaseLogs
            // 
            this.tabPageDatabaseLogs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.tabPageDatabaseLogs.Controls.Add(this.grpDbMetrics);
            this.tabPageDatabaseLogs.Controls.Add(this.lblSearchLogTitle);
            this.tabPageDatabaseLogs.Controls.Add(this.txtLogSearch);
            this.tabPageDatabaseLogs.Controls.Add(this.btnFilterLogs);
            this.tabPageDatabaseLogs.Controls.Add(this.chkAutoRefreshLogs);
            this.tabPageDatabaseLogs.Controls.Add(this.txtFullLogs);
            this.tabPageDatabaseLogs.Location = new System.Drawing.Point(4, 24);
            this.tabPageDatabaseLogs.Name = "tabPageDatabaseLogs";
            this.tabPageDatabaseLogs.Padding = new System.Windows.Forms.Padding(12);
            this.tabPageDatabaseLogs.Size = new System.Drawing.Size(752, 552);
            this.tabPageDatabaseLogs.TabIndex = 3;
            this.tabPageDatabaseLogs.Text = "Database & Logs";

            // 
            // grpDbMetrics
            // 
            this.grpDbMetrics.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDbMetrics.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            this.grpDbMetrics.Controls.Add(this.lblDbPoolStatus);
            this.grpDbMetrics.Controls.Add(this.lblCpuLoad);
            this.grpDbMetrics.Controls.Add(this.lblRamUtil);
            this.grpDbMetrics.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            this.grpDbMetrics.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.grpDbMetrics.Location = new System.Drawing.Point(12, 8);
            this.grpDbMetrics.Name = "grpDbMetrics";
            this.grpDbMetrics.Size = new System.Drawing.Size(728, 62);
            this.grpDbMetrics.TabIndex = 0;
            this.grpDbMetrics.TabStop = false;
            this.grpDbMetrics.Text = "System Metrics & Database Health";

            // 
            // lblDbPoolStatus
            // 
            this.lblDbPoolStatus.AutoSize = true;
            this.lblDbPoolStatus.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblDbPoolStatus.ForeColor = System.Drawing.Color.White;
            this.lblDbPoolStatus.Location = new System.Drawing.Point(15, 28);
            this.lblDbPoolStatus.Name = "lblDbPoolStatus";
            this.lblDbPoolStatus.Size = new System.Drawing.Size(193, 15);
            this.lblDbPoolStatus.TabIndex = 0;
            this.lblDbPoolStatus.Text = "MySQL Connections: Active (0/10)";

            // 
            // lblCpuLoad
            // 
            this.lblCpuLoad.AutoSize = true;
            this.lblCpuLoad.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblCpuLoad.ForeColor = System.Drawing.Color.White;
            this.lblCpuLoad.Location = new System.Drawing.Point(260, 28);
            this.lblCpuLoad.Name = "lblCpuLoad";
            this.lblCpuLoad.Size = new System.Drawing.Size(89, 15);
            this.lblCpuLoad.TabIndex = 1;
            this.lblCpuLoad.Text = "CPU Load: 0.0%";

            // 
            // lblRamUtil
            // 
            this.lblRamUtil.AutoSize = true;
            this.lblRamUtil.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblRamUtil.ForeColor = System.Drawing.Color.White;
            this.lblRamUtil.Location = new System.Drawing.Point(470, 28);
            this.lblRamUtil.Name = "lblRamUtil";
            this.lblRamUtil.Size = new System.Drawing.Size(115, 15);
            this.lblRamUtil.TabIndex = 2;
            this.lblRamUtil.Text = "RAM Utilized: 0.0 MB";

            // 
            // lblSearchLogTitle
            // 
            this.lblSearchLogTitle.AutoSize = true;
            this.lblSearchLogTitle.Font = new System.Drawing.Font("Segoe UI Bold", 9F);
            this.lblSearchLogTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.lblSearchLogTitle.Location = new System.Drawing.Point(12, 85);
            this.lblSearchLogTitle.Name = "lblSearchLogTitle";
            this.lblSearchLogTitle.Size = new System.Drawing.Size(66, 15);
            this.lblSearchLogTitle.TabIndex = 1;
            this.lblSearchLogTitle.Text = "Filter Logs:";

            // 
            // txtLogSearch
            // 
            this.txtLogSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.txtLogSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtLogSearch.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtLogSearch.ForeColor = System.Drawing.Color.White;
            this.txtLogSearch.Location = new System.Drawing.Point(82, 81);
            this.txtLogSearch.Name = "txtLogSearch";
            this.txtLogSearch.Size = new System.Drawing.Size(300, 23);
            this.txtLogSearch.TabIndex = 2;
            this.txtLogSearch.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtLogSearch_KeyDown);

            // 
            // btnFilterLogs
            // 
            this.btnFilterLogs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.btnFilterLogs.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnFilterLogs.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(200)))), ((int)(((byte)(255)))));
            this.btnFilterLogs.FlatAppearance.BorderSize = 1;
            this.btnFilterLogs.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFilterLogs.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnFilterLogs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnFilterLogs.Location = new System.Drawing.Point(390, 80);
            this.btnFilterLogs.Name = "btnFilterLogs";
            this.btnFilterLogs.Size = new System.Drawing.Size(95, 25);
            this.btnFilterLogs.TabIndex = 3;
            this.btnFilterLogs.Text = "Search Logs";
            this.btnFilterLogs.UseVisualStyleBackColor = false;
            this.btnFilterLogs.Click += new System.EventHandler(this.btnFilterLogs_Click);

            // 
            // chkAutoRefreshLogs
            // 
            this.chkAutoRefreshLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkAutoRefreshLogs.AutoSize = true;
            this.chkAutoRefreshLogs.Checked = true;
            this.chkAutoRefreshLogs.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoRefreshLogs.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.chkAutoRefreshLogs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.chkAutoRefreshLogs.Location = new System.Drawing.Point(620, 83);
            this.chkAutoRefreshLogs.Name = "chkAutoRefreshLogs";
            this.chkAutoRefreshLogs.Size = new System.Drawing.Size(120, 19);
            this.chkAutoRefreshLogs.TabIndex = 4;
            this.chkAutoRefreshLogs.Text = "Auto-Refresh Live";
            this.chkAutoRefreshLogs.UseVisualStyleBackColor = true;

            // 
            // txtFullLogs
            // 
            this.txtFullLogs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtFullLogs.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            this.txtFullLogs.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtFullLogs.Font = new System.Drawing.Font("Consolas", 9F);
            this.txtFullLogs.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(255)))), ((int)(((byte)(102)))));
            this.txtFullLogs.Location = new System.Drawing.Point(12, 112);
            this.txtFullLogs.Multiline = true;
            this.txtFullLogs.Name = "txtFullLogs";
            this.txtFullLogs.ReadOnly = true;
            this.txtFullLogs.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtFullLogs.Size = new System.Drawing.Size(728, 428);
            this.txtFullLogs.TabIndex = 5;

            // 
            // timerTelemetry
            // 
            this.timerTelemetry.Interval = 4000;
            this.timerTelemetry.Tick += new System.EventHandler(this.TimerTelemetry_Tick);

            // 
            // DashboardContents
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.Controls.Add(this.tabControlMain);
            this.Name = "DashboardContents";
            this.Size = new System.Drawing.Size(760, 580);
            this.Load += new System.EventHandler(this.DashboardContents_Load);

            this.tabControlMain.ResumeLayout(false);
            this.tabPageDashboard.ResumeLayout(false);
            this.tabPageDashboard.PerformLayout();
            this.grpServerToolbar.ResumeLayout(false);
            this.grpServerToolbar.PerformLayout();
            this.grpEnvironment.ResumeLayout(false);
            this.grpEnvironment.PerformLayout();
            this.grpHostingControls.ResumeLayout(false);
            this.grpHostingControls.PerformLayout();
            this.tabPageWebsites.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvRoutes)).EndInit();
            this.tabPageClients.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvClients)).EndInit();
            this.tabPageDatabaseLogs.ResumeLayout(false);
            this.tabPageDatabaseLogs.PerformLayout();
            this.grpDbMetrics.ResumeLayout(false);
            this.grpDbMetrics.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TabControl tabControlMain;
        private System.Windows.Forms.TabPage tabPageDashboard;
        private System.Windows.Forms.TabPage tabPageWebsites;
        private System.Windows.Forms.TabPage tabPageClients;
        private System.Windows.Forms.TabPage tabPageDatabaseLogs;

        private System.Windows.Forms.GroupBox grpServerToolbar;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtIP;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServerPort;

        private System.Windows.Forms.Label lblStatusTitle;
        private System.Windows.Forms.Label lblStatusValue;

        private System.Windows.Forms.Button btnStartServer;
        private System.Windows.Forms.Button btnStopServer;
        private System.Windows.Forms.Button btnOpenBrowser;
        private System.Windows.Forms.Button btnOpenFolder;
        private System.Windows.Forms.Button btnOpenIni;
        private System.Windows.Forms.Button btnHTTPD;
        private System.Windows.Forms.Button btnDBbase;
        private System.Windows.Forms.Button btnRefreshAll;

        private System.Windows.Forms.GroupBox grpEnvironment;
        private System.Windows.Forms.Label lblPhpVersion;
        private System.Windows.Forms.Label lblDbState;
        private System.Windows.Forms.Label lblDiskSpace;

        private System.Windows.Forms.GroupBox grpHostingControls;
        private System.Windows.Forms.CheckBox chkWebpageVisible;
        private System.Windows.Forms.CheckBox chkMaintenanceMode;
        private System.Windows.Forms.Button btnSaveConfig;

        private System.Windows.Forms.Label lblConsoleTitle;
        private System.Windows.Forms.TextBox txtConsoleOutput;

        private System.Windows.Forms.DataGridView dgvRoutes;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRouteName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colTargetController;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUrlPath;
        private System.Windows.Forms.DataGridViewTextBoxColumn colPhysicalLocation;
        private System.Windows.Forms.DataGridViewButtonColumn colActionStatus;
        private System.Windows.Forms.DataGridViewButtonColumn colActionOpen;
        private System.Windows.Forms.DataGridViewButtonColumn colActionVisibility;

        private System.Windows.Forms.DataGridView dgvClients;
        private System.Windows.Forms.DataGridViewTextBoxColumn colClientIp;
        private System.Windows.Forms.DataGridViewTextBoxColumn colHostname;
        private System.Windows.Forms.DataGridViewTextBoxColumn colConnectedSince;
        private System.Windows.Forms.DataGridViewTextBoxColumn colRequestStatus;
        private System.Windows.Forms.DataGridViewTextBoxColumn colUserAgent;

        private System.Windows.Forms.GroupBox grpDbMetrics;
        private System.Windows.Forms.Label lblDbPoolStatus;
        private System.Windows.Forms.Label lblCpuLoad;
        private System.Windows.Forms.Label lblRamUtil;

        private System.Windows.Forms.Label lblSearchLogTitle;
        private System.Windows.Forms.TextBox txtLogSearch;
        private System.Windows.Forms.Button btnFilterLogs;
        private System.Windows.Forms.CheckBox chkAutoRefreshLogs;
        private System.Windows.Forms.TextBox txtFullLogs;

        private System.Windows.Forms.Timer timerTelemetry;
    }
}