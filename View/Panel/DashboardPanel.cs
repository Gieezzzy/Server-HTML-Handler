using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server_HTML_Handler.Services;
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

        private bool _isExplicitExit = false;
        private Button btnShareMonitor;

        public DashboardPanel()
        {
            InitializeComponent();
            SetupTrayIcon();
            SetupShareMonitorButton();
        }

        private void SetupTrayIcon()
        {
            try
            {
                if (this.Icon != null)
                {
                    notifyIconServer.Icon = this.Icon;
                }
                else
                {
                    notifyIconServer.Icon = SystemIcons.Application;
                }
            }
            catch
            {
                notifyIconServer.Icon = SystemIcons.Application;
            }

            CallSignalingService.OnServerStateChanged += HandleServerStateChanged;
            UpdateTrayServerState(CallSignalingService.IsRunning);
        }

        private void DashboardPanel_Load(object sender, EventArgs e)
        {
            // Auto-start Call Signaling Server on port 9000 if not yet started
            if (!CallSignalingService.IsRunning)
            {
                CallSignalingService.Start(out _, 9000);
            }

            // Load default view and set Dashboard button as active
            LoadUserControlSafely(() => new DashboardContents(), btnDashboard);
        }

        private void HandleServerStateChanged(bool isRunning)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleServerStateChanged(isRunning)));
                return;
            }
            UpdateTrayServerState(isRunning);
        }

        private void UpdateTrayServerState(bool isRunning)
        {
            if (isRunning)
            {
                trayItemCallServer.Text = $"Call Server: RUNNING (Port {CallSignalingService.Port})";
                trayItemCallServer.ForeColor = Color.DarkGreen;
                notifyIconServer.Text = $"Server HTML Handler - Call Server Active ({CallSignalingService.Port})";
            }
            else
            {
                trayItemCallServer.Text = "Call Server: STOPPED (Click to Start)";
                trayItemCallServer.ForeColor = Color.DarkRed;
                notifyIconServer.Text = "Server HTML Handler (Call Server Inactive)";
            }
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
                btnCi4Creator,
                btnCodeEditor,
                btnConsole,
                btnFullServerControl,
                btnCallServer,
                btnNotifications,
                btnShareMonitor
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
            try
            {
                Cursor = Cursors.WaitCursor;
                pnlContent.SuspendLayout();

                // Highlight corresponding sidebar button
                if (sourceButton != null)
                {
                    HighlightActiveButton(sourceButton);
                }

                // Cleanly dispose and remove existing hosted controls
                while (pnlContent.Controls.Count > 0)
                {
                    Control oldControl = pnlContent.Controls[0];
                    pnlContent.Controls.RemoveAt(0);
                    oldControl.Dispose();
                }

                // Instantiate new view instance
                Control newControl = controlFactory();

                if (newControl == null)
                {
                    throw new InvalidOperationException("Failed to load: Factory returned a null control reference.");
                }

                // Configure control layout inside host container
                newControl.Dock = DockStyle.Fill;
                newControl.BringToFront();

                pnlContent.Controls.Add(newControl);
                newControl.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"An error occurred while switching views:\n\n{ex.Message}",
                    "Navigation Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
            finally
            {
                pnlContent.ResumeLayout(true);
                Cursor = Cursors.Default;
            }
        }

        // ==========================================
        // Sidebar Navigation Button Click Handlers
        // ==========================================
        private void btnDashboard_Click(object sender, System.EventArgs e)
        {
            LoadUserControlSafely(() => new DashboardContents(), sender as Button);
        }

        private void btnServerControl_Click(object sender, System.EventArgs e)
        {
            LoadUserControlSafely(() => new ServerControlUserControl(), sender as Button);
        }

        private void btnProjectViews_Click(object sender, System.EventArgs e)
        {
            LoadUserControlSafely(() => new ProjectViewUserControl(), sender as Button);
        }

        private void btnCodeEditor_Click(object sender, System.EventArgs e)
        {
            LoadUserControlSafely(() => new CodeEditorUserControl(), sender as Button);
        }

        private void btnConsole_Click(object sender, System.EventArgs e)
        {
            LoadUserControlSafely(() => new ConsoleUserControl(), sender as Button);
        }

        private void btnFullServerControl_Click(object sender, EventArgs e)
        {
            LoadUserControlSafely(() => new ServerControlForm(), sender as Button);
        }

        private void btnCi4Creator_Click(object sender, EventArgs e)
        {
            HighlightActiveButton(btnCi4Creator);

            string hostingPath = Properties.Settings.Default.HTMLPath;
            if (string.IsNullOrWhiteSpace(hostingPath) || !System.IO.Directory.Exists(hostingPath))
            {
                hostingPath = @"D:\html";
            }
            string hostIp = Properties.Settings.Default.IP;
            if (string.IsNullOrWhiteSpace(hostIp)) hostIp = "192.168.32.43";

            using (var dlg = new Dialogs.CreateCi4ProjectDialog(hostingPath, hostIp))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK || !string.IsNullOrEmpty(dlg.CreatedProjectPath))
                {
                    // Refresh project views if already loaded or navigate to project views
                    LoadUserControlSafely(() => new ProjectViewUserControl(), btnProjectViews);
                }
            }
        }

        private void btnCallServer_Click(object sender, EventArgs e)
        {
            LoadUserControlSafely(() => new CallServerUserControl(), sender as Button);
        }

        // ==========================================
        // Shared Folder Monitor (who is connected to the hosting folder)
        // ==========================================
        private const string ShareButtonText = "🖥 Folder Access";

        private void SetupShareMonitorButton()
        {
            btnShareMonitor = new Button
            {
                Dock = DockStyle.Top,
                FlatStyle = FlatStyle.Flat,
                Font = btnNotifications.Font,
                ForeColor = Color.FromArgb(0, 188, 212),
                Size = btnNotifications.Size,
                Margin = btnNotifications.Margin,
                Name = "btnShareMonitor",
                Text = ShareButtonText,
                TextAlign = ContentAlignment.MiddleLeft,
                UseVisualStyleBackColor = true
            };
            btnShareMonitor.FlatAppearance.BorderSize = 0;
            btnShareMonitor.Click += btnShareMonitor_Click;
            pnlSidebar.Controls.Add(btnShareMonitor);
            // Docked-top buttons stack in reverse child order: index 1 puts this one right under "Live Notifications".
            pnlSidebar.Controls.SetChildIndex(btnShareMonitor, 1);

            var share = Services.ShareControlService.Instance;
            string path = Properties.Settings.Default.HTMLPath;
            share.FolderPath = string.IsNullOrWhiteSpace(path) ? @"D:\html" : path;
            share.OnNewAccessRequest += HandleShareAccessRequest;
            share.StartMonitor();
        }

        private void btnShareMonitor_Click(object sender, EventArgs e)
        {
            LoadUserControlSafely(() => new ShareMonitorUserControl(), sender as Button);
            UpdateShareBadge();
        }

        private void HandleShareAccessRequest(Services.AccessRequestInfo r)
        {
            if (IsDisposed || !IsHandleCreated) return;
            BeginInvoke(new Action(() =>
            {
                UpdateShareBadge();
                string who = string.IsNullOrWhiteSpace(r.User) ? r.Ip : $"{r.User} ({r.Ip})";
                notifyIconServer.BalloonTipTitle = "Folder access request";
                notifyIconServer.BalloonTipText = r.Kind == "Access denied"
                    ? $"{who} tried to open your hosting folder but has no permission.\nOpen Folder Access to allow Read / Write or block it."
                    : $"{who} could not sign in to your hosting folder.\nOpen Folder Access to see the details.";
                notifyIconServer.BalloonTipClicked -= ShareBalloonClicked;
                notifyIconServer.BalloonTipClicked += ShareBalloonClicked;
                notifyIconServer.ShowBalloonTip(10000);
            }));
        }

        private void ShareBalloonClicked(object sender, EventArgs e)
        {
            notifyIconServer.BalloonTipClicked -= ShareBalloonClicked;
            RestoreDashboard();
            btnShareMonitor_Click(btnShareMonitor, EventArgs.Empty);
        }

        private void UpdateShareBadge()
        {
            int n = Services.ShareControlService.Instance.PendingRequests.Count;
            btnShareMonitor.Text = n > 0 ? $"{ShareButtonText} ({n})" : ShareButtonText;
            btnShareMonitor.ForeColor = n > 0 ? Color.FromArgb(255, 99, 71) : Color.FromArgb(0, 188, 212);
        }

        private void btnNotifications_Click(object sender, EventArgs e)
        {
            LoadUserControlSafely(() => new NotificationHubUserControl(), sender as Button);
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

        // ==========================================
        // System Tray & Background Execution
        // ==========================================
        private void notifyIconServer_DoubleClick(object sender, EventArgs e)
        {
            RestoreDashboard();
        }

        private void trayItemOpen_Click(object sender, EventArgs e)
        {
            RestoreDashboard();
        }

        private void RestoreDashboard()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            this.Activate();
        }

        private void trayItemCallServer_Click(object sender, EventArgs e)
        {
            if (CallSignalingService.IsRunning)
            {
                CallSignalingService.Stop();
            }
            else
            {
                CallSignalingService.Start(out string err, 9000);
            }
        }

        private void trayItemExit_Click(object sender, EventArgs e)
        {
            _isExplicitExit = true;
            CallSignalingService.Stop();
            notifyIconServer.Visible = false;
            notifyIconServer.Dispose();
            Application.Exit();
        }

        // ==========================================
        // Session & Form Closing Handlers
        // ==========================================
        private void btnLogout_Click(object sender, System.EventArgs e)
        {
            this.Hide();
            Helper.AppLoginStore.Forget();      // logging out also forgets "Remember me"
            LoginForm.SkipAutoLogin = true;
            LoginForm login = new LoginForm();
            login.Show();
        }

        private void DashboardPanel_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_isExplicitExit)
            {
                return;
            }

            // If user closed the form or X button was clicked, minimize to system tray
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                this.Hide();

                notifyIconServer.BalloonTipTitle = "Server HTML Handler Active";
                string serverStatus = CallSignalingService.IsRunning 
                    ? $"Call Server is running on port {CallSignalingService.Port}." 
                    : "Running in background.";
                notifyIconServer.BalloonTipText = $"{serverStatus}\nDouble-click this tray icon or right-click to manage.";
                notifyIconServer.ShowBalloonTip(2000);
            }
        }
    }
}
