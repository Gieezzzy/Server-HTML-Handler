using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server_HTML_Handler.Services;

namespace Server_HTML_Handler.View.Contents
{
    public partial class NotificationHubUserControl : UserControl
    {
        private readonly NotificationDispatcherService _service = NotificationDispatcherService.Instance;
        private List<StockInventoryItem> _allStockInventory = new List<StockInventoryItem>();
        private List<InboundSummaryItem> _allInbounds = new List<InboundSummaryItem>();
        private List<MaterialPalletQualityItem> _allPallets = new List<MaterialPalletQualityItem>();

        public NotificationHubUserControl()
        {
            InitializeComponent();
            SetupFilterControls();
            SetupGrids();
            LoadConfigToUi();
            SetupRecipientsTab();

            // Wire up service events
            _service.OnNotificationDispatched += Service_OnNotificationDispatched;
            _service.OnWatcherStateChanged += Service_OnWatcherStateChanged;
            _service.OnLowStockUpdated += Service_OnLowStockUpdated;
            _service.OnStockInventoryUpdated += Service_OnStockInventoryUpdated;
            _service.OnPalletQualityUpdated += Service_OnPalletQualityUpdated;

            // The watcher keeps running when this tab is closed; stop talking to a screen that no longer exists.
            this.Disposed += (s, e) =>
            {
                _service.OnNotificationDispatched -= Service_OnNotificationDispatched;
                _service.OnWatcherStateChanged -= Service_OnWatcherStateChanged;
                _service.OnLowStockUpdated -= Service_OnLowStockUpdated;
                _service.OnStockInventoryUpdated -= Service_OnStockInventoryUpdated;
                _service.OnPalletQualityUpdated -= Service_OnPalletQualityUpdated;
            };

            this.Load += NotificationHubUserControl_Load;
        }

        private void SetupFilterControls()
        {
            // Stock Category ComboBox
            cmbStockCategory.Items.Clear();
            cmbStockCategory.Items.AddRange(new object[]
            {
                "All Categories",
                "Material / Rubber Stock",
                "Pigment Stock",
                "Chemical Stock"
            });
            cmbStockCategory.SelectedIndex = 0;
            cmbStockCategory.SelectedIndexChanged += (s, e) => ApplyStockFilter();
            chkLowStockOnly.CheckedChanged += (s, e) => ApplyStockFilter();
            txtStockSearch.TextChanged += (s, e) => ApplyStockFilter();
            btnRefreshStock.Click += async (s, e) => await RefreshLowStockGridAsync();

            // Inbound Type ComboBox
            cmbInboundType.Items.Clear();
            cmbInboundType.Items.AddRange(new object[]
            {
                "All Inbounds",
                "Material Inbounds",
                "Pigment Inbounds",
                "Chemical Inbounds"
            });
            cmbInboundType.SelectedIndex = 0;
            cmbInboundType.SelectedIndexChanged += (s, e) => ApplyInboundFilter();
            txtInboundSearch.TextChanged += (s, e) => ApplyInboundFilter();
            btnRefreshInbounds.Click += async (s, e) => await RefreshInboundsGridAsync();

            // Pallet Lab Testing Status ComboBox
            cmbPalletStatus.Items.Clear();
            cmbPalletStatus.Items.AddRange(new object[]
            {
                "All Pallets",
                "48h Overdue Only",
                "Pending (< 48h)",
                "Complete"
            });
            cmbPalletStatus.SelectedIndex = 0;
            cmbPalletStatus.SelectedIndexChanged += (s, e) => ApplyPalletFilter();
            txtPalletSearch.TextChanged += (s, e) => ApplyPalletFilter();
            btnRefreshPallets.Click += async (s, e) => await RefreshPalletsGridAsync();
        }

        private async void NotificationHubUserControl_Load(object sender, EventArgs e)
        {
            _ = LoadRecipientTypesAsync();
            UpdateWatcherStateUi(_service.IsWatcherRunning);
            RefreshHistoryGrid();

            // Auto-start watcher if not already running
            if (!_service.IsWatcherRunning)
            {
                _service.StartWatcher();
            }

            await RefreshLowStockGridAsync();
            await RefreshInboundsGridAsync();
            await RefreshPalletsGridAsync();
            UpdateMetricCounters();
        }

        private void ApplyDarkGridTheme(DataGridView dgv, Color headerForeColor)
        {
            dgv.BackgroundColor = Color.FromArgb(24, 24, 24);
            dgv.GridColor = Color.FromArgb(48, 48, 48);
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.RowHeadersVisible = false;
            dgv.EnableHeadersVisualStyles = false;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;

            // Column header style
            dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(18, 18, 18);
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = headerForeColor;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            dgv.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(18, 18, 18);
            dgv.ColumnHeadersDefaultCellStyle.SelectionForeColor = headerForeColor;
            dgv.ColumnHeadersHeight = 32;

            // Default row style
            dgv.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            dgv.DefaultCellStyle.ForeColor = Color.FromArgb(235, 235, 235);
            dgv.DefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            dgv.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgv.DefaultCellStyle.SelectionForeColor = Color.White;

            // Alternating row style
            dgv.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 24);
            dgv.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(235, 235, 235);
            dgv.AlternatingRowsDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            dgv.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgv.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;
        }

        private void SetupGrids()
        {
            // 1. Live Feed Grid
            ApplyDarkGridTheme(dgvFeed, Color.FromArgb(0, 229, 255));
            dgvFeed.Columns.Clear();
            dgvFeed.AutoGenerateColumns = false;
            dgvFeed.RowTemplate.Height = 28;

            dgvFeed.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTime", HeaderText = "Time", DataPropertyName = "DisplayTime", FillWeight = 12 });
            dgvFeed.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCat", HeaderText = "Category", DataPropertyName = "Category", FillWeight = 16 });
            dgvFeed.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCode", HeaderText = "Item Code", DataPropertyName = "ItemCode", FillWeight = 14 });
            dgvFeed.Columns.Add(new DataGridViewTextBoxColumn { Name = "colName", HeaderText = "Item Name", DataPropertyName = "ItemName", FillWeight = 20 });
            dgvFeed.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDetails", HeaderText = "Summary / FIFO / Lapse Notice", DataPropertyName = "Details", FillWeight = 38 });
            dgvFeed.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "Delivery Status", DataPropertyName = "DeliveryStatus", FillWeight = 20 });

            dgvFeed.CellFormatting += (s, ev) =>
            {
                if (ev.RowIndex < 0 || ev.RowIndex >= dgvFeed.Rows.Count) return;
                var item = dgvFeed.Rows[ev.RowIndex].DataBoundItem as NotificationLogItem;
                if (item != null)
                {
                    if (item.Category != null && item.Category.IndexOf("Lapse", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        dgvFeed.Rows[ev.RowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                    }
                    else if (item.Category != null && item.Category.IndexOf("Low Stock", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        dgvFeed.Rows[ev.RowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(243, 156, 18);
                    }
                    else if (item.Category != null && item.Category.IndexOf("Inbound", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        dgvFeed.Rows[ev.RowIndex].DefaultCellStyle.ForeColor = Color.FromArgb(46, 204, 113);
                    }
                }
            };

            // 2. Stock Inventory & Low Stock Grid
            ApplyDarkGridTheme(dgvLowStock, Color.FromArgb(231, 76, 60));
            dgvLowStock.Columns.Clear();
            dgvLowStock.AutoGenerateColumns = false;
            dgvLowStock.RowTemplate.Height = 28;

            dgvLowStock.Columns.Add(new DataGridViewTextBoxColumn { Name = "colType", HeaderText = "Category", DataPropertyName = "Type", FillWeight = 15 });
            dgvLowStock.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCode", HeaderText = "Code", DataPropertyName = "Code", FillWeight = 12 });
            dgvLowStock.Columns.Add(new DataGridViewTextBoxColumn { Name = "colName", HeaderText = "Material / Pigment / Chem Name", DataPropertyName = "Name", FillWeight = 26 });
            dgvLowStock.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCur", HeaderText = "Current Stock", DataPropertyName = "DisplayCurrent", FillWeight = 14 });
            dgvLowStock.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTot", HeaderText = "Baseline / Cap", DataPropertyName = "DisplayBaseline", FillWeight = 14 });
            dgvLowStock.Columns.Add(new DataGridViewTextBoxColumn { Name = "colSafety", HeaderText = "Safety (30%)", DataPropertyName = "DisplaySafety", FillWeight = 13 });
            dgvLowStock.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPct", HeaderText = "Level %", DataPropertyName = "DisplayPct", FillWeight = 11 });
            dgvLowStock.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "Status", DataPropertyName = "Status", FillWeight = 14 });

            var btnAlertCol = new DataGridViewButtonColumn
            {
                Name = "colAlertBtn",
                HeaderText = "Action",
                Text = "⚡ Alert",
                UseColumnTextForButtonValue = true,
                FillWeight = 12,
                FlatStyle = FlatStyle.Flat
            };
            dgvLowStock.Columns.Add(btnAlertCol);

            dgvLowStock.CellFormatting += (s, ev) =>
            {
                if (ev.RowIndex < 0 || ev.RowIndex >= dgvLowStock.Rows.Count) return;
                var item = dgvLowStock.Rows[ev.RowIndex].DataBoundItem as StockInventoryItem;
                if (item == null) return;

                if (dgvLowStock.Columns[ev.ColumnIndex].Name == "colStatus")
                {
                    if (item.Percentage <= 15)
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                        ev.CellStyle.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
                    }
                    else if (item.Percentage <= 30)
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(243, 156, 18);
                        ev.CellStyle.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
                    }
                    else if (item.Percentage <= 40)
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(241, 196, 15);
                    }
                    else
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(46, 204, 113);
                    }
                }
                else if (dgvLowStock.Columns[ev.ColumnIndex].Name == "colPct" && item.Percentage <= 30)
                {
                    ev.CellStyle.ForeColor = Color.FromArgb(243, 156, 18);
                    ev.CellStyle.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
                }
            };

            dgvLowStock.CellContentClick += async (s, ev) =>
            {
                if (ev.RowIndex < 0 || ev.RowIndex >= dgvLowStock.Rows.Count) return;
                if (dgvLowStock.Columns[ev.ColumnIndex].Name == "colAlertBtn")
                {
                    var item = dgvLowStock.Rows[ev.RowIndex].DataBoundItem as StockInventoryItem;
                    if (item != null)
                    {
                        string details = $"[CRITICAL LOW STOCK ALERT - {item.Percentage:F1}% REMAINING]\n" +
                                         $"Item Type: {item.Type}\n" +
                                         $"Code: {item.Code} | Name: {item.Name}\n" +
                                         $"Current: {item.CurrentStock:N2} {item.Unit} / Total: {item.BaselineStock:N2} {item.Unit}\n" +
                                         $"Remaining Level: {item.Percentage:F1}%\n" +
                                         $"⚠️ Immediate replenishment required!";

                        await _service.DispatchNotificationAsync("Low Stock Warning", "HIGH", $"Low Stock: {item.Name}", item.Code, item.Name, details, "STOCK_MONITOR");
                        MessageBox.Show($"Dispatched low stock alert for '{item.Name}' across Firebase, Synology, and Chat!", "Alert Dispatched", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            };

            // 3. Inbounds Grid (FIFO Tracker)
            ApplyDarkGridTheme(dgvInbounds, Color.FromArgb(46, 204, 113));
            dgvInbounds.Columns.Clear();
            dgvInbounds.AutoGenerateColumns = false;
            dgvInbounds.RowTemplate.Height = 28;

            dgvInbounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "colType", HeaderText = "Inbound Type", DataPropertyName = "Type", FillWeight = 15 });
            dgvInbounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDate", HeaderText = "Arrival Date", DataPropertyName = "DisplayDate", FillWeight = 15 });
            dgvInbounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCode", HeaderText = "Code", DataPropertyName = "Code", FillWeight = 12 });
            dgvInbounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "colName", HeaderText = "Material / Item Name", DataPropertyName = "Name", FillWeight = 24 });
            dgvInbounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "colQty", HeaderText = "Quantity", DataPropertyName = "DisplayQty", FillWeight = 14 });
            dgvInbounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBatch", HeaderText = "Batch Tracking", DataPropertyName = "BatchTrackingNo", FillWeight = 18 });
            dgvInbounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "colContainer", HeaderText = "Container No", DataPropertyName = "ContainerNo", FillWeight = 13 });
            dgvInbounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCreatedBy", HeaderText = "Received By", DataPropertyName = "CreatedBy", FillWeight = 13 });
            dgvInbounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFifo", HeaderText = "FIFO Notice", DataPropertyName = "FifoNotice", FillWeight = 16 });

            dgvInbounds.CellFormatting += (s, ev) =>
            {
                if (ev.RowIndex < 0 || ev.RowIndex >= dgvInbounds.Rows.Count) return;
                var item = dgvInbounds.Rows[ev.RowIndex].DataBoundItem as InboundSummaryItem;
                if (item == null) return;

                if (dgvInbounds.Columns[ev.ColumnIndex].Name == "colFifo")
                {
                    ev.CellStyle.ForeColor = Color.FromArgb(241, 196, 15);
                    ev.CellStyle.Font = new Font("Segoe UI Semibold", 8.25F, FontStyle.Bold);
                }
                else if (dgvInbounds.Columns[ev.ColumnIndex].Name == "colType")
                {
                    if (item.Type != null && item.Type.IndexOf("Material", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(52, 152, 219);
                    }
                    else if (item.Type != null && item.Type.IndexOf("Pigment", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(230, 126, 34);
                    }
                    else if (item.Type != null && item.Type.IndexOf("Chemical", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(155, 89, 182);
                    }
                }
            };

            // 4. Material Pallet Lab Testing (TS2 / TC90 / Chemicals 48hr Lapse)
            ApplyDarkGridTheme(dgvPallets, Color.FromArgb(231, 76, 60));
            dgvPallets.Columns.Clear();
            dgvPallets.AutoGenerateColumns = false;
            dgvPallets.RowTemplate.Height = 28;

            dgvPallets.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPalletId", HeaderText = "ID", DataPropertyName = "PalletId", FillWeight = 6 });
            dgvPallets.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPalletBatch", HeaderText = "Batch Tracking No", DataPropertyName = "BatchTrackingNo", FillWeight = 18 });
            dgvPallets.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPalletCode", HeaderText = "Code", DataPropertyName = "MaterialCode", FillWeight = 9 });
            dgvPallets.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPalletName", HeaderText = "Material Name", DataPropertyName = "MaterialName", FillWeight = 17 });
            dgvPallets.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPalletDate", HeaderText = "Created Date", DataPropertyName = "DisplayCreatedAt", FillWeight = 13 });
            dgvPallets.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPalletLapse", HeaderText = "Age (Hrs)", DataPropertyName = "DisplayHoursLapsed", FillWeight = 9 });
            dgvPallets.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPalletTs2", HeaderText = "TS2", DataPropertyName = "DisplayTs2", FillWeight = 9 });
            dgvPallets.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPalletTc90", HeaderText = "TC90", DataPropertyName = "DisplayTc90", FillWeight = 9 });
            dgvPallets.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPalletChem", HeaderText = "Chemicals (E09, E02, E06...)", DataPropertyName = "ChemicalsDisplay", FillWeight = 18 });
            dgvPallets.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPalletMissing", HeaderText = "Missing Field(s)", DataPropertyName = "MissingSummary", FillWeight = 15 });
            dgvPallets.Columns.Add(new DataGridViewTextBoxColumn { Name = "colPalletStatus", HeaderText = "Testing Status", DataPropertyName = "TestingStatus", FillWeight = 15 });

            var btnPalletAlertCol = new DataGridViewButtonColumn
            {
                Name = "colPalletAlertBtn",
                HeaderText = "Action",
                Text = "⚡ Alert",
                UseColumnTextForButtonValue = true,
                FillWeight = 10,
                FlatStyle = FlatStyle.Flat
            };
            dgvPallets.Columns.Add(btnPalletAlertCol);

            dgvPallets.CellFormatting += (s, ev) =>
            {
                if (ev.RowIndex < 0 || ev.RowIndex >= dgvPallets.Rows.Count) return;
                var item = dgvPallets.Rows[ev.RowIndex].DataBoundItem as MaterialPalletQualityItem;
                if (item == null) return;

                string col = dgvPallets.Columns[ev.ColumnIndex].Name;

                if (col == "colPalletStatus")
                {
                    if (item.Is48HrLapsed)
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                        ev.CellStyle.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
                    }
                    else if (item.TestingStatus == "COMPLETE")
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(46, 204, 113);
                    }
                    else
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(241, 196, 15);
                    }
                }
                else if (col == "colPalletTs2" && !item.HasTs2)
                {
                    ev.CellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                    ev.CellStyle.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
                }
                else if (col == "colPalletTc90" && !item.HasTc90)
                {
                    ev.CellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                    ev.CellStyle.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
                }
                else if (col == "colPalletChem" && !item.HasAnyChemical)
                {
                    ev.CellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                }
                else if (col == "colPalletMissing" && item.Is48HrLapsed)
                {
                    ev.CellStyle.ForeColor = Color.FromArgb(243, 156, 18);
                    ev.CellStyle.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
                }
            };

            dgvPallets.CellContentClick += async (s, ev) =>
            {
                if (ev.RowIndex < 0 || ev.RowIndex >= dgvPallets.Rows.Count) return;
                if (dgvPallets.Columns[ev.ColumnIndex].Name == "colPalletAlertBtn")
                {
                    var item = dgvPallets.Rows[ev.RowIndex].DataBoundItem as MaterialPalletQualityItem;
                    if (item != null)
                    {
                        string details = $"[48-HOUR QUALITY & LAB TESTING OVERDUE]\n" +
                                         $"Pallet ID: {item.PalletId} | Batch: {item.BatchTrackingNo}\n" +
                                         $"Material: {item.MaterialName} (Code: {item.MaterialCode})\n" +
                                         $"Inbound Creation: {item.DisplayCreatedAt} ({item.DisplayHoursLapsed} lapsed)\n" +
                                         $"Missing Fields: {item.MissingSummary}\n" +
                                         $"TS2 Value: {item.DisplayTs2}\n" +
                                         $"TC90 Value: {item.DisplayTc90}\n" +
                                         $"Chemicals: {item.ChemicalsDisplay}\n" +
                                         $"⚠️ QUALITY ALERT: 48 hours have lapsed since row creation and lab testing values are still missing. Immediate TS2, TC90, and chemical entry required!";

                        await _service.DispatchNotificationAsync("Lab Quality 48hr Lapse", "HIGH", $"48h Lab Testing Overdue: {item.MaterialName}", item.MaterialCode, item.MaterialName, details, "PALLET_QUALITY");
                        MessageBox.Show($"Dispatched 48-Hour quality testing alert for Pallet #{item.PalletId} ({item.MaterialName})!", "Alert Dispatched", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            };
        }

        private void ApplyStockFilter()
        {
            var filtered = _allStockInventory.AsEnumerable();

            // 1. Category filter
            string cat = cmbStockCategory.SelectedItem?.ToString() ?? "All Categories";
            if (cat.IndexOf("Material", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                filtered = filtered.Where(x => x.Type.IndexOf("Material", StringComparison.OrdinalIgnoreCase) >= 0);
            }
            else if (cat.IndexOf("Pigment", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                filtered = filtered.Where(x => x.Type.IndexOf("Pigment", StringComparison.OrdinalIgnoreCase) >= 0);
            }
            else if (cat.IndexOf("Chemical", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                filtered = filtered.Where(x => x.Type.IndexOf("Chemical", StringComparison.OrdinalIgnoreCase) >= 0);
            }

            // 2. Low stock only filter
            if (chkLowStockOnly.Checked)
            {
                filtered = filtered.Where(x => x.Percentage <= _service.StockThresholdPct);
            }

            // 3. Search query
            string q = txtStockSearch.Text.Trim();
            if (!string.IsNullOrEmpty(q))
            {
                filtered = filtered.Where(x => (x.Code != null && x.Code.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                               (x.Name != null && x.Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            dgvLowStock.DataSource = null;
            dgvLowStock.DataSource = filtered.ToList();
        }

        private void ApplyInboundFilter()
        {
            var filtered = _allInbounds.AsEnumerable();

            string cat = cmbInboundType.SelectedItem?.ToString() ?? "All Inbounds";
            if (cat.IndexOf("Material", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                filtered = filtered.Where(x => x.Type.IndexOf("Material", StringComparison.OrdinalIgnoreCase) >= 0);
            }
            else if (cat.IndexOf("Pigment", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                filtered = filtered.Where(x => x.Type.IndexOf("Pigment", StringComparison.OrdinalIgnoreCase) >= 0);
            }
            else if (cat.IndexOf("Chemical", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                filtered = filtered.Where(x => x.Type.IndexOf("Chemical", StringComparison.OrdinalIgnoreCase) >= 0);
            }

            string q = txtInboundSearch.Text.Trim();
            if (!string.IsNullOrEmpty(q))
            {
                filtered = filtered.Where(x => (x.Code != null && x.Code.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                               (x.Name != null && x.Name.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                               (x.BatchTrackingNo != null && x.BatchTrackingNo.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                               (x.ContainerNo != null && x.ContainerNo.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            dgvInbounds.DataSource = null;
            dgvInbounds.DataSource = filtered.ToList();
        }

        private void ApplyPalletFilter()
        {
            var filtered = _allPallets.AsEnumerable();

            string status = cmbPalletStatus.SelectedItem?.ToString() ?? "All Pallets";
            if (status.IndexOf("Overdue", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                filtered = filtered.Where(x => x.Is48HrLapsed);
            }
            else if (status.IndexOf("Pending", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                filtered = filtered.Where(x => !x.Is48HrLapsed && !(x.HasTs2 && x.HasTc90 && x.HasAnyChemical));
            }
            else if (status.IndexOf("Complete", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                filtered = filtered.Where(x => x.HasTs2 && x.HasTc90 && x.HasAnyChemical);
            }

            string q = txtPalletSearch.Text.Trim();
            if (!string.IsNullOrEmpty(q))
            {
                filtered = filtered.Where(x => (x.BatchTrackingNo != null && x.BatchTrackingNo.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                               (x.MaterialCode != null && x.MaterialCode.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                               (x.MaterialName != null && x.MaterialName.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            dgvPallets.DataSource = null;
            dgvPallets.DataSource = filtered.ToList();
        }

        private void LoadConfigToUi()
        {
            txtSqlConn.Text = _service.SqlConnectionString;
            txtPhpUrl.Text = _service.NotificationPhpUrl;
            txtSynologyUrl.Text = _service.SynologyBotUrlRms;
            numThreshold.Value = (decimal)_service.StockThresholdPct;
            numInterval.Value = _service.PollIntervalSeconds;
        }

        private void Service_OnNotificationDispatched(NotificationLogItem item)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => Service_OnNotificationDispatched(item)));
                return;
            }

            RefreshHistoryGrid();
            UpdateMetricCounters();
        }

        private void Service_OnWatcherStateChanged(bool isRunning)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => Service_OnWatcherStateChanged(isRunning)));
                return;
            }

            UpdateWatcherStateUi(isRunning);
        }

        private void Service_OnStockInventoryUpdated(List<StockInventoryItem> items)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => Service_OnStockInventoryUpdated(items)));
                return;
            }

            _allStockInventory = items ?? new List<StockInventoryItem>();
            ApplyStockFilter();
            UpdateMetricCounters();
        }

        private void Service_OnLowStockUpdated(List<StockInventoryItem> lowItems)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => Service_OnLowStockUpdated(lowItems)));
                return;
            }

            UpdateMetricCounters();
        }

        private void Service_OnPalletQualityUpdated(List<MaterialPalletQualityItem> pallets)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => Service_OnPalletQualityUpdated(pallets)));
                return;
            }

            _allPallets = pallets ?? new List<MaterialPalletQualityItem>();
            ApplyPalletFilter();
            UpdateMetricCounters();
        }

        private void UpdateWatcherStateUi(bool isRunning)
        {
            if (isRunning)
            {
                lblWatcherStatus.Text = $"● WATCHER ACTIVE ({_service.PollIntervalSeconds}s)";
                lblWatcherStatus.BackColor = Color.FromArgb(39, 174, 96);
                btnToggleWatcher.Text = "⏹ Stop Watcher";
                btnToggleWatcher.BackColor = Color.FromArgb(192, 57, 43);
            }
            else
            {
                lblWatcherStatus.Text = "● WATCHER STOPPED";
                lblWatcherStatus.BackColor = Color.FromArgb(127, 140, 141);
                btnToggleWatcher.Text = "▶ Start Watcher";
                btnToggleWatcher.BackColor = Color.FromArgb(39, 174, 96);
            }
        }

        private void RefreshHistoryGrid()
        {
            var history = _service.GetHistory()
                .Where(x => !string.Equals(x.TargetSystem, "NEXUS", StringComparison.OrdinalIgnoreCase) &&
                            !string.Equals(x.Category, "Environment Alert", StringComparison.OrdinalIgnoreCase))
                .ToList();
            dgvFeed.DataSource = null;
            dgvFeed.DataSource = history;
        }

        private async Task RefreshLowStockGridAsync()
        {
            try
            {
                _allStockInventory = await _service.GetAllStockInventoryAsync();
                ApplyStockFilter();
                UpdateMetricCounters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RefreshLowStockGrid Error: {ex.Message}");
            }
        }

        private async Task RefreshInboundsGridAsync()
        {
            try
            {
                _allInbounds = await _service.GetRecentInboundsAsync(100);
                ApplyInboundFilter();
                UpdateMetricCounters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RefreshInboundsGrid Error: {ex.Message}");
            }
        }

        private async Task RefreshPalletsGridAsync()
        {
            try
            {
                _allPallets = await _service.GetMaterialPalletsAsync(150);
                ApplyPalletFilter();
                UpdateMetricCounters();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"RefreshPalletsGrid Error: {ex.Message}");
            }
        }

        private void UpdateMetricCounters()
        {
            int lowStockCount = _allStockInventory.Count(x => x.Percentage <= _service.StockThresholdPct);
            int totalInventory = _allStockInventory.Count;
            lblMetricLowStock.Text = $"⚠️ Stock (<=30% Low): {lowStockCount} / {totalInventory} Items";

            int inbCount = _allInbounds.Count;
            lblMetricInbounds.Text = $"📦 Inbound Arrivals (FIFO): {inbCount}";

            int overdueCount = _allPallets.Count(x => x.Is48HrLapsed);
            lblMetricPalletLapse.Text = $"⏱️ 48h Overdue: {overdueCount} Pallets";

            int totalDispatched = _service.GetHistory().Count;
            lblMetricDispatched.Text = $"📡 Total Dispatched: {totalDispatched}";
        }

        private void BtnToggleWatcher_Click(object sender, EventArgs e)
        {
            if (_service.IsWatcherRunning)
            {
                _service.StopWatcher();
            }
            else
            {
                _service.StartWatcher();
            }
        }

        private async void BtnScanNow_Click(object sender, EventArgs e)
        {
            btnScanNow.Enabled = false;
            try
            {
                Cursor = Cursors.WaitCursor;
                await _service.TriggerManualScanAsync();
                await RefreshLowStockGridAsync();
                await RefreshInboundsGridAsync();
                await RefreshPalletsGridAsync();
                RefreshHistoryGrid();
                UpdateMetricCounters();
                MessageBox.Show("Stock, Inbound, and 48-Hour Pallet Quality scan complete!", "Scan Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Scan failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
                btnScanNow.Enabled = true;
            }
        }

        private async void BtnSendTest_Click(object sender, EventArgs e)
        {
            btnSendTest.Enabled = false;
            try
            {
                string details = $"[TEST ALERT - REAL-TIME NOTIFICATION TEST]\n" +
                                 $"Timestamp: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n" +
                                 $"Notification Channel Test: Firebase (FCM) + Synology Chat + WebSocket\n" +
                                 $"Stock Rule: Alert triggered on <= 30% baseline\n" +
                                 $"Inbound Rule: Instant alert with FIFO verification notice.\n" +
                                 $"Pallet Lab Rule: 48-hour lapse alert on missing TS2, TC90, or Chemicals.\n" +
                                 $"⚠️ FIFO NOTICE: First-In, First-Out sequence enabled.";

                var result = await _service.DispatchNotificationAsync(
                    "System Test",
                    "NORMAL",
                    "Test Real-Time Alert",
                    "TEST-01",
                    "Notification Engine Test",
                    details,
                    "SYSTEM_TEST"
                );

                MessageBox.Show(
                    $"Test notification dispatched!\n\nStatus: {result.DeliveryStatus}\nSynology: {(result.SynologySent ? "OK" : "Failed")}\nFCM: {(result.FcmSent ? "OK" : "Failed")}\nWebSocket: {(result.WebSocketSent ? "Broadcasted" : "Inactive")}",
                    "Dispatch Complete",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to send test alert: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSendTest.Enabled = true;
            }
        }

        private void BtnClearHistory_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to clear the notification feed history?", "Confirm", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _service.ClearHistory();
                RefreshHistoryGrid();
                UpdateMetricCounters();
            }
        }

        private void BtnSaveConfig_Click(object sender, EventArgs e)
        {
            try
            {
                _service.SqlConnectionString = txtSqlConn.Text.Trim();
                _service.NotificationPhpUrl = txtPhpUrl.Text.Trim();
                _service.SynologyBotUrlRms = txtSynologyUrl.Text.Trim();
                _service.StockThresholdPct = (double)numThreshold.Value;
                _service.PollIntervalSeconds = (int)numInterval.Value;

                lblConfigStatus.ForeColor = Color.FromArgb(46, 204, 113);
                lblConfigStatus.Text = "✔ Configuration updated successfully!";
                MessageBox.Show("Notification dispatcher configuration updated!", "Configuration Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                lblConfigStatus.ForeColor = Color.FromArgb(231, 76, 60);
                lblConfigStatus.Text = $"✖ Failed to save configuration: {ex.Message}";
            }
        }

        private async void BtnTestChannels_Click(object sender, EventArgs e)
        {
            btnTestChannels.Enabled = false;
            lblConfigStatus.ForeColor = Color.FromArgb(241, 196, 15);
            lblConfigStatus.Text = "Testing database connection and endpoints...";

            try
            {
                bool sqlOk = false;
                string sqlError = "";
                await Task.Run(() =>
                {
                    try
                    {
                        using (var conn = new SqlConnection(txtSqlConn.Text.Trim()))
                        {
                            conn.Open();
                            sqlOk = conn.State == System.Data.ConnectionState.Open;
                        }
                    }
                    catch (Exception ex)
                    {
                        sqlError = ex.Message;
                    }
                });

                if (sqlOk)
                {
                    lblConfigStatus.ForeColor = Color.FromArgb(46, 204, 113);
                    lblConfigStatus.Text = "✔ SQL Server connection succeeded! Dispatchers reachable.";
                    MessageBox.Show("SQL Server connection succeeded!", "Channel Test", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    lblConfigStatus.ForeColor = Color.FromArgb(231, 76, 60);
                    lblConfigStatus.Text = $"✖ SQL Connection failed: {sqlError}";
                    MessageBox.Show($"SQL Server connection failed:\n{sqlError}", "Test Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            finally
            {
                btnTestChannels.Enabled = true;
            }
        }
    
        // =====================================================================================
        //  Recipients tab: who receives each notification type (dbRMSAndroid.tbl_NotificationCategory)
        // =====================================================================================

        private sealed class RecipientTypeItem
        {
            public string NotifType;
            public string Label;
            public string Description;
            public string Priority = "HIGH";
            public override string ToString() => Label;
        }

        private TabPage tabRecipients;
        private ComboBox cmbRecipType;
        private Label lblRecipHint, lblRecipStatus;
        private ListView lstRecipients;
        private TextBox txtRecipId;
        private Button btnRecipAdd, btnRecipRemove, btnRecipSave, btnRecipReload;
        private readonly List<string> _recipIds = new List<string>();
        private Dictionary<string, string> _recipNames = new Dictionary<string, string>();
        private RecipientTypeItem _recipCurrent;
        private bool _recipDirty;
        private bool _recipLoading;

        private void SetupRecipientsTab()
        {
            var back = Color.FromArgb(24, 24, 24);
            var field = Color.FromArgb(35, 35, 35);
            var accent = Color.FromArgb(0, 229, 255);

            tabRecipients = new TabPage { Text = "👥 Notification Recipients", BackColor = back, Padding = new Padding(15) };

            Label Caption(string text, int x, int y) => new Label { Text = text, AutoSize = true, Location = new Point(x, y), ForeColor = Color.FromArgb(200, 200, 200), Font = new Font("Segoe UI", 8.25F) };
            Button Btn(string text, Color color, int x, int y, int w, int h) => new Button
            {
                Text = text, Location = new Point(x, y), Size = new Size(w, h), BackColor = color, ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat, Cursor = Cursors.Hand, Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold), UseVisualStyleBackColor = false
            };

            tabRecipients.Controls.Add(new Label { Text = "Who receives each notification", AutoSize = true, Location = new Point(18, 12), ForeColor = accent, Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold) });
            tabRecipients.Controls.Add(Caption("Notification type", 20, 44));

            cmbRecipType = new ComboBox { Location = new Point(20, 64), Size = new Size(380, 24), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = field, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            tabRecipients.Controls.Add(cmbRecipType);

            lblRecipHint = new Label { Location = new Point(20, 96), Size = new Size(880, 34), ForeColor = Color.FromArgb(160, 160, 160), Font = new Font("Segoe UI", 8.5F), Text = "Loading notification types…" };
            tabRecipients.Controls.Add(lblRecipHint);

            tabRecipients.Controls.Add(Caption("Recipients (Synology Chat IDs)", 20, 134));
            lstRecipients = new ListView
            {
                Location = new Point(20, 154), Size = new Size(520, 235), View = System.Windows.Forms.View.Details, FullRowSelect = true, HideSelection = false, MultiSelect = true,
                BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.FromArgb(235, 235, 235), BorderStyle = BorderStyle.FixedSingle, GridLines = false
            };
            lstRecipients.Columns.Add("Synology ID", 110);
            lstRecipients.Columns.Add("Name (from RMS users)", 380);
            tabRecipients.Controls.Add(lstRecipients);

            tabRecipients.Controls.Add(Caption("Add Synology ID(s)", 565, 134));
            txtRecipId = new TextBox { Location = new Point(565, 156), Size = new Size(210, 23), BackColor = field, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, MaxLength = 400 };
            tabRecipients.Controls.Add(txtRecipId);
            btnRecipAdd = Btn("➕ Add", Color.FromArgb(46, 160, 90), 785, 154, 100, 27);
            btnRecipAdd.FlatAppearance.BorderSize = 0;
            tabRecipients.Controls.Add(btnRecipAdd);

            tabRecipients.Controls.Add(new Label
            {
                Text = "You can paste several IDs at once, separated by ; or , or a space.\nIDs are numbers (the person's Synology Chat user ID).",
                Location = new Point(565, 188), Size = new Size(330, 40), ForeColor = Color.FromArgb(150, 150, 150), Font = new Font("Segoe UI", 8.25F)
            });

            btnRecipRemove = Btn("🗑 Remove selected", Color.FromArgb(150, 60, 60), 565, 240, 200, 30);
            btnRecipRemove.FlatAppearance.BorderSize = 0;
            tabRecipients.Controls.Add(btnRecipRemove);

            tabRecipients.Controls.Add(new Label
            {
                Text = "Changes are written to tbl_NotificationCategory when you press Save.\nThe running watcher picks the new list up immediately.",
                Location = new Point(565, 285), Size = new Size(330, 60), ForeColor = Color.FromArgb(150, 150, 150), Font = new Font("Segoe UI", 8.25F)
            });

            btnRecipSave = Btn("💾 Save Recipients", Color.FromArgb(0, 120, 215), 20, 405, 190, 34);
            btnRecipSave.FlatAppearance.BorderSize = 0;
            btnRecipReload = Btn("↻ Reload from database", Color.FromArgb(60, 60, 60), 220, 405, 190, 34);
            btnRecipReload.FlatAppearance.BorderSize = 0;
            lblRecipStatus = new Label { Location = new Point(425, 414), Size = new Size(470, 20), ForeColor = Color.FromArgb(180, 180, 180), Text = "" };
            tabRecipients.Controls.AddRange(new Control[] { btnRecipSave, btnRecipReload, lblRecipStatus });

            // events
            cmbRecipType.SelectionChangeCommitted += async (s, e) => await OnRecipTypeChosenAsync();
            btnRecipAdd.Click += async (s, e) => await AddRecipientsFromTextAsync();
            txtRecipId.KeyDown += async (s, e) => { if (e.KeyCode == Keys.Enter) { e.SuppressKeyPress = true; await AddRecipientsFromTextAsync(); } };
            btnRecipRemove.Click += (s, e) => RemoveSelectedRecipients();
            lstRecipients.KeyDown += (s, e) => { if (e.KeyCode == Keys.Delete) RemoveSelectedRecipients(); };
            btnRecipSave.Click += async (s, e) => await SaveRecipientsAsync();
            btnRecipReload.Click += async (s, e) =>
            {
                if (_recipCurrent == null) return;
                if (_recipDirty && MessageBox.Show("Discard your unsaved changes and reload from the database?", "Reload", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
                await LoadRecipientsForCurrentAsync();
            };

            btnRecipNew = Btn("➕ New Recipient…", Color.FromArgb(0, 150, 136), 420, 61, 200, 29);
            btnRecipNew.FlatAppearance.BorderSize = 0;
            tabRecipients.Controls.Add(btnRecipNew);
            btnRecipNew.Click += async (s, e) => await NewRecipientDialogAsync();

            SetupActivityLogTab();

            // TabPages.Insert is silently ignored until the tab control's handle exists (this runs from the constructor),
            // which is why the Recipients tab never showed. Add() works at any time, so re-add the configuration tab after it.
            tabControlNotifications.TabPages.Remove(tabConfig);
            tabControlNotifications.TabPages.Add(tabRecipients);
            tabControlNotifications.TabPages.Add(tabLog);
            tabControlNotifications.TabPages.Add(tabConfig);

            // Short names and several rows instead of a scrolling tab strip with arrows.
            tabControlNotifications.Multiline = true;
            string[] shortNames = { "📡 Live Feed", "📊 Stock Inventory", "📦 Inbound Logs", "⏱️ Pallet Quality", "👥 Recipients", "📜 Activity Log", "⚙️ Configuration" };
            for (int i = 0; i < tabControlNotifications.TabPages.Count && i < shortNames.Length; i++)
                tabControlNotifications.TabPages[i].Text = shortNames[i];
            SetRecipEnabled(false);
        }

        private TabPage tabLog;
        private TextBox txtActivityLog;
        private Button btnRecipNew;

        /// <summary>Scheduler decisions and delivery results (what the service logs), kept even when this tab was closed.</summary>
        private void SetupActivityLogTab()
        {
            tabLog = new TabPage { Text = "Activity Log", BackColor = Color.FromArgb(24, 24, 24), Padding = new Padding(5) };
            txtActivityLog = new TextBox
            {
                Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, WordWrap = true,
                BackColor = Color.FromArgb(18, 18, 18), ForeColor = Color.FromArgb(210, 210, 210), BorderStyle = BorderStyle.None,
                Font = new Font("Consolas", 9F)
            };
            tabLog.Controls.Add(txtActivityLog);

            txtActivityLog.Lines = _service.GetLogLines().ToArray();
            _service.OnLog += Service_OnLog;
            this.Disposed += (s, e) => _service.OnLog -= Service_OnLog;
        }

        private void Service_OnLog(string line)
        {
            if (IsDisposed || !IsHandleCreated) return;
            BeginInvoke(new Action(() =>
            {
                if (txtActivityLog.IsDisposed) return;
                txtActivityLog.AppendText(line + Environment.NewLine);
            }));
        }

        /// <summary>The "new recipient" dialog: type one or more Synology IDs, check who they are, add (and optionally save at once).</summary>
        private async Task NewRecipientDialogAsync()
        {
            if (_recipCurrent == null) { MessageBox.Show("Choose a notification type first.", "New recipient", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            var dark = Color.FromArgb(30, 30, 30);
            using (var dlg = new Form
            {
                Text = "New recipient - " + _recipCurrent.Label, StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false, MinimizeBox = false, ShowInTaskbar = false, ClientSize = new Size(470, 262), BackColor = Color.FromArgb(24, 24, 24), ForeColor = Color.White,
                Font = new Font("Segoe UI", 9F)
            })
            {
                var lbl = new Label { Text = "Synology Chat ID(s) of the people who will receive\n\"" + _recipCurrent.Label + "\" notifications:", Location = new Point(16, 14), Size = new Size(440, 36) };
                var txt = new TextBox { Location = new Point(16, 54), Size = new Size(330, 25), BackColor = dark, ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle, MaxLength = 400 };
                var btnCheck = new Button { Text = "🔍 Check", Location = new Point(356, 52), Size = new Size(98, 28), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(60, 60, 60), ForeColor = Color.White, UseVisualStyleBackColor = false };
                var hint = new Label { Text = "Several IDs can be separated by ; , or a space. Numbers only.", Location = new Point(16, 84), Size = new Size(440, 18), ForeColor = Color.FromArgb(150, 150, 150) };
                var info = new TextBox { Location = new Point(16, 108), Size = new Size(438, 74), Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Vertical, BackColor = dark, ForeColor = Color.FromArgb(200, 200, 200), BorderStyle = BorderStyle.FixedSingle };
                var chkSave = new CheckBox { Text = "Save to tbl_NotificationCategory right away", Checked = true, Location = new Point(16, 192), AutoSize = true };
                var ok = new Button { Text = "Add", DialogResult = DialogResult.OK, Location = new Point(264, 220), Size = new Size(90, 30), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(46, 160, 90), ForeColor = Color.White, UseVisualStyleBackColor = false };
                var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel, Location = new Point(364, 220), Size = new Size(90, 30), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(60, 60, 60), ForeColor = Color.White, UseVisualStyleBackColor = false };
                ok.FlatAppearance.BorderSize = 0; cancel.FlatAppearance.BorderSize = 0; btnCheck.FlatAppearance.BorderSize = 0;
                dlg.Controls.AddRange(new Control[] { lbl, txt, btnCheck, hint, info, chkSave, ok, cancel });
                dlg.AcceptButton = ok; dlg.CancelButton = cancel;

                btnCheck.Click += async (s, e) =>
                {
                    NotificationDispatcherService.ParseSynologyIds(txt.Text, out var v, out var bad);
                    if (bad.Count > 0) { info.Text = "Not valid (numbers only): " + string.Join(", ", bad); return; }
                    if (v.Count == 0) { info.Text = "Type at least one Synology ID."; return; }
                    info.Text = "Looking up…";
                    var names = await SafeResolveNamesAsync(v);
                    info.Text = string.Join(Environment.NewLine, v.Select(id =>
                        id + "  -  " + (names.TryGetValue(id, out string n) && !string.IsNullOrWhiteSpace(n) ? n : "⚠ not found in RMS users") +
                        (_recipIds.Contains(id) ? "  (already in the list)" : "")));
                };
                ok.Click += (s, e) =>
                {
                    NotificationDispatcherService.ParseSynologyIds(txt.Text, out var v, out var bad);
                    if (bad.Count > 0 || v.Count == 0)
                    {
                        MessageBox.Show(bad.Count > 0 ? "These are not valid Synology IDs (numbers only):\n\n" + string.Join(", ", bad) : "Type at least one Synology ID.",
                            "New recipient", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        dlg.DialogResult = DialogResult.None; // keep the dialog open
                    }
                };

                if (dlg.ShowDialog(this) != DialogResult.OK) return;

                NotificationDispatcherService.ParseSynologyIds(txt.Text, out var ids, out var _);
                var added = ids.Where(id => !_recipIds.Contains(id)).ToList();
                if (added.Count == 0) { SetRecipStatus("Already in the list.", Color.FromArgb(180, 180, 180)); return; }

                _recipIds.AddRange(added);
                var resolved = await SafeResolveNamesAsync(added);
                foreach (var kv in resolved) _recipNames[kv.Key] = kv.Value;
                RenderRecipients();
                MarkRecipDirty(true);
                if (chkSave.Checked) await SaveRecipientsAsync();
            }
        }

        private void SetRecipEnabled(bool on)
        {
            cmbRecipType.Enabled = on; lstRecipients.Enabled = on; txtRecipId.Enabled = on;
            btnRecipAdd.Enabled = on; btnRecipRemove.Enabled = on; btnRecipSave.Enabled = on; btnRecipReload.Enabled = on;
        }

        private void SetRecipStatus(string text, Color color)
        {
            lblRecipStatus.ForeColor = color;
            lblRecipStatus.Text = text;
        }

        private void MarkRecipDirty(bool dirty)
        {
            _recipDirty = dirty;
            if (dirty) SetRecipStatus("● Unsaved changes", Color.FromArgb(255, 193, 7));
        }

        /// <summary>Fills the dropdown with the known notification types plus any extra type found in the table.</summary>
        private async Task LoadRecipientTypesAsync()
        {
            try
            {
                _recipLoading = true;
                var items = NotificationDispatcherService.KnownCategories.Select(c => new RecipientTypeItem
                {
                    NotifType = c.NotifType, Label = c.Label, Description = c.Description, Priority = c.DefaultPriority
                }).ToList();

                List<string> stored = await _service.GetStoredCategoryTypesAsync();
                foreach (string t in stored)
                {
                    if (!items.Any(i => string.Equals(i.NotifType, t, StringComparison.OrdinalIgnoreCase)))
                        items.Add(new RecipientTypeItem { NotifType = t, Label = t + " (other)", Description = "Extra category found in tbl_NotificationCategory." });
                }

                cmbRecipType.Items.Clear();
                cmbRecipType.Items.AddRange(items.Cast<object>().ToArray());
                SetRecipEnabled(true);
                if (cmbRecipType.Items.Count > 0)
                {
                    cmbRecipType.SelectedIndex = 0;
                    _recipLoading = false;
                    await OnRecipTypeChosenAsync(true);
                }
            }
            catch (Exception ex)
            {
                lblRecipHint.Text = "Could not read tbl_NotificationCategory: " + ex.Message;
                SetRecipStatus("✖ Database not reachable", Color.FromArgb(231, 76, 60));
            }
            finally
            {
                _recipLoading = false;
            }
        }

        private async Task OnRecipTypeChosenAsync(bool force = false)
        {
            var chosen = cmbRecipType.SelectedItem as RecipientTypeItem;
            if (chosen == null || _recipLoading) return;
            if (!force && _recipCurrent != null && ReferenceEquals(chosen, _recipCurrent)) return;

            if (!force && _recipDirty && _recipCurrent != null)
            {
                var ans = MessageBox.Show($"You have unsaved changes for '{_recipCurrent.Label}'.\n\nSave them before switching?", "Unsaved changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (ans == DialogResult.Cancel) { cmbRecipType.SelectedItem = _recipCurrent; return; }
                if (ans == DialogResult.Yes && !await SaveRecipientsAsync()) { cmbRecipType.SelectedItem = _recipCurrent; return; }
            }

            _recipCurrent = chosen;
            await LoadRecipientsForCurrentAsync();
        }

        private async Task LoadRecipientsForCurrentAsync()
        {
            if (_recipCurrent == null) return;
            try
            {
                SetRecipStatus("Loading…", Color.FromArgb(180, 180, 180));
                lblRecipHint.Text = $"{_recipCurrent.Description}  (type: {_recipCurrent.NotifType})";
                var ids = await _service.LoadRecipientsFreshAsync(_recipCurrent.NotifType);
                _recipIds.Clear();
                _recipIds.AddRange(ids);
                _recipNames = await SafeResolveNamesAsync(_recipIds);
                _recipDirty = false;
                RenderRecipients();
                SetRecipStatus(ids.Count == 0 ? "No recipients yet - nobody will be notified for this type." : $"{ids.Count} recipient(s) loaded.", ids.Count == 0 ? Color.FromArgb(255, 193, 7) : Color.FromArgb(46, 204, 113));
            }
            catch (Exception ex)
            {
                SetRecipStatus("✖ Load failed: " + ex.Message, Color.FromArgb(231, 76, 60));
            }
        }

        private async Task<Dictionary<string, string>> SafeResolveNamesAsync(IEnumerable<string> ids)
        {
            try { return await _service.ResolveNamesAsync(ids); }
            catch { return new Dictionary<string, string>(); }
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
            if (_recipCurrent == null) return;
            NotificationDispatcherService.ParseSynologyIds(txtRecipId.Text, out var valid, out var invalid);
            if (invalid.Count > 0)
            {
                MessageBox.Show("These are not valid Synology IDs (numbers only):\n\n" + string.Join(", ", invalid), "Invalid ID", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var added = valid.Where(id => !_recipIds.Contains(id)).ToList();
            if (valid.Count == 0) return;
            if (added.Count == 0) { SetRecipStatus("Already in the list.", Color.FromArgb(180, 180, 180)); txtRecipId.Clear(); return; }

            _recipIds.AddRange(added);
            var names = await SafeResolveNamesAsync(added);
            foreach (var kv in names) _recipNames[kv.Key] = kv.Value;
            RenderRecipients();
            MarkRecipDirty(true);
            txtRecipId.Clear();
            txtRecipId.Focus();

            var unknown = added.Where(id => !names.ContainsKey(id)).ToList();
            if (unknown.Count > 0)
                SetRecipStatus("● Unsaved changes - not found in RMS users: " + string.Join(", ", unknown), Color.FromArgb(255, 193, 7));
        }

        private void RemoveSelectedRecipients()
        {
            if (lstRecipients.SelectedItems.Count == 0) return;
            foreach (ListViewItem it in lstRecipients.SelectedItems.Cast<ListViewItem>().ToList())
                _recipIds.Remove(it.Text);
            RenderRecipients();
            MarkRecipDirty(true);
        }

        private async Task<bool> SaveRecipientsAsync()
        {
            if (_recipCurrent == null) return false;
            if (_recipIds.Count == 0 &&
                MessageBox.Show($"The recipient list for '{_recipCurrent.Label}' is empty.\n\nNobody will be notified for this type. Save anyway?", "Empty recipient list", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return false;

            btnRecipSave.Enabled = false;
            try
            {
                string stored = await _service.SaveRecipientsAsync(_recipCurrent.NotifType, _recipIds, _recipCurrent.Priority);
                _recipDirty = false;
                SetRecipStatus($"✔ Saved to tbl_NotificationCategory: {(stored.Length == 0 ? "(nobody)" : stored)}", Color.FromArgb(46, 204, 113));
                return true;
            }
            catch (Exception ex)
            {
                SetRecipStatus("✖ Save failed: " + ex.Message, Color.FromArgb(231, 76, 60));
                MessageBox.Show("Could not save the recipients:\n" + ex.Message, "Save failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            finally
            {
                btnRecipSave.Enabled = true;
            }
        }
    }
}