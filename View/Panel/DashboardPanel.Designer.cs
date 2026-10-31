namespace Server_HTML_Handler.View.Panel
{
    partial class DashboardPanel
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private System.Windows.Forms.Panel pnlSidebar;
        private System.Windows.Forms.Panel pnlContent;
        private System.Windows.Forms.Label lblBrand;
        private System.Windows.Forms.Button btnDashboard;
        private System.Windows.Forms.Button btnServerControl;
        private System.Windows.Forms.Button btnProjectViews;
        private System.Windows.Forms.Button btnCi4Creator;
        private System.Windows.Forms.Button btnCodeEditor;
        private System.Windows.Forms.Button btnConsole;
        private System.Windows.Forms.Button btnFullServerControl;
        private System.Windows.Forms.Button btnCallServer;
        private System.Windows.Forms.Button btnNotifications;
        private System.Windows.Forms.Button btnLogout;
        private System.Windows.Forms.NotifyIcon notifyIconServer;
        private System.Windows.Forms.ContextMenuStrip trayContextMenu;
        private System.Windows.Forms.ToolStripMenuItem trayItemOpen;
        private System.Windows.Forms.ToolStripMenuItem trayItemCallServer;
        private System.Windows.Forms.ToolStripSeparator traySeparator;
        private System.Windows.Forms.ToolStripMenuItem trayItemExit;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pnlSidebar = new System.Windows.Forms.Panel();
            this.lblBrand = new System.Windows.Forms.Label();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.btnServerControl = new System.Windows.Forms.Button();
            this.btnProjectViews = new System.Windows.Forms.Button();
            this.btnCi4Creator = new System.Windows.Forms.Button();
            this.btnCodeEditor = new System.Windows.Forms.Button();
            this.btnConsole = new System.Windows.Forms.Button();
            this.btnFullServerControl = new System.Windows.Forms.Button();
            this.btnCallServer = new System.Windows.Forms.Button();
            this.btnNotifications = new System.Windows.Forms.Button();
            this.btnLogout = new System.Windows.Forms.Button();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.notifyIconServer = new System.Windows.Forms.NotifyIcon(this.components);
            this.trayContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.trayItemOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.trayItemCallServer = new System.Windows.Forms.ToolStripMenuItem();
            this.traySeparator = new System.Windows.Forms.ToolStripSeparator();
            this.trayItemExit = new System.Windows.Forms.ToolStripMenuItem();
            this.pnlSidebar.SuspendLayout();
            this.trayContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.pnlSidebar.Controls.Add(this.btnLogout);
            this.pnlSidebar.Controls.Add(this.btnNotifications);
            this.pnlSidebar.Controls.Add(this.btnCallServer);
            this.pnlSidebar.Controls.Add(this.btnFullServerControl);
            this.pnlSidebar.Controls.Add(this.btnConsole);
            this.pnlSidebar.Controls.Add(this.btnCodeEditor);
            this.pnlSidebar.Controls.Add(this.btnCi4Creator);
            this.pnlSidebar.Controls.Add(this.btnProjectViews);
            this.pnlSidebar.Controls.Add(this.btnServerControl);
            this.pnlSidebar.Controls.Add(this.btnDashboard);
            this.pnlSidebar.Controls.Add(this.lblBrand);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Location = new System.Drawing.Point(0, 0);
            this.pnlSidebar.Margin = new System.Windows.Forms.Padding(2);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(185, 652);
            this.pnlSidebar.TabIndex = 0;
            // 
            // lblBrand
            // 
            this.lblBrand.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblBrand.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblBrand.ForeColor = System.Drawing.Color.White;
            this.lblBrand.Location = new System.Drawing.Point(0, 0);
            this.lblBrand.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(185, 49);
            this.lblBrand.TabIndex = 0;
            this.lblBrand.Text = "Web Management";
            this.lblBrand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btnDashboard
            // 
            this.btnDashboard.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnDashboard.FlatAppearance.BorderSize = 0;
            this.btnDashboard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDashboard.ForeColor = System.Drawing.Color.White;
            this.btnDashboard.Location = new System.Drawing.Point(0, 49);
            this.btnDashboard.Margin = new System.Windows.Forms.Padding(2);
            this.btnDashboard.Name = "btnDashboard";
            this.btnDashboard.Size = new System.Drawing.Size(185, 37);
            this.btnDashboard.TabIndex = 1;
            this.btnDashboard.Text = "📊 Dashboard";
            this.btnDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDashboard.UseVisualStyleBackColor = true;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            // 
            // btnServerControl
            // 
            this.btnServerControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnServerControl.FlatAppearance.BorderSize = 0;
            this.btnServerControl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnServerControl.ForeColor = System.Drawing.Color.White;
            this.btnServerControl.Location = new System.Drawing.Point(0, 86);
            this.btnServerControl.Margin = new System.Windows.Forms.Padding(2);
            this.btnServerControl.Name = "btnServerControl";
            this.btnServerControl.Size = new System.Drawing.Size(185, 37);
            this.btnServerControl.TabIndex = 2;
            this.btnServerControl.Text = "🛡️ Server Control / Labs";
            this.btnServerControl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnServerControl.UseVisualStyleBackColor = true;
            this.btnServerControl.Click += new System.EventHandler(this.btnServerControl_Click);
            // 
            // btnProjectViews
            // 
            this.btnProjectViews.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnProjectViews.FlatAppearance.BorderSize = 0;
            this.btnProjectViews.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnProjectViews.ForeColor = System.Drawing.Color.Cyan;
            this.btnProjectViews.Location = new System.Drawing.Point(0, 123);
            this.btnProjectViews.Margin = new System.Windows.Forms.Padding(2);
            this.btnProjectViews.Name = "btnProjectViews";
            this.btnProjectViews.Size = new System.Drawing.Size(185, 37);
            this.btnProjectViews.TabIndex = 3;
            this.btnProjectViews.Text = "📁 HTML / CI4 Views";
            this.btnProjectViews.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnProjectViews.UseVisualStyleBackColor = true;
            this.btnProjectViews.Click += new System.EventHandler(this.btnProjectViews_Click);
            // 
            // btnCi4Creator
            // 
            this.btnCi4Creator.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCi4Creator.FlatAppearance.BorderSize = 0;
            this.btnCi4Creator.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCi4Creator.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCi4Creator.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(140)))), ((int)(((byte)(0)))));
            this.btnCi4Creator.Location = new System.Drawing.Point(0, 160);
            this.btnCi4Creator.Margin = new System.Windows.Forms.Padding(2);
            this.btnCi4Creator.Name = "btnCi4Creator";
            this.btnCi4Creator.Size = new System.Drawing.Size(185, 37);
            this.btnCi4Creator.TabIndex = 4;
            this.btnCi4Creator.Text = "🔥 Create CI4 Project";
            this.btnCi4Creator.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCi4Creator.UseVisualStyleBackColor = true;
            this.btnCi4Creator.Click += new System.EventHandler(this.btnCi4Creator_Click);
            // 
            // btnCodeEditor
            // 
            this.btnCodeEditor.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCodeEditor.FlatAppearance.BorderSize = 0;
            this.btnCodeEditor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCodeEditor.ForeColor = System.Drawing.Color.White;
            this.btnCodeEditor.Location = new System.Drawing.Point(0, 197);
            this.btnCodeEditor.Margin = new System.Windows.Forms.Padding(2);
            this.btnCodeEditor.Name = "btnCodeEditor";
            this.btnCodeEditor.Size = new System.Drawing.Size(185, 37);
            this.btnCodeEditor.TabIndex = 5;
            this.btnCodeEditor.Text = "📝 Quick Code Editor";
            this.btnCodeEditor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCodeEditor.UseVisualStyleBackColor = true;
            this.btnCodeEditor.Click += new System.EventHandler(this.btnCodeEditor_Click);
            // 
            // btnConsole
            // 
            this.btnConsole.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnConsole.FlatAppearance.BorderSize = 0;
            this.btnConsole.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConsole.ForeColor = System.Drawing.Color.White;
            this.btnConsole.Location = new System.Drawing.Point(0, 234);
            this.btnConsole.Margin = new System.Windows.Forms.Padding(2);
            this.btnConsole.Name = "btnConsole";
            this.btnConsole.Size = new System.Drawing.Size(185, 37);
            this.btnConsole.TabIndex = 6;
            this.btnConsole.Text = "💻 CLI Console";
            this.btnConsole.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConsole.UseVisualStyleBackColor = true;
            this.btnConsole.Click += new System.EventHandler(this.btnConsole_Click);
            // 
            // btnFullServerControl
            // 
            this.btnFullServerControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnFullServerControl.FlatAppearance.BorderSize = 0;
            this.btnFullServerControl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFullServerControl.ForeColor = System.Drawing.Color.White;
            this.btnFullServerControl.Location = new System.Drawing.Point(0, 271);
            this.btnFullServerControl.Margin = new System.Windows.Forms.Padding(2);
            this.btnFullServerControl.Name = "btnFullServerControl";
            this.btnFullServerControl.Size = new System.Drawing.Size(185, 37);
            this.btnFullServerControl.TabIndex = 7;
            this.btnFullServerControl.Text = "🛡️ Full Server Control";
            this.btnFullServerControl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFullServerControl.UseVisualStyleBackColor = true;
            this.btnFullServerControl.Click += new System.EventHandler(this.btnFullServerControl_Click);
            // 
            // btnNotifications
            // 
            this.btnNotifications.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnNotifications.FlatAppearance.BorderSize = 0;
            this.btnNotifications.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNotifications.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNotifications.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(196)))), ((int)(((byte)(15)))));
            this.btnNotifications.Location = new System.Drawing.Point(0, 345);
            this.btnNotifications.Margin = new System.Windows.Forms.Padding(2);
            this.btnNotifications.Name = "btnNotifications";
            this.btnNotifications.Size = new System.Drawing.Size(185, 37);
            this.btnNotifications.TabIndex = 9;
            this.btnNotifications.Text = "🔔 Live Notifications";
            this.btnNotifications.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnNotifications.UseVisualStyleBackColor = true;
            this.btnNotifications.Click += new System.EventHandler(this.btnNotifications_Click);
            // 
            // btnCallServer
            // 
            this.btnCallServer.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCallServer.FlatAppearance.BorderSize = 0;
            this.btnCallServer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCallServer.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCallServer.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(46)))), ((int)(((byte)(204)))), ((int)(((byte)(113)))));
            this.btnCallServer.Location = new System.Drawing.Point(0, 308);
            this.btnCallServer.Margin = new System.Windows.Forms.Padding(2);
            this.btnCallServer.Name = "btnCallServer";
            this.btnCallServer.Size = new System.Drawing.Size(185, 37);
            this.btnCallServer.TabIndex = 8;
            this.btnCallServer.Text = "📞 Local Call Server";
            this.btnCallServer.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCallServer.UseVisualStyleBackColor = true;
            this.btnCallServer.Click += new System.EventHandler(this.btnCallServer_Click);
            // 
            // btnLogout
            // 
            this.btnLogout.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnLogout.FlatAppearance.BorderSize = 0;
            this.btnLogout.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnLogout.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.btnLogout.Location = new System.Drawing.Point(0, 615);
            this.btnLogout.Margin = new System.Windows.Forms.Padding(2);
            this.btnLogout.Name = "btnLogout";
            this.btnLogout.Size = new System.Drawing.Size(185, 37);
            this.btnLogout.TabIndex = 9;
            this.btnLogout.Text = "🚪 Log Out";
            this.btnLogout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // pnlContent
            // 
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(185, 0);
            this.pnlContent.Margin = new System.Windows.Forms.Padding(2);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(890, 652);
            this.pnlContent.TabIndex = 1;
            // 
            // notifyIconServer
            // 
            this.notifyIconServer.ContextMenuStrip = this.trayContextMenu;
            this.notifyIconServer.Text = "Server HTML Handler & Call Server";
            this.notifyIconServer.Visible = false;
            this.notifyIconServer.DoubleClick += new System.EventHandler(this.notifyIconServer_DoubleClick);
            // 
            // trayContextMenu
            // 
            this.trayContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.trayItemOpen,
            this.trayItemCallServer,
            this.traySeparator,
            this.trayItemExit});
            this.trayContextMenu.Name = "trayContextMenu";
            this.trayContextMenu.Size = new System.Drawing.Size(185, 76);
            // 
            // trayItemOpen
            // 
            this.trayItemOpen.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.trayItemOpen.Name = "trayItemOpen";
            this.trayItemOpen.Size = new System.Drawing.Size(184, 22);
            this.trayItemOpen.Text = "Open Dashboard";
            this.trayItemOpen.Click += new System.EventHandler(this.trayItemOpen_Click);
            // 
            // trayItemCallServer
            // 
            this.trayItemCallServer.Name = "trayItemCallServer";
            this.trayItemCallServer.Size = new System.Drawing.Size(184, 22);
            this.trayItemCallServer.Text = "Call Server: Starting...";
            this.trayItemCallServer.Click += new System.EventHandler(this.trayItemCallServer_Click);
            // 
            // traySeparator
            // 
            this.traySeparator.Name = "traySeparator";
            this.traySeparator.Size = new System.Drawing.Size(181, 6);
            // 
            // trayItemExit
            // 
            this.trayItemExit.Name = "trayItemExit";
            this.trayItemExit.Size = new System.Drawing.Size(184, 22);
            this.trayItemExit.Text = "Exit Application";
            this.trayItemExit.Click += new System.EventHandler(this.trayItemExit_Click);
            // 
            // DashboardPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1075, 652);
            this.Controls.Add(this.pnlContent);
            this.Controls.Add(this.pnlSidebar);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "DashboardPanel";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Web Management System - Dashboard";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DashboardPanel_FormClosing);
            this.Load += new System.EventHandler(this.DashboardPanel_Load);
            this.pnlSidebar.ResumeLayout(false);
            this.trayContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion
    }
}
