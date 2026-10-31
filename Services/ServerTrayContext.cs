using System.Runtime.InteropServices;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server_HTML_Handler.Helper;
using AppHelper = Server_HTML_Handler.Helper.Helper;
using Server_HTML_Handler.View;
using Server_HTML_Handler.View.Panel;

namespace Server_HTML_Handler.Services
{
    /// <summary>
    /// Unified Tray-First ApplicationContext.
    /// Runs seamlessly in the Windows System Notification Tray on startup without blocking the user
    /// or demanding an interactive login before Apache and background server handlers can operate.
    /// Manages the tray icon, live server state, single-instance activation, and on-demand dashboard presentation.
    /// </summary>
    public sealed class ServerTrayContext : ApplicationContext
    {
        public static ServerTrayContext Current { get; private set; }

        private readonly NotifyIcon _trayIcon;
        private readonly ContextMenuStrip _trayMenu;
        private readonly ToolStripMenuItem _itemOpen;
        private readonly ToolStripMenuItem _itemApacheHeader;
        private readonly ToolStripMenuItem _itemApacheStart;
        private readonly ToolStripMenuItem _itemApacheRestart;
        private readonly ToolStripMenuItem _itemApacheStop;
        private readonly ToolStripMenuItem _itemApacheServiceAuto;
        private readonly ToolStripMenuItem _itemCallServerHeader;
        private readonly ToolStripMenuItem _itemNotifHeader;
        private readonly ToolStripMenuItem _itemAutoStart;
        private readonly ToolStripMenuItem _itemExit;

        private readonly System.Windows.Forms.Timer _statusTimer;
        private readonly SynchronizationContext _syncContext;
        private EventWaitHandle _showDashboardEvent;
        private volatile bool _isExiting;

        private DashboardPanel _dashboard;
        private LoginForm _loginForm;

        private static string ShowEventName => @"Local\ServerHTMLHandler_ShowEvent_" + Environment.UserName;

        /// <summary>
        /// Signals an already running tray instance to open and bring the Dashboard to the front.
        /// </summary>
        public static void SignalExistingInstanceToOpenDashboard()
        {
            try
            {
                using (var evt = EventWaitHandle.OpenExisting(ShowEventName))
                {
                    evt.Set();
                }
            }
            catch { }
        }

        public ServerTrayContext(bool startMinimized = true)
        {
            Current = this;
            _syncContext = SynchronizationContext.Current ?? new WindowsFormsSynchronizationContext();

            // 1. Setup Single-Instance IPC Listener
            SetupIpcListener();

            // 2. Setup System Tray Icon & Context Menu
            _trayMenu = new ContextMenuStrip();

            _itemOpen = new ToolStripMenuItem("Open Dashboard")
            {
                Font = new Font("Segoe UI", 9.5f, FontStyle.Bold)
            };
            _itemOpen.Click += (s, e) => ShowDashboard();

            // Apache control group
            _itemApacheHeader = new ToolStripMenuItem("Apache: Checking...");
            _itemApacheStart = new ToolStripMenuItem("Start Apache", null, async (s, e) =>
            {
                await ApacheWatchdogService.Instance.EnsureApacheAsync(false);
                UpdateStatus(true);
            });
            _itemApacheRestart = new ToolStripMenuItem("Restart Apache", null, (s, e) =>
            {
                Task.Run(() =>
                {
                    try { ServerBackupHelper.RestartApacheService(); } catch { }
                    _syncContext.Post(_ => UpdateStatus(true), null);
                });
            });
            _itemApacheStop = new ToolStripMenuItem("Stop Apache", null, (s, e) =>
            {
                Task.Run(() =>
                {
                    try { ServerBackupHelper.StopApacheService(out _); } catch { }
                    _syncContext.Post(_ => UpdateStatus(true), null);
                });
            });
            _itemApacheServiceAuto = new ToolStripMenuItem("Enable Apache Windows Auto-Start on Boot", null, (s, e) =>
            {
                if (ServerBackupHelper.ConfigureApacheServiceAutoStart(out string msg))
                {
                    MessageBox.Show(msg, "Apache Boot Auto-Start", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show(msg, "Apache Boot Auto-Start", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            });

            _itemApacheHeader.DropDownItems.Add(_itemApacheStart);
            _itemApacheHeader.DropDownItems.Add(_itemApacheRestart);
            _itemApacheHeader.DropDownItems.Add(_itemApacheStop);
            _itemApacheHeader.DropDownItems.Add(new ToolStripSeparator());
            _itemApacheHeader.DropDownItems.Add(_itemApacheServiceAuto);

            // Call signaling server control
            _itemCallServerHeader = new ToolStripMenuItem("Call Server: Starting...");
            _itemCallServerHeader.Click += (s, e) =>
            {
                if (CallSignalingService.IsRunning)
                {
                    CallSignalingService.Stop();
                }
                else
                {
                    CallSignalingService.Start(out _, 9000);
                }
                UpdateStatus(true);
            };

            // Notification Watcher item
            _itemNotifHeader = new ToolStripMenuItem("Notification Watcher: Active");
            _itemNotifHeader.Enabled = false;

            // Auto-start with Windows toggle
            _itemAutoStart = new ToolStripMenuItem("Start with Windows in Tray")
            {
                CheckOnClick = true,
                Checked = ServerBackupHelper.IsWindowsStartupEnabled()
            };
            _itemAutoStart.Click += (s, e) =>
            {
                ServerBackupHelper.SetWindowsStartup(_itemAutoStart.Checked);
                _itemAutoStart.Checked = ServerBackupHelper.IsWindowsStartupEnabled();
            };

            // Exit
            _itemExit = new ToolStripMenuItem("Exit Server HTML Handler", null, (s, e) => ShutdownApplication());

            // Build Context Menu layout
            _trayMenu.Items.Add(_itemOpen);
            _trayMenu.Items.Add(new ToolStripSeparator());
            _trayMenu.Items.Add(_itemApacheHeader);
            _trayMenu.Items.Add(_itemCallServerHeader);
            _trayMenu.Items.Add(_itemNotifHeader);
            _trayMenu.Items.Add(new ToolStripSeparator());
            _trayMenu.Items.Add(_itemAutoStart);
            _trayMenu.Items.Add(new ToolStripSeparator());
            _trayMenu.Items.Add(_itemExit);

            // Create NotifyIcon
            _trayIcon = new NotifyIcon
            {
                ContextMenuStrip = _trayMenu,
                Visible = true
            };
            LoadTrayIcon();

            _trayIcon.DoubleClick += (s, e) => ShowDashboard();

            // 3. Start Core Background Services immediately (Zero-click auto-start)
            StartBackgroundServices();

            // 4. Setup live status timer
            _statusTimer = new System.Windows.Forms.Timer
            {
                Interval = 3000
            };
            _statusTimer.Tick += (s, e) => UpdateStatus(false);
            _statusTimer.Start();
            UpdateStatus(true);

            // 5. Initial notification or view presentation
            if (startMinimized)
            {
                ShowNotification("Server HTML Handler Active",
                    "Apache and Call Server have started automatically in the system notification tray.");
            }
            else
            {
                ShowDashboard();
            }
        }

        private void SetupIpcListener()
        {
            try
            {
                _showDashboardEvent = new EventWaitHandle(false, EventResetMode.AutoReset, ShowEventName, out _);
                Task.Run(() =>
                {
                    while (!_isExiting)
                    {
                        try
                        {
                            if (_showDashboardEvent != null && _showDashboardEvent.WaitOne(250))
                            {
                                if (_isExiting) break;
                                _syncContext.Post(_ => ShowDashboard(), null);
                            }
                        }
                        catch { break; }
                    }
                });
            }
            catch (Exception ex)
            {
                Program.LogCrash(ex);
            }
        }

        private void LoadTrayIcon()
        {
            try
            {
                string iconPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "gemini-svg.ico");
                if (File.Exists(iconPath))
                {
                    _trayIcon.Icon = new Icon(iconPath);
                    return;
                }
            }
            catch { }

            try
            {
                _trayIcon.Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath) ?? SystemIcons.Application;
            }
            catch
            {
                _trayIcon.Icon = SystemIcons.Application;
            }
        }

        private void StartBackgroundServices()
        {
            // Auto-start Apache via watchdog
            try { ApacheWatchdogService.Instance.Start(); }
            catch (Exception ex) { Program.LogCrash(ex); }

            // Auto-start Call Signaling WebSocket server on Port 9000
            try
            {
                if (!CallSignalingService.IsRunning)
                {
                    CallSignalingService.Start(out _, 9000);
                }
            }
            catch (Exception ex) { Program.LogCrash(ex); }

            // Auto-start Notification Dispatcher Watcher
            try { NotificationDispatcherService.Instance.StartWatcher(); }
            catch (Exception ex) { Program.LogCrash(ex); }
        }

        public void UpdateStatus(bool force)
        {
            try
            {
                bool apacheUp = ServerBackupHelper.IsXamppApacheRunning();
                bool callUp = CallSignalingService.IsRunning;

                // Update Apache Menu State
                if (apacheUp)
                {
                    _itemApacheHeader.Text = "Apache: RUNNING (Port 80)";
                    _itemApacheHeader.ForeColor = Color.DarkGreen;
                    _itemApacheStart.Enabled = false;
                    _itemApacheRestart.Enabled = true;
                    _itemApacheStop.Enabled = true;
                }
                else
                {
                    _itemApacheHeader.Text = "Apache: STOPPED";
                    _itemApacheHeader.ForeColor = Color.DarkRed;
                    _itemApacheStart.Enabled = true;
                    _itemApacheRestart.Enabled = false;
                    _itemApacheStop.Enabled = false;
                }

                // Update Call Server Menu State
                if (callUp)
                {
                    _itemCallServerHeader.Text = $"Call Server: RUNNING (Port {CallSignalingService.Port})";
                    _itemCallServerHeader.ForeColor = Color.DarkGreen;
                }
                else
                {
                    _itemCallServerHeader.Text = "Call Server: STOPPED (Click to Start)";
                    _itemCallServerHeader.ForeColor = Color.DarkRed;
                }

                // Update Tooltip (Windows NotifyIcon max text length is 63 characters)
                string summary = $"Server Handler | Apache: {(apacheUp ? "RUN" : "STOP")} | Call: {(callUp ? "RUN" : "STOP")}";
                if (summary.Length > 63) summary = summary.Substring(0, 60) + "...";
                _trayIcon.Text = summary;
            }
            catch { }
        }

        public void ShowNotification(string title, string text, ToolTipIcon icon = ToolTipIcon.Info)
        {
            try
            {
                if (_trayIcon != null && _trayIcon.Visible)
                {
                    _trayIcon.BalloonTipTitle = title;
                    _trayIcon.BalloonTipText = text;
                    _trayIcon.BalloonTipIcon = icon;
                    _trayIcon.ShowBalloonTip(3000);
                }
            }
            catch { }
        }

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;

        private static void ForceForeground(Form form)
        {
            if (form == null || form.IsDisposed) return;
            try
            {
                if (!form.Visible) form.Show();
                if (form.WindowState == FormWindowState.Minimized)
                {
                    form.WindowState = FormWindowState.Normal;
                }
                ShowWindow(form.Handle, SW_RESTORE);
                SetForegroundWindow(form.Handle);
                form.BringToFront();
                form.Activate();
            }
            catch { }
        }

        public void ShowDashboard()
        {
            if (_isExiting) return;

            // 1. If Dashboard already exists, restore and bring it to front
            if (_dashboard != null && !_dashboard.IsDisposed)
            {
                ForceForeground(_dashboard);
                return;
            }

            // 2. Check remembered sign-in
            if (AppLoginStore.TryGetRemembered(out string user, out string hash))
            {
                CreateAndShowDashboard();
            }
            else
            {
                // If not remembered, show LoginForm
                if (_loginForm != null && !_loginForm.IsDisposed)
                {
                    ForceForeground(_loginForm);
                    return;
                }

                _loginForm = new LoginForm();
                _loginForm.FormClosed += (s, e) => _loginForm = null;
                ForceForeground(_loginForm);
            }
        }

        public void NotifyLoginSuccess()
        {
            if (_loginForm != null && !_loginForm.IsDisposed)
            {
                _loginForm.Hide();
                _loginForm.Close();
                _loginForm = null;
            }
            CreateAndShowDashboard();
        }

        private void CreateAndShowDashboard()
        {
            if (_dashboard == null || _dashboard.IsDisposed)
            {
                _dashboard = new DashboardPanel();
                _dashboard.FormClosing += (s, e) =>
                {
                    if (!_isExiting && e.CloseReason == CloseReason.UserClosing)
                    {
                        e.Cancel = true;
                        _dashboard.Hide();
                        ShowNotification("Server HTML Handler Active",
                            "Dashboard minimized to system notification tray. Apache and Call Server are still active.");
                    }
                };
            }

            ForceForeground(_dashboard);
        }

        public void HandleLogout()
        {
            if (_dashboard != null && !_dashboard.IsDisposed)
            {
                _dashboard.Hide();
                _dashboard.Dispose();
                _dashboard = null;
            }

            AppLoginStore.Forget();
            LoginForm.SkipAutoLogin = true;

            // Open login screen
            ShowDashboard();
        }

        public void ShutdownApplication()
        {
            _isExiting = true;
            _statusTimer?.Stop();
            _statusTimer?.Dispose();

            try
            {
                if (_showDashboardEvent != null)
                {
                    _showDashboardEvent.Set();
                    _showDashboardEvent.Dispose();
                    _showDashboardEvent = null;
                }
            }
            catch { }

            try { CallSignalingService.Stop(); } catch { }
            try { ApacheWatchdogService.Instance.Stop(); } catch { }

            if (_trayIcon != null)
            {
                _trayIcon.Visible = false;
                _trayIcon.Dispose();
            }

            if (_dashboard != null && !_dashboard.IsDisposed)
            {
                _dashboard.Dispose();
                _dashboard = null;
            }

            if (_loginForm != null && !_loginForm.IsDisposed)
            {
                _loginForm.Dispose();
                _loginForm = null;
            }

            ExitThread();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ShutdownApplication();
            }
            base.Dispose(disposing);
        }
    }
}
