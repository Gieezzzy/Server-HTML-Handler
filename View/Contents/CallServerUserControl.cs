using System;
using System.Drawing;
using System.Windows.Forms;
using Server_HTML_Handler.Services;

namespace Server_HTML_Handler.View.Contents
{
    public partial class CallServerUserControl : UserControl
    {
        public CallServerUserControl()
        {
            InitializeComponent();
            WireEvents();
        }

        private void WireEvents()
        {
            this.Load += CallServerUserControl_Load;
            btnStartStop.Click += BtnStartStop_Click;
            btnClearLogs.Click += (s, e) => txtLogs.Clear();
            txtPort.KeyPress += (s, e) =>
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar)) e.Handled = true;
            };
        }

        private void CallServerUserControl_Load(object sender, EventArgs e)
        {
            CallSignalingService.OnLog += HandleLog;
            CallSignalingService.OnUsersUpdated += HandleUsersUpdated;
            CallSignalingService.OnServerStateChanged += HandleServerStateChanged;

            txtPort.Text = CallSignalingService.Port.ToString();
            UpdateUIState(CallSignalingService.IsRunning);
            RefreshUsersList();
        }

        private void BtnStartStop_Click(object sender, EventArgs e)
        {
            if (CallSignalingService.IsRunning)
            {
                CallSignalingService.Stop();
            }
            else
            {
                if (!int.TryParse(txtPort.Text.Trim(), out int port) || port <= 0 || port > 65535)
                {
                    MessageBox.Show("Please enter a valid port number between 1 and 65535.", "Invalid Port", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!CallSignalingService.Start(out string error, port))
                {
                    MessageBox.Show($"Failed to start Call Server on port {port}:\n{error}", "Server Start Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void UpdateUIState(bool isRunning)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => UpdateUIState(isRunning)));
                return;
            }

            if (isRunning)
            {
                lblStatus.Text = $"RUNNING on port {CallSignalingService.Port}";
                lblStatus.ForeColor = Color.DarkGreen;
                btnStartStop.Text = "Stop Server";
                btnStartStop.BackColor = Color.IndianRed;
                txtPort.Enabled = false;
            }
            else
            {
                lblStatus.Text = "STOPPED";
                lblStatus.ForeColor = Color.Gray;
                btnStartStop.Text = "Start Server";
                btnStartStop.BackColor = Color.FromArgb(41, 128, 185);
                txtPort.Enabled = true;
            }
        }

        private void HandleServerStateChanged(bool isRunning)
        {
            UpdateUIState(isRunning);
        }

        private void HandleLog(string message)
        {
            if (txtLogs.InvokeRequired)
            {
                txtLogs.Invoke(new Action(() => HandleLog(message)));
                return;
            }

            if (txtLogs.TextLength > 50000)
            {
                txtLogs.Text = txtLogs.Text.Substring(txtLogs.TextLength - 25000);
            }
            txtLogs.AppendText(message + Environment.NewLine);
        }

        private void HandleUsersUpdated()
        {
            RefreshUsersList();
        }

        private void RefreshUsersList()
        {
            if (lstUsers.InvokeRequired)
            {
                lstUsers.Invoke(new Action(RefreshUsersList));
                return;
            }

            lstUsers.Items.Clear();
            foreach (var kvp in CallSignalingService.ActiveUsers)
            {
                string clientIp = kvp.Value?.ConnectionInfo?.ClientIpAddress ?? "unknown";
                int clientPort = kvp.Value?.ConnectionInfo?.ClientPort ?? 0;
                bool isAvail = kvp.Value?.IsAvailable ?? false;
                lstUsers.Items.Add($"{kvp.Key} ({clientIp}:{clientPort}) [{(isAvail ? "ONLINE" : "OFFLINE")}]");
            }
            lblUserCount.Text = $"Active Registered Clients: {lstUsers.Items.Count}";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                CallSignalingService.OnLog -= HandleLog;
                CallSignalingService.OnUsersUpdated -= HandleUsersUpdated;
                CallSignalingService.OnServerStateChanged -= HandleServerStateChanged;
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
