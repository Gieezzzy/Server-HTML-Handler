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
        private System.Windows.Forms.Button btnCodeEditor;
        private System.Windows.Forms.Button btnConsole;
        private System.Windows.Forms.Button btnLogout;

        private void InitializeComponent()
        {
            this.pnlSidebar = new System.Windows.Forms.Panel();
            this.btnLogout = new System.Windows.Forms.Button();
            this.btnConsole = new System.Windows.Forms.Button();
            this.btnCodeEditor = new System.Windows.Forms.Button();
            this.btnProjectViews = new System.Windows.Forms.Button();
            this.btnServerControl = new System.Windows.Forms.Button();
            this.btnDashboard = new System.Windows.Forms.Button();
            this.lblBrand = new System.Windows.Forms.Label();
            this.pnlContent = new System.Windows.Forms.Panel();
            this.btnFullServerControl = new System.Windows.Forms.Button();
            this.pnlSidebar.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlSidebar
            // 
            this.pnlSidebar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(33)))), ((int)(((byte)(37)))), ((int)(((byte)(41)))));
            this.pnlSidebar.Controls.Add(this.btnFullServerControl);
            this.pnlSidebar.Controls.Add(this.btnLogout);
            this.pnlSidebar.Controls.Add(this.btnConsole);
            this.pnlSidebar.Controls.Add(this.btnCodeEditor);
            this.pnlSidebar.Controls.Add(this.btnProjectViews);
            this.pnlSidebar.Controls.Add(this.btnServerControl);
            this.pnlSidebar.Controls.Add(this.btnDashboard);
            this.pnlSidebar.Controls.Add(this.lblBrand);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Location = new System.Drawing.Point(0, 0);
            this.pnlSidebar.Margin = new System.Windows.Forms.Padding(2);
            this.pnlSidebar.Name = "pnlSidebar";
            this.pnlSidebar.Size = new System.Drawing.Size(175, 652);
            this.pnlSidebar.TabIndex = 0;
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
            this.btnLogout.Size = new System.Drawing.Size(175, 37);
            this.btnLogout.TabIndex = 6;
            this.btnLogout.Text = "🚪 Log Out";
            this.btnLogout.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnLogout.UseVisualStyleBackColor = true;
            this.btnLogout.Click += new System.EventHandler(this.btnLogout_Click);
            // 
            // btnConsole
            // 
            this.btnConsole.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnConsole.FlatAppearance.BorderSize = 0;
            this.btnConsole.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnConsole.ForeColor = System.Drawing.Color.White;
            this.btnConsole.Location = new System.Drawing.Point(0, 197);
            this.btnConsole.Margin = new System.Windows.Forms.Padding(2);
            this.btnConsole.Name = "btnConsole";
            this.btnConsole.Size = new System.Drawing.Size(175, 37);
            this.btnConsole.TabIndex = 5;
            this.btnConsole.Text = "💻 CLI Console";
            this.btnConsole.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnConsole.UseVisualStyleBackColor = true;
            this.btnConsole.Click += new System.EventHandler(this.btnConsole_Click);
            // 
            // btnCodeEditor
            // 
            this.btnCodeEditor.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnCodeEditor.FlatAppearance.BorderSize = 0;
            this.btnCodeEditor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCodeEditor.ForeColor = System.Drawing.Color.White;
            this.btnCodeEditor.Location = new System.Drawing.Point(0, 160);
            this.btnCodeEditor.Margin = new System.Windows.Forms.Padding(2);
            this.btnCodeEditor.Name = "btnCodeEditor";
            this.btnCodeEditor.Size = new System.Drawing.Size(175, 37);
            this.btnCodeEditor.TabIndex = 4;
            this.btnCodeEditor.Text = "📝 Quick Code Editor";
            this.btnCodeEditor.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCodeEditor.UseVisualStyleBackColor = true;
            this.btnCodeEditor.Click += new System.EventHandler(this.btnCodeEditor_Click);
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
            this.btnProjectViews.Size = new System.Drawing.Size(175, 37);
            this.btnProjectViews.TabIndex = 3;
            this.btnProjectViews.Text = "📁 HTML / CI4 Views";
            this.btnProjectViews.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnProjectViews.UseVisualStyleBackColor = true;
            this.btnProjectViews.Click += new System.EventHandler(this.btnProjectViews_Click);
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
            this.btnServerControl.Size = new System.Drawing.Size(175, 37);
            this.btnServerControl.TabIndex = 2;
            this.btnServerControl.Text = "🛡️ Server Control / Labs";
            this.btnServerControl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnServerControl.UseVisualStyleBackColor = true;
            this.btnServerControl.Click += new System.EventHandler(this.btnServerControl_Click);
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
            this.btnDashboard.Size = new System.Drawing.Size(175, 37);
            this.btnDashboard.TabIndex = 1;
            this.btnDashboard.Text = "📊 Dashboard";
            this.btnDashboard.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnDashboard.UseVisualStyleBackColor = true;
            this.btnDashboard.Click += new System.EventHandler(this.btnDashboard_Click);
            // 
            // lblBrand
            // 
            this.lblBrand.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblBrand.Font = new System.Drawing.Font("Microsoft Sans Serif", 10.5F, System.Drawing.FontStyle.Bold);
            this.lblBrand.ForeColor = System.Drawing.Color.White;
            this.lblBrand.Location = new System.Drawing.Point(0, 0);
            this.lblBrand.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblBrand.Name = "lblBrand";
            this.lblBrand.Size = new System.Drawing.Size(175, 49);
            this.lblBrand.TabIndex = 0;
            this.lblBrand.Text = "Web Management";
            this.lblBrand.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pnlContent
            // 
            this.pnlContent.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContent.Location = new System.Drawing.Point(175, 0);
            this.pnlContent.Margin = new System.Windows.Forms.Padding(2);
            this.pnlContent.Name = "pnlContent";
            this.pnlContent.Size = new System.Drawing.Size(900, 652);
            this.pnlContent.TabIndex = 1;
            // 
            // btnFullServerControl
            // 
            this.btnFullServerControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.btnFullServerControl.FlatAppearance.BorderSize = 0;
            this.btnFullServerControl.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnFullServerControl.ForeColor = System.Drawing.Color.White;
            this.btnFullServerControl.Location = new System.Drawing.Point(0, 234);
            this.btnFullServerControl.Margin = new System.Windows.Forms.Padding(2);
            this.btnFullServerControl.Name = "btnFullServerControl";
            this.btnFullServerControl.Size = new System.Drawing.Size(175, 37);
            this.btnFullServerControl.TabIndex = 7;
            this.btnFullServerControl.Text = "🛡️ Full Server Control";
            this.btnFullServerControl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFullServerControl.UseVisualStyleBackColor = true;
            this.btnFullServerControl.Click += new System.EventHandler(this.btnFullServerControl_Click);
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
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnFullServerControl;
    }
}