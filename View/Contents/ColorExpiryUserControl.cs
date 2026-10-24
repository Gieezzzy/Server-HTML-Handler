using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server_HTML_Handler.Services;

namespace Server_HTML_Handler.View.Contents
{
    public class ColorExpiryUserControl : UserControl
    {
        private readonly NotificationDispatcherService _service = NotificationDispatcherService.Instance;
        private List<ColorExpiryItem> _allColors = new List<ColorExpiryItem>();

        private System.Windows.Forms.Panel pnlHeader;
        private Label lblHeaderTitle;
        private Label lblHeaderSubtitle;
        private System.Windows.Forms.Panel pnlFilter;
        private Label lblFilterStatus;
        private ComboBox cmbStatus;
        private Label lblSearch;
        private TextBox txtSearch;
        private Button btnRefresh;
        private DataGridView dgvColors;

        public ColorExpiryUserControl()
        {
            InitializeComponent();
            SetupGrid();
            this.Load += async (s, e) => await RefreshColorsGridAsync();
        }

        private void InitializeComponent()
        {
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.lblHeaderTitle = new Label();
            this.lblHeaderSubtitle = new Label();
            this.pnlFilter = new System.Windows.Forms.Panel();
            this.lblFilterStatus = new Label();
            this.cmbStatus = new ComboBox();
            this.lblSearch = new Label();
            this.txtSearch = new TextBox();
            this.btnRefresh = new Button();
            this.dgvColors = new DataGridView();

            this.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlFilter.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvColors)).BeginInit();

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
            this.lblHeaderTitle.ForeColor = Color.FromArgb(230, 126, 34);
            this.lblHeaderTitle.Location = new Point(14, 9);
            this.lblHeaderTitle.Name = "lblHeaderTitle";
            this.lblHeaderTitle.Text = "🎨 Color Expiration & Library Lifetime Monitor";

            // lblHeaderSubtitle
            this.lblHeaderSubtitle.AutoSize = true;
            this.lblHeaderSubtitle.Font = new Font("Segoe UI", 8.5F);
            this.lblHeaderSubtitle.ForeColor = Color.FromArgb(170, 175, 185);
            this.lblHeaderSubtitle.Location = new Point(16, 36);
            this.lblHeaderSubtitle.Name = "lblHeaderSubtitle";
            this.lblHeaderSubtitle.Text = "Tracking RDMS color formulations (dark: 6 months, light: 2 months). Scheduled alerts dispatch twice daily (07:00 - 17:00).";

            // pnlFilter
            this.pnlFilter.BackColor = Color.FromArgb(32, 34, 40);
            this.pnlFilter.Controls.Add(this.btnRefresh);
            this.pnlFilter.Controls.Add(this.txtSearch);
            this.pnlFilter.Controls.Add(this.lblSearch);
            this.pnlFilter.Controls.Add(this.cmbStatus);
            this.pnlFilter.Controls.Add(this.lblFilterStatus);
            this.pnlFilter.Dock = DockStyle.Top;
            this.pnlFilter.Location = new Point(0, 65);
            this.pnlFilter.Name = "pnlFilter";
            this.pnlFilter.Padding = new Padding(10, 8, 10, 8);
            this.pnlFilter.Size = new Size(1000, 44);

            // lblFilterStatus
            this.lblFilterStatus.AutoSize = true;
            this.lblFilterStatus.ForeColor = Color.White;
            this.lblFilterStatus.Location = new Point(12, 13);
            this.lblFilterStatus.Text = "Expiry Status:";

            // cmbStatus
            this.cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbStatus.BackColor = Color.FromArgb(45, 45, 45);
            this.cmbStatus.ForeColor = Color.White;
            this.cmbStatus.FlatStyle = FlatStyle.Flat;
            this.cmbStatus.Items.AddRange(new object[] { "All Colors", "Expiring Soon (<= 14 Days)", "Already Expired" });
            this.cmbStatus.SelectedIndex = 0;
            this.cmbStatus.Location = new Point(98, 10);
            this.cmbStatus.Size = new Size(175, 23);
            this.cmbStatus.SelectedIndexChanged += (s, e) => ApplyFilter();

            // lblSearch
            this.lblSearch.AutoSize = true;
            this.lblSearch.ForeColor = Color.White;
            this.lblSearch.Location = new Point(285, 13);
            this.lblSearch.Text = "Search:";

            // txtSearch
            this.txtSearch.BackColor = Color.FromArgb(45, 45, 45);
            this.txtSearch.ForeColor = Color.White;
            this.txtSearch.BorderStyle = BorderStyle.FixedSingle;
            this.txtSearch.Location = new Point(335, 10);
            this.txtSearch.Size = new Size(250, 23);
            this.txtSearch.TextChanged += (s, e) => ApplyFilter();

            // btnRefresh
            this.btnRefresh.BackColor = Color.FromArgb(230, 126, 34);
            this.btnRefresh.FlatStyle = FlatStyle.Flat;
            this.btnRefresh.ForeColor = Color.White;
            this.btnRefresh.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            this.btnRefresh.Location = new Point(600, 8);
            this.btnRefresh.Size = new Size(130, 27);
            this.btnRefresh.Text = "↻ Refresh Colors";
            this.btnRefresh.UseVisualStyleBackColor = false;
            this.btnRefresh.Click += async (s, e) => await RefreshColorsGridAsync();

            // dgvColors
            this.dgvColors.Dock = DockStyle.Fill;
            this.dgvColors.Location = new Point(0, 109);
            this.dgvColors.Name = "dgvColors";
            this.dgvColors.ReadOnly = true;
            this.dgvColors.Size = new Size(1000, 591);

            // UserControl
            this.BackColor = Color.FromArgb(20, 22, 27);
            this.Controls.Add(this.dgvColors);
            this.Controls.Add(this.pnlFilter);
            this.Controls.Add(this.pnlHeader);
            this.Font = new Font("Segoe UI", 9F);
            this.Name = "ColorExpiryUserControl";
            this.Size = new Size(1000, 700);

            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlFilter.ResumeLayout(false);
            this.pnlFilter.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvColors)).EndInit();
            this.ResumeLayout(false);
        }

        private void SetupGrid()
        {
            dgvColors.BackgroundColor = Color.FromArgb(24, 24, 24);
            dgvColors.GridColor = Color.FromArgb(48, 48, 48);
            dgvColors.BorderStyle = BorderStyle.None;
            dgvColors.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgvColors.RowHeadersVisible = false;
            dgvColors.EnableHeadersVisualStyles = false;
            dgvColors.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvColors.MultiSelect = false;

            dgvColors.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(18, 18, 18);
            dgvColors.ColumnHeadersDefaultCellStyle.ForeColor = Color.FromArgb(230, 126, 34);
            dgvColors.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
            dgvColors.ColumnHeadersHeight = 34;

            dgvColors.DefaultCellStyle.BackColor = Color.FromArgb(30, 30, 30);
            dgvColors.DefaultCellStyle.ForeColor = Color.FromArgb(235, 235, 235);
            dgvColors.DefaultCellStyle.SelectionBackColor = Color.FromArgb(0, 120, 215);
            dgvColors.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvColors.RowTemplate.Height = 28;

            dgvColors.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(24, 24, 24);
            dgvColors.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(235, 235, 235);

            dgvColors.Columns.Clear();
            dgvColors.AutoGenerateColumns = false;

            dgvColors.Columns.Add(new DataGridViewTextBoxColumn { Name = "colFormulaId", HeaderText = "Formula ID", DataPropertyName = "FormulaId", Width = 90 });
            dgvColors.Columns.Add(new DataGridViewTextBoxColumn { Name = "colColorCode", HeaderText = "Color Code", DataPropertyName = "ColorCode", Width = 110 });
            dgvColors.Columns.Add(new DataGridViewTextBoxColumn { Name = "colColorName", HeaderText = "Color Name", DataPropertyName = "ColorName", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });
            dgvColors.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTone", HeaderText = "Tone / Basis", DataPropertyName = "Basis", Width = 160 });
            dgvColors.Columns.Add(new DataGridViewTextBoxColumn { Name = "colEnteredDate", HeaderText = "Entered Date", DataPropertyName = "EnteredDate", Width = 120 });
            dgvColors.Columns.Add(new DataGridViewTextBoxColumn { Name = "colExpiryDate", HeaderText = "Expiry Date", DataPropertyName = "ExpiryDate", Width = 120 });
            dgvColors.Columns.Add(new DataGridViewTextBoxColumn { Name = "colLifeMonths", HeaderText = "Lifetime", DataPropertyName = "LifeMonths", Width = 80 });
            dgvColors.Columns.Add(new DataGridViewTextBoxColumn { Name = "colWhenText", HeaderText = "Status / Notice", DataPropertyName = "WhenText", Width = 170 });

            dgvColors.CellFormatting += (s, ev) =>
            {
                if (ev.RowIndex < 0 || ev.RowIndex >= dgvColors.Rows.Count) return;
                var item = dgvColors.Rows[ev.RowIndex].DataBoundItem as ColorExpiryItem;
                if (item == null) return;

                if (dgvColors.Columns[ev.ColumnIndex].Name == "colEnteredDate" && ev.Value is DateTime dt1)
                {
                    ev.Value = dt1.ToString("yyyy-MM-dd");
                    ev.FormattingApplied = true;
                }
                else if (dgvColors.Columns[ev.ColumnIndex].Name == "colExpiryDate" && ev.Value is DateTime dt2)
                {
                    ev.Value = dt2.ToString("yyyy-MM-dd");
                    ev.FormattingApplied = true;
                }
                else if (dgvColors.Columns[ev.ColumnIndex].Name == "colLifeMonths")
                {
                    ev.Value = $"{item.LifeMonths} mos";
                    ev.FormattingApplied = true;
                }
                else if (dgvColors.Columns[ev.ColumnIndex].Name == "colWhenText")
                {
                    if (item.IsExpired)
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(231, 76, 60);
                        ev.CellStyle.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
                    }
                    else if (item.DaysLeft <= 7)
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(241, 196, 15);
                        ev.CellStyle.Font = new Font("Segoe UI Semibold", 8.5F, FontStyle.Bold);
                    }
                    else
                    {
                        ev.CellStyle.ForeColor = Color.FromArgb(46, 204, 113);
                    }
                }
            };
        }

        public async Task RefreshColorsGridAsync()
        {
            try
            {
                Cursor = Cursors.WaitCursor;
                _allColors = await _service.ScanColorExpiryAsync();
                ApplyFilter();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Could not load color expiry: {ex.Message}", "Color Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ApplyFilter()
        {
            var filtered = _allColors.AsEnumerable();

            string status = cmbStatus.SelectedItem?.ToString() ?? "All Colors";
            if (status.IndexOf("Already Expired", StringComparison.OrdinalIgnoreCase) >= 0)
                filtered = filtered.Where(x => x.IsExpired);
            else if (status.IndexOf("Expiring Soon", StringComparison.OrdinalIgnoreCase) >= 0)
                filtered = filtered.Where(x => !x.IsExpired && x.DaysLeft <= 14);

            string q = txtSearch.Text.Trim();
            if (!string.IsNullOrEmpty(q))
            {
                filtered = filtered.Where(x => (x.ColorCode != null && x.ColorCode.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                               (x.ColorName != null && x.ColorName.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                               (x.Basis != null && x.Basis.IndexOf(q, StringComparison.OrdinalIgnoreCase) >= 0));
            }

            dgvColors.DataSource = null;
            dgvColors.DataSource = filtered.ToList();
        }
    }
}
