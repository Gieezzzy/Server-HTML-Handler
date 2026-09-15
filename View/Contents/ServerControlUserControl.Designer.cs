namespace Server_HTML_Handler.View.Contents
{
    partial class ServerControlUserControl
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
            this.pnlTopHeader = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnRefreshHealth = new System.Windows.Forms.Button();
            this.gridFlowPanel = new System.Windows.Forms.FlowLayoutPanel();

            // DVWA Card
            this.grpDvwa = new System.Windows.Forms.GroupBox();
            this.lblDvwaStatus = new System.Windows.Forms.Label();
            this.lblDvwaDesc = new System.Windows.Forms.Label();
            this.btnDvwaLaunch = new System.Windows.Forms.Button();
            this.btnDvwaReset = new System.Windows.Forms.Button();
            this.comboDvwaDiff = new System.Windows.Forms.ComboBox();
            this.btnDvwaSetDiff = new System.Windows.Forms.Button();

            // Juice Shop Card
            this.grpJuice = new System.Windows.Forms.GroupBox();
            this.lblJuiceStatus = new System.Windows.Forms.Label();
            this.lblJuiceDesc = new System.Windows.Forms.Label();
            this.btnJuiceLaunch = new System.Windows.Forms.Button();
            this.btnJuiceStart = new System.Windows.Forms.Button();
            this.btnJuiceStop = new System.Windows.Forms.Button();

            // WebGoat Card
            this.grpGoat = new System.Windows.Forms.GroupBox();
            this.lblGoatStatus = new System.Windows.Forms.Label();
            this.lblGoatDesc = new System.Windows.Forms.Label();
            this.btnGoatLaunch = new System.Windows.Forms.Button();
            this.btnGoatStart = new System.Windows.Forms.Button();
            this.btnGoatStop = new System.Windows.Forms.Button();

            // Mutillidae Card
            this.grpMutillidae = new System.Windows.Forms.GroupBox();
            this.lblMutillidaeStatus = new System.Windows.Forms.Label();
            this.lblMutillidaeDesc = new System.Windows.Forms.Label();
            this.btnMutillidaeLaunch = new System.Windows.Forms.Button();
            this.btnMutillidaeReset = new System.Windows.Forms.Button();

            // bWAPP Card
            this.grpBwapp = new System.Windows.Forms.GroupBox();
            this.lblBwappStatus = new System.Windows.Forms.Label();
            this.lblBwappDesc = new System.Windows.Forms.Label();
            this.btnBwappLaunch = new System.Windows.Forms.Button();
            this.btnBwappReset = new System.Windows.Forms.Button();

            this.pnlTopHeader.SuspendLayout();
            this.gridFlowPanel.SuspendLayout();
            this.grpDvwa.SuspendLayout();
            this.grpJuice.SuspendLayout();
            this.grpGoat.SuspendLayout();
            this.grpMutillidae.SuspendLayout();
            this.grpBwapp.SuspendLayout();
            this.SuspendLayout();

            // 
            // pnlTopHeader
            // 
            this.pnlTopHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.pnlTopHeader.Controls.Add(this.btnRefreshHealth);
            this.pnlTopHeader.Controls.Add(this.lblTitle);
            this.pnlTopHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlTopHeader.Name = "pnlTopHeader";
            this.pnlTopHeader.Size = new System.Drawing.Size(950, 48);
            this.pnlTopHeader.TabIndex = 0;

            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.Cyan;
            this.lblTitle.Location = new System.Drawing.Point(15, 11);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(306, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "🛡️ SECURITY TEST LAB MANAGER";

            // 
            // btnRefreshHealth
            // 
            this.btnRefreshHealth.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRefreshHealth.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.btnRefreshHealth.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefreshHealth.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshHealth.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnRefreshHealth.ForeColor = System.Drawing.Color.LimeGreen;
            this.btnRefreshHealth.Location = new System.Drawing.Point(795, 9);
            this.btnRefreshHealth.Name = "btnRefreshHealth";
            this.btnRefreshHealth.Size = new System.Drawing.Size(140, 30);
            this.btnRefreshHealth.TabIndex = 1;
            this.btnRefreshHealth.Text = "🔄 Refresh Status";
            this.btnRefreshHealth.UseVisualStyleBackColor = false;
            this.btnRefreshHealth.Click += new System.EventHandler(this.BtnRefreshHealth_Click);

            // 
            // gridFlowPanel
            // 
            this.gridFlowPanel.AutoScroll = true;
            this.gridFlowPanel.Controls.Add(this.grpDvwa);
            this.gridFlowPanel.Controls.Add(this.grpJuice);
            this.gridFlowPanel.Controls.Add(this.grpGoat);
            this.gridFlowPanel.Controls.Add(this.grpMutillidae);
            this.gridFlowPanel.Controls.Add(this.grpBwapp);
            this.gridFlowPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridFlowPanel.Location = new System.Drawing.Point(0, 48);
            this.gridFlowPanel.Name = "gridFlowPanel";
            this.gridFlowPanel.Padding = new System.Windows.Forms.Padding(15);
            this.gridFlowPanel.Size = new System.Drawing.Size(950, 502);
            this.gridFlowPanel.TabIndex = 1;

            // 
            // grpDvwa
            // 
            this.grpDvwa.Controls.Add(this.lblDvwaStatus);
            this.grpDvwa.Controls.Add(this.btnDvwaSetDiff);
            this.grpDvwa.Controls.Add(this.comboDvwaDiff);
            this.grpDvwa.Controls.Add(this.btnDvwaReset);
            this.grpDvwa.Controls.Add(this.btnDvwaLaunch);
            this.grpDvwa.Controls.Add(this.lblDvwaDesc);
            this.grpDvwa.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpDvwa.ForeColor = System.Drawing.Color.White;
            this.grpDvwa.Location = new System.Drawing.Point(18, 18);
            this.grpDvwa.Name = "grpDvwa";
            this.grpDvwa.Size = new System.Drawing.Size(270, 225);
            this.grpDvwa.TabIndex = 0;
            this.grpDvwa.TabStop = false;
            this.grpDvwa.Text = "DVWA (Damn Vulnerable App)";

            // 
            // lblDvwaDesc
            // 
            this.lblDvwaDesc.AutoSize = true;
            this.lblDvwaDesc.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblDvwaDesc.ForeColor = System.Drawing.Color.DarkGray;
            this.lblDvwaDesc.Location = new System.Drawing.Point(12, 23);
            this.lblDvwaDesc.Name = "lblDvwaDesc";
            this.lblDvwaDesc.Size = new System.Drawing.Size(163, 15);
            this.lblDvwaDesc.TabIndex = 0;
            this.lblDvwaDesc.Text = "Local PHP/MySQL Environment";

            // 
            // lblDvwaStatus
            // 
            this.lblDvwaStatus.AutoSize = true;
            this.lblDvwaStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblDvwaStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblDvwaStatus.Location = new System.Drawing.Point(12, 42);
            this.lblDvwaStatus.Name = "lblDvwaStatus";
            this.lblDvwaStatus.Size = new System.Drawing.Size(102, 15);
            this.lblDvwaStatus.TabIndex = 1;
            this.lblDvwaStatus.Text = "Status: Checking...";

            // 
            // btnDvwaLaunch
            // 
            this.btnDvwaLaunch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnDvwaLaunch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDvwaLaunch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDvwaLaunch.ForeColor = System.Drawing.Color.Cyan;
            this.btnDvwaLaunch.Location = new System.Drawing.Point(12, 65);
            this.btnDvwaLaunch.Name = "btnDvwaLaunch";
            this.btnDvwaLaunch.Size = new System.Drawing.Size(245, 30);
            this.btnDvwaLaunch.TabIndex = 2;
            this.btnDvwaLaunch.Text = "🚀 Launch in Browser";
            this.btnDvwaLaunch.UseVisualStyleBackColor = false;
            this.btnDvwaLaunch.Click += new System.EventHandler(this.BtnDvwaLaunch_Click);

            // 
            // btnDvwaReset
            // 
            this.btnDvwaReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnDvwaReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDvwaReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDvwaReset.ForeColor = System.Drawing.Color.Orange;
            this.btnDvwaReset.Location = new System.Drawing.Point(12, 102);
            this.btnDvwaReset.Name = "btnDvwaReset";
            this.btnDvwaReset.Size = new System.Drawing.Size(245, 30);
            this.btnDvwaReset.TabIndex = 3;
            this.btnDvwaReset.Text = "🔄 Reset Database";
            this.btnDvwaReset.UseVisualStyleBackColor = false;
            this.btnDvwaReset.Click += new System.EventHandler(this.BtnDvwaReset_Click);

            // 
            // comboDvwaDiff
            // 
            this.comboDvwaDiff.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboDvwaDiff.FormattingEnabled = true;
            this.comboDvwaDiff.Items.AddRange(new object[] { "low", "medium", "high", "impossible" });
            this.comboDvwaDiff.Location = new System.Drawing.Point(12, 142);
            this.comboDvwaDiff.Name = "comboDvwaDiff";
            this.comboDvwaDiff.Size = new System.Drawing.Size(145, 25);
            this.comboDvwaDiff.TabIndex = 4;

            // 
            // btnDvwaSetDiff
            // 
            this.btnDvwaSetDiff.BackColor = System.Drawing.Color.DarkRed;
            this.btnDvwaSetDiff.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDvwaSetDiff.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDvwaSetDiff.ForeColor = System.Drawing.Color.White;
            this.btnDvwaSetDiff.Location = new System.Drawing.Point(163, 141);
            this.btnDvwaSetDiff.Name = "btnDvwaSetDiff";
            this.btnDvwaSetDiff.Size = new System.Drawing.Size(94, 27);
            this.btnDvwaSetDiff.TabIndex = 5;
            this.btnDvwaSetDiff.Text = "Set Level";
            this.btnDvwaSetDiff.UseVisualStyleBackColor = false;
            this.btnDvwaSetDiff.Click += new System.EventHandler(this.BtnDvwaSetDiff_Click);

            // 
            // grpJuice
            // 
            this.grpJuice.Controls.Add(this.lblJuiceStatus);
            this.grpJuice.Controls.Add(this.btnJuiceStop);
            this.grpJuice.Controls.Add(this.btnJuiceStart);
            this.grpJuice.Controls.Add(this.btnJuiceLaunch);
            this.grpJuice.Controls.Add(this.lblJuiceDesc);
            this.grpJuice.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpJuice.ForeColor = System.Drawing.Color.White;
            this.grpJuice.Location = new System.Drawing.Point(304, 18);
            this.grpJuice.Name = "grpJuice";
            this.grpJuice.Size = new System.Drawing.Size(270, 225);
            this.grpJuice.TabIndex = 1;
            this.grpJuice.TabStop = false;
            this.grpJuice.Text = "OWASP Juice Shop";

            // 
            // lblJuiceDesc
            // 
            this.lblJuiceDesc.AutoSize = true;
            this.lblJuiceDesc.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblJuiceDesc.ForeColor = System.Drawing.Color.DarkGray;
            this.lblJuiceDesc.Location = new System.Drawing.Point(12, 23);
            this.lblJuiceDesc.Name = "lblJuiceDesc";
            this.lblJuiceDesc.Size = new System.Drawing.Size(142, 15);
            this.lblJuiceDesc.TabIndex = 0;
            this.lblJuiceDesc.Text = "Node.js Docker Container";

            // 
            // lblJuiceStatus
            // 
            this.lblJuiceStatus.AutoSize = true;
            this.lblJuiceStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblJuiceStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblJuiceStatus.Location = new System.Drawing.Point(12, 42);
            this.lblJuiceStatus.Name = "lblJuiceStatus";
            this.lblJuiceStatus.Size = new System.Drawing.Size(102, 15);
            this.lblJuiceStatus.TabIndex = 1;
            this.lblJuiceStatus.Text = "Status: Checking...";

            // 
            // btnJuiceLaunch
            // 
            this.btnJuiceLaunch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnJuiceLaunch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnJuiceLaunch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJuiceLaunch.ForeColor = System.Drawing.Color.Cyan;
            this.btnJuiceLaunch.Location = new System.Drawing.Point(12, 65);
            this.btnJuiceLaunch.Name = "btnJuiceLaunch";
            this.btnJuiceLaunch.Size = new System.Drawing.Size(245, 30);
            this.btnJuiceLaunch.TabIndex = 2;
            this.btnJuiceLaunch.Text = "🚀 Launch in Browser";
            this.btnJuiceLaunch.UseVisualStyleBackColor = false;
            this.btnJuiceLaunch.Click += new System.EventHandler(this.BtnJuiceLaunch_Click);

            // 
            // btnJuiceStart
            // 
            this.btnJuiceStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnJuiceStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnJuiceStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJuiceStart.ForeColor = System.Drawing.Color.LimeGreen;
            this.btnJuiceStart.Location = new System.Drawing.Point(12, 102);
            this.btnJuiceStart.Name = "btnJuiceStart";
            this.btnJuiceStart.Size = new System.Drawing.Size(245, 30);
            this.btnJuiceStart.TabIndex = 3;
            this.btnJuiceStart.Text = "▶️ Start Docker Container";
            this.btnJuiceStart.UseVisualStyleBackColor = false;
            this.btnJuiceStart.Click += new System.EventHandler(this.BtnJuiceStart_Click);

            // 
            // btnJuiceStop
            // 
            this.btnJuiceStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnJuiceStop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnJuiceStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnJuiceStop.ForeColor = System.Drawing.Color.IndianRed;
            this.btnJuiceStop.Location = new System.Drawing.Point(12, 139);
            this.btnJuiceStop.Name = "btnJuiceStop";
            this.btnJuiceStop.Size = new System.Drawing.Size(245, 30);
            this.btnJuiceStop.TabIndex = 4;
            this.btnJuiceStop.Text = "⏹️ Stop Container";
            this.btnJuiceStop.UseVisualStyleBackColor = false;
            this.btnJuiceStop.Click += new System.EventHandler(this.BtnJuiceStop_Click);

            // 
            // grpGoat
            // 
            this.grpGoat.Controls.Add(this.lblGoatStatus);
            this.grpGoat.Controls.Add(this.btnGoatStop);
            this.grpGoat.Controls.Add(this.btnGoatStart);
            this.grpGoat.Controls.Add(this.btnGoatLaunch);
            this.grpGoat.Controls.Add(this.lblGoatDesc);
            this.grpGoat.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpGoat.ForeColor = System.Drawing.Color.White;
            this.grpGoat.Location = new System.Drawing.Point(590, 18);
            this.grpGoat.Name = "grpGoat";
            this.grpGoat.Size = new System.Drawing.Size(270, 225);
            this.grpGoat.TabIndex = 2;
            this.grpGoat.TabStop = false;
            this.grpGoat.Text = "OWASP WebGoat";

            // 
            // lblGoatDesc
            // 
            this.lblGoatDesc.AutoSize = true;
            this.lblGoatDesc.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblGoatDesc.ForeColor = System.Drawing.Color.DarkGray;
            this.lblGoatDesc.Location = new System.Drawing.Point(12, 23);
            this.lblGoatDesc.Name = "lblGoatDesc";
            this.lblGoatDesc.Size = new System.Drawing.Size(135, 15);
            this.lblGoatDesc.TabIndex = 0;
            this.lblGoatDesc.Text = "Java Docker Container";

            // 
            // lblGoatStatus
            // 
            this.lblGoatStatus.AutoSize = true;
            this.lblGoatStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblGoatStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblGoatStatus.Location = new System.Drawing.Point(12, 42);
            this.lblGoatStatus.Name = "lblGoatStatus";
            this.lblGoatStatus.Size = new System.Drawing.Size(102, 15);
            this.lblGoatStatus.TabIndex = 1;
            this.lblGoatStatus.Text = "Status: Checking...";

            // 
            // btnGoatLaunch
            // 
            this.btnGoatLaunch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnGoatLaunch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGoatLaunch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGoatLaunch.ForeColor = System.Drawing.Color.Cyan;
            this.btnGoatLaunch.Location = new System.Drawing.Point(12, 65);
            this.btnGoatLaunch.Name = "btnGoatLaunch";
            this.btnGoatLaunch.Size = new System.Drawing.Size(245, 30);
            this.btnGoatLaunch.TabIndex = 2;
            this.btnGoatLaunch.Text = "🚀 Launch in Browser";
            this.btnGoatLaunch.UseVisualStyleBackColor = false;
            this.btnGoatLaunch.Click += new System.EventHandler(this.BtnGoatLaunch_Click);

            // 
            // btnGoatStart
            // 
            this.btnGoatStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnGoatStart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGoatStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGoatStart.ForeColor = System.Drawing.Color.LimeGreen;
            this.btnGoatStart.Location = new System.Drawing.Point(12, 102);
            this.btnGoatStart.Name = "btnGoatStart";
            this.btnGoatStart.Size = new System.Drawing.Size(245, 30);
            this.btnGoatStart.TabIndex = 3;
            this.btnGoatStart.Text = "▶️ Start Docker Container";
            this.btnGoatStart.UseVisualStyleBackColor = false;
            this.btnGoatStart.Click += new System.EventHandler(this.BtnGoatStart_Click);

            // 
            // btnGoatStop
            // 
            this.btnGoatStop.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnGoatStop.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnGoatStop.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnGoatStop.ForeColor = System.Drawing.Color.IndianRed;
            this.btnGoatStop.Location = new System.Drawing.Point(12, 139);
            this.btnGoatStop.Name = "btnGoatStop";
            this.btnGoatStop.Size = new System.Drawing.Size(245, 30);
            this.btnGoatStop.TabIndex = 4;
            this.btnGoatStop.Text = "⏹️ Stop Container";
            this.btnGoatStop.UseVisualStyleBackColor = false;
            this.btnGoatStop.Click += new System.EventHandler(this.BtnGoatStop_Click);

            // 
            // grpMutillidae
            // 
            this.grpMutillidae.Controls.Add(this.lblMutillidaeStatus);
            this.grpMutillidae.Controls.Add(this.btnMutillidaeReset);
            this.grpMutillidae.Controls.Add(this.btnMutillidaeLaunch);
            this.grpMutillidae.Controls.Add(this.lblMutillidaeDesc);
            this.grpMutillidae.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpMutillidae.ForeColor = System.Drawing.Color.White;
            this.grpMutillidae.Location = new System.Drawing.Point(18, 249);
            this.grpMutillidae.Name = "grpMutillidae";
            this.grpMutillidae.Size = new System.Drawing.Size(270, 225);
            this.grpMutillidae.TabIndex = 3;
            this.grpMutillidae.TabStop = false;
            this.grpMutillidae.Text = "OWASP Mutillidae II";

            // 
            // lblMutillidaeDesc
            // 
            this.lblMutillidaeDesc.AutoSize = true;
            this.lblMutillidaeDesc.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblMutillidaeDesc.ForeColor = System.Drawing.Color.DarkGray;
            this.lblMutillidaeDesc.Location = new System.Drawing.Point(12, 23);
            this.lblMutillidaeDesc.Name = "lblMutillidaeDesc";
            this.lblMutillidaeDesc.Size = new System.Drawing.Size(163, 15);
            this.lblMutillidaeDesc.TabIndex = 0;
            this.lblMutillidaeDesc.Text = "Local PHP/MySQL Environment";

            // 
            // lblMutillidaeStatus
            // 
            this.lblMutillidaeStatus.AutoSize = true;
            this.lblMutillidaeStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblMutillidaeStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblMutillidaeStatus.Location = new System.Drawing.Point(12, 42);
            this.lblMutillidaeStatus.Name = "lblMutillidaeStatus";
            this.lblMutillidaeStatus.Size = new System.Drawing.Size(102, 15);
            this.lblMutillidaeStatus.TabIndex = 1;
            this.lblMutillidaeStatus.Text = "Status: Checking...";

            // 
            // btnMutillidaeLaunch
            // 
            this.btnMutillidaeLaunch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnMutillidaeLaunch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMutillidaeLaunch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMutillidaeLaunch.ForeColor = System.Drawing.Color.Cyan;
            this.btnMutillidaeLaunch.Location = new System.Drawing.Point(12, 65);
            this.btnMutillidaeLaunch.Name = "btnMutillidaeLaunch";
            this.btnMutillidaeLaunch.Size = new System.Drawing.Size(245, 30);
            this.btnMutillidaeLaunch.TabIndex = 2;
            this.btnMutillidaeLaunch.Text = "🚀 Launch in Browser";
            this.btnMutillidaeLaunch.UseVisualStyleBackColor = false;
            this.btnMutillidaeLaunch.Click += new System.EventHandler(this.BtnMutillidaeLaunch_Click);

            // 
            // btnMutillidaeReset
            // 
            this.btnMutillidaeReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnMutillidaeReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnMutillidaeReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMutillidaeReset.ForeColor = System.Drawing.Color.Orange;
            this.btnMutillidaeReset.Location = new System.Drawing.Point(12, 102);
            this.btnMutillidaeReset.Name = "btnMutillidaeReset";
            this.btnMutillidaeReset.Size = new System.Drawing.Size(245, 30);
            this.btnMutillidaeReset.TabIndex = 3;
            this.btnMutillidaeReset.Text = "🔄 Reset Database";
            this.btnMutillidaeReset.UseVisualStyleBackColor = false;
            this.btnMutillidaeReset.Click += new System.EventHandler(this.BtnMutillidaeReset_Click);

            // 
            // grpBwapp
            // 
            this.grpBwapp.Controls.Add(this.lblBwappStatus);
            this.grpBwapp.Controls.Add(this.btnBwappReset);
            this.grpBwapp.Controls.Add(this.btnBwappLaunch);
            this.grpBwapp.Controls.Add(this.lblBwappDesc);
            this.grpBwapp.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            this.grpBwapp.ForeColor = System.Drawing.Color.White;
            this.grpBwapp.Location = new System.Drawing.Point(304, 249);
            this.grpBwapp.Name = "grpBwapp";
            this.grpBwapp.Size = new System.Drawing.Size(270, 225);
            this.grpBwapp.TabIndex = 4;
            this.grpBwapp.TabStop = false;
            this.grpBwapp.Text = "bWAPP (Buggy Web App)";

            // 
            // lblBwappDesc
            // 
            this.lblBwappDesc.AutoSize = true;
            this.lblBwappDesc.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblBwappDesc.ForeColor = System.Drawing.Color.DarkGray;
            this.lblBwappDesc.Location = new System.Drawing.Point(12, 23);
            this.lblBwappDesc.Name = "lblBwappDesc";
            this.lblBwappDesc.Size = new System.Drawing.Size(163, 15);
            this.lblBwappDesc.TabIndex = 0;
            this.lblBwappDesc.Text = "Local PHP/MySQL Environment";

            // 
            // lblBwappStatus
            // 
            this.lblBwappStatus.AutoSize = true;
            this.lblBwappStatus.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblBwappStatus.ForeColor = System.Drawing.Color.Gray;
            this.lblBwappStatus.Location = new System.Drawing.Point(12, 42);
            this.lblBwappStatus.Name = "lblBwappStatus";
            this.lblBwappStatus.Size = new System.Drawing.Size(102, 15);
            this.lblBwappStatus.TabIndex = 1;
            this.lblBwappStatus.Text = "Status: Checking...";

            // 
            // btnBwappLaunch
            // 
            this.btnBwappLaunch.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnBwappLaunch.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBwappLaunch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBwappLaunch.ForeColor = System.Drawing.Color.Cyan;
            this.btnBwappLaunch.Location = new System.Drawing.Point(12, 65);
            this.btnBwappLaunch.Name = "btnBwappLaunch";
            this.btnBwappLaunch.Size = new System.Drawing.Size(245, 30);
            this.btnBwappLaunch.TabIndex = 2;
            this.btnBwappLaunch.Text = "🚀 Launch in Browser";
            this.btnBwappLaunch.UseVisualStyleBackColor = false;
            this.btnBwappLaunch.Click += new System.EventHandler(this.BtnBwappLaunch_Click);

            // 
            // btnBwappReset
            // 
            this.btnBwappReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.btnBwappReset.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnBwappReset.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnBwappReset.ForeColor = System.Drawing.Color.Orange;
            this.btnBwappReset.Location = new System.Drawing.Point(12, 102);
            this.btnBwappReset.Name = "btnBwappReset";
            this.btnBwappReset.Size = new System.Drawing.Size(245, 30);
            this.btnBwappReset.TabIndex = 3;
            this.btnBwappReset.Text = "🔄 Reset Database";
            this.btnBwappReset.UseVisualStyleBackColor = false;
            this.btnBwappReset.Click += new System.EventHandler(this.BtnBwappReset_Click);

            // 
            // ServerControlUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Controls.Add(this.gridFlowPanel);
            this.Controls.Add(this.pnlTopHeader);
            this.Name = "ServerControlUserControl";
            this.Size = new System.Drawing.Size(950, 550);
            this.Load += new System.EventHandler(this.ServerControlUserControl_Load);
            this.pnlTopHeader.ResumeLayout(false);
            this.pnlTopHeader.PerformLayout();
            this.gridFlowPanel.ResumeLayout(false);
            this.grpDvwa.ResumeLayout(false);
            this.grpDvwa.PerformLayout();
            this.grpJuice.ResumeLayout(false);
            this.grpJuice.PerformLayout();
            this.grpGoat.ResumeLayout(false);
            this.grpGoat.PerformLayout();
            this.grpMutillidae.ResumeLayout(false);
            this.grpMutillidae.PerformLayout();
            this.grpBwapp.ResumeLayout(false);
            this.grpBwapp.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel pnlTopHeader;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Button btnRefreshHealth;
        private System.Windows.Forms.FlowLayoutPanel gridFlowPanel;

        private System.Windows.Forms.GroupBox grpDvwa;
        private System.Windows.Forms.Label lblDvwaDesc;
        private System.Windows.Forms.Label lblDvwaStatus;
        private System.Windows.Forms.Button btnDvwaLaunch;
        private System.Windows.Forms.Button btnDvwaReset;
        private System.Windows.Forms.ComboBox comboDvwaDiff;
        private System.Windows.Forms.Button btnDvwaSetDiff;

        private System.Windows.Forms.GroupBox grpJuice;
        private System.Windows.Forms.Label lblJuiceDesc;
        private System.Windows.Forms.Label lblJuiceStatus;
        private System.Windows.Forms.Button btnJuiceLaunch;
        private System.Windows.Forms.Button btnJuiceStart;
        private System.Windows.Forms.Button btnJuiceStop;

        private System.Windows.Forms.GroupBox grpGoat;
        private System.Windows.Forms.Label lblGoatDesc;
        private System.Windows.Forms.Label lblGoatStatus;
        private System.Windows.Forms.Button btnGoatLaunch;
        private System.Windows.Forms.Button btnGoatStart;
        private System.Windows.Forms.Button btnGoatStop;

        private System.Windows.Forms.GroupBox grpMutillidae;
        private System.Windows.Forms.Label lblMutillidaeDesc;
        private System.Windows.Forms.Label lblMutillidaeStatus;
        private System.Windows.Forms.Button btnMutillidaeLaunch;
        private System.Windows.Forms.Button btnMutillidaeReset;

        private System.Windows.Forms.GroupBox grpBwapp;
        private System.Windows.Forms.Label lblBwappDesc;
        private System.Windows.Forms.Label lblBwappStatus;
        private System.Windows.Forms.Button btnBwappLaunch;
        private System.Windows.Forms.Button btnBwappReset;
    }
}