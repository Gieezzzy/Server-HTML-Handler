namespace Server_HTML_Handler.View.Dialogs
{
    partial class CreateCi4ProjectDialog
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblHeaderTitle;
        private System.Windows.Forms.Label lblHeaderSubtitle;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.GroupBox grpProject;
        private System.Windows.Forms.Label lblProjectName;
        private System.Windows.Forms.TextBox txtProjectName;
        private System.Windows.Forms.Label lblHostingPath;
        private System.Windows.Forms.TextBox txtHostingPath;
        private System.Windows.Forms.Button btnBrowseFolder;
        private System.Windows.Forms.Label lblHostIp;
        private System.Windows.Forms.TextBox txtHostIp;
        private System.Windows.Forms.Label lblEnv;
        private System.Windows.Forms.ComboBox comboEnvironment;
        private System.Windows.Forms.Label lblUrlPreview;
        private System.Windows.Forms.Label lblCalculatedUrl;
        private System.Windows.Forms.GroupBox grpDb;
        private System.Windows.Forms.Label lblDbHost;
        private System.Windows.Forms.TextBox txtDbHost;
        private System.Windows.Forms.Label lblDbPort;
        private System.Windows.Forms.TextBox txtDbPort;
        private System.Windows.Forms.Label lblDbName;
        private System.Windows.Forms.TextBox txtDbName;
        private System.Windows.Forms.CheckBox chkAutoDbName;
        private System.Windows.Forms.CheckBox chkCreateDb;
        private System.Windows.Forms.Label lblDbUser;
        private System.Windows.Forms.TextBox txtDbUser;
        private System.Windows.Forms.Label lblDbPass;
        private System.Windows.Forms.TextBox txtDbPass;
        private System.Windows.Forms.GroupBox grpOutput;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOpenBrowser;

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
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblHeaderSubtitle = new System.Windows.Forms.Label();
            this.lblHeaderTitle = new System.Windows.Forms.Label();
            this.grpProject = new System.Windows.Forms.GroupBox();
            this.lblCalculatedUrl = new System.Windows.Forms.Label();
            this.lblUrlPreview = new System.Windows.Forms.Label();
            this.comboEnvironment = new System.Windows.Forms.ComboBox();
            this.lblEnv = new System.Windows.Forms.Label();
            this.txtHostIp = new System.Windows.Forms.TextBox();
            this.lblHostIp = new System.Windows.Forms.Label();
            this.btnBrowseFolder = new System.Windows.Forms.Button();
            this.txtHostingPath = new System.Windows.Forms.TextBox();
            this.lblHostingPath = new System.Windows.Forms.Label();
            this.txtProjectName = new System.Windows.Forms.TextBox();
            this.lblProjectName = new System.Windows.Forms.Label();
            this.grpDb = new System.Windows.Forms.GroupBox();
            this.chkCreateDb = new System.Windows.Forms.CheckBox();
            this.chkAutoDbName = new System.Windows.Forms.CheckBox();
            this.txtDbPass = new System.Windows.Forms.TextBox();
            this.lblDbPass = new System.Windows.Forms.Label();
            this.txtDbUser = new System.Windows.Forms.TextBox();
            this.lblDbUser = new System.Windows.Forms.Label();
            this.txtDbName = new System.Windows.Forms.TextBox();
            this.lblDbName = new System.Windows.Forms.Label();
            this.txtDbPort = new System.Windows.Forms.TextBox();
            this.lblDbPort = new System.Windows.Forms.Label();
            this.txtDbHost = new System.Windows.Forms.TextBox();
            this.lblDbHost = new System.Windows.Forms.Label();
            this.grpOutput = new System.Windows.Forms.GroupBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.btnCreate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOpenBrowser = new System.Windows.Forms.Button();
            this.pnlHeader.SuspendLayout();
            this.grpProject.SuspendLayout();
            this.grpDb.SuspendLayout();
            this.grpOutput.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(77)))), ((int)(((byte)(45)))));
            this.pnlHeader.Controls.Add(this.lblHeaderSubtitle);
            this.pnlHeader.Controls.Add(this.lblHeaderTitle);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(764, 65);
            this.pnlHeader.TabIndex = 0;
            // 
            // lblHeaderSubtitle
            // 
            this.lblHeaderSubtitle.AutoSize = true;
            this.lblHeaderSubtitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderSubtitle.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.lblHeaderSubtitle.Location = new System.Drawing.Point(18, 38);
            this.lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            this.lblHeaderSubtitle.Size = new System.Drawing.Size(434, 15);
            this.lblHeaderSubtitle.TabIndex = 1;
            this.lblHeaderSubtitle.Text = "Automates Composer scaffolding, database setup, .env parameters, and rewrites.";
            // 
            // lblHeaderTitle
            // 
            this.lblHeaderTitle.AutoSize = true;
            this.lblHeaderTitle.Font = new System.Drawing.Font("Segoe UI", 13.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHeaderTitle.ForeColor = System.Drawing.Color.White;
            this.lblHeaderTitle.Location = new System.Drawing.Point(16, 11);
            this.lblHeaderTitle.Name = "lblHeaderTitle";
            this.lblHeaderTitle.Size = new System.Drawing.Size(325, 25);
            this.lblHeaderTitle.TabIndex = 0;
            this.lblHeaderTitle.Text = "🔥 Create CodeIgniter 4 (CI4) Project";
            // 
            // grpProject
            // 
            this.grpProject.Controls.Add(this.lblCalculatedUrl);
            this.grpProject.Controls.Add(this.lblUrlPreview);
            this.grpProject.Controls.Add(this.comboEnvironment);
            this.grpProject.Controls.Add(this.lblEnv);
            this.grpProject.Controls.Add(this.txtHostIp);
            this.grpProject.Controls.Add(this.lblHostIp);
            this.grpProject.Controls.Add(this.btnBrowseFolder);
            this.grpProject.Controls.Add(this.txtHostingPath);
            this.grpProject.Controls.Add(this.lblHostingPath);
            this.grpProject.Controls.Add(this.txtProjectName);
            this.grpProject.Controls.Add(this.lblProjectName);
            this.grpProject.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpProject.Location = new System.Drawing.Point(18, 77);
            this.grpProject.Name = "grpProject";
            this.grpProject.Size = new System.Drawing.Size(728, 142);
            this.grpProject.TabIndex = 1;
            this.grpProject.TabStop = false;
            this.grpProject.Text = "Project & Hosting Details";
            // 
            // lblCalculatedUrl
            // 
            this.lblCalculatedUrl.AutoSize = true;
            this.lblCalculatedUrl.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCalculatedUrl.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(128)))), ((int)(((byte)(185)))));
            this.lblCalculatedUrl.Location = new System.Drawing.Point(145, 115);
            this.lblCalculatedUrl.Name = "lblCalculatedUrl";
            this.lblCalculatedUrl.Size = new System.Drawing.Size(183, 15);
            this.lblCalculatedUrl.TabIndex = 10;
            this.lblCalculatedUrl.Text = "http://192.168.32.43/my-system/";
            // 
            // lblUrlPreview
            // 
            this.lblUrlPreview.AutoSize = true;
            this.lblUrlPreview.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblUrlPreview.ForeColor = System.Drawing.Color.DimGray;
            this.lblUrlPreview.Location = new System.Drawing.Point(16, 115);
            this.lblUrlPreview.Name = "lblUrlPreview";
            this.lblUrlPreview.Size = new System.Drawing.Size(117, 15);
            this.lblUrlPreview.TabIndex = 9;
            this.lblUrlPreview.Text = "Ready-to-Access URL:";
            // 
            // comboEnvironment
            // 
            this.comboEnvironment.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboEnvironment.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.comboEnvironment.FormattingEnabled = true;
            this.comboEnvironment.Items.AddRange(new object[] {
            "development",
            "production",
            "testing"});
            this.comboEnvironment.Location = new System.Drawing.Point(546, 82);
            this.comboEnvironment.Name = "comboEnvironment";
            this.comboEnvironment.Size = new System.Drawing.Size(164, 21);
            this.comboEnvironment.TabIndex = 8;
            // 
            // lblEnv
            // 
            this.lblEnv.AutoSize = true;
            this.lblEnv.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblEnv.Location = new System.Drawing.Point(463, 86);
            this.lblEnv.Name = "lblEnv";
            this.lblEnv.Size = new System.Drawing.Size(76, 13);
            this.lblEnv.TabIndex = 7;
            this.lblEnv.Text = "Environment:";
            // 
            // txtHostIp
            // 
            this.txtHostIp.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txtHostIp.Location = new System.Drawing.Point(148, 83);
            this.txtHostIp.Name = "txtHostIp";
            this.txtHostIp.Size = new System.Drawing.Size(262, 22);
            this.txtHostIp.TabIndex = 6;
            this.txtHostIp.Text = "192.168.32.43";
            this.txtHostIp.TextChanged += new System.EventHandler(this.TxtHostIp_TextChanged);
            // 
            // lblHostIp
            // 
            this.lblHostIp.AutoSize = true;
            this.lblHostIp.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblHostIp.Location = new System.Drawing.Point(16, 86);
            this.lblHostIp.Name = "lblHostIp";
            this.lblHostIp.Size = new System.Drawing.Size(117, 13);
            this.lblHostIp.TabIndex = 5;
            this.lblHostIp.Text = "Server IP / Hostname:";
            // 
            // btnBrowseFolder
            // 
            this.btnBrowseFolder.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.btnBrowseFolder.Location = new System.Drawing.Point(636, 52);
            this.btnBrowseFolder.Name = "btnBrowseFolder";
            this.btnBrowseFolder.Size = new System.Drawing.Size(74, 24);
            this.btnBrowseFolder.TabIndex = 4;
            this.btnBrowseFolder.Text = "Browse...";
            this.btnBrowseFolder.UseVisualStyleBackColor = true;
            this.btnBrowseFolder.Click += new System.EventHandler(this.BtnBrowseFolder_Click);
            // 
            // txtHostingPath
            // 
            this.txtHostingPath.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txtHostingPath.Location = new System.Drawing.Point(148, 53);
            this.txtHostingPath.Name = "txtHostingPath";
            this.txtHostingPath.Size = new System.Drawing.Size(480, 22);
            this.txtHostingPath.TabIndex = 3;
            this.txtHostingPath.Text = "D:\\html";
            // 
            // lblHostingPath
            // 
            this.lblHostingPath.AutoSize = true;
            this.lblHostingPath.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblHostingPath.Location = new System.Drawing.Point(16, 56);
            this.lblHostingPath.Name = "lblHostingPath";
            this.lblHostingPath.Size = new System.Drawing.Size(85, 13);
            this.lblHostingPath.TabIndex = 2;
            this.lblHostingPath.Text = "Hosting Folder:";
            // 
            // txtProjectName
            // 
            this.txtProjectName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.txtProjectName.Location = new System.Drawing.Point(148, 22);
            this.txtProjectName.Name = "txtProjectName";
            this.txtProjectName.Size = new System.Drawing.Size(562, 23);
            this.txtProjectName.TabIndex = 1;
            this.txtProjectName.TextChanged += new System.EventHandler(this.TxtProjectName_TextChanged);
            // 
            // lblProjectName
            // 
            this.lblProjectName.AutoSize = true;
            this.lblProjectName.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblProjectName.Location = new System.Drawing.Point(16, 26);
            this.lblProjectName.Name = "lblProjectName";
            this.lblProjectName.Size = new System.Drawing.Size(124, 13);
            this.lblProjectName.TabIndex = 0;
            this.lblProjectName.Text = "Project / Folder Name:";
            // 
            // grpDb
            // 
            this.grpDb.Controls.Add(this.chkCreateDb);
            this.grpDb.Controls.Add(this.chkAutoDbName);
            this.grpDb.Controls.Add(this.txtDbPass);
            this.grpDb.Controls.Add(this.lblDbPass);
            this.grpDb.Controls.Add(this.txtDbUser);
            this.grpDb.Controls.Add(this.lblDbUser);
            this.grpDb.Controls.Add(this.txtDbName);
            this.grpDb.Controls.Add(this.lblDbName);
            this.grpDb.Controls.Add(this.txtDbPort);
            this.grpDb.Controls.Add(this.lblDbPort);
            this.grpDb.Controls.Add(this.txtDbHost);
            this.grpDb.Controls.Add(this.lblDbHost);
            this.grpDb.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpDb.Location = new System.Drawing.Point(18, 226);
            this.grpDb.Name = "grpDb";
            this.grpDb.Size = new System.Drawing.Size(728, 120);
            this.grpDb.TabIndex = 2;
            this.grpDb.TabStop = false;
            this.grpDb.Text = "Database Setup (MySQL / MariaDB)";
            // 
            // chkCreateDb
            // 
            this.chkCreateDb.AutoSize = true;
            this.chkCreateDb.Checked = true;
            this.chkCreateDb.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkCreateDb.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkCreateDb.Location = new System.Drawing.Point(466, 88);
            this.chkCreateDb.Name = "chkCreateDb";
            this.chkCreateDb.Size = new System.Drawing.Size(242, 17);
            this.chkCreateDb.TabIndex = 11;
            this.chkCreateDb.Text = "Auto-create database in MySQL if missing";
            this.chkCreateDb.UseVisualStyleBackColor = true;
            // 
            // chkAutoDbName
            // 
            this.chkAutoDbName.AutoSize = true;
            this.chkAutoDbName.Checked = true;
            this.chkAutoDbName.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkAutoDbName.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.chkAutoDbName.Location = new System.Drawing.Point(466, 58);
            this.chkAutoDbName.Name = "chkAutoDbName";
            this.chkAutoDbName.Size = new System.Drawing.Size(183, 17);
            this.chkAutoDbName.TabIndex = 10;
            this.chkAutoDbName.Text = "Sync DB name to project name";
            this.chkAutoDbName.UseVisualStyleBackColor = true;
            // 
            // txtDbPass
            // 
            this.txtDbPass.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txtDbPass.Location = new System.Drawing.Point(308, 86);
            this.txtDbPass.Name = "txtDbPass";
            this.txtDbPass.Size = new System.Drawing.Size(130, 22);
            this.txtDbPass.TabIndex = 9;
            this.txtDbPass.UseSystemPasswordChar = true;
            // 
            // lblDbPass
            // 
            this.lblDbPass.AutoSize = true;
            this.lblDbPass.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblDbPass.Location = new System.Drawing.Point(234, 89);
            this.lblDbPass.Name = "lblDbPass";
            this.lblDbPass.Size = new System.Drawing.Size(59, 13);
            this.lblDbPass.TabIndex = 8;
            this.lblDbPass.Text = "Password:";
            // 
            // txtDbUser
            // 
            this.txtDbUser.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txtDbUser.Location = new System.Drawing.Point(92, 86);
            this.txtDbUser.Name = "txtDbUser";
            this.txtDbUser.Size = new System.Drawing.Size(124, 22);
            this.txtDbUser.TabIndex = 7;
            this.txtDbUser.Text = "root";
            // 
            // lblDbUser
            // 
            this.lblDbUser.AutoSize = true;
            this.lblDbUser.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblDbUser.Location = new System.Drawing.Point(16, 89);
            this.lblDbUser.Name = "lblDbUser";
            this.lblDbUser.Size = new System.Drawing.Size(61, 13);
            this.lblDbUser.TabIndex = 6;
            this.lblDbUser.Text = "Username:";
            // 
            // txtDbName
            // 
            this.txtDbName.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txtDbName.Location = new System.Drawing.Point(92, 56);
            this.txtDbName.Name = "txtDbName";
            this.txtDbName.Size = new System.Drawing.Size(346, 22);
            this.txtDbName.TabIndex = 5;
            // 
            // lblDbName
            // 
            this.lblDbName.AutoSize = true;
            this.lblDbName.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblDbName.Location = new System.Drawing.Point(16, 59);
            this.lblDbName.Name = "lblDbName";
            this.lblDbName.Size = new System.Drawing.Size(58, 13);
            this.lblDbName.TabIndex = 4;
            this.lblDbName.Text = "DB Name:";
            // 
            // txtDbPort
            // 
            this.txtDbPort.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txtDbPort.Location = new System.Drawing.Point(378, 26);
            this.txtDbPort.Name = "txtDbPort";
            this.txtDbPort.Size = new System.Drawing.Size(60, 22);
            this.txtDbPort.TabIndex = 3;
            this.txtDbPort.Text = "3306";
            // 
            // lblDbPort
            // 
            this.lblDbPort.AutoSize = true;
            this.lblDbPort.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblDbPort.Location = new System.Drawing.Point(336, 29);
            this.lblDbPort.Name = "lblDbPort";
            this.lblDbPort.Size = new System.Drawing.Size(31, 13);
            this.lblDbPort.TabIndex = 2;
            this.lblDbPort.Text = "Port:";
            // 
            // txtDbHost
            // 
            this.txtDbHost.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.txtDbHost.Location = new System.Drawing.Point(92, 26);
            this.txtDbHost.Name = "txtDbHost";
            this.txtDbHost.Size = new System.Drawing.Size(228, 22);
            this.txtDbHost.TabIndex = 1;
            this.txtDbHost.Text = "localhost";
            // 
            // lblDbHost
            // 
            this.lblDbHost.AutoSize = true;
            this.lblDbHost.Font = new System.Drawing.Font("Segoe UI", 8.25F);
            this.lblDbHost.Location = new System.Drawing.Point(16, 29);
            this.lblDbHost.Name = "lblDbHost";
            this.lblDbHost.Size = new System.Drawing.Size(52, 13);
            this.lblDbHost.TabIndex = 0;
            this.lblDbHost.Text = "DB Host:";
            // 
            // grpOutput
            // 
            this.grpOutput.Controls.Add(this.txtLog);
            this.grpOutput.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grpOutput.Location = new System.Drawing.Point(18, 352);
            this.grpOutput.Name = "grpOutput";
            this.grpOutput.Size = new System.Drawing.Size(728, 160);
            this.grpOutput.TabIndex = 3;
            this.grpOutput.TabStop = false;
            this.grpOutput.Text = "Setup & Installation Output";
            // 
            // txtLog
            // 
            this.txtLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Font = new System.Drawing.Font("Consolas", 8.5F);
            this.txtLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(220)))), ((int)(((byte)(220)))));
            this.txtLog.Location = new System.Drawing.Point(3, 19);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtLog.Size = new System.Drawing.Size(722, 138);
            this.txtLog.TabIndex = 0;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(18, 518);
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(728, 10);
            this.progressBar.TabIndex = 4;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ForeColor = System.Drawing.Color.DimGray;
            this.lblStatus.Location = new System.Drawing.Point(18, 536);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 15);
            this.lblStatus.TabIndex = 5;
            this.lblStatus.Text = "Ready";
            // 
            // btnCreate
            // 
            this.btnCreate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(77)))), ((int)(((byte)(45)))));
            this.btnCreate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnCreate.FlatAppearance.BorderSize = 0;
            this.btnCreate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCreate.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCreate.ForeColor = System.Drawing.Color.White;
            this.btnCreate.Location = new System.Drawing.Point(606, 535);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(140, 36);
            this.btnCreate.TabIndex = 6;
            this.btnCreate.Text = "Create CI4 Project";
            this.btnCreate.UseVisualStyleBackColor = false;
            this.btnCreate.Click += new System.EventHandler(this.BtnCreate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Location = new System.Drawing.Point(516, 535);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(84, 36);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOpenBrowser
            // 
            this.btnOpenBrowser.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnOpenBrowser.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenBrowser.FlatAppearance.BorderSize = 0;
            this.btnOpenBrowser.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenBrowser.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOpenBrowser.ForeColor = System.Drawing.Color.White;
            this.btnOpenBrowser.Location = new System.Drawing.Point(366, 535);
            this.btnOpenBrowser.Name = "btnOpenBrowser";
            this.btnOpenBrowser.Size = new System.Drawing.Size(144, 36);
            this.btnOpenBrowser.TabIndex = 8;
            this.btnOpenBrowser.Text = "🌐 Launch in Browser";
            this.btnOpenBrowser.UseVisualStyleBackColor = false;
            this.btnOpenBrowser.Visible = false;
            this.btnOpenBrowser.Click += new System.EventHandler(this.BtnOpenBrowser_Click);
            // 
            // CreateCi4ProjectDialog
            // 
            this.AcceptButton = this.btnCreate;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(250)))), ((int)(((byte)(250)))), ((int)(((byte)(250)))));
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(764, 585);
            this.Controls.Add(this.btnOpenBrowser);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.grpOutput);
            this.Controls.Add(this.grpDb);
            this.Controls.Add(this.grpProject);
            this.Controls.Add(this.pnlHeader);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CreateCi4ProjectDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Create New CodeIgniter 4 Project";
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.grpProject.ResumeLayout(false);
            this.grpProject.PerformLayout();
            this.grpDb.ResumeLayout(false);
            this.grpDb.PerformLayout();
            this.grpOutput.ResumeLayout(false);
            this.grpOutput.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}
