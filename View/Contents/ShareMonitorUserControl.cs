using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server_HTML_Handler.Services;

namespace Server_HTML_Handler.View.Contents
{
    /// <summary>
    /// Shared-folder monitor for the hosting folder (HTMLPath): who is connected, kick / block, who was refused (and a one-click
    /// Read / Write / Full grant), permissions, open files, firewall tools and folder shortcuts.
    /// </summary>
    public class ShareMonitorUserControl : UserControl
    {
        private readonly ShareControlService _svc = ShareControlService.Instance;

        private static readonly Color Back = Color.FromArgb(24, 24, 24);
        private static readonly Color Field = Color.FromArgb(35, 35, 35);
        private static readonly Color Accent = Color.FromArgb(0, 229, 255);
        private static readonly Color Muted = Color.FromArgb(170, 170, 170);

        private Label _lblFolder, _lblStatus, _lblAudit;
        private System.Windows.Forms.Panel _bannerAdmin;
        private CheckBox _chkAuto, _chkShowAll;
        private TabControl _tabs;
        private System.Windows.Forms.Timer _timer;
        private bool _busy, _disposed;

        // users
        private DataGridView _dgvClients;
        private Button _btnKick, _btnKickBlock, _btnKickAll;
        // requests
        private DataGridView _dgvRequests;
        private Button _btnGrantRead, _btnGrantWrite, _btnGrantFull, _btnReqBlock, _btnReqDismiss, _btnEnableAudit;
        // permissions
        private DataGridView _dgvShareAcl, _dgvNtfs;
        private TextBox _txtAccount;
        private ComboBox _cmbLevel;
        private Button _btnGrant, _btnRevoke, _btnPermReload;
        // open files
        private DataGridView _dgvOpens;
        private Button _btnCloseFile;
        // network
        private ListBox _lstBlocked;
        private TextBox _txtBlockIp, _txtSubnet, _txtTestIp, _txtTestOut;
        private Button _btnUnblock, _btnBlockAdd, _btnAllowSubnet, _btnTest;

        private List<SmbClientInfo> _clients = new List<SmbClientInfo>();
        private List<AccessRequestInfo> _requests = new List<AccessRequestInfo>();

        public ShareMonitorUserControl()
        {
            Dock = DockStyle.Fill;
            BackColor = Back;
            ForeColor = Color.White;
            Font = new Font("Segoe UI", 9F);

            BuildLayout();

            _svc.OnNewAccessRequest += Svc_OnNewAccessRequest;
            Disposed += (s, e) =>
            {
                _disposed = true;
                _svc.OnNewAccessRequest -= Svc_OnNewAccessRequest;
                _timer?.Stop();
                _timer?.Dispose();
            };
            Load += async (s, e) => await InitialLoadAsync();
        }

        // ------------------------------------------------------------------ layout helpers

        private static Button Btn(string text, Color color, int w = 150)
        {
            var b = new Button
            {
                Text = text, AutoSize = false, Size = new Size(w, 32), BackColor = color, ForeColor = Color.White, FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand, Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold), UseVisualStyleBackColor = false, Margin = new Padding(0, 0, 8, 0)
            };
            b.FlatAppearance.BorderSize = 0;
            return b;
        }

        private static DataGridView Grid(params string[] columns)
        {
            var g = new DataGridView
            {
                Dock = DockStyle.Fill, ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false, AllowUserToResizeRows = false,
                RowHeadersVisible = false, SelectionMode = DataGridViewSelectionMode.FullRowSelect, MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, BackgroundColor = Color.FromArgb(24, 24, 24),
                GridColor = Color.FromArgb(48, 48, 48), BorderStyle = BorderStyle.None, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                EnableHeadersVisualStyles = false, ColumnHeadersHeight = 30
            };
            g.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(18, 18, 18);
            g.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(0, 229, 255);
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(18, 18, 18);
            g.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            g.DefaultCellStyle.ForeColor = Color.FromArgb(235, 235, 235);
            g.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 90, 130);
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            foreach (string c in columns) g.Columns.Add(new DataGridViewTextBoxColumn { Name = c, HeaderText = c });
            return g;
        }

        private static FlowLayoutPanel Bar(params Control[] controls)
        {
            var p = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 46, Padding = new Padding(8, 7, 8, 0), WrapContents = false, BackColor = Back };
            p.Controls.AddRange(controls);
            return p;
        }

        private static Label Caption(string text, Color? color = null) =>
            new Label { Text = text, AutoSize = true, ForeColor = color ?? Muted, Margin = new Padding(0, 8, 10, 0) };

        private void BuildLayout()
        {
            // ---------------- header
            var header = new System.Windows.Forms.Panel { Dock = DockStyle.Top, Height = 118, BackColor = Back, Padding = new Padding(16, 10, 16, 0) };
            var title = new Label { Text = "🖥  Shared Folder Monitor", AutoSize = true, ForeColor = Accent, Font = new Font("Segoe UI Semibold", 13F, FontStyle.Bold), Location = new Point(16, 8) };
            _lblFolder = new Label { AutoSize = true, ForeColor = Muted, Location = new Point(18, 40), Text = "Folder: …" };
            _lblStatus = new Label { AutoSize = true, ForeColor = Muted, Location = new Point(18, 62), Text = "Loading…" };

            _chkAuto = new CheckBox { Text = "Auto-refresh (5 s)", Checked = true, AutoSize = true, ForeColor = Color.White, Location = new Point(18, 88) };
            var btnRefresh = Btn("↻ Refresh", Color.FromArgb(60, 60, 60), 110);
            btnRefresh.Location = new Point(170, 82);
            btnRefresh.Click += async (s, e) => await RefreshAllAsync();

            _bannerAdmin = new System.Windows.Forms.Panel { Dock = DockStyle.None, Size = new Size(520, 62), Location = new Point(420, 8), BackColor = Color.FromArgb(80, 55, 0), Visible = false };
            var bannerText = new Label { Text = "Administrator rights are required to see and control network users.", ForeColor = Color.White, Location = new Point(10, 6), Size = new Size(500, 18) };
            var btnAdmin = Btn("Restart as Administrator", Color.FromArgb(200, 120, 0), 210);
            btnAdmin.Location = new Point(10, 26);
            btnAdmin.Click += (s, e) =>
            {
                if (ShareControlService.RestartElevated()) Application.Exit();
            };
            _bannerAdmin.Controls.AddRange(new Control[] { bannerText, btnAdmin });

            header.Controls.AddRange(new Control[] { title, _lblFolder, _lblStatus, _chkAuto, btnRefresh, _bannerAdmin });

            // ---------------- tabs
            _tabs = new TabControl { Dock = DockStyle.Fill, Font = new Font("Segoe UI", 9F) };
            _tabs.TabPages.Add(BuildUsersTab());
            _tabs.TabPages.Add(BuildRequestsTab());
            _tabs.TabPages.Add(BuildPermissionsTab());
            _tabs.TabPages.Add(BuildOpenFilesTab());
            _tabs.TabPages.Add(BuildNetworkTab());
            _tabs.TabPages.Add(BuildToolsTab());

            Controls.Add(_tabs);
            Controls.Add(header);

            _timer = new System.Windows.Forms.Timer { Interval = 5000 };
            _timer.Tick += async (s, e) => { if (_chkAuto.Checked && !_busy) await RefreshLiveAsync(); };
        }

        private TabPage BuildUsersTab()
        {
            var tab = new TabPage("👥 Connected users") { BackColor = Back };
            _dgvClients = Grid("IP address", "User", "Open files", "Connected", "Idle");
            _btnKick = Btn("⛔ Kick selected", Color.FromArgb(192, 57, 43), 150);
            _btnKickBlock = Btn("⛔ Kick && block IP", Color.FromArgb(150, 40, 40), 160);
            _btnKickAll = Btn("Kick everyone", Color.FromArgb(110, 40, 40), 130);
            _chkShowAll = new CheckBox { Text = "Show all SMB sessions (not only this folder)", AutoSize = true, ForeColor = Color.White, Margin = new Padding(12, 8, 0, 0) };
            _chkShowAll.CheckedChanged += async (s, e) => await RefreshLiveAsync();
            _btnKick.Click += async (s, e) => await KickSelectedAsync(false);
            _btnKickBlock.Click += async (s, e) => await KickSelectedAsync(true);
            _btnKickAll.Click += async (s, e) => await KickEveryoneAsync();
            tab.Controls.Add(_dgvClients);
            tab.Controls.Add(Bar(_btnKick, _btnKickBlock, _btnKickAll, _chkShowAll));
            return tab;
        }

        private TabPage BuildRequestsTab()
        {
            var tab = new TabPage("🔔 Access requests") { BackColor = Back };
            _dgvRequests = Grid("Time", "What happened", "IP address", "User", "Folder / file", "Times");
            _btnGrantRead = Btn("✔ Allow Read only", Color.FromArgb(46, 160, 90), 160);
            _btnGrantWrite = Btn("✔ Allow Read + Write", Color.FromArgb(0, 120, 215), 180);
            _btnGrantFull = Btn("Full control", Color.FromArgb(110, 80, 170), 120);
            _btnReqBlock = Btn("Block IP", Color.FromArgb(150, 40, 40), 100);
            _btnReqDismiss = Btn("Dismiss", Color.FromArgb(60, 60, 60), 100);
            _btnGrantRead.Click += async (s, e) => await GrantFromRequestAsync("Read");
            _btnGrantWrite.Click += async (s, e) => await GrantFromRequestAsync("Write");
            _btnGrantFull.Click += async (s, e) => await GrantFromRequestAsync("Full");
            _btnReqBlock.Click += async (s, e) => await BlockFromRequestAsync();
            _btnReqDismiss.Click += (s, e) => DismissSelectedRequest();

            _lblAudit = new Label { Dock = DockStyle.Bottom, Height = 54, ForeColor = Muted, Padding = new Padding(10, 6, 10, 0),
                Text = "A PC that has an account here but no permission on the folder is only reported when auditing of failed file-share access is on." };
            _btnEnableAudit = Btn("Turn on detection", Color.FromArgb(200, 120, 0), 160);
            _btnEnableAudit.Click += async (s, e) => await EnableAuditAsync();
            var auditBar = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 44, Padding = new Padding(8, 4, 8, 0), BackColor = Back };
            auditBar.Controls.Add(_btnEnableAudit);

            tab.Controls.Add(_dgvRequests);
            tab.Controls.Add(Bar(_btnGrantRead, _btnGrantWrite, _btnGrantFull, _btnReqBlock, _btnReqDismiss));
            tab.Controls.Add(auditBar);
            tab.Controls.Add(_lblAudit);
            return tab;
        }

        private TabPage BuildPermissionsTab()
        {
            var tab = new TabPage("🔐 Permissions") { BackColor = Back };
            _dgvShareAcl = Grid("Account", "Type", "Share right");
            _dgvNtfs = Grid("Identity", "Rights", "Type", "Inherited");
            var split = new SplitContainer { Dock = DockStyle.Fill, Orientation = Orientation.Horizontal, BackColor = Back };
            var l1 = new Label { Text = "Share permissions (network access to the share)", Dock = DockStyle.Top, Height = 22, ForeColor = Accent, Padding = new Padding(8, 3, 0, 0) };
            var l2 = new Label { Text = "Folder (NTFS) permissions", Dock = DockStyle.Top, Height = 22, ForeColor = Accent, Padding = new Padding(8, 3, 0, 0) };
            split.Panel1.Controls.Add(_dgvShareAcl); split.Panel1.Controls.Add(l1);
            split.Panel2.Controls.Add(_dgvNtfs); split.Panel2.Controls.Add(l2);

            _txtAccount = new TextBox { Width = 230, BackColor = Field, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, Margin = new Padding(0, 4, 8, 0) };
            _cmbLevel = new ComboBox { Width = 90, DropDownStyle = ComboBoxStyle.DropDownList, BackColor = Field, ForeColor = Color.White, FlatStyle = FlatStyle.Flat, Margin = new Padding(0, 4, 8, 0) };
            _cmbLevel.Items.AddRange(new object[] { "Read", "Write", "Full" });
            _cmbLevel.SelectedIndex = 0;
            _btnGrant = Btn("Grant", Color.FromArgb(46, 160, 90), 90);
            _btnRevoke = Btn("Remove account", Color.FromArgb(150, 60, 60), 140);
            _btnPermReload = Btn("↻ Reload", Color.FromArgb(60, 60, 60), 90);
            _btnGrant.Click += async (s, e) => await GrantManualAsync();
            _btnRevoke.Click += async (s, e) => await RevokeManualAsync();
            _btnPermReload.Click += async (s, e) => await LoadPermissionsAsync();
            var bar = Bar(Caption("Account (PC\\user or user):"), _txtAccount, _cmbLevel, _btnGrant, _btnRevoke, _btnPermReload);
            bar.Dock = DockStyle.Bottom;

            tab.Controls.Add(split);
            tab.Controls.Add(bar);
            _dgvShareAcl.SelectionChanged += (s, e) => { if (_dgvShareAcl.CurrentRow != null) _txtAccount.Text = Convert.ToString(_dgvShareAcl.CurrentRow.Cells[0].Value); };
            return tab;
        }

        private TabPage BuildOpenFilesTab()
        {
            var tab = new TabPage("📂 Open files") { BackColor = Back };
            _dgvOpens = Grid("User", "IP address", "File", "FileId");
            _dgvOpens.Columns["FileId"].Visible = false;
            _btnCloseFile = Btn("Close selected file", Color.FromArgb(192, 57, 43), 170);
            _btnCloseFile.Click += async (s, e) => await CloseSelectedFileAsync();
            tab.Controls.Add(_dgvOpens);
            tab.Controls.Add(Bar(_btnCloseFile));
            return tab;
        }

        private TabPage BuildNetworkTab()
        {
            var tab = new TabPage("🧱 Block / network") { BackColor = Back, AutoScroll = true };
            int y = 14;
            Label Head(string t) { var l = new Label { Text = t, AutoSize = true, ForeColor = Accent, Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold), Location = new Point(16, y) }; y += 26; return l; }
            Label Note(string t) { var l = new Label { Text = t, ForeColor = Muted, Location = new Point(16, y), Size = new Size(860, 34) }; y += 38; return l; }

            tab.Controls.Add(Head("Blocked PCs"));
            _lstBlocked = new ListBox { Location = new Point(16, y), Size = new Size(260, 90), BackColor = Field, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            _btnUnblock = Btn("Unblock selected", Color.FromArgb(60, 60, 60), 140); _btnUnblock.Location = new Point(290, y);
            _txtBlockIp = new TextBox { Location = new Point(290, y + 40), Width = 140, BackColor = Field, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            _btnBlockAdd = Btn("Block this IP", Color.FromArgb(150, 40, 40), 120); _btnBlockAdd.Location = new Point(440, y + 36);
            tab.Controls.AddRange(new Control[] { _lstBlocked, _btnUnblock, _txtBlockIp, _btnBlockAdd });
            y += 104;

            tab.Controls.Add(Head("Let another network (subnet) reach this PC"));
            tab.Controls.Add(Note("Windows Firewall limits file sharing and ping to the local subnet by default. A PC on a different subnet (for example 192.168.33.x while this PC is 192.168.32.x) needs this rule."));
            _txtSubnet = new TextBox { Location = new Point(16, y), Width = 180, Text = "192.168.33.0/24", BackColor = Field, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            _btnAllowSubnet = Btn("Allow this subnet", Color.FromArgb(46, 160, 90), 160); _btnAllowSubnet.Location = new Point(210, y - 4);
            tab.Controls.AddRange(new Control[] { _txtSubnet, _btnAllowSubnet });
            y += 44;

            tab.Controls.Add(Head("Test a PC from here"));
            _txtTestIp = new TextBox { Location = new Point(16, y), Width = 180, Text = "192.168.33.127", BackColor = Field, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            _btnTest = Btn("Test connection", Color.FromArgb(0, 120, 215), 150); _btnTest.Location = new Point(210, y - 4);
            _txtTestOut = new TextBox { Location = new Point(16, y + 36), Size = new Size(560, 90), Multiline = true, ReadOnly = true, BackColor = Field, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };
            tab.Controls.AddRange(new Control[] { _txtTestIp, _btnTest, _txtTestOut });

            _btnUnblock.Click += async (s, e) => await UnblockSelectedAsync();
            _btnBlockAdd.Click += async (s, e) => await BlockTypedAsync();
            _btnAllowSubnet.Click += async (s, e) => await AllowSubnetAsync();
            _btnTest.Click += async (s, e) => await TestAsync();
            return tab;
        }

        private TabPage BuildToolsTab()
        {
            var tab = new TabPage("🧰 Folder tools") { BackColor = Back };
            var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown, Padding = new Padding(16), WrapContents = false, AutoScroll = true };
            var info = new Label { AutoSize = true, ForeColor = Muted, MaximumSize = new Size(820, 0), Margin = new Padding(0, 0, 0, 12),
                Text = "Your hosting folder is shared on the network. Use the other tabs to see who is connected, disconnect or block a PC, answer access requests and set Read / Write / Full permissions." };
            var open = Btn("📁 Open the folder", Color.FromArgb(60, 60, 60), 260);
            var manage = Btn("🗂 Windows shared-folders manager", Color.FromArgb(60, 60, 60), 260);
            var copy = Btn("📋 Copy the network path", Color.FromArgb(60, 60, 60), 260);
            var props = Btn("🔧 Open folder security settings", Color.FromArgb(60, 60, 60), 260);
            open.Click += (s, e) => TryStart("explorer.exe", "\"" + _svc.FolderPath + "\"");
            manage.Click += (s, e) => TryStart("fsmgmt.msc", "", true);
            copy.Click += (s, e) =>
            {
                string host = Properties.Settings.Default.IP;
                if (string.IsNullOrWhiteSpace(host)) host = Environment.MachineName;
                string text = $@"\\{host}\{(string.IsNullOrEmpty(_svc.ShareName) ? "<share>" : _svc.ShareName)}";
                Clipboard.SetText(text);
                SetStatus("Copied " + text, Color.FromArgb(46, 204, 113));
            };
            props.Click += (s, e) => TryStart("rundll32.exe", "shell32.dll,Options_RunDLL 0", false);
            flow.Controls.AddRange(new Control[] { info, open, manage, copy });
            tab.Controls.Add(flow);
            return tab;
        }

        private void TryStart(string file, string args, bool shell = false)
        {
            try { Process.Start(new ProcessStartInfo { FileName = file, Arguments = args, UseShellExecute = true }); }
            catch (Exception ex) { SetStatus("Could not open: " + ex.Message, Color.FromArgb(231, 76, 60)); }
        }

        // ------------------------------------------------------------------ state

        private void SetStatus(string text, Color? color = null)
        {
            if (_disposed || IsDisposed) return;
            _lblStatus.ForeColor = color ?? Muted;
            _lblStatus.Text = text;
        }

        private void ApplyElevationState(bool elevated)
        {
            _bannerAdmin.Visible = !elevated;
            foreach (var b in new Control[] { _btnKick, _btnKickBlock, _btnKickAll, _btnGrantRead, _btnGrantWrite, _btnGrantFull, _btnReqBlock, _btnEnableAudit,
                                              _btnGrant, _btnRevoke, _btnCloseFile, _btnUnblock, _btnBlockAdd, _btnAllowSubnet })
                b.Enabled = elevated;
        }

        private async Task InitialLoadAsync()
        {
            string path = Properties.Settings.Default.HTMLPath;
            _svc.FolderPath = string.IsNullOrWhiteSpace(path) ? @"D:\html" : path;
            _lblFolder.Text = "Folder: " + _svc.FolderPath;

            bool elevated = ShareControlService.IsElevated();
            ApplyElevationState(elevated);
            _svc.StartMonitor();

            await RefreshAllAsync();
            _timer.Start();
        }

        private async Task RefreshAllAsync()
        {
            await RefreshLiveAsync();
            if (ShareControlService.IsElevated())
            {
                await LoadPermissionsAsync();
                await LoadBlockedAsync();
                await UpdateAuditLabelAsync();
            }
        }

        private async Task RefreshLiveAsync()
        {
            if (_busy || _disposed) return;
            _busy = true;
            try
            {
                bool elevated = ShareControlService.IsElevated();
                ApplyElevationState(elevated);
                RenderRequests();
                if (!elevated)
                {
                    SetStatus("Not running as Administrator: the list of connected PCs cannot be read.", Color.FromArgb(255, 193, 7));
                    return;
                }

                if (string.IsNullOrEmpty(_svc.ShareName))
                {
                    string name = await _svc.FindShareAsync();
                    if (string.IsNullOrEmpty(name))
                    {
                        SetStatus("This folder is not shared on the network. Share it from the 🧰 Folder tools tab (Windows shared-folders manager).", Color.FromArgb(255, 193, 7));
                        return;
                    }
                    _lblFolder.Text = $"Folder: {_svc.FolderPath}   |   share: {name}";
                }

                var snap = await _svc.GetSnapshotAsync(_chkShowAll.Checked);
                _clients = snap.Clients;
                RenderClients();
                RenderOpens(snap);
                SetStatus($"{_clients.Count} PC(s) connected · updated {DateTime.Now:HH:mm:ss}", Color.FromArgb(46, 204, 113));
            }
            catch (Exception ex)
            {
                SetStatus("✖ " + ex.Message, Color.FromArgb(231, 76, 60));
            }
            finally
            {
                _busy = false;
            }
        }

        // ------------------------------------------------------------------ rendering

        private void RenderClients()
        {
            string keepIp = SelectedClient()?.Ip, keepUser = SelectedClient()?.User;
            _dgvClients.Rows.Clear();
            foreach (var c in _clients)
            {
                int i = _dgvClients.Rows.Add(c.Ip, c.User, c.OpenFiles, c.ConnectedText, c.IdleText);
                _dgvClients.Rows[i].Tag = c;
                if (c.Ip == keepIp && c.User == keepUser) _dgvClients.Rows[i].Selected = true;
            }
        }

        private SmbClientInfo SelectedClient() => _dgvClients.CurrentRow?.Tag as SmbClientInfo;

        private void RenderOpens(ShareSnapshot snap)
        {
            _dgvOpens.Rows.Clear();
            foreach (var o in snap.Opens)
                _dgvOpens.Rows.Add(o.User, o.Ip, o.Path, o.FileId);
        }

        private void RenderRequests()
        {
            _requests = _svc.PendingRequests.ToList();
            string key = SelectedRequest()?.Key;
            _dgvRequests.Rows.Clear();
            foreach (var r in _requests)
            {
                int i = _dgvRequests.Rows.Add(r.Time.ToString("MMM d HH:mm:ss"), r.Kind, r.Ip, r.User, string.IsNullOrEmpty(r.Target) ? "(sign-in refused)" : r.Target, r.Count);
                _dgvRequests.Rows[i].Tag = r;
                _dgvRequests.Rows[i].DefaultCellStyle.ForeColor = r.Kind == "Access denied" ? Color.FromArgb(255, 193, 7) : Color.FromArgb(255, 120, 110);
                if (r.Key == key) _dgvRequests.Rows[i].Selected = true;
            }
            _tabs.TabPages[1].Text = _requests.Count > 0 ? $"🔔 Access requests ({_requests.Count})" : "🔔 Access requests";
        }

        private AccessRequestInfo SelectedRequest() => _dgvRequests.CurrentRow?.Tag as AccessRequestInfo;

        private void Svc_OnNewAccessRequest(AccessRequestInfo r)
        {
            if (_disposed || IsDisposed || !IsHandleCreated) return;
            try { BeginInvoke(new Action(RenderRequests)); } catch { }
        }

        // ------------------------------------------------------------------ actions

        private async Task RunActionAsync(string doing, Func<Task> action, string done)
        {
            try
            {
                SetStatus(doing + "…", Color.FromArgb(180, 180, 180));
                await action();
                SetStatus("✔ " + done, Color.FromArgb(46, 204, 113));
            }
            catch (Exception ex)
            {
                SetStatus("✖ " + ex.Message, Color.FromArgb(231, 76, 60));
                MessageBox.Show(ex.Message, "Action failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task KickSelectedAsync(bool block)
        {
            var c = SelectedClient();
            if (c == null) { SetStatus("Select a PC first.", Color.FromArgb(255, 193, 7)); return; }
            string what = block ? "Disconnect AND block" : "Disconnect";
            if (MessageBox.Show($"{what} {c.Ip} ({c.User}) from your folder right now?", "Kick", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            await RunActionAsync("Disconnecting " + c.Ip, () => _svc.KickAsync(c.Ip, block), $"{c.Ip} was disconnected{(block ? " and blocked" : "")}.");
            await RefreshLiveAsync();
            if (block) await LoadBlockedAsync();
        }

        private async Task KickEveryoneAsync()
        {
            if (MessageBox.Show("Disconnect every PC that is using your folder right now?", "Kick everyone", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            await RunActionAsync("Disconnecting everyone", () => _svc.KickEveryoneAsync(), "All users of the folder were disconnected.");
            await RefreshLiveAsync();
        }

        private async Task GrantFromRequestAsync(string level)
        {
            var r = SelectedRequest();
            if (r == null) { SetStatus("Select a request first.", Color.FromArgb(255, 193, 7)); return; }
            string who = r.User;
            if (MessageBox.Show($"Give {who} ({r.Ip}) {(level == "Read" ? "READ-ONLY" : level == "Write" ? "READ + WRITE" : "FULL")} access to your folder?", "Allow access", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;

            bool ok = false;
            await RunActionAsync("Granting " + level + " to " + who, async () => { await _svc.GrantAsync(who, level); ok = true; }, $"{who} now has {level} access.");
            if (ok)
            {
                _svc.Dismiss(r);
                RenderRequests();
                await LoadPermissionsAsync();
            }
            else if (r.Kind == "Logon failed")
            {
                MessageBox.Show($"{who} has no account on this PC, so Windows cannot grant it anything.\n\nCreate a local account with that name on this PC (Computer Management > Local Users), then press the button again. The user must sign in from their PC with that account and its password.", "No account", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private async Task BlockFromRequestAsync()
        {
            var r = SelectedRequest();
            if (r == null) return;
            if (MessageBox.Show($"Block {r.Ip} from your file share?", "Block", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            await RunActionAsync("Blocking " + r.Ip, async () => { await _svc.BlockIpAsync(r.Ip); _svc.Dismiss(r); }, r.Ip + " is blocked.");
            RenderRequests();
            await LoadBlockedAsync();
        }

        private void DismissSelectedRequest()
        {
            var r = SelectedRequest();
            if (r == null) return;
            _svc.Dismiss(r);
            RenderRequests();
        }

        private async Task UpdateAuditLabelAsync()
        {
            try
            {
                bool on = await _svc.IsAuditEnabledAsync();
                _btnEnableAudit.Visible = !on;
                _lblAudit.Text = on
                    ? "Detection is on: refused sign-ins and 'access denied' attempts appear here and as a tray notification."
                    : "Detection of 'access denied' is OFF: a PC that has an account here but no permission on the folder is not reported. Press 'Turn on detection' (Windows audit policy: File Share, failures).";
            }
            catch { }
        }

        private async Task EnableAuditAsync()
        {
            await RunActionAsync("Turning on detection", () => _svc.EnableAuditAsync(), "Failed share access is now logged and will be reported here.");
            await UpdateAuditLabelAsync();
        }

        private async Task LoadPermissionsAsync()
        {
            try
            {
                var share = await _svc.GetShareAccessAsync();
                _dgvShareAcl.Rows.Clear();
                foreach (var a in share) _dgvShareAcl.Rows.Add(a.Account, a.Type, a.Right);

                var ntfs = await _svc.GetNtfsAccessAsync();
                _dgvNtfs.Rows.Clear();
                foreach (var a in ntfs) _dgvNtfs.Rows.Add(a.Identity, a.Rights, a.Type, a.Inherited ? "yes" : "no");
            }
            catch (Exception ex)
            {
                SetStatus("✖ Permissions: " + ex.Message, Color.FromArgb(231, 76, 60));
            }
        }

        private async Task GrantManualAsync()
        {
            string acct = _txtAccount.Text.Trim();
            string level = Convert.ToString(_cmbLevel.SelectedItem);
            if (acct.Length == 0) { SetStatus("Type the account first (for example PC127\\audit).", Color.FromArgb(255, 193, 7)); return; }
            await RunActionAsync("Granting " + level + " to " + acct, () => _svc.GrantAsync(acct, level), $"{acct} now has {level} access.");
            await LoadPermissionsAsync();
        }

        private async Task RevokeManualAsync()
        {
            string acct = _txtAccount.Text.Trim();
            if (acct.Length == 0) return;
            if (MessageBox.Show($"Remove all access of '{acct}' to the share and the folder?", "Remove account", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            await RunActionAsync("Removing " + acct, () => _svc.RevokeAsync(acct), $"{acct} no longer has access.");
            await LoadPermissionsAsync();
        }

        private async Task CloseSelectedFileAsync()
        {
            var row = _dgvOpens.CurrentRow;
            if (row == null) return;
            string id = Convert.ToString(row.Cells["FileId"].Value);
            await RunActionAsync("Closing the file", () => _svc.CloseOpenFileAsync(id), "File closed.");
            await RefreshLiveAsync();
        }

        private async Task LoadBlockedAsync()
        {
            try
            {
                var list = await _svc.GetBlockedIpsAsync();
                _lstBlocked.Items.Clear();
                foreach (string ip in list) _lstBlocked.Items.Add(ip);
            }
            catch { }
        }

        private async Task UnblockSelectedAsync()
        {
            string ip = _lstBlocked.SelectedItem as string;
            if (ip == null) return;
            await RunActionAsync("Unblocking " + ip, () => _svc.UnblockIpAsync(ip), ip + " is no longer blocked.");
            await LoadBlockedAsync();
        }

        private async Task BlockTypedAsync()
        {
            string ip = _txtBlockIp.Text.Trim();
            await RunActionAsync("Blocking " + ip, () => _svc.BlockIpAsync(ip), ip + " is blocked.");
            _txtBlockIp.Clear();
            await LoadBlockedAsync();
        }

        private async Task AllowSubnetAsync()
        {
            string cidr = _txtSubnet.Text.Trim();
            if (MessageBox.Show($"Allow file sharing and ping from {cidr} through the Windows Firewall of this PC?", "Allow subnet", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            await RunActionAsync("Opening the firewall for " + cidr, () => _svc.AllowSubnetAsync(cidr), $"The firewall now allows {cidr}.");
        }

        private async Task TestAsync()
        {
            string ip = _txtTestIp.Text.Trim();
            _btnTest.Enabled = false;
            _txtTestOut.Text = "Testing " + ip + " …";
            try
            {
                _txtTestOut.Text = "From this PC to " + ip + ":\r\n" + (await _svc.TestConnectionAsync(ip)).Replace("\n", "\r\n");
            }
            catch (Exception ex)
            {
                _txtTestOut.Text = "✖ " + ex.Message;
            }
            finally
            {
                _btnTest.Enabled = true;
            }
        }
    }
}
