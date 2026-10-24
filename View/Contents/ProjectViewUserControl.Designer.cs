namespace Server_HTML_Handler.View.Contents
{
    partial class ProjectViewUserControl
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.topControlPanel = new System.Windows.Forms.Panel();
            this.btnOpenExplorer = new System.Windows.Forms.Button();
            this.btnRefreshFiles = new System.Windows.Forms.Button();
            this.btnNewCi4Project = new System.Windows.Forms.Button();
            this.btnSelectFolder = new System.Windows.Forms.Button();
            this.lblSelectedPath = new System.Windows.Forms.Label();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.pnlLeftHeader = new System.Windows.Forms.Panel();
            this.txtSearchFile = new System.Windows.Forms.TextBox();
            this.lblFileListHeader = new System.Windows.Forms.Label();
            this.fileTreeView = new System.Windows.Forms.TreeView();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.textEditor = new FastColoredTextBoxNS.FastColoredTextBox();
            this.editorStatusPanel = new System.Windows.Forms.Panel();
            this.lblEditorStats = new System.Windows.Forms.Label();
            this.editorTopPanel = new System.Windows.Forms.Panel();
            this.btnNewFile = new System.Windows.Forms.Button();
            this.btnDeleteFile = new System.Windows.Forms.Button();
            this.btnPreview = new System.Windows.Forms.Button();
            this.btnSaveFile = new System.Windows.Forms.Button();
            this.lblCurrentFile = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

            this.topControlPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.pnlLeftHeader.SuspendLayout();
            this.rightPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.textEditor)).BeginInit();
            this.editorStatusPanel.SuspendLayout();
            this.editorTopPanel.SuspendLayout();
            this.SuspendLayout();

            // 
            // topControlPanel
            // 
            this.topControlPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.topControlPanel.Controls.Add(this.btnNewCi4Project);
            this.topControlPanel.Controls.Add(this.btnOpenExplorer);
            this.topControlPanel.Controls.Add(this.btnRefreshFiles);
            this.topControlPanel.Controls.Add(this.btnSelectFolder);
            this.topControlPanel.Controls.Add(this.lblSelectedPath);
            this.topControlPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topControlPanel.Location = new System.Drawing.Point(0, 0);
            this.topControlPanel.Name = "topControlPanel";
            this.topControlPanel.Size = new System.Drawing.Size(950, 45);
            this.topControlPanel.TabIndex = 0;

            // 
            // btnSelectFolder
            // 
            this.btnSelectFolder.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.btnSelectFolder.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSelectFolder.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelectFolder.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSelectFolder.ForeColor = System.Drawing.Color.Cyan;
            this.btnSelectFolder.Location = new System.Drawing.Point(12, 8);
            this.btnSelectFolder.Name = "btnSelectFolder";
            this.btnSelectFolder.Size = new System.Drawing.Size(155, 29);
            this.btnSelectFolder.TabIndex = 0;
            this.btnSelectFolder.Text = "📁 Select Project Root";
            this.btnSelectFolder.UseVisualStyleBackColor = false;
            this.btnSelectFolder.Click += new System.EventHandler(this.BtnSelectFolder_Click);

            // 
            // btnRefreshFiles
            // 
            this.btnRefreshFiles.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.btnRefreshFiles.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRefreshFiles.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnRefreshFiles.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnRefreshFiles.ForeColor = System.Drawing.Color.White;
            this.btnRefreshFiles.Location = new System.Drawing.Point(173, 8);
            this.btnRefreshFiles.Name = "btnRefreshFiles";
            this.btnRefreshFiles.Size = new System.Drawing.Size(85, 29);
            this.btnRefreshFiles.TabIndex = 1;
            this.btnRefreshFiles.Text = "🔄 Refresh";
            this.btnRefreshFiles.UseVisualStyleBackColor = false;
            this.btnRefreshFiles.Click += new System.EventHandler(this.BtnRefreshFiles_Click);

            // 
            // btnOpenExplorer
            // 
            this.btnOpenExplorer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.btnOpenExplorer.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnOpenExplorer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnOpenExplorer.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnOpenExplorer.ForeColor = System.Drawing.Color.LightGray;
            this.btnOpenExplorer.Location = new System.Drawing.Point(264, 8);
            this.btnOpenExplorer.Name = "btnOpenExplorer";
            this.btnOpenExplorer.Size = new System.Drawing.Size(130, 29);
            this.btnOpenExplorer.TabIndex = 2;
            this.btnOpenExplorer.Text = "📂 Open Folder";
            this.btnOpenExplorer.UseVisualStyleBackColor = false;
            this.btnOpenExplorer.Click += new System.EventHandler(this.BtnOpenExplorer_Click);

            // 
            // lblSelectedPath
            // 
            this.lblSelectedPath.AutoSize = true;
            this.lblSelectedPath.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.lblSelectedPath.ForeColor = System.Drawing.Color.LightGray;
            this.lblSelectedPath.Location = new System.Drawing.Point(575, 15);
            this.lblSelectedPath.Name = "lblSelectedPath";
            this.lblSelectedPath.Size = new System.Drawing.Size(117, 15);
            this.lblSelectedPath.TabIndex = 3;
            this.lblSelectedPath.Text = "Current Path: Not Set";

            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 45);
            this.splitContainer.Name = "splitContainer";

            // splitContainer.Panel1
            this.splitContainer.Panel1.Controls.Add(this.fileTreeView);
            this.splitContainer.Panel1.Controls.Add(this.pnlLeftHeader);

            // splitContainer.Panel2
            this.splitContainer.Panel2.Controls.Add(this.rightPanel);
            this.splitContainer.Size = new System.Drawing.Size(950, 505);
            this.splitContainer.SplitterDistance = 280;
            this.splitContainer.TabIndex = 1;

            // 
            // pnlLeftHeader
            // 
            this.pnlLeftHeader.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(51)))), ((int)(((byte)(55)))));
            this.pnlLeftHeader.Controls.Add(this.txtSearchFile);
            this.pnlLeftHeader.Controls.Add(this.lblFileListHeader);
            this.pnlLeftHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlLeftHeader.Location = new System.Drawing.Point(0, 0);
            this.pnlLeftHeader.Name = "pnlLeftHeader";
            this.pnlLeftHeader.Size = new System.Drawing.Size(280, 55);
            this.pnlLeftHeader.TabIndex = 0;

            // 
            // lblFileListHeader
            // 
            this.lblFileListHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblFileListHeader.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblFileListHeader.ForeColor = System.Drawing.Color.Orange;
            this.lblFileListHeader.Location = new System.Drawing.Point(0, 0);
            this.lblFileListHeader.Name = "lblFileListHeader";
            this.lblFileListHeader.Padding = new System.Windows.Forms.Padding(6, 0, 0, 0);
            this.lblFileListHeader.Size = new System.Drawing.Size(280, 24);
            this.lblFileListHeader.TabIndex = 0;
            this.lblFileListHeader.Text = "Project Explorer";
            this.lblFileListHeader.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // txtSearchFile
            // 
            this.txtSearchFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.txtSearchFile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txtSearchFile.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtSearchFile.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.txtSearchFile.ForeColor = System.Drawing.Color.Gray;
            this.txtSearchFile.Location = new System.Drawing.Point(0, 28);
            this.txtSearchFile.Name = "txtSearchFile";
            this.txtSearchFile.Size = new System.Drawing.Size(280, 23);
            this.txtSearchFile.TabIndex = 1;
            this.txtSearchFile.Text = "🔍 Search views...";
            this.txtSearchFile.Enter += new System.EventHandler(this.TxtSearchFile_Enter);
            this.txtSearchFile.Leave += new System.EventHandler(this.TxtSearchFile_Leave);
            this.txtSearchFile.TextChanged += new System.EventHandler(this.TxtSearchFile_TextChanged);

            // 
            // fileTreeView
            // 
            this.fileTreeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.fileTreeView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileTreeView.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.fileTreeView.ForeColor = System.Drawing.Color.White;
            this.fileTreeView.LineColor = System.Drawing.Color.LightGray;
            this.fileTreeView.Location = new System.Drawing.Point(0, 55);
            this.fileTreeView.Name = "fileTreeView";
            this.fileTreeView.Size = new System.Drawing.Size(280, 450);
            this.fileTreeView.TabIndex = 1;
            this.fileTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.FileTreeView_AfterSelect);

            // 
            // rightPanel
            // 
            this.rightPanel.Controls.Add(this.textEditor);
            this.rightPanel.Controls.Add(this.editorStatusPanel);
            this.rightPanel.Controls.Add(this.editorTopPanel);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.Location = new System.Drawing.Point(0, 0);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(666, 505);
            this.rightPanel.TabIndex = 0;

            // 
            // editorTopPanel
            // 
            this.editorTopPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.editorTopPanel.Controls.Add(this.btnNewFile);
            this.editorTopPanel.Controls.Add(this.btnDeleteFile);
            this.editorTopPanel.Controls.Add(this.btnPreview);
            this.editorTopPanel.Controls.Add(this.btnSaveFile);
            this.editorTopPanel.Controls.Add(this.lblCurrentFile);
            this.editorTopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.editorTopPanel.Location = new System.Drawing.Point(0, 0);
            this.editorTopPanel.Name = "editorTopPanel";
            this.editorTopPanel.Size = new System.Drawing.Size(666, 35);
            this.editorTopPanel.TabIndex = 0;

            // 
            // lblCurrentFile
            // 
            this.lblCurrentFile.AutoSize = true;
            this.lblCurrentFile.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCurrentFile.ForeColor = System.Drawing.Color.Cyan;
            this.lblCurrentFile.Location = new System.Drawing.Point(12, 10);
            this.lblCurrentFile.Name = "lblCurrentFile";
            this.lblCurrentFile.Size = new System.Drawing.Size(94, 15);
            this.lblCurrentFile.TabIndex = 0;
            this.lblCurrentFile.Text = "No file selected";

            // 
            // btnNewFile
            // 
            this.btnNewFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.btnNewFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNewFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnNewFile.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btnNewFile.ForeColor = System.Drawing.Color.White;
            this.btnNewFile.Location = new System.Drawing.Point(280, 4);
            this.btnNewFile.Name = "btnNewFile";
            this.btnNewFile.Size = new System.Drawing.Size(80, 27);
            this.btnNewFile.TabIndex = 1;
            this.btnNewFile.Text = "➕ New View";
            this.btnNewFile.UseVisualStyleBackColor = false;
            this.btnNewFile.Click += new System.EventHandler(this.BtnNewFile_Click);

            // 
            // btnDeleteFile
            // 
            this.btnDeleteFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.btnDeleteFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnDeleteFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDeleteFile.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btnDeleteFile.ForeColor = System.Drawing.Color.IndianRed;
            this.btnDeleteFile.Location = new System.Drawing.Point(366, 4);
            this.btnDeleteFile.Name = "btnDeleteFile";
            this.btnDeleteFile.Size = new System.Drawing.Size(75, 27);
            this.btnDeleteFile.TabIndex = 2;
            this.btnDeleteFile.Text = "🗑️ Delete";
            this.btnDeleteFile.UseVisualStyleBackColor = false;
            this.btnDeleteFile.Click += new System.EventHandler(this.BtnDeleteFile_Click);

            // 
            // btnPreview
            // 
            this.btnPreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPreview.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.btnPreview.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPreview.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPreview.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold);
            this.btnPreview.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.btnPreview.Location = new System.Drawing.Point(447, 4);
            this.btnPreview.Name = "btnPreview";
            this.btnPreview.Size = new System.Drawing.Size(110, 27);
            this.btnPreview.TabIndex = 3;
            this.btnPreview.Text = "🌐 Browser Preview";
            this.btnPreview.UseVisualStyleBackColor = false;
            this.btnPreview.Click += new System.EventHandler(this.BtnPreview_Click);

            // 
            // btnSaveFile
            // 
            this.btnSaveFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSaveFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.btnSaveFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSaveFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSaveFile.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnSaveFile.ForeColor = System.Drawing.Color.LimeGreen;
            this.btnSaveFile.Location = new System.Drawing.Point(563, 4);
            this.btnSaveFile.Name = "btnSaveFile";
            this.btnSaveFile.Size = new System.Drawing.Size(95, 27);
            this.btnSaveFile.TabIndex = 4;
            this.btnSaveFile.Text = "💾 Save View";
            this.btnSaveFile.UseVisualStyleBackColor = false;
            this.btnSaveFile.Click += new System.EventHandler(this.BtnSaveFile_Click);

            // 
            // textEditor
            // 
            this.textEditor.AutoScrollMinSize = new System.Drawing.Size(25, 18);
            this.textEditor.BackBrush = null;
            this.textEditor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.textEditor.CharHeight = 18;
            this.textEditor.CharWidth = 10;
            this.textEditor.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.textEditor.DisabledColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))), ((int)(((byte)(180)))));
            this.textEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditor.Font = new System.Drawing.Font("Consolas", 11F);
            this.textEditor.ForeColor = System.Drawing.Color.LightGray;
            this.textEditor.Location = new System.Drawing.Point(0, 35);
            this.textEditor.Name = "textEditor";
            this.textEditor.Paddings = new System.Windows.Forms.Padding(0);
            this.textEditor.SelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(60)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(255)))));
            this.textEditor.ServiceColors = null;
            this.textEditor.Size = new System.Drawing.Size(666, 445);
            this.textEditor.TabIndex = 1;
            this.textEditor.Zoom = 100;
            this.textEditor.TextChanged += new System.EventHandler<FastColoredTextBoxNS.TextChangedEventArgs>(this.TextEditor_TextChanged);

            // 
            // editorStatusPanel
            // 
            this.editorStatusPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.editorStatusPanel.Controls.Add(this.lblEditorStats);
            this.editorStatusPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.editorStatusPanel.Location = new System.Drawing.Point(0, 480);
            this.editorStatusPanel.Name = "editorStatusPanel";
            this.editorStatusPanel.Size = new System.Drawing.Size(666, 25);
            this.editorStatusPanel.TabIndex = 2;

            // 
            // lblEditorStats
            // 
            this.lblEditorStats.AutoSize = true;
            this.lblEditorStats.Font = new System.Drawing.Font("Segoe UI", 8.5F);
            this.lblEditorStats.ForeColor = System.Drawing.Color.Gray;
            this.lblEditorStats.Location = new System.Drawing.Point(10, 5);
            this.lblEditorStats.Name = "lblEditorStats";
            this.lblEditorStats.Size = new System.Drawing.Size(124, 15);
            this.lblEditorStats.TabIndex = 0;
            this.lblEditorStats.Text = "Lines: 0 | Characters: 0";

            // 
            // ProjectViewUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.topControlPanel);
            this.Name = "ProjectViewUserControl";
            this.Size = new System.Drawing.Size(950, 550);

            this.topControlPanel.ResumeLayout(false);
            this.topControlPanel.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.pnlLeftHeader.ResumeLayout(false);
            this.pnlLeftHeader.PerformLayout();
            this.rightPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.textEditor)).EndInit();
            this.editorStatusPanel.ResumeLayout(false);
            this.editorStatusPanel.PerformLayout();
            this.editorTopPanel.ResumeLayout(false);
            this.editorTopPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel topControlPanel;
        private System.Windows.Forms.Button btnNewCi4Project;
        private System.Windows.Forms.Button btnSelectFolder;
        private System.Windows.Forms.Button btnRefreshFiles;
        private System.Windows.Forms.Button btnOpenExplorer;
        private System.Windows.Forms.Label lblSelectedPath;

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.Panel pnlLeftHeader;
        private System.Windows.Forms.Label lblFileListHeader;
        private System.Windows.Forms.TextBox txtSearchFile;
        private System.Windows.Forms.TreeView fileTreeView;

        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.Panel editorTopPanel;
        private System.Windows.Forms.Label lblCurrentFile;
        private System.Windows.Forms.Button btnNewFile;
        private System.Windows.Forms.Button btnDeleteFile;
        private System.Windows.Forms.Button btnPreview;
        private System.Windows.Forms.Button btnSaveFile;

        private FastColoredTextBoxNS.FastColoredTextBox textEditor;
        private System.Windows.Forms.Panel editorStatusPanel;
        private System.Windows.Forms.Label lblEditorStats;

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}