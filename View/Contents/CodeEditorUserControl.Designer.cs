namespace Server_HTML_Handler.View.Contents
{
    partial class CodeEditorUserControl
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
            this.components = new System.ComponentModel.Container();
            this.topPanel = new System.Windows.Forms.Panel();
            this.flowTop = new System.Windows.Forms.FlowLayoutPanel();
            this.btnSave = new System.Windows.Forms.Button();
            this.btnChangeRoot = new System.Windows.Forms.Button();
            this.btnVhosts = new System.Windows.Forms.Button();
            this.btnPhpIni = new System.Windows.Forms.Button();
            this.btnHttpdConf = new System.Windows.Forms.Button();
            this.btnHostsFile = new System.Windows.Forms.Button();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.fileTree = new System.Windows.Forms.TreeView();
            this.cmsTree = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmiNewFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiOpenExplorer = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRefresh = new System.Windows.Forms.ToolStripMenuItem();
            this.rightPanel = new System.Windows.Forms.Panel();
            this.textEditor = new System.Windows.Forms.TextBox();
            this.editorStatusPanel = new System.Windows.Forms.Panel();
            this.lblEditorStats = new System.Windows.Forms.Label();
            this.lblCurrentFile = new System.Windows.Forms.Label();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();

            this.topPanel.SuspendLayout();
            this.flowTop.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.cmsTree.SuspendLayout();
            this.rightPanel.SuspendLayout();
            this.editorStatusPanel.SuspendLayout();
            this.SuspendLayout();

            // 
            // topPanel
            // 
            this.topPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.topPanel.Controls.Add(this.flowTop);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Name = "topPanel";
            this.topPanel.Size = new System.Drawing.Size(950, 42);
            this.topPanel.TabIndex = 0;

            // 
            // flowTop
            // 
            this.flowTop.Controls.Add(this.btnSave);
            this.flowTop.Controls.Add(this.btnChangeRoot);
            this.flowTop.Controls.Add(this.btnVhosts);
            this.flowTop.Controls.Add(this.btnPhpIni);
            this.flowTop.Controls.Add(this.btnHttpdConf);
            this.flowTop.Controls.Add(this.btnHostsFile);
            this.flowTop.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowTop.Location = new System.Drawing.Point(0, 0);
            this.flowTop.Name = "flowTop";
            this.flowTop.Padding = new System.Windows.Forms.Padding(5);
            this.flowTop.Size = new System.Drawing.Size(950, 42);
            this.flowTop.TabIndex = 0;

            // 
            // btnSave
            // 
            this.btnSave.AutoSize = true;
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.btnSave.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnSave.ForeColor = System.Drawing.Color.LimeGreen;
            this.btnSave.Location = new System.Drawing.Point(8, 8);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(85, 27);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "💾 Save File";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);

            // 
            // btnChangeRoot
            // 
            this.btnChangeRoot.AutoSize = true;
            this.btnChangeRoot.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.btnChangeRoot.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnChangeRoot.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnChangeRoot.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnChangeRoot.ForeColor = System.Drawing.Color.Cyan;
            this.btnChangeRoot.Location = new System.Drawing.Point(99, 8);
            this.btnChangeRoot.Name = "btnChangeRoot";
            this.btnChangeRoot.Size = new System.Drawing.Size(120, 27);
            this.btnChangeRoot.TabIndex = 1;
            this.btnChangeRoot.Text = "📁 Root Folder";
            this.btnChangeRoot.UseVisualStyleBackColor = false;
            this.btnChangeRoot.Click += new System.EventHandler(this.BtnChangeRoot_Click);

            // 
            // btnVhosts
            // 
            this.btnVhosts.AutoSize = true;
            this.btnVhosts.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.btnVhosts.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnVhosts.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnVhosts.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnVhosts.ForeColor = System.Drawing.Color.DeepSkyBlue;
            this.btnVhosts.Location = new System.Drawing.Point(225, 8);
            this.btnVhosts.Name = "btnVhosts";
            this.btnVhosts.Size = new System.Drawing.Size(135, 27);
            this.btnVhosts.TabIndex = 2;
            this.btnVhosts.Text = "🌐 httpd-vhosts.conf";
            this.btnVhosts.UseVisualStyleBackColor = false;
            this.btnVhosts.Click += new System.EventHandler(this.BtnVhosts_Click);

            // 
            // btnPhpIni
            // 
            this.btnPhpIni.AutoSize = true;
            this.btnPhpIni.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.btnPhpIni.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnPhpIni.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPhpIni.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnPhpIni.ForeColor = System.Drawing.Color.Orange;
            this.btnPhpIni.Location = new System.Drawing.Point(366, 8);
            this.btnPhpIni.Name = "btnPhpIni";
            this.btnPhpIni.Size = new System.Drawing.Size(85, 27);
            this.btnPhpIni.TabIndex = 3;
            this.btnPhpIni.Text = "⚙️ php.ini";
            this.btnPhpIni.UseVisualStyleBackColor = false;
            this.btnPhpIni.Click += new System.EventHandler(this.BtnPhpIni_Click);

            // 
            // btnHttpdConf
            // 
            this.btnHttpdConf.AutoSize = true;
            this.btnHttpdConf.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.btnHttpdConf.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHttpdConf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHttpdConf.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnHttpdConf.ForeColor = System.Drawing.Color.Khaki;
            this.btnHttpdConf.Location = new System.Drawing.Point(457, 8);
            this.btnHttpdConf.Name = "btnHttpdConf";
            this.btnHttpdConf.Size = new System.Drawing.Size(95, 27);
            this.btnHttpdConf.TabIndex = 4;
            this.btnHttpdConf.Text = "📝 httpd.conf";
            this.btnHttpdConf.UseVisualStyleBackColor = false;
            this.btnHttpdConf.Click += new System.EventHandler(this.BtnHttpdConf_Click);

            // 
            // btnHostsFile
            // 
            this.btnHostsFile.AutoSize = true;
            this.btnHostsFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(62)))), ((int)(((byte)(62)))), ((int)(((byte)(66)))));
            this.btnHostsFile.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnHostsFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnHostsFile.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.btnHostsFile.ForeColor = System.Drawing.Color.IndianRed;
            this.btnHostsFile.Location = new System.Drawing.Point(558, 8);
            this.btnHostsFile.Name = "btnHostsFile";
            this.btnHostsFile.Size = new System.Drawing.Size(105, 27);
            this.btnHostsFile.TabIndex = 5;
            this.btnHostsFile.Text = "💻 Win hosts";
            this.btnHostsFile.UseVisualStyleBackColor = false;
            this.btnHostsFile.Click += new System.EventHandler(this.BtnHostsFile_Click);

            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 42);
            this.splitContainer.Name = "splitContainer";

            // splitContainer.Panel1
            this.splitContainer.Panel1.Controls.Add(this.fileTree);

            // splitContainer.Panel2
            this.splitContainer.Panel2.Controls.Add(this.rightPanel);
            this.splitContainer.Size = new System.Drawing.Size(950, 508);
            this.splitContainer.SplitterDistance = 270;
            this.splitContainer.TabIndex = 1;

            // 
            // fileTree
            // 
            this.fileTree.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(37)))), ((int)(((byte)(37)))), ((int)(((byte)(38)))));
            this.fileTree.ContextMenuStrip = this.cmsTree;
            this.fileTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fileTree.Font = new System.Drawing.Font("Consolas", 9.5F);
            this.fileTree.ForeColor = System.Drawing.Color.White;
            this.fileTree.Location = new System.Drawing.Point(0, 0);
            this.fileTree.Name = "fileTree";
            this.fileTree.Size = new System.Drawing.Size(270, 508);
            this.fileTree.TabIndex = 0;
            this.fileTree.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.FileTree_BeforeExpand);
            this.fileTree.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.FileTree_NodeDoubleClick);
            this.fileTree.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.FileTree_NodeMouseClick);

            // 
            // cmsTree
            // 
            this.cmsTree.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmiNewFile,
            this.tsmiDelete,
            this.toolStripSeparator1,
            this.tsmiOpenExplorer,
            this.tsmiRefresh});
            this.cmsTree.Name = "cmsTree";
            this.cmsTree.Size = new System.Drawing.Size(170, 98);

            // 
            // tsmiNewFile
            // 
            this.tsmiNewFile.Name = "tsmiNewFile";
            this.tsmiNewFile.Size = new System.Drawing.Size(169, 22);
            this.tsmiNewFile.Text = "➕ New File";
            this.tsmiNewFile.Click += new System.EventHandler(this.TsmiNewFile_Click);

            // 
            // tsmiDelete
            // 
            this.tsmiDelete.Name = "tsmiDelete";
            this.tsmiDelete.Size = new System.Drawing.Size(169, 22);
            this.tsmiDelete.Text = "🗑️ Delete";
            this.tsmiDelete.Click += new System.EventHandler(this.TsmiDelete_Click);

            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(166, 6);

            // 
            // tsmiOpenExplorer
            // 
            this.tsmiOpenExplorer.Name = "tsmiOpenExplorer";
            this.tsmiOpenExplorer.Size = new System.Drawing.Size(169, 22);
            this.tsmiOpenExplorer.Text = "📂 Open in Explorer";
            this.tsmiOpenExplorer.Click += new System.EventHandler(this.TsmiOpenExplorer_Click);

            // 
            // tsmiRefresh
            // 
            this.tsmiRefresh.Name = "tsmiRefresh";
            this.tsmiRefresh.Size = new System.Drawing.Size(169, 22);
            this.tsmiRefresh.Text = "🔄 Refresh Tree";
            this.tsmiRefresh.Click += new System.EventHandler(this.TsmiRefresh_Click);

            // 
            // rightPanel
            // 
            this.rightPanel.Controls.Add(this.textEditor);
            this.rightPanel.Controls.Add(this.editorStatusPanel);
            this.rightPanel.Controls.Add(this.lblCurrentFile);
            this.rightPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.rightPanel.Location = new System.Drawing.Point(0, 0);
            this.rightPanel.Name = "rightPanel";
            this.rightPanel.Size = new System.Drawing.Size(676, 508);
            this.rightPanel.TabIndex = 0;

            // 
            // lblCurrentFile
            // 
            this.lblCurrentFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.lblCurrentFile.Dock = System.Windows.Forms.DockStyle.Top;
            this.lblCurrentFile.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblCurrentFile.ForeColor = System.Drawing.Color.Cyan;
            this.lblCurrentFile.Location = new System.Drawing.Point(0, 0);
            this.lblCurrentFile.Name = "lblCurrentFile";
            this.lblCurrentFile.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.lblCurrentFile.Size = new System.Drawing.Size(676, 28);
            this.lblCurrentFile.TabIndex = 0;
            this.lblCurrentFile.Text = "No file selected";
            this.lblCurrentFile.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;

            // 
            // textEditor
            // 
            this.textEditor.AcceptsTab = true;
            this.textEditor.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.textEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textEditor.Font = new System.Drawing.Font("Consolas", 11F);
            this.textEditor.ForeColor = System.Drawing.Color.LightGray;
            this.textEditor.Location = new System.Drawing.Point(0, 28);
            this.textEditor.Multiline = true;
            this.textEditor.Name = "textEditor";
            this.textEditor.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textEditor.Size = new System.Drawing.Size(676, 455);
            this.textEditor.TabIndex = 1;
            this.textEditor.TextChanged += new System.EventHandler(this.TextEditor_TextChanged);

            // 
            // editorStatusPanel
            // 
            this.editorStatusPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(45)))), ((int)(((byte)(45)))), ((int)(((byte)(48)))));
            this.editorStatusPanel.Controls.Add(this.lblEditorStats);
            this.editorStatusPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.editorStatusPanel.Location = new System.Drawing.Point(0, 483);
            this.editorStatusPanel.Name = "editorStatusPanel";
            this.editorStatusPanel.Size = new System.Drawing.Size(676, 25);
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
            // CodeEditorUserControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(30)))), ((int)(((byte)(30)))));
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.topPanel);
            this.Name = "CodeEditorUserControl";
            this.Size = new System.Drawing.Size(950, 550);

            this.topPanel.ResumeLayout(false);
            this.flowTop.ResumeLayout(false);
            this.flowTop.PerformLayout();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.cmsTree.ResumeLayout(false);
            this.rightPanel.ResumeLayout(false);
            this.rightPanel.PerformLayout();
            this.editorStatusPanel.ResumeLayout(false);
            this.editorStatusPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.Panel topPanel;
        private System.Windows.Forms.FlowLayoutPanel flowTop;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnChangeRoot;
        private System.Windows.Forms.Button btnVhosts;
        private System.Windows.Forms.Button btnPhpIni;
        private System.Windows.Forms.Button btnHttpdConf;
        private System.Windows.Forms.Button btnHostsFile;

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.TreeView fileTree;
        private System.Windows.Forms.ContextMenuStrip cmsTree;
        private System.Windows.Forms.ToolStripMenuItem tsmiNewFile;
        private System.Windows.Forms.ToolStripMenuItem tsmiDelete;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem tsmiOpenExplorer;
        private System.Windows.Forms.ToolStripMenuItem tsmiRefresh;

        private System.Windows.Forms.Panel rightPanel;
        private System.Windows.Forms.Label lblCurrentFile;
        private System.Windows.Forms.TextBox textEditor;
        private System.Windows.Forms.Panel editorStatusPanel;
        private System.Windows.Forms.Label lblEditorStats;

        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
    }
}