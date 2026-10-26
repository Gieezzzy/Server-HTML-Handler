namespace Server_HTML_Handler.View.Contents
{
    partial class NotificationHubUserControl
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblHeaderTitle;
        private System.Windows.Forms.Label lblHeaderSubtitle;
        private System.Windows.Forms.Label lblWatcherStatus;
        private System.Windows.Forms.Panel pnlMetrics;
        private System.Windows.Forms.Label lblMetricLowStock;
        private System.Windows.Forms.Label lblMetricInbounds;
        private System.Windows.Forms.Label lblMetricPalletLapse;
        private System.Windows.Forms.Label lblMetricDispatched;
        private System.Windows.Forms.Panel pnlActions;
        private System.Windows.Forms.Button btnToggleWatcher;
        private System.Windows.Forms.Button btnScanNow;
        private System.Windows.Forms.Button btnSendTest;
        private System.Windows.Forms.Button btnClearHistory;
        private System.Windows.Forms.TabControl tabControlNotifications;
        private System.Windows.Forms.TabPage tabFeed;
        private System.Windows.Forms.DataGridView dgvFeed;
        private System.Windows.Forms.TabPage tabLowStock;
        private System.Windows.Forms.Panel pnlStockFilter;
        private System.Windows.Forms.Label lblStockCategory;
        private System.Windows.Forms.ComboBox cmbStockCategory;
        private System.Windows.Forms.CheckBox chkLowStockOnly;
        private System.Windows.Forms.Label lblStockSearch;
        private System.Windows.Forms.TextBox txtStockSearch;
        private System.Windows.Forms.Button btnRefreshStock;
        private System.Windows.Forms.DataGridView dgvLowStock;
        private System.Windows.Forms.TabPage tabInbounds;
        private System.Windows.Forms.Panel pnlInboundsFilter;
        private System.Windows.Forms.Label lblInboundFilter;
        private System.Windows.Forms.ComboBox cmbInboundType;
        private System.Windows.Forms.Label lblInboundSearch;
        private System.Windows.Forms.TextBox txtInboundSearch;
        private System.Windows.Forms.Button btnRefreshInbounds;
        private System.Windows.Forms.DataGridView dgvInbounds;
        private System.Windows.Forms.TabPage tabPallets;
        private System.Windows.Forms.Panel pnlPalletFilter;
        private System.Windows.Forms.Label lblPalletStatus;
        private System.Windows.Forms.ComboBox cmbPalletStatus;
        private System.Windows.Forms.Label lblPalletSearch;
        private System.Windows.Forms.TextBox txtPalletSearch;
        private System.Windows.Forms.Button btnRefreshPallets;
        private System.Windows.Forms.DataGridView dgvPallets;
        private System.Windows.Forms.TabPage tabConfig;
        private System.Windows.Forms.GroupBox grpSqlConfig;
        private System.Windows.Forms.Label lblSqlConn;
        private System.Windows.Forms.TextBox txtSqlConn;
        private System.Windows.Forms.GroupBox grpChannels;
        private System.Windows.Forms.Label lblPhpUrl;
        private System.Windows.Forms.TextBox txtPhpUrl;
        private System.Windows.Forms.Label lblSynologyUrl;
        private System.Windows.Forms.TextBox txtSynologyUrl;
        private System.Windows.Forms.Label lblThreshold;
        private System.Windows.Forms.NumericUpDown numThreshold;
        private System.Windows.Forms.Label lblInterval;
        private System.Windows.Forms.NumericUpDown numInterval;
        private System.Windows.Forms.Button btnSaveConfig;
        private System.Windows.Forms.Button btnTestChannels;
        private System.Windows.Forms.Label lblConfigStatus;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle cellStyleDark1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle cellStyleDark2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle cellStyleDark3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblWatcherStatus = new System.Windows.Forms.Label();
            this.lblHeaderSubtitle = new System.Windows.Forms.Label();
            this.lblHeaderTitle = new System.Windows.Forms.Label();
            this.pnlMetrics = new System.Windows.Forms.Panel();
            this.lblMetricDispatched = new System.Windows.Forms.Label();
            this.lblMetricPalletLapse = new System.Windows.Forms.Label();
            this.lblMetricInbounds = new System.Windows.Forms.Label();
            this.lblMetricLowStock = new System.Windows.Forms.Label();
            this.pnlActions = new System.Windows.Forms.Panel();
            this.btnClearHistory = new System.Windows.Forms.Button();
            this.btnSendTest = new System.Windows.Forms.Button();
            this.btnScanNow = new System.Windows.Forms.Button();
            this.btnToggleWatcher = new System.Windows.Forms.Button();
            this.tabControlNotifications = new System.Windows.Forms.TabControl();
            this.tabFeed = new System.Windows.Forms.TabPage();
            this.dgvFeed = new System.Windows.Forms.DataGridView();
            this.tabLowStock = new System.Windows.Forms.TabPage();
            this.pnlStockFilter = new System.Windows.Forms.Panel();
            this.lblStockCategory = new System.Windows.Forms.Label();
            this.cmbStockCategory = new System.Windows.Forms.ComboBox();
            this.chkLowStockOnly = new System.Windows.Forms.CheckBox();
            this.lblStockSearch = new System.Windows.Forms.Label();
            this.txtStockSearch = new System.Windows.Forms.TextBox();
            this.btnRefreshStock = new System.Windows.Forms.Button();
            this.dgvLowStock = new System.Windows.Forms.DataGridView();
            this.tabInbounds = new System.Windows.Forms.TabPage();
            this.pnlInboundsFilter = new System.Windows.Forms.Panel();
            this.lblInboundFilter = new System.Windows.Forms.Label();
            this.cmbInboundType = new System.Windows.Forms.ComboBox();
            this.lblInboundSearch = new System.Windows.Forms.Label();
            this.txtInboundSearch = new System.Windows.Forms.TextBox();
            this.btnRefreshInbounds = new System.Windows.Forms.Button();
            this.dgvInbounds = new System.Windows.Forms.DataGridView();
            this.tabPallets = new System.Windows.Forms.TabPage();
            this.pnlPalletFilter = new System.Windows.Forms.Panel();
            this.lblPalletStatus = new System.Windows.Forms.Label();
            this.cmbPalletStatus = new System.Windows.Forms.ComboBox();
            this.lblPalletSearch = new System.Windows.Forms.Label();
            this.txtPalletSearch = new System.Windows.Forms.TextBox();
            this.btnRefreshPallets = new System.Windows.Forms.Button();
            this.dgvPallets = new System.Windows.Forms.DataGridView();
            this.tabConfig = new System.Windows.Forms.TabPage();
            this.lblConfigStatus = new System.Windows.Forms.Label();
            this.btnTestChannels = new System.Windows.Forms.Button();
            this.btnSaveConfig = new System.Windows.Forms.Button();
            this.grpChannels = new System.Windows.Forms.GroupBox();
            this.numInterval = new System.Windows.Forms.NumericUpDown();
            this.lblInterval = new System.Windows.Forms.Label();
            this.numThreshold = new System.Windows.Forms.NumericUpDown();
            this.lblThreshold = new System.Windows.Forms.Label();
            this.txtSynologyUrl = new System.Windows.Forms.TextBox();
            this.lblSynologyUrl = new System.Windows.Forms.Label();
            this.txtPhpUrl = new System.Windows.Forms.TextBox();
            this.lblPhpUrl = new System.Windows.Forms.Label();
            this.grpSqlConfig = new System.Windows.Forms.GroupBox();
            this.txtSqlConn = new System.Windows.Forms.TextBox();
            this.lblSqlConn = new System.Windows.Forms.Label();
            this.pnlHeader.SuspendLayout();
            this.pnlMetrics.SuspendLayout();
            this.pnlActions.SuspendLayout();
            this.tabControlNotifications.SuspendLayout();
            this.tabFeed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvFeed)).BeginInit();
            this.tabLowStock.SuspendLayout();
            this.pnlStockFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLowStock)).BeginInit();
            this.tabInbounds.SuspendLayout();
            this.pnlInboundsFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInbounds)).BeginInit();
            this.tabPallets.SuspendLayout();
            this.pnlPalletFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPallets)).BeginInit();
            this.tabConfig.SuspendLayout();
            this.grpChannels.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThreshold)).BeginInit();
            this.grpSqlConfig.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(20)))), ((int)(((byte)(20)))));
            this.pnlHeader.Controls.Add(this.lblWatcherStatus);
            this.pnlHeader.Controls.Add(this.lblHeaderSubtitle);
            this.pnlHeader.Controls.Add(this.lblHeaderTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(15, 12, 15, 12);
            this.pnlHeader.Size = new System.Drawing.Size(950, 68);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblWatcherStatus
            // 
            this.lblWatcherStatus.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblWatcherStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(174)))), ((int)(((byte)(96)))));
            this.lblWatcherStatus.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWatcherStatus.ForeColor = System.Drawing.Color.White;
            this.lblWatcherStatus.Location = new System.Drawing.Point(760, 18);
            this.lblWatcherStatus.Name = "lblWatcherStatus";
            this.lblWatcherStatus.Size = new System.Drawing.Size(175, 30);
            this.lblWatcherStatus.TabIndex = 2;
            this.lblWatcherStatus.Text = "● WATCHER ACTIVE (15s)";
            this.lblWatcherStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblHeaderSubtitle
            // 
            this.lblHeaderSubtitle.AutoSize = true;
            this.lblHeaderSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(160)))), ((int)(((byte)(160)))));
            this.lblHeaderSubtitle.Location = new System.Drawing.Point(15, 38);
            this.lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            this.lblHeaderSubtitle.Size = new System.Drawing.Size(650, 13);
            this.lblHeaderSubtitle.TabIndex = 1;
            this.lblHeaderSubtitle.Text = "Stock Monitoring (<=30% Alert) • Real-Time Inbound Watcher (FIFO Notice) • Synology Chat • Firebase (FCM v1) • WebSocket Chat";
            // 
            // lblHeaderTitle
            // 
            this.lblHeaderTitle.AutoSize = true;
            this.lblHeaderTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.lblHeaderTitle.Location = new System.Drawing.Point(14, 11);
            this.lblHeaderTitle.Name = "lblHeaderTitle";
            this.lblHeaderTitle.Size = new System.Drawing.Size(355, 25);
            this.lblHeaderTitle.TabIndex = 0;
            this.lblHeaderTitle.Text = "🔔 Real-Time Notification & Inbound Hub";
            // 
            // pnlMetrics
            // 
            this.pnlMetrics.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(26)))), ((int)(((byte)(26)))));
            this.pnlMetrics.Controls.Add(this.lblMetricDispatched);
            this.pnlMetrics.Controls.Add(this.lblMetricPalletLapse);
            this.pnlMetrics.Controls.Add(this.lblMetricInbounds);
            this.pnlMetrics.Controls.Add(this.lblMetricLowStock);
            this.pnlMetrics.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlMetrics.Location = new System.Drawing.Point(0, 68);
            this.pnlMetrics.Name = "pnlMetrics";
            this.pnlMetrics.Size = new System.Drawing.Size(950, 42);
            this.pnlMetrics.TabIndex = 1;
            // 
            // lblMetricDispatched
            // 
            this.lblMetricDispatched.AutoSize = true;
            this.lblMetricDispatched.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMetricDispatched.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(52)))), ((int)(((byte)(152)))), ((int)(((byte)(219)))));
            this.lblMetricDispatched.Location = new System.Drawing.Point(660, 13);
            this.lblMetricDispatched.Name = "lblMetricDispatched";
            this.lblMetricDispatched.Size = new System.Drawing.Size(133, 15);
            this.lblMetricDispatched.TabIndex = 3;
            this.lblMetricDispatched.Text = "📡 Total Dispatched: 0";
            // 
            // lblMetricPalletLapse
            // 
            this.lblMetricPalletLapse.AutoSize = true;
            this.lblMetricPalletLapse.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMetricPalletLapse.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.lblMetricPalletLapse.Location = new System.Drawing.Point(420, 13);
            this.lblMetricPalletLapse.Name = "lblMetricPalletLapse";
            this.lblMetricPalletLapse.Size = new System.Drawing.Size(195, 15);
            this.lblMetricPalletLapse.TabIndex = 2;
            this.lblMetricPalletLapse.Text = "⏱️ 48h Testing Overdue: 0 Pallets";
            // 
            // lblMetricInbounds
            // 
            this.lblMetricInbounds.AutoSize = true;
            this.lblMetricInbounds.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMetricInbounds.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.lblMetricInbounds.Location = new System.Drawing.Point(210, 13);
            this.lblMetricInbounds.Name = "lblMetricInbounds";
            this.lblMetricInbounds.Size = new System.Drawing.Size(175, 15);
            this.lblMetricInbounds.TabIndex = 1;
            this.lblMetricInbounds.Text = "📦 Inbound Arrivals (FIFO): 0";
            // 
            // lblMetricLowStock
            // 
            this.lblMetricLowStock.AutoSize = true;
            this.lblMetricLowStock.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMetricLowStock.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.lblMetricLowStock.Location = new System.Drawing.Point(15, 13);
            this.lblMetricLowStock.Name = "lblMetricLowStock";
            this.lblMetricLowStock.Size = new System.Drawing.Size(155, 15);
            this.lblMetricLowStock.TabIndex = 0;
            this.lblMetricLowStock.Text = "⚠️ Low Stock Items (<=30%): 0";
            // 
            // pnlActions
            // 
            this.pnlActions.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlActions.Controls.Add(this.btnClearHistory);
            this.pnlActions.Controls.Add(this.btnSendTest);
            this.pnlActions.Controls.Add(this.btnScanNow);
            this.pnlActions.Controls.Add(this.btnToggleWatcher);
            this.pnlActions.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlActions.Location = new System.Drawing.Point(0, 110);
            this.pnlActions.Name = "pnlActions";
            this.pnlActions.Padding = new System.Windows.Forms.Padding(10, 5, 10, 5);
            this.pnlActions.Size = new System.Drawing.Size(950, 44);
            this.pnlActions.TabIndex = 2;
            // 
            // btnClearHistory
            // 
            this.btnClearHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClearHistory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.btnClearHistory.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClearHistory.FlatAppearance.BorderSize = 0;
            this.btnClearHistory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClearHistory.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClearHistory.ForeColor = System.Drawing.Color.LightGray;
            this.btnClearHistory.Location = new System.Drawing.Point(825, 8);
            this.btnClearHistory.Name = "btnClearHistory";
            this.btnClearHistory.Size = new System.Drawing.Size(110, 28);
            this.btnClearHistory.TabIndex = 3;
            this.btnClearHistory.Text = "🗑 Clear Feed";
            this.btnClearHistory.UseVisualStyleBackColor = false;
            this.btnClearHistory.Click += new System.EventHandler(this.BtnClearHistory_Click);
            // 
            // btnSendTest
            // 
            this.btnSendTest.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(68)))), ((int)(((byte)(173)))));
            this.btnSendTest.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSendTest.FlatAppearance.BorderSize = 0;
            this.btnSendTest.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSendTest.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSendTest.ForeColor = System.Drawing.Color.White;
            this.btnSendTest.Location = new System.Drawing.Point(310, 8);
            this.btnSendTest.Name = "btnSendTest";
            this.btnSendTest.Size = new System.Drawing.Size(155, 28);
            this.btnSendTest.TabIndex = 2;
            this.btnSendTest.Text = "✉ Send Test Alert";
            this.btnSendTest.UseVisualStyleBackColor = false;
            this.btnSendTest.Click += new System.EventHandler(this.BtnSendTest_Click);
            // 
            // btnScanNow
            // 
            this.btnScanNow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.btnScanNow.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnScanNow.FlatAppearance.BorderSize = 0;
            this.btnScanNow.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnScanNow.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnScanNow.ForeColor = System.Drawing.Color.White;
            this.btnScanNow.Location = new System.Drawing.Point(160, 8);
            this.btnScanNow.Name = "btnScanNow";
            this.btnScanNow.Size = new System.Drawing.Size(140, 28);
            this.btnScanNow.TabIndex = 1;
            this.btnScanNow.Text = "🔄 Scan Now";
            this.btnScanNow.UseVisualStyleBackColor = false;
            this.btnScanNow.Click += new System.EventHandler(this.BtnScanNow_Click);
            // 
            // btnToggleWatcher
            // 
            this.btnToggleWatcher.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(57)))), ((int)(((byte)(43)))));
            this.btnToggleWatcher.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnToggleWatcher.FlatAppearance.BorderSize = 0;
            this.btnToggleWatcher.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnToggleWatcher.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnToggleWatcher.ForeColor = System.Drawing.Color.White;
            this.btnToggleWatcher.Location = new System.Drawing.Point(10, 8);
            this.btnToggleWatcher.Name = "btnToggleWatcher";
            this.btnToggleWatcher.Size = new System.Drawing.Size(140, 28);
            this.btnToggleWatcher.TabIndex = 0;
            this.btnToggleWatcher.Text = "⏹ Stop Watcher";
            this.btnToggleWatcher.UseVisualStyleBackColor = false;
            this.btnToggleWatcher.Click += new System.EventHandler(this.BtnToggleWatcher_Click);
            // 
            // tabControlNotifications
            // 
            this.tabControlNotifications.Controls.Add(this.tabFeed);
            this.tabControlNotifications.Controls.Add(this.tabLowStock);
            this.tabControlNotifications.Controls.Add(this.tabInbounds);
            this.tabControlNotifications.Controls.Add(this.tabPallets);
            this.tabControlNotifications.Controls.Add(this.tabConfig);
            this.tabControlNotifications.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlNotifications.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabControlNotifications.Location = new System.Drawing.Point(0, 154);
            this.tabControlNotifications.Name = "tabControlNotifications";
            this.tabControlNotifications.SelectedIndex = 0;
            this.tabControlNotifications.Size = new System.Drawing.Size(950, 496);
            this.tabControlNotifications.TabIndex = 3;
            // 
            // tabFeed
            // 
            this.tabFeed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.tabFeed.Controls.Add(this.dgvFeed);
            this.tabFeed.Location = new System.Drawing.Point(4, 24);
            this.tabFeed.Name = "tabFeed";
            this.tabFeed.Padding = new System.Windows.Forms.Padding(5);
            this.tabFeed.Size = new System.Drawing.Size(942, 468);
            this.tabFeed.TabIndex = 0;
            this.tabFeed.Text = "📡 Live Notification Feed";
            // 
            // dgvFeed
            // 
            this.dgvFeed.AllowUserToAddRows = false;
            this.dgvFeed.AllowUserToDeleteRows = false;
            this.dgvFeed.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvFeed.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.dgvFeed.BorderStyle = System.Windows.Forms.BorderStyle.None;
            cellStyleDark1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyleDark1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            cellStyleDark1.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cellStyleDark1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            cellStyleDark1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            cellStyleDark1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.dgvFeed.ColumnHeadersDefaultCellStyle = cellStyleDark1;
            this.dgvFeed.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            System.Windows.Forms.DataGridViewCellStyle cellStyleFeedRow = new System.Windows.Forms.DataGridViewCellStyle();
            cellStyleFeedRow.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyleFeedRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            cellStyleFeedRow.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cellStyleFeedRow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            cellStyleFeedRow.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            cellStyleFeedRow.SelectionForeColor = System.Drawing.Color.White;
            this.dgvFeed.DefaultCellStyle = cellStyleFeedRow;
            this.dgvFeed.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvFeed.EnableHeadersVisualStyles = false;
            this.dgvFeed.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.dgvFeed.Location = new System.Drawing.Point(5, 5);
            this.dgvFeed.Name = "dgvFeed";
            this.dgvFeed.ReadOnly = true;
            this.dgvFeed.RowHeadersVisible = false;
            this.dgvFeed.Size = new System.Drawing.Size(932, 458);
            this.dgvFeed.TabIndex = 0;
            // 
            // tabLowStock
            // 
            this.tabLowStock.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.tabLowStock.Controls.Add(this.dgvLowStock);
            this.tabLowStock.Controls.Add(this.pnlStockFilter);
            this.tabLowStock.Location = new System.Drawing.Point(4, 24);
            this.tabLowStock.Name = "tabLowStock";
            this.tabLowStock.Padding = new System.Windows.Forms.Padding(5);
            this.tabLowStock.Size = new System.Drawing.Size(942, 468);
            this.tabLowStock.TabIndex = 1;
            this.tabLowStock.Text = "📊 Stock Inventory & Low Stock (Material, Pigment, Chem)";
            // 
            // pnlStockFilter
            // 
            this.pnlStockFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlStockFilter.Controls.Add(this.btnRefreshStock);
            this.pnlStockFilter.Controls.Add(this.txtStockSearch);
            this.pnlStockFilter.Controls.Add(this.lblStockSearch);
            this.pnlStockFilter.Controls.Add(this.chkLowStockOnly);
            this.pnlStockFilter.Controls.Add(this.cmbStockCategory);
            this.pnlStockFilter.Controls.Add(this.lblStockCategory);
            this.pnlStockFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlStockFilter.Location = new System.Drawing.Point(5, 5);
            this.pnlStockFilter.Name = "pnlStockFilter";
            this.pnlStockFilter.Size = new System.Drawing.Size(932, 38);
            this.pnlStockFilter.TabIndex = 1;
            // 
            // lblStockCategory
            // 
            this.lblStockCategory.AutoSize = true;
            this.lblStockCategory.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblStockCategory.Location = new System.Drawing.Point(10, 11);
            this.lblStockCategory.Name = "lblStockCategory";
            this.lblStockCategory.Size = new System.Drawing.Size(56, 13);
            this.lblStockCategory.TabIndex = 0;
            this.lblStockCategory.Text = "Category:";
            // 
            // cmbStockCategory
            // 
            this.cmbStockCategory.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cmbStockCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStockCategory.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbStockCategory.ForeColor = System.Drawing.Color.White;
            this.cmbStockCategory.FormattingEnabled = true;
            this.cmbStockCategory.Location = new System.Drawing.Point(72, 7);
            this.cmbStockCategory.Name = "cmbStockCategory";
            this.cmbStockCategory.Size = new System.Drawing.Size(205, 21);
            this.cmbStockCategory.TabIndex = 1;
            // 
            // chkLowStockOnly
            // 
            this.chkLowStockOnly.AutoSize = true;
            this.chkLowStockOnly.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkLowStockOnly.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(156)))), ((int)(((byte)(18)))));
            this.chkLowStockOnly.Location = new System.Drawing.Point(290, 10);
            this.chkLowStockOnly.Name = "chkLowStockOnly";
            this.chkLowStockOnly.Size = new System.Drawing.Size(185, 17);
            this.chkLowStockOnly.TabIndex = 2;
            this.chkLowStockOnly.Text = "⚠️ Critical / Low Only (<= 30%)";
            this.chkLowStockOnly.UseVisualStyleBackColor = true;
            // 
            // lblStockSearch
            // 
            this.lblStockSearch.AutoSize = true;
            this.lblStockSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblStockSearch.Location = new System.Drawing.Point(485, 11);
            this.lblStockSearch.Name = "lblStockSearch";
            this.lblStockSearch.Size = new System.Drawing.Size(44, 13);
            this.lblStockSearch.TabIndex = 3;
            this.lblStockSearch.Text = "Search:";
            // 
            // txtStockSearch
            // 
            this.txtStockSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.txtStockSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtStockSearch.ForeColor = System.Drawing.Color.White;
            this.txtStockSearch.Location = new System.Drawing.Point(535, 8);
            this.txtStockSearch.Name = "txtStockSearch";
            this.txtStockSearch.Size = new System.Drawing.Size(180, 22);
            this.txtStockSearch.TabIndex = 4;
            // 
            // btnRefreshStock
            // 
            this.btnRefreshStock.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.btnRefreshStock.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefreshStock.FlatAppearance.BorderSize = 0;
            this.btnRefreshStock.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshStock.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefreshStock.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnRefreshStock.Location = new System.Drawing.Point(725, 6);
            this.btnRefreshStock.Name = "btnRefreshStock";
            this.btnRefreshStock.Size = new System.Drawing.Size(95, 26);
            this.btnRefreshStock.TabIndex = 5;
            this.btnRefreshStock.Text = "🔄 Refresh";
            this.btnRefreshStock.UseVisualStyleBackColor = false;
            // 
            // dgvLowStock
            // 
            this.dgvLowStock.AllowUserToAddRows = false;
            this.dgvLowStock.AllowUserToDeleteRows = false;
            this.dgvLowStock.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvLowStock.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.dgvLowStock.BorderStyle = System.Windows.Forms.BorderStyle.None;
            cellStyleDark2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyleDark2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            cellStyleDark2.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cellStyleDark2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            cellStyleDark2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            cellStyleDark2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.dgvLowStock.ColumnHeadersDefaultCellStyle = cellStyleDark2;
            this.dgvLowStock.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            System.Windows.Forms.DataGridViewCellStyle cellStyleStockRow = new System.Windows.Forms.DataGridViewCellStyle();
            cellStyleStockRow.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyleStockRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            cellStyleStockRow.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cellStyleStockRow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            cellStyleStockRow.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            cellStyleStockRow.SelectionForeColor = System.Drawing.Color.White;
            this.dgvLowStock.DefaultCellStyle = cellStyleStockRow;
            this.dgvLowStock.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvLowStock.EnableHeadersVisualStyles = false;
            this.dgvLowStock.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.dgvLowStock.Location = new System.Drawing.Point(5, 43);
            this.dgvLowStock.Name = "dgvLowStock";
            this.dgvLowStock.ReadOnly = true;
            this.dgvLowStock.RowHeadersVisible = false;
            this.dgvLowStock.Size = new System.Drawing.Size(932, 420);
            this.dgvLowStock.TabIndex = 0;
            // 
            // tabInbounds
            // 
            this.tabInbounds.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.tabInbounds.Controls.Add(this.dgvInbounds);
            this.tabInbounds.Controls.Add(this.pnlInboundsFilter);
            this.tabInbounds.Location = new System.Drawing.Point(4, 24);
            this.tabInbounds.Name = "tabInbounds";
            this.tabInbounds.Padding = new System.Windows.Forms.Padding(5);
            this.tabInbounds.Size = new System.Drawing.Size(942, 468);
            this.tabInbounds.TabIndex = 2;
            this.tabInbounds.Text = "📦 Inbound Logs (FIFO Tracker)";
            // 
            // pnlInboundsFilter
            // 
            this.pnlInboundsFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlInboundsFilter.Controls.Add(this.btnRefreshInbounds);
            this.pnlInboundsFilter.Controls.Add(this.txtInboundSearch);
            this.pnlInboundsFilter.Controls.Add(this.lblInboundSearch);
            this.pnlInboundsFilter.Controls.Add(this.cmbInboundType);
            this.pnlInboundsFilter.Controls.Add(this.lblInboundFilter);
            this.pnlInboundsFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlInboundsFilter.Location = new System.Drawing.Point(5, 5);
            this.pnlInboundsFilter.Name = "pnlInboundsFilter";
            this.pnlInboundsFilter.Size = new System.Drawing.Size(932, 38);
            this.pnlInboundsFilter.TabIndex = 1;
            // 
            // lblInboundFilter
            // 
            this.lblInboundFilter.AutoSize = true;
            this.lblInboundFilter.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblInboundFilter.Location = new System.Drawing.Point(10, 11);
            this.lblInboundFilter.Name = "lblInboundFilter";
            this.lblInboundFilter.Size = new System.Drawing.Size(33, 13);
            this.lblInboundFilter.TabIndex = 0;
            this.lblInboundFilter.Text = "Type:";
            // 
            // cmbInboundType
            // 
            this.cmbInboundType.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cmbInboundType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbInboundType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbInboundType.ForeColor = System.Drawing.Color.White;
            this.cmbInboundType.FormattingEnabled = true;
            this.cmbInboundType.Location = new System.Drawing.Point(50, 7);
            this.cmbInboundType.Name = "cmbInboundType";
            this.cmbInboundType.Size = new System.Drawing.Size(205, 21);
            this.cmbInboundType.TabIndex = 1;
            // 
            // lblInboundSearch
            // 
            this.lblInboundSearch.AutoSize = true;
            this.lblInboundSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblInboundSearch.Location = new System.Drawing.Point(265, 11);
            this.lblInboundSearch.Name = "lblInboundSearch";
            this.lblInboundSearch.Size = new System.Drawing.Size(44, 13);
            this.lblInboundSearch.TabIndex = 2;
            this.lblInboundSearch.Text = "Search:";
            // 
            // txtInboundSearch
            // 
            this.txtInboundSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.txtInboundSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtInboundSearch.ForeColor = System.Drawing.Color.White;
            this.txtInboundSearch.Location = new System.Drawing.Point(315, 8);
            this.txtInboundSearch.Name = "txtInboundSearch";
            this.txtInboundSearch.Size = new System.Drawing.Size(210, 22);
            this.txtInboundSearch.TabIndex = 3;
            // 
            // btnRefreshInbounds
            // 
            this.btnRefreshInbounds.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.btnRefreshInbounds.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefreshInbounds.FlatAppearance.BorderSize = 0;
            this.btnRefreshInbounds.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshInbounds.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefreshInbounds.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnRefreshInbounds.Location = new System.Drawing.Point(535, 6);
            this.btnRefreshInbounds.Name = "btnRefreshInbounds";
            this.btnRefreshInbounds.Size = new System.Drawing.Size(95, 26);
            this.btnRefreshInbounds.TabIndex = 4;
            this.btnRefreshInbounds.Text = "🔄 Refresh";
            this.btnRefreshInbounds.UseVisualStyleBackColor = false;
            // 
            // dgvInbounds
            // 
            this.dgvInbounds.AllowUserToAddRows = false;
            this.dgvInbounds.AllowUserToDeleteRows = false;
            this.dgvInbounds.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvInbounds.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.dgvInbounds.BorderStyle = System.Windows.Forms.BorderStyle.None;
            cellStyleDark3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyleDark3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            cellStyleDark3.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cellStyleDark3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            cellStyleDark3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            cellStyleDark3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.dgvInbounds.ColumnHeadersDefaultCellStyle = cellStyleDark3;
            this.dgvInbounds.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            System.Windows.Forms.DataGridViewCellStyle cellStyleInboundsRow = new System.Windows.Forms.DataGridViewCellStyle();
            cellStyleInboundsRow.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStyleInboundsRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            cellStyleInboundsRow.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cellStyleInboundsRow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            cellStyleInboundsRow.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            cellStyleInboundsRow.SelectionForeColor = System.Drawing.Color.White;
            this.dgvInbounds.DefaultCellStyle = cellStyleInboundsRow;
            this.dgvInbounds.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvInbounds.EnableHeadersVisualStyles = false;
            this.dgvInbounds.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.dgvInbounds.Location = new System.Drawing.Point(5, 43);
            this.dgvInbounds.Name = "dgvInbounds";
            this.dgvInbounds.ReadOnly = true;
            this.dgvInbounds.RowHeadersVisible = false;
            this.dgvInbounds.Size = new System.Drawing.Size(932, 420);
            this.dgvInbounds.TabIndex = 0;
            // 
            // tabPallets
            // 
            this.tabPallets.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.tabPallets.Controls.Add(this.dgvPallets);
            this.tabPallets.Controls.Add(this.pnlPalletFilter);
            this.tabPallets.Location = new System.Drawing.Point(4, 24);
            this.tabPallets.Name = "tabPallets";
            this.tabPallets.Padding = new System.Windows.Forms.Padding(5);
            this.tabPallets.Size = new System.Drawing.Size(942, 468);
            this.tabPallets.TabIndex = 3;
            this.tabPallets.Text = "⏱️ 48-Hour Pallet Quality (TS2 / TC90 / Chem)";
            // 
            // pnlPalletFilter
            // 
            this.pnlPalletFilter.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlPalletFilter.Controls.Add(this.btnRefreshPallets);
            this.pnlPalletFilter.Controls.Add(this.txtPalletSearch);
            this.pnlPalletFilter.Controls.Add(this.lblPalletSearch);
            this.pnlPalletFilter.Controls.Add(this.cmbPalletStatus);
            this.pnlPalletFilter.Controls.Add(this.lblPalletStatus);
            this.pnlPalletFilter.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlPalletFilter.Location = new System.Drawing.Point(5, 5);
            this.pnlPalletFilter.Name = "pnlPalletFilter";
            this.pnlPalletFilter.Size = new System.Drawing.Size(932, 38);
            this.pnlPalletFilter.TabIndex = 1;
            // 
            // lblPalletStatus
            // 
            this.lblPalletStatus.AutoSize = true;
            this.lblPalletStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblPalletStatus.Location = new System.Drawing.Point(10, 11);
            this.lblPalletStatus.Name = "lblPalletStatus";
            this.lblPalletStatus.Size = new System.Drawing.Size(42, 13);
            this.lblPalletStatus.TabIndex = 0;
            this.lblPalletStatus.Text = "Status:";
            // 
            // cmbPalletStatus
            // 
            this.cmbPalletStatus.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.cmbPalletStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbPalletStatus.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbPalletStatus.ForeColor = System.Drawing.Color.White;
            this.cmbPalletStatus.FormattingEnabled = true;
            this.cmbPalletStatus.Location = new System.Drawing.Point(58, 7);
            this.cmbPalletStatus.Name = "cmbPalletStatus";
            this.cmbPalletStatus.Size = new System.Drawing.Size(205, 21);
            this.cmbPalletStatus.TabIndex = 1;
            // 
            // lblPalletSearch
            // 
            this.lblPalletSearch.AutoSize = true;
            this.lblPalletSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblPalletSearch.Location = new System.Drawing.Point(275, 11);
            this.lblPalletSearch.Name = "lblPalletSearch";
            this.lblPalletSearch.Size = new System.Drawing.Size(44, 13);
            this.lblPalletSearch.TabIndex = 2;
            this.lblPalletSearch.Text = "Search:";
            // 
            // txtPalletSearch
            // 
            this.txtPalletSearch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(40)))), ((int)(((byte)(40)))), ((int)(((byte)(40)))));
            this.txtPalletSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPalletSearch.ForeColor = System.Drawing.Color.White;
            this.txtPalletSearch.Location = new System.Drawing.Point(325, 8);
            this.txtPalletSearch.Name = "txtPalletSearch";
            this.txtPalletSearch.Size = new System.Drawing.Size(210, 22);
            this.txtPalletSearch.TabIndex = 3;
            // 
            // btnRefreshPallets
            // 
            this.btnRefreshPallets.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.btnRefreshPallets.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefreshPallets.FlatAppearance.BorderSize = 0;
            this.btnRefreshPallets.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshPallets.Font = new System.Drawing.Font("Segoe UI Semibold", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefreshPallets.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.btnRefreshPallets.Location = new System.Drawing.Point(545, 6);
            this.btnRefreshPallets.Name = "btnRefreshPallets";
            this.btnRefreshPallets.Size = new System.Drawing.Size(105, 26);
            this.btnRefreshPallets.TabIndex = 4;
            this.btnRefreshPallets.Text = "🔄 Refresh";
            this.btnRefreshPallets.UseVisualStyleBackColor = false;
            // 
            // dgvPallets
            // 
            this.dgvPallets.AllowUserToAddRows = false;
            this.dgvPallets.AllowUserToDeleteRows = false;
            this.dgvPallets.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvPallets.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(28)))), ((int)(((byte)(28)))), ((int)(((byte)(28)))));
            this.dgvPallets.BorderStyle = System.Windows.Forms.BorderStyle.None;
            System.Windows.Forms.DataGridViewCellStyle cellStylePalletHeader = new System.Windows.Forms.DataGridViewCellStyle();
            cellStylePalletHeader.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStylePalletHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            cellStylePalletHeader.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cellStylePalletHeader.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            cellStylePalletHeader.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(18)))), ((int)(((byte)(18)))), ((int)(((byte)(18)))));
            cellStylePalletHeader.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(76)))), ((int)(((byte)(60)))));
            this.dgvPallets.ColumnHeadersDefaultCellStyle = cellStylePalletHeader;
            this.dgvPallets.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            System.Windows.Forms.DataGridViewCellStyle cellStylePalletRow = new System.Windows.Forms.DataGridViewCellStyle();
            cellStylePalletRow.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            cellStylePalletRow.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            cellStylePalletRow.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            cellStylePalletRow.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(230)))), ((int)(((byte)(230)))));
            cellStylePalletRow.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            cellStylePalletRow.SelectionForeColor = System.Drawing.Color.White;
            this.dgvPallets.DefaultCellStyle = cellStylePalletRow;
            this.dgvPallets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvPallets.EnableHeadersVisualStyles = false;
            this.dgvPallets.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.dgvPallets.Location = new System.Drawing.Point(5, 43);
            this.dgvPallets.Name = "dgvPallets";
            this.dgvPallets.ReadOnly = true;
            this.dgvPallets.RowHeadersVisible = false;
            this.dgvPallets.Size = new System.Drawing.Size(932, 420);
            this.dgvPallets.TabIndex = 0;
            // 
            // tabConfig
            // 
            this.tabConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.tabConfig.Controls.Add(this.lblConfigStatus);
            this.tabConfig.Controls.Add(this.btnTestChannels);
            this.tabConfig.Controls.Add(this.btnSaveConfig);
            this.tabConfig.Controls.Add(this.grpChannels);
            this.tabConfig.Controls.Add(this.grpSqlConfig);
            this.tabConfig.Location = new System.Drawing.Point(4, 24);
            this.tabConfig.Name = "tabConfig";
            this.tabConfig.Padding = new System.Windows.Forms.Padding(15);
            this.tabConfig.Size = new System.Drawing.Size(942, 468);
            this.tabConfig.TabIndex = 3;
            this.tabConfig.Text = "⚙️ Dispatcher Configuration";
            // 
            // lblConfigStatus
            // 
            this.lblConfigStatus.AutoSize = true;
            this.lblConfigStatus.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.lblConfigStatus.Location = new System.Drawing.Point(20, 395);
            this.lblConfigStatus.Name = "lblConfigStatus";
            this.lblConfigStatus.Size = new System.Drawing.Size(262, 15);
            this.lblConfigStatus.TabIndex = 4;
            this.lblConfigStatus.Text = "Settings ready. Click Save to apply modifications.";
            // 
            // btnTestChannels
            // 
            this.btnTestChannels.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(45)))));
            this.btnTestChannels.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnTestChannels.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnTestChannels.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnTestChannels.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnTestChannels.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.btnTestChannels.Location = new System.Drawing.Point(215, 350);
            this.btnTestChannels.Name = "btnTestChannels";
            this.btnTestChannels.Size = new System.Drawing.Size(190, 34);
            this.btnTestChannels.TabIndex = 3;
            this.btnTestChannels.Text = "⚡ Test SQL & Dispatchers";
            this.btnTestChannels.UseVisualStyleBackColor = false;
            this.btnTestChannels.Click += new System.EventHandler(this.BtnTestChannels_Click);
            // 
            // btnSaveConfig
            // 
            this.btnSaveConfig.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(120)))), ((int)(((byte)(215)))));
            this.btnSaveConfig.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveConfig.FlatAppearance.BorderSize = 0;
            this.btnSaveConfig.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveConfig.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSaveConfig.ForeColor = System.Drawing.Color.White;
            this.btnSaveConfig.Location = new System.Drawing.Point(20, 350);
            this.btnSaveConfig.Name = "btnSaveConfig";
            this.btnSaveConfig.Size = new System.Drawing.Size(180, 34);
            this.btnSaveConfig.TabIndex = 2;
            this.btnSaveConfig.Text = "💾 Save Configuration";
            this.btnSaveConfig.UseVisualStyleBackColor = false;
            this.btnSaveConfig.Click += new System.EventHandler(this.BtnSaveConfig_Click);
            // 
            // grpChannels
            // 
            this.grpChannels.Controls.Add(this.numInterval);
            this.grpChannels.Controls.Add(this.lblInterval);
            this.grpChannels.Controls.Add(this.numThreshold);
            this.grpChannels.Controls.Add(this.lblThreshold);
            this.grpChannels.Controls.Add(this.txtSynologyUrl);
            this.grpChannels.Controls.Add(this.lblSynologyUrl);
            this.grpChannels.Controls.Add(this.txtPhpUrl);
            this.grpChannels.Controls.Add(this.lblPhpUrl);
            this.grpChannels.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpChannels.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.grpChannels.Location = new System.Drawing.Point(20, 115);
            this.grpChannels.Name = "grpChannels";
            this.grpChannels.Size = new System.Drawing.Size(890, 220);
            this.grpChannels.TabIndex = 1;
            this.grpChannels.TabStop = false;
            this.grpChannels.Text = "Notification Channels & Threshold Parameters";
            // 
            // numInterval
            // 
            this.numInterval.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.numInterval.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numInterval.ForeColor = System.Drawing.Color.White;
            this.numInterval.Location = new System.Drawing.Point(520, 168);
            this.numInterval.Maximum = new decimal(new int[] { 3600, 0, 0, 0 });
            this.numInterval.Minimum = new decimal(new int[] { 5, 0, 0, 0 });
            this.numInterval.Name = "numInterval";
            this.numInterval.Size = new System.Drawing.Size(100, 23);
            this.numInterval.TabIndex = 7;
            this.numInterval.Value = new decimal(new int[] { 15, 0, 0, 0 });
            // 
            // lblInterval
            // 
            this.lblInterval.AutoSize = true;
            this.lblInterval.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInterval.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblInterval.Location = new System.Drawing.Point(360, 172);
            this.lblInterval.Name = "lblInterval";
            this.lblInterval.Size = new System.Drawing.Size(143, 13);
            this.lblInterval.TabIndex = 6;
            this.lblInterval.Text = "Inbound Poll Interval (sec):";
            // 
            // numThreshold
            // 
            this.numThreshold.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.numThreshold.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.numThreshold.ForeColor = System.Drawing.Color.White;
            this.numThreshold.Location = new System.Drawing.Point(210, 168);
            this.numThreshold.Maximum = new decimal(new int[] { 99, 0, 0, 0 });
            this.numThreshold.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            this.numThreshold.Name = "numThreshold";
            this.numThreshold.Size = new System.Drawing.Size(100, 23);
            this.numThreshold.TabIndex = 5;
            this.numThreshold.Value = new decimal(new int[] { 30, 0, 0, 0 });
            // 
            // lblThreshold
            // 
            this.lblThreshold.AutoSize = true;
            this.lblThreshold.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblThreshold.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblThreshold.Location = new System.Drawing.Point(20, 172);
            this.lblThreshold.Name = "lblThreshold";
            this.lblThreshold.Size = new System.Drawing.Size(173, 13);
            this.lblThreshold.TabIndex = 4;
            this.lblThreshold.Text = "Low Stock Warning Threshold (%):";
            // 
            // txtSynologyUrl
            // 
            this.txtSynologyUrl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.txtSynologyUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSynologyUrl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSynologyUrl.ForeColor = System.Drawing.Color.White;
            this.txtSynologyUrl.Location = new System.Drawing.Point(20, 120);
            this.txtSynologyUrl.Name = "txtSynologyUrl";
            this.txtSynologyUrl.Size = new System.Drawing.Size(850, 23);
            this.txtSynologyUrl.TabIndex = 3;
            // 
            // lblSynologyUrl
            // 
            this.lblSynologyUrl.AutoSize = true;
            this.lblSynologyUrl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSynologyUrl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblSynologyUrl.Location = new System.Drawing.Point(17, 98);
            this.lblSynologyUrl.Name = "lblSynologyUrl";
            this.lblSynologyUrl.Size = new System.Drawing.Size(193, 13);
            this.lblSynologyUrl.TabIndex = 2;
            this.lblSynologyUrl.Text = "Synology Chat External Bot Webhook (RMS):";
            // 
            // txtPhpUrl
            // 
            this.txtPhpUrl.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.txtPhpUrl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtPhpUrl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtPhpUrl.ForeColor = System.Drawing.Color.White;
            this.txtPhpUrl.Location = new System.Drawing.Point(20, 55);
            this.txtPhpUrl.Name = "txtPhpUrl";
            this.txtPhpUrl.Size = new System.Drawing.Size(850, 23);
            this.txtPhpUrl.TabIndex = 1;
            // 
            // lblPhpUrl
            // 
            this.lblPhpUrl.AutoSize = true;
            this.lblPhpUrl.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblPhpUrl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblPhpUrl.Location = new System.Drawing.Point(17, 33);
            this.lblPhpUrl.Name = "lblPhpUrl";
            this.lblPhpUrl.Size = new System.Drawing.Size(264, 13);
            this.lblPhpUrl.TabIndex = 0;
            this.lblPhpUrl.Text = "Firebase & RMS Notification Dispatcher Endpoint (PHP):";
            // 
            // grpSqlConfig
            // 
            this.grpSqlConfig.Controls.Add(this.txtSqlConn);
            this.grpSqlConfig.Controls.Add(this.lblSqlConn);
            this.grpSqlConfig.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpSqlConfig.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(229)))), ((int)(((byte)(255)))));
            this.grpSqlConfig.Location = new System.Drawing.Point(20, 15);
            this.grpSqlConfig.Name = "grpSqlConfig";
            this.grpSqlConfig.Size = new System.Drawing.Size(890, 85);
            this.grpSqlConfig.TabIndex = 0;
            this.grpSqlConfig.TabStop = false;
            this.grpSqlConfig.Text = "Production Database Connection (dbRubber)";
            // 
            // txtSqlConn
            // 
            this.txtSqlConn.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.txtSqlConn.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSqlConn.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSqlConn.ForeColor = System.Drawing.Color.White;
            this.txtSqlConn.Location = new System.Drawing.Point(20, 45);
            this.txtSqlConn.Name = "txtSqlConn";
            this.txtSqlConn.Size = new System.Drawing.Size(850, 23);
            this.txtSqlConn.TabIndex = 1;
            // 
            // lblSqlConn
            // 
            this.lblSqlConn.AutoSize = true;
            this.lblSqlConn.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSqlConn.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(200)))), ((int)(((byte)(200)))), ((int)(((byte)(200)))));
            this.lblSqlConn.Location = new System.Drawing.Point(17, 25);
            this.lblSqlConn.Name = "lblSqlConn";
            this.lblSqlConn.Size = new System.Drawing.Size(189, 13);
            this.lblSqlConn.TabIndex = 0;
            this.lblSqlConn.Text = "SQL Server ADO.NET Connection String:";
            // 
            // NotificationHubUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.Controls.Add(this.tabControlNotifications);
            this.Controls.Add(this.pnlActions);
            this.Controls.Add(this.pnlMetrics);
            this.Controls.Add(this.pnlHeader);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Name = "NotificationHubUserControl";
            this.Size = new System.Drawing.Size(950, 650);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlMetrics.ResumeLayout(false);
            this.pnlMetrics.PerformLayout();
            this.pnlActions.ResumeLayout(false);
            this.tabControlNotifications.ResumeLayout(false);
            this.tabFeed.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvFeed)).EndInit();
            this.tabLowStock.ResumeLayout(false);
            this.pnlStockFilter.ResumeLayout(false);
            this.pnlStockFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLowStock)).EndInit();
            this.tabInbounds.ResumeLayout(false);
            this.pnlInboundsFilter.ResumeLayout(false);
            this.pnlInboundsFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInbounds)).EndInit();
            this.tabPallets.ResumeLayout(false);
            this.pnlPalletFilter.ResumeLayout(false);
            this.pnlPalletFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvPallets)).EndInit();
            this.tabConfig.ResumeLayout(false);
            this.tabConfig.PerformLayout();
            this.grpChannels.ResumeLayout(false);
            this.grpChannels.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numThreshold)).EndInit();
            this.grpSqlConfig.ResumeLayout(false);
            this.grpSqlConfig.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}
