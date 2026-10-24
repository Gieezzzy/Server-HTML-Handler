using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server_HTML_Handler.Services;

namespace Server_HTML_Handler.View.Contents
{
    public class InboundStocksUserControl : UserControl
    {
        private readonly NotificationDispatcherService _service = NotificationDispatcherService.Instance;
        private List<InboundSummaryItem> _allInbounds = new List<InboundSummaryItem>();

        private System.Windows.Forms.Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;
        private System.Windows.Forms.Panel pnlFilter;
        private Label lblInboundFilter;
        private ComboBox cmbInboundType;
        private Label lblInboundSearch;
        private TextBox txtInboundSearch;
        private Button btnRefresh;
        private DataGridView dgvInbounds;

        public InboundStocksUserControl()
        {
            InitializeComponent();
            SetupGrid();
            this.Load += async (s, e) => await RefreshInboundsGridAsync();
        }

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblHeaderTitle = new Label();
            this.lblHeaderSubtitle = new Label();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.lblInboundFilter = new Label();
            this.cmbInboundType = new ComboBox();
            this.lblInboundSearch = new Label();
            this.txtInboundSearch = new TextBox();
            this.btnRefresh = new Button();
            this.dgvInbounds = new DataGridView();

            this.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInbounds)).BeginInit();

            // pnlHeader
            this.pnlHeader.BackColor = Color.FromArgb(28, 30, 36);
            this.pnlHeader.Controls.Add(this.lblHeaderSubtitle);
            this.pnlHeader.Controls.Add(this.lblHeaderTitle);
            this.pnlHeader.Dock = DockStyle.Top;
            this.pnlHeader.Location = new Point(0, 0);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new Padding(15, 10, 15, 10);
            this.pnlHeader.Size = new Size(1000, 65);

            // lblHeaderTitle
            this.lblHeaderTitle.AutoSize = true;
            this.lblHeaderTitle.Font = new Font("Segoe UI", 13.5F, FontStyle.Bold);
            this.lblHeaderTitle.ForeColor = Color.FromArgb(46, 204, 113);
            this.lblHeaderTitle.Location = new Point(14, 9);
            this.lblHeaderTitle.Name = "lblHeaderTitle";
            this.lblHeaderTitle.Text = "📦 Inbound Stocks & FIFO Rotation Tracking";

            // lblHeaderSubtitle
            this.lblHeaderSubtitle.AutoSize = true;
            this.lblHeaderSubtitle.Font = new Font("Segoe UI", 8.5F);
            this.lblHeaderSubtitle.ForeColor = Color.FromArgb(170, 175, 185);
            this.lblHeaderSubtitle.Location = new Point(16, 36);
            this.lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            this.lblHeaderSubtitle.Text = "Real-time logs for Material, Pigment, and Chemical receipts. Scheduled alerts dispatch twice daily (07:00 - 17:00).";

            // pnlFilter
            this.pnlFilter.BackColor = Color.FromArgb(32, 34, 40);
            this.pnlFilter.Controls.Add(this.btnRefresh);
            this.pnlFilter.Controls.Add(this.txtInboundSearch);
            this.pnlFilter.Controls.Add(this.lblInboundSearch);
            this.pnlFilter.Controls.Add(this.cmbInboundType);
            this.pnlFilter.Controls.Add(this.lblInboundFilter);
            this.pnlFilter.Dock = DockStyle.Top;
            this.pnlFilter.Location = new Point(0, 65);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Padding = new Padding(10, 8, 10, 8);
            this.pnlFilter.Size = new Size(1000, 44);

            // lblInboundFilter
            this.lblInboundFilter.AutoSize = true;
            this.lblInboundFilter.ForeColor = Color.White;
            this.lblInboundFilter.Location = new Point(12, 13);
            this.lblInboundFilter.Text = "Inbound Type:";

            // cmbInboundType
            this.cmbInboundType.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbInboundType.BackColor = Color.FromArgb(45, 45, 45);
            this.cmbInboundType.ForeColor = Color.White;
            this.cmbInboundType.FlatStyle = FlatStyle.Flat;
            this.cmbInboundType.Items.AddRange(new object[] { "All Inbounds", "Material Inbounds", "Pigment Inbounds", "Chemical Inbounds" });
            this.cmbInboundType.SelectedIndex = 0;
            this.cmbInboundType.Location = new Point(98, 10);
            this.cmbInboundType.Size = new Size(160, 23);
            this.cmbInboundType.SelectedIndexChanged += (s, e) => ApplyFilter();

            // lblInboundSearch
            this.lblInboundSearch.AutoSize = true;
            this.lblInboundSearch.ForeColor = Color.White;
            this.lblInboundSearch.Location = new Point(275, 13);
            this.lblInboundSearch.Text = "Search:";

            // txtInboundSearch
            this.txtInboundSearch.BackColor = Color.FromArgb(45, 45, 45);
            this.txtInboundSearch.ForeColor = Color.White;
            this.txtInboundSearch.BorderStyle = BorderStyle.FixedSingle;
            this.txtInboundSearch.Location = new Point(325, 10);
            this.txtInboundSearch.Size = new Size(250, 23);
            this.txtInboundSearch.TextChanged += (s, e) => ApplyFilter();

            // btnRefresh
            this.btnRefresh.BackColor = Color.FromArgb(46, 204, 113);
            this.btnRefresh.FlatStyle = FlatStyle.Flat;
            this.btnRefresh.ForeColor = Color.White;
            this.btnRefresh.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            this.btnRefresh.Location = new Point(590, 8);
            this.btnRefresh.Size = new Size(130, 27);
            this.btnRefresh.Text = "↻ Refresh Logs";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += async (s, e) => await RefreshInboundsGridAsync();

            // dgvInbounds
            this.dgvInbounds.Dock = DockStyle.Fill;
            this.dgvInbounds.Location = new Point(0, 109);
            this.dgvInbounds.Name = "dgvInbounds";
            this.dgvInbounds.ReadOnly = true;
            this.dgvInbounds.Size = new Size(1000, 591);

            // UserControl
            this.BackColor = Color.FromArgb(20, 22, 27);
            this.Controls.Add(this.dgvInbounds);
            this.Controls.Add(this.pnlFilter);
            this.Controls.Add(this.pnlHeader);
            this.Font = new Font("Segoe UI", 9F);
            this.Name = "InboundStocksUserControl";
            this.Size = new Size(1000, 700);

            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvInbounds)).EndInit();
            this.ResumeLayout(false);
        }

        private void SetupGrid()
        {
            dgvInbounds.BackgroundColor = Color.FromArgb(24, 24, 24);
            dgvInbounds.GridColor = Color.FromArgb(48, 48, 48);
            dgvInbounds.BorderStyle = BorderStyle.None;
            dgvInbounds.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvInbounds.RowHeadersVisible = false;
            dgvInbounds.EnableHeadersVisualStyles = false;
            dgvInbounds.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvInbounds.MultiSelect = false;

            dgvInbounds.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(18, 18, 18);
            dgvInbounds.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(46, 204, 113);
            dgvInbounds.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            dgvInbounds.ColumnHeadersHeight = 34;

            dgvInbounds.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            dgvInbounds.DefaultCellStyle.ForeColor = Color.FromArgb(235, 235, 235);
            dgvInbounds.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgvInbounds.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvInbounds.RowTemplate.Height = 28;

            dgvInbounds.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 24);
            dgvInbounds.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(235, 235, 235);

            dgvInbounds.Columns.Clear();
            dgvInbounds.AutoGenerateColumns = false;

            dgvInbounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "colType", HeaderText = "Inbound Type", DataPropertyName = "Type", Width = 130 });
            dgvInbounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "colDate", HeaderText = "Arrival Date", DataPropertyName = "DisplayDate", Width = 140 });
            dgvInbounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCode", HeaderText = "Item Code", DataPropertyName = "Code", Width = 110 });
            dgvInbounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "colName", HeaderText = "Material / Item Name", DataPropertyName = "Name", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvInbounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "colQty", HeaderText = "Quantity", DataPropertyName = "DisplayQty", Width = 120 });
            dgvInbounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "colBatch", HeaderText = "Batch Tracking", DataPropertyName = "BatchTrackingNo", Width = 140 });
            dgvInbounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "colContainer", HeaderText = "Container No", DataPropertyName = "ContainerNo", Width = 110 });
            dgvInbounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCreatedBy", HeaderText = "Received By", DataPropertyName = "CreatedBy", Width = 110 });
            dgvInbounds.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFifo", HeaderText = "FIFO Notice", DataPropertyName = "FifoNotice", Width = 140 });

            dgvInbounds.CellFormatting += (s, ev) =>
            {
                if (ev.RowIndex < 0 || ev.RowIndex >= dgvInbounds.Rows.Count) return;
                var item = dgvInbounds.Rows[ev.RowIndex].DataBoundItem as InboundSummaryItem;
                if (item == null) return;

                if (dgvInbounds.Columns[ev.ColumnIndex].Name == "colFifo")
                {
                    ev.CellStyle.ForeColor = Color.FromArgb(241, 196, 15);
                    ev.CellStyle.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
                }
                else if (dgvInbounds.Columns[ev.ColumnIndex].Name == "colType")
                {
                    if (item.Type != null && item.Type.IndexOf("Material", StringComparison.OrdinalIgnoreCase) >= 0)
                        ev.CellStyle.ForeColor = Color.FromArgb(52, 152, 219);
                    else if (item.Type != null && item.Type.IndexOf("Pigment", StringComparison.OrdinalIgnoreCase) >= 0)
                        ev.CellStyle.ForeColor = Color.FromArgb(230, 126, 34);
                    else if (item.Type != null && item.Type.IndexOf("Chemical", StringComparison.OrdinalIgnoreCase) >= 0)
                        ev.CellStyle.ForeColor = Color.FromArgb(155, 89, 182);
                }
            };
        }

        public async Task RefreshInboundsGridAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _allInbounds = await _service.GetRecentInboundsAsync(250);
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not load inbounds: {ex.Message}", "Inbounds Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ApplyFilter()
        {
            var filtered = _allInbounds.AsEnumerable();

            string cat = cmbInboundType.SelectedItem?.ToString() ?? "All Inbounds";
            if (cat.IndexOf("Material", StringComparison.OrdinalIgnoreCase) >= 0)
                filtered = filtered.Where(x => x.Type.IndexOf("Material", StringComparison.OrdinalIgnoreCase) >= 0);
            else if (cat.IndexOf("Pigment", StringComparison.OrdinalIgnoreCase) >= 0)
                filtered = filtered.Where(x => x.Type.IndexOf("Pigment", StringComparison.OrdinalIgnoreCase) >= 0);
            else if (cat.IndexOf("Chemical", StringComparison.OrdinalIgnoreCase) >= 0)
                filtered = filtered.Where(x => x.Type.IndexOf("Chemical", StringComparison.OrdinalIgnoreCase) >= 0);

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
    }
}
