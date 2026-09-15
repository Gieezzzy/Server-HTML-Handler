using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server_HTML_Handler.View.Contents;

namespace Server_HTML_Handler.View.Panel
{
    public partial class DashboardPanel : System.Windows.Forms.Form
    {
        // Sidebar active / inactive visual color definitions
        private readonly Color ActiveBgColor = Color.FromArgb(50, 54, 62);      // Lighter dark background for active tab
        private readonly Color InactiveBgColor = Color.Transparent;             // Standard background for inactive tabs
        private readonly Color ActiveTextColor = Color.FromArgb(0, 229, 255);   // Cyan text for active tab
        private readonly Color InactiveTextColor = Color.FromArgb(200, 200, 200); // Muted light gray text for inactive tabs

        public DashboardPanel()
        {
            InitializeComponent();
        }

        private void DashboardPanel_Load(object sender, EventArgs e)
        {
            // Load default view and set Dashboard button as active
            LoadUserControlSafely(() => new DashboardContents(), btnDashboard);
        }

        /// <summary>
        /// Highlights the currently selected sidebar button and resets inactive ones.
        /// </summary>
        private void HighlightActiveButton(Button activeButton)
        {
            Button[] navButtons = new Button[]
            {
                btnDashboard,
                btnServerControl,
                btnProjectViews,
                btnCodeEditor,
                btnConsole,
                btnProjectViews,
                btnCodeEditor,
                btnConsole,
                btnFullServerControl,
            };

            foreach (var btn in navButtons)
            {
                if (btn == null) continue;

                if (btn == activeButton)
                {
                    btn.BackColor = ActiveBgColor;
                    btn.ForeColor = ActiveTextColor;
                    btn.UseVisualStyleBackColor = false;
                }
                else
                {
                    btn.BackColor = InactiveBgColor;
                    btn.ForeColor = InactiveTextColor;
                    btn.UseVisualStyleBackColor = false;
                }
            }
        }

        /// <summary>
        /// Safely instantiates and mounts a UserControl or Form into pnlContent.
        /// Manages memory disposal, layout docking, sidebar highlights, and exception handling.
        /// </summary>
        private void LoadUserControlSafely(Func<System.Windows.Forms.Control> controlFactory, Button sourceButton = null)
        {
            System.Windows.Forms.Control newControl = null;
            try
            {
                // Highlight sidebar active button state
                if (sourceButton != null)
                {
                    HighlightActiveButton(sourceButton);
                }

                // Instantiate the requested control
                newControl = controlFactory();
                if (newControl == null) return;

                // Guard: Prevent reloading if the exact same control type is already active
                if (pnlContent.Controls.Count > 0 && pnlContent.Controls[0].GetType() == newControl.GetType())
                {
                    newControl.Dispose();
                    return;
                }

                pnlContent.SuspendLayout();

                // Safely dispose existing controls to free system resources and unbind listeners
                foreach (System.Windows.Forms.Control existingCtrl in pnlContent.Controls)
                {
                    existingCtrl.Dispose();
                }
                pnlContent.Controls.Clear();

                // Force control to expand and fill pnlContent completely
                newControl.Dock = System.Windows.Forms.DockStyle.Fill;
                newControl.AutoSize = false;
                newControl.Visible = true;

                // Handle embedded Form instances if necessary
                if (newControl is System.Windows.Forms.Form frm)
                {
                    frm.TopLevel = false;
                    frm.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
                    frm.Show();
                }

                pnlContent.Controls.Add(newControl);
                newControl.BringToFront();
                pnlContent.ResumeLayout(true);
            }
            catch (Exception ex)
            {
                if (newControl != null && !newControl.IsDisposed)
                {
                    newControl.Dispose();
                }

                MessageBox.Show($"Failed to load view:\n{ex.Message}\n\nStack Trace:\n{ex.StackTrace}",
                                "Navigation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // Sidebar Navigation Button Handlers
        // ==========================================

        // 1. Dashboard Metrics Tab
        private void btnDashboard_Click(object sender, System.EventArgs e)
        {
            LoadUserControlSafely(() => new DashboardContents(), sender as Button);
        }

        // 2. Server Control & Security Test Labs
        private void btnServerControl_Click(object sender, System.EventArgs e)
        {
            LoadUserControlSafely(() => new ServerControlUserControl(), sender as Button);
        }

        // 3. HTML & CodeIgniter 4 Quick Views
        private void btnProjectViews_Click(object sender, System.EventArgs e)
        {
            LoadUserControlSafely(() => new ProjectViewUserControl(), sender as Button);
        }

        // 4. Quick Code Editor
        private void btnCodeEditor_Click(object sender, System.EventArgs e)
        {
            LoadUserControlSafely(() => new CodeEditorUserControl(), sender as Button);
        }

        // 5. CLI Console & Terminal Engine
        private void btnConsole_Click(object sender, System.EventArgs e)
        {
            LoadUserControlSafely(() => new ConsoleUserControl(), sender as Button);
        }

        private void btnFileManager_Click(object sender, System.EventArgs e)
        {
            LoadUserControlSafely(() => new ConsoleUserControl(), sender as Button);
        }

        // Optional / Auxiliary Button Handlers
        private void btnDatabases_Click(object sender, System.EventArgs e)
        {
            LoadUserControlSafely(() => new ConsoleUserControl(), sender as Button);
        }

        private void btnLogs_Click(object sender, System.EventArgs e)
        {
            LoadUserControlSafely(() => new ConsoleUserControl(), sender as Button);
        }

        private void btnSettings_Click(object sender, System.EventArgs e)
        {
            LoadUserControlSafely(() => new CodeEditorUserControl(), sender as Button);
        }

        private void btnFullServerControl_Click(object sender, EventArgs e)
        {
            LoadUserControlSafely(() => new ServerControlForm(), sender as Button);
        }

        // ==========================================
        // Session & Form Closing Handlers
        // ==========================================
        private void btnLogout_Click(object sender, System.EventArgs e)
        {
            this.Hide();
            LoginForm login = new LoginForm();
            login.Show();
        }

        private void DashboardPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm is LoginForm)
                {
                    openForm.Show();
                    return;
                }
            }

            LoginForm newLogin = new LoginForm();
            newLogin.Show();
        }
    }
}