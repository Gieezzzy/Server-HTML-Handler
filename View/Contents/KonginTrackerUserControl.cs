using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server_HTML_Handler.Services;

namespace Server_HTML_Handler.View.Contents
{
    public class KonginTrackerUserControl : UserControl
    {
        private readonly KonginTrackerService _service = KonginTrackerService.Instance;
        private readonly NotificationDispatcherService _dispatcher = NotificationDispatcherService.Instance;

        private TabControl tabControl;
        private TabPage tabDashboard;
        private TabPage tabRecipients;

        // Dashboard controls
        private System.Windows.Forms.Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;
        private System.Windows.Forms.Panel pnlCards;
        private System.Windows.Forms.Panel cardTemp;
        private Label lblTempTitle;
        private Label lblTempValue;
        private Label lblTempStandard;
        private System.Windows.Forms.Panel cardHum;
        private Label lblHumTitle;
        private Label lblHumValue;
        private Label lblHumStandard;
        private System.Windows.Forms.Panel cardStatus;
        private Label lblStatusTitle;
        private Label lblStatusValue;
        private Label lblStatusSub;

        private System.Windows.Forms.Panel pnlActions;
        private Button btnRefresh;
        private Button btnSendTestAlert;
        private Label lblInterval;
        private NumericUpDown numAlertInterval;
        private Button btnSaveInterval;
        private Label lblLastChecked;

        private DataGridView dgvReadings;

        // Recipients tab controls
        private ListView lstRecipients;
        private TextBox txtRecipId;
        private Button btnRecipAdd;
        private Button btnRecipRemove;
        private Button btnRecipSave;
        private Button btnRecipReload;
        private Label lblRecipStatus;
        private readonly List<string> _recipIds = new List<string>();
        private Dictionary<string, string> _recipNames = new Dictionary<string, string>();
        private bool _recipDirty;

        public KonginTrackerUserControl()
        {
            InitializeComponent();
            SetupGrid();
            SetupRecipientsControls();

            _service.OnReadingUpdated += Service_OnReadingUpdated;
            this.Disposed += (s, e) => _service.OnReadingUpdated -= Service_OnReadingUpdated;

            this.Load += async (s, e) =>
            {
                numAlertInterval.Value = Math.Max(1, Math.Min(1440, _service.AlertIntervalMinutes));
                await RefreshDataAsync();
                await LoadRecipientsAsync();
            };

            // Immediate initial load in case Load event does not fire when dynamically hosted
            try
            {
                numAlertInterval.Value = Math.Max(1, Math.Min(1440, _service.AlertIntervalMinutes));
            }
            catch { }
            _ = RefreshDataAsync();
            _ = LoadRecipientsAsync();
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (!DesignMode)
            {
                _ = RefreshDataAsync();
                _ = LoadRecipientsAsync();
            }
        }

        private void InitializeComponent()
        {
            this.tabControl = new TabControl();
            this.tabDashboard = new TabPage();
            this.tabRecipients = new TabPage();

            // Setup Tab Control
            this.tabControl.Dock = DockStyle.Fill;
            this.tabControl.Font = new Font("Segoe UI", 9.5F);
            this.tabControl.Controls.Add(this.tabDashboard);
            this.tabControl.Controls.Add(this.tabRecipients);

            // Tab 1: Dashboard
            this.tabDashboard.Text = "🌡️ Kongin Environment Monitor";
            this.tabDashboard.BackColor = Color.FromArgb(20, 22, 27);
            this.tabDashboard.Padding = new Padding(0);

            // Header
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblHeaderTitle = new Label();
            this.lblHeaderSubtitle = new Label();
            this.pnlHeader.BackColor = Color.FromArgb(28, 30, 36);
            this.pnlHeader.Dock = DockStyle.Top;
            this.pnlHeader.Size = new Size(1000, 65);
            this.pnlHeader.Controls.Add(this.lblHeaderSubtitle);
            this.pnlHeader.Controls.Add(this.lblHeaderTitle);

            this.lblHeaderTitle.AutoSize = true;
            this.lblHeaderTitle.Font = new Font("Segoe UI", 13.5F, FontStyle.Bold);
            this.lblHeaderTitle.ForeColor = Color.FromArgb(0, 229, 255);
            this.lblHeaderTitle.Location = new Point(14, 9);
            this.lblHeaderTitle.Text = "🌡️ Kongin Tracker: Temperature & Humidity Live Monitoring";

            this.lblHeaderSubtitle.AutoSize = true;
            this.lblHeaderSubtitle.Font = new Font("Segoe UI", 8.5F);
            this.lblHeaderSubtitle.ForeColor = Color.FromArgb(170, 175, 185);
            this.lblHeaderSubtitle.Location = new Point(16, 36);
            this.lblHeaderSubtitle.Text = "Standard: Temp 23 ± 2°C (21.0 - 25.0°C) | Humidity 55 ± 10% (45.0 - 65.0%). Configurable custom alert interval.";

            // Cards Panel
            this.pnlCards = new System.Windows.Forms.Panel();
            this.pnlCards.BackColor = Color.FromArgb(24, 26, 32);
            this.pnlCards.Dock = DockStyle.Top;
            this.pnlCards.Size = new Size(1000, 95);
            this.pnlCards.Padding = new Padding(15, 8, 15, 8);

            // Card 1: Temp
            this.cardTemp = CreateCard("🌡️ Temperature", out lblTempTitle, out lblTempValue, out lblTempStandard, 15, Color.FromArgb(52, 152, 219));
            lblTempStandard.Text = "Standard: 23 ± 2°C (21 - 25°C)";

            // Card 2: Humidity
            this.cardHum = CreateCard("💧 Humidity", out lblHumTitle, out lblHumValue, out lblHumStandard, 310, Color.FromArgb(46, 204, 113));
            lblHumStandard.Text = "Standard: 55 ± 10% (45 - 65%)";

            // Card 3: Alert Status
            this.cardStatus = CreateCard("🛡️ Threshold Status", out lblStatusTitle, out lblStatusValue, out lblStatusSub, 605, Color.FromArgb(241, 196, 15));
            lblStatusValue.Text = "Normal";
            lblStatusSub.Text = $"Alert interval: {_service.AlertIntervalMinutes} min";

            this.pnlCards.Controls.AddRange(new Control[] { cardTemp, cardHum, cardStatus });

            // Actions Panel
            this.pnlActions = new System.Windows.Forms.Panel();
            this.pnlActions.BackColor = Color.FromArgb(30, 32, 38);
            this.pnlActions.Dock = DockStyle.Top;
            this.pnlActions.Size = new Size(1000, 42);
            this.pnlActions.Padding = new Padding(15, 6, 15, 6);

            this.btnRefresh = new Button
            {
                Text = "↻ Refresh",
                Location = new Point(15, 7),
                Size = new Size(95, 28),
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold),
                UseVisualStyleBackColor = false
            };
            this.btnRefresh.Click += async (s, e) => await RefreshDataAsync();

            this.btnSendTestAlert = new Button
            {
                Text = "⚡ Test Alert",
                Location = new Point(118, 7),
                Size = new Size(100, 28),
                BackColor = Color.FromArgb(230, 126, 34),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold),
                UseVisualStyleBackColor = false
            };
            this.btnSendTestAlert.Click += async (s, e) =>
            {
                if (_service.LatestReading != null)
                {
                    await _service.DispatchEnvironmentAlertAsync(_service.LatestReading);
                    MessageBox.Show("Test environment alert dispatched to Synology & mobile recipients!", "Alert Sent", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("No reading available yet to dispatch.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            // Custom Notification Interval Controls
            this.lblInterval = new Label
            {
                Text = "⏱ Alert Every (min):",
                Location = new Point(230, 12),
                AutoSize = true,
                ForeColor = Color.FromArgb(220, 220, 220),
                Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold)
            };

            this.numAlertInterval = new NumericUpDown
            {
                Location = new Point(360, 10),
                Size = new Size(65, 23),
                Minimum = 1,
                Maximum = 1440,
                Value = _service.AlertIntervalMinutes,
                BackColor = Color.FromArgb(45, 45, 45),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            this.btnSaveInterval = new Button
            {
                Text = "💾 Set Interval",
                Location = new Point(432, 7),
                Size = new Size(105, 28),
                BackColor = Color.FromArgb(46, 160, 90),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold),
                UseVisualStyleBackColor = false
            };
            this.btnSaveInterval.Click += (s, e) =>
            {
                int newMins = (int)numAlertInterval.Value;
                _service.AlertIntervalMinutes = newMins;
                _service.SaveConfig();
                lblStatusSub.Text = $"Alert interval: {newMins} min";
                MessageBox.Show($"Out-of-range notification interval set to every {newMins} minute(s)!", "Interval Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            this.lblLastChecked = new Label
            {
                Location = new Point(555, 12),
                AutoSize = true,
                ForeColor = Color.FromArgb(170, 175, 185),
                Text = "Last Checked: None"
            };

            this.pnlActions.Controls.AddRange(new Control[] {
                btnRefresh, btnSendTestAlert, lblInterval, numAlertInterval, btnSaveInterval, lblLastChecked
            });

            // Grid
            this.dgvReadings = new DataGridView();
            this.dgvReadings.Dock = DockStyle.Fill;
            this.dgvReadings.ReadOnly = true;

            this.tabDashboard.Controls.Add(this.dgvReadings);
            this.tabDashboard.Controls.Add(this.pnlActions);
            this.tabDashboard.Controls.Add(this.pnlCards);
            this.tabDashboard.Controls.Add(this.pnlHeader);

            // Tab 2: Recipients Tab
            this.tabRecipients.Text = "👥 Environment Recipients";
            this.tabRecipients.BackColor = Color.FromArgb(20, 22, 27);
            this.tabRecipients.Padding = new Padding(15);

            this.Controls.Add(this.tabControl);
            this.Size = new Size(1000, 700);
            this.Font = new Font("Segoe UI", 9F);
        }

        private System.Windows.Forms.Panel CreateCard(string title, out Label lblTitle, out Label lblVal, out Label lblSub, int left, Color accent)
        {
            var pnl = new System.Windows.Forms.Panel
            {
                Location = new Point(left, 8),
                Size = new Size(275, 76),
                BackColor = Color.FromArgb(32, 35, 42),
                BorderStyle = BorderStyle.FixedSingle
            };

            lblTitle = new Label
            {
                Text = title,
                ForeColor = accent,
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                Location = new Point(10, 6),
                AutoSize = true
            };

            lblVal = new Label
            {
                Text = "--",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 15F, FontStyle.Bold),
                Location = new Point(8, 26),
                AutoSize = true
            };

            lblSub = new Label
            {
                Text = "--",
                ForeColor = Color.FromArgb(170, 175, 185),
                Font = new Font("Segoe UI", 8F),
                Location = new Point(10, 53),
                AutoSize = true
            };

            pnl.Controls.AddRange(new Control[] { lblTitle, lblVal, lblSub });
            return pnl;
        }

        private void SetupGrid()
        {
            dgvReadings.BackgroundColor = Color.FromArgb(24, 24, 24);
            dgvReadings.GridColor = Color.FromArgb(48, 48, 48);
            dgvReadings.BorderStyle = BorderStyle.None;
            dgvReadings.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvReadings.RowHeadersVisible = false;
            dgvReadings.EnableHeadersVisualStyles = false;
            dgvReadings.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvReadings.MultiSelect = false;

            dgvReadings.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(18, 18, 18);
            dgvReadings.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(0, 229, 255);
            dgvReadings.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            dgvReadings.ColumnHeadersHeight = 34;

            dgvReadings.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            dgvReadings.DefaultCellStyle.ForeColor = Color.FromArgb(235, 235, 235);
            dgvReadings.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgvReadings.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvReadings.RowTemplate.Height = 28;

            dgvReadings.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 24);
            dgvReadings.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(235, 235, 235);

            dgvReadings.Columns.Clear();
            dgvReadings.AutoGenerateColumns = false;

            dgvReadings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", HeaderText = "Log ID", DataPropertyName = "Id", Width = 90 });
            dgvReadings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTime", HeaderText = "Timestamp", DataPropertyName = "DisplayTime", Width = 160 });
            dgvReadings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colLocation", HeaderText = "Location / Sensor", DataPropertyName = "Location", Width = 150 });
            dgvReadings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTemp", HeaderText = "Temperature", DataPropertyName = "DisplayTemp", Width = 130 });
            dgvReadings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colHum", HeaderText = "Humidity", DataPropertyName = "DisplayHum", Width = 130 });
            dgvReadings.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "Condition / Alert Status", DataPropertyName = "AlertDescription", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            dgvReadings.CellFormatting += (s, ev) =>
            {
                if (ev.RowIndex < 0 || ev.RowIndex >= dgvReadings.Rows.Count) return;
                var item = dgvReadings.Rows[ev.RowIndex].DataBoundItem as KonginReadingItem;
                if (item == null) return;

                if (dgvReadings.Columns[ev.ColumnIndex].Name == "colTemp")
                {
                    if (item.IsTempAlert)
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                        ev.CellStyle.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
                    }
                    else
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(46, 204, 113);
                    }
                }
                else if (dgvReadings.Columns[ev.ColumnIndex].Name == "colHum")
                {
                    if (item.IsHumAlert)
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(241, 196, 15);
                        ev.CellStyle.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
                    }
                    else
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(46, 204, 113);
                    }
                }
                else if (dgvReadings.Columns[ev.ColumnIndex].Name == "colStatus")
                {
                    if (item.IsAlert)
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                        ev.CellStyle.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
                    }
                    else
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(170, 175, 185);
                    }
                }
            };
        }

        private void SetupRecipientsControls()
        {
            var field = Color.FromArgb(35, 35, 35);
            var accent = Color.FromArgb(0, 229, 255);

            Label Caption(string text, int x, int y) => new Label
            {
                Text = text, AutoSize = true, Location = new Point(x, y),
                ForeColor = Color.FromArgb(200, 200, 200), Font = new Font("Segoe UI", 8.5F)
            };

            Button Btn(string text, Color color, int x, int y, int w, int h) => new Button
            {
                Text = text, Location = new Point(x, y), Size = new Size(w, h), BackColor = color, ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                UseVisualStyleBackColor = false
            };

            this.tabRecipients.Controls.Add(new Label
            {
                Text = "👥 Environment Alert Recipients (Synology Chat & Push)",
                AutoSize = true, Location = new Point(18, 12),
                ForeColor = accent, Font = new Font("Segoe UI Semibold", 11.5F, FontStyle.Bold)
            });

            this.tabRecipients.Controls.Add(new Label
            {
                Text = "Recipients listed here will receive alerts when QC Lab Temperature (< 21°C or > 25°C) or Humidity (< 45% or > 65%) is out of spec.\n" +
                       "Global 'ALL' recipients will also automatically receive this alert.",
                Location = new Point(20, 42), Size = new Size(880, 36),
                ForeColor = Color.FromArgb(160, 160, 160), Font = new Font("Segoe UI", 8.5F)
            });

            this.tabRecipients.Controls.Add(Caption("Recipients (Synology Chat IDs)", 20, 88));

            this.lstRecipients = new ListView
            {
                Location = new Point(20, 110), Size = new Size(520, 260),
                View = System.Windows.Forms.View.Details, FullRowSelect = true, HideSelection = false, MultiSelect = true,
                BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.FromArgb(235, 235, 235),
                BorderStyle = BorderStyle.FixedSingle, GridLines = false
            };
            this.lstRecipients.Columns.Add("Synology ID", 110);
            this.lstRecipients.Columns.Add("Name (from RMS users)", 380);
            this.tabRecipients.Controls.Add(this.lstRecipients);

            this.tabRecipients.Controls.Add(Caption("Add Synology ID(s)", 565, 88));
            this.txtRecipId = new TextBox
            {
                Location = new Point(565, 110), Size = new Size(210, 24),
                BackColor = field, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, MaxLength = 400
            };
            this.tabRecipients.Controls.Add(this.txtRecipId);

            this.btnRecipAdd = Btn("➕ Add", Color.FromArgb(46, 160, 90), 785, 108, 100, 28);
            this.btnRecipAdd.FlatAppearance.BorderSize = 0;
            this.tabRecipients.Controls.Add(this.btnRecipAdd);

            this.btnRecipRemove = Btn("🗑 Remove selected", Color.FromArgb(150, 60, 60), 565, 150, 200, 30);
            this.btnRecipRemove.FlatAppearance.BorderSize = 0;
            this.tabRecipients.Controls.Add(this.btnRecipRemove);

            this.btnRecipSave = Btn("💾 Save Recipients", Color.FromArgb(0, 120, 215), 20, 385, 190, 34);
            this.btnRecipSave.FlatAppearance.BorderSize = 0;
            this.btnRecipReload = Btn("↻ Reload", Color.FromArgb(60, 60, 60), 220, 385, 140, 34);
            this.btnRecipReload.FlatAppearance.BorderSize = 0;
            this.lblRecipStatus = new Label { Location = new Point(380, 394), Size = new Size(500, 20), ForeColor = Color.FromArgb(180, 180, 180), Text = "" };
            this.tabRecipients.Controls.AddRange(new Control[] { this.btnRecipSave, this.btnRecipReload, this.lblRecipStatus });

            // Events
            this.btnRecipAdd.Click += async (s, e) => await AddRecipientsFromTextAsync();
            this.txtRecipId.KeyDown += async (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await AddRecipientsFromTextAsync(); } };
            this.btnRecipRemove.Click += (s, e) => RemoveSelectedRecipients();
            this.btnRecipSave.Click += async (s, e) => await SaveRecipientsAsync();
            this.btnRecipReload.Click += async (s, e) =>
            {
                if (_recipDirty && MessageBox.Show("Discard unsaved changes and reload?", "Reload", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                await LoadRecipientsAsync();
            };
        }

        private void Service_OnReadingUpdated(KonginReadingItem item)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => Service_OnReadingUpdated(item)));
                return;
            }
            UpdateCards(item);
        }

        private void UpdateCards(KonginReadingItem item)
        {
            if (item == null) return;
            lblTempValue.Text = item.DisplayTemp;
            lblTempValue.ForeColor = item.IsTempAlert ? Color.FromArgb(231, 76, 60) : Color.FromArgb(46, 204, 113);

            lblHumValue.Text = item.DisplayHum;
            lblHumValue.ForeColor = item.IsHumAlert ? Color.FromArgb(241, 196, 15) : Color.FromArgb(46, 204, 113);

            if (item.IsAlert)
            {
                lblStatusValue.Text = "⚠️ OUT OF RANGE";
                lblStatusValue.ForeColor = Color.FromArgb(231, 76, 60);
                lblStatusSub.Text = item.AlertDescription;
            }
            else
            {
                lblStatusValue.Text = "✔ IN SPEC";
                lblStatusValue.ForeColor = Color.FromArgb(46, 204, 113);
                lblStatusSub.Text = $"Alert interval: {_service.AlertIntervalMinutes} min";
            }

            lblLastChecked.Text = $"Last Checked: {item.DisplayTime} (Device #{item.DeviceId})";
        }

        public async Task RefreshDataAsync()
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Func<Task>(RefreshDataAsync));
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                var latest = await _service.CheckLatestReadingAndAlertAsync();
                UpdateCards(latest);

                var recents = await _service.GetRecentReadingsAsync(100);
                dgvReadings.DataSource = null;
                dgvReadings.DataSource = recents;
                if (recents == null || recents.Count == 0)
                {
                    lblStatusSub.Text = "No readings returned from database.";
                }
            }
            catch (Exception ex)
            {
                lblStatusValue.Text = "ERR";
                lblStatusValue.ForeColor = Color.FromArgb(231, 76, 60);
                lblStatusSub.Text = ex.Message;
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private async Task LoadRecipientsAsync()
        {
            try
            {
                lblRecipStatus.Text = "Loading recipients…";
                var ids = await _dispatcher.LoadRecipientsFreshAsync(KonginTrackerService.CategoryEnvironmentAlert);
                _recipIds.Clear();
                _recipIds.AddRange(ids);
                _recipNames = await _dispatcher.ResolveNamesAsync(_recipIds);
                _recipDirty = false;
                RenderRecipients();
                lblRecipStatus.Text = $"{_recipIds.Count} recipient(s) loaded.";
                lblRecipStatus.ForeColor = Color.FromArgb(46, 204, 113);
            }
            catch (Exception ex)
            {
                lblRecipStatus.Text = "Failed to load: " + ex.Message;
                lblRecipStatus.ForeColor = Color.FromArgb(231, 76, 60);
            }
        }

        private void RenderRecipients()
        {
            lstRecipients.BeginUpdate();
            lstRecipients.Items.Clear();
            foreach (string id in _recipIds)
            {
                bool known = _recipNames.TryGetValue(id, out string name) && !string.IsNullOrWhiteSpace(name);
                var item = new ListViewItem(id);
                item.SubItems.Add(known ? name : "⚠ not found in RMS users");
                if (!known) item.ForeColor = Color.FromArgb(255, 193, 7);
                lstRecipients.Items.Add(item);
            }
            lstRecipients.EndUpdate();
        }

        private async Task AddRecipientsFromTextAsync()
        {
            NotificationDispatcherService.ParseSynologyIds(txtRecipId.Text, out var valid, out var invalid);
            if (invalid.Count > 0)
            {
                MessageBox.Show("These are not valid Synology IDs (numbers only):\n\n" + string.Join(", ", invalid), "Invalid ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var added = valid.Where(id => !_recipIds.Contains(id)).ToList();
            if (added.Count == 0) return;

            _recipIds.AddRange(added);
            var names = await _dispatcher.ResolveNamesAsync(added);
            foreach (var kv in names) _recipNames[kv.Key] = kv.Value;
            RenderRecipients();
            _recipDirty = true;
            lblRecipStatus.Text = "● Unsaved changes";
            lblRecipStatus.ForeColor = Color.FromArgb(255, 193, 7);
            txtRecipId.Clear();
        }

        private void RemoveSelectedRecipients()
        {
            if (lstRecipients.SelectedItems.Count == 0) return;
            foreach (ListViewItem it in lstRecipients.SelectedItems.Cast<ListViewItem>().ToList())
                _recipIds.Remove(it.Text);
            RenderRecipients();
            _recipDirty = true;
            lblRecipStatus.Text = "● Unsaved changes";
            lblRecipStatus.ForeColor = Color.FromArgb(255, 193, 7);
        }

        private async Task SaveRecipientsAsync()
        {
            btnRecipSave.Enabled = false;
            try
            {
                string stored = await _dispatcher.SaveRecipientsAsync(KonginTrackerService.CategoryEnvironmentAlert, _recipIds, "HIGH");
                _recipDirty = false;
                lblRecipStatus.Text = $"✔ Saved recipients: {(stored.Length == 0 ? "(nobody)" : stored)}";
                lblRecipStatus.ForeColor = Color.FromArgb(46, 204, 113);
            }
            catch (Exception ex)
            {
                lblRecipStatus.Text = "Save failed: " + ex.Message;
                lblRecipStatus.ForeColor = Color.FromArgb(231, 76, 60);
            }
            finally
            {
                btnRecipSave.Enabled = true;
            }
        }
    }
}
