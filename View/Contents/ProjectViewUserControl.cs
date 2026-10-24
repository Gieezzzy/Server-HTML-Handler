using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using FastColoredTextBoxNS;

namespace Server_HTML_Handler.View.Contents
{
    public partial class ProjectViewUserControl : UserControl
    {
        private string _activeFilePath = "";
        private bool _isDirty = false;
        private bool _isProgrammaticChange = false;

        private string DynamicHostingPath
        {
            get
            {
                string path = Properties.Settings.Default.HTMLPath;
                return !string.IsNullOrWhiteSpace(path) ? path : @"C:\xampp\htdocs";
            }
        }

        public ProjectViewUserControl()
        {
            InitializeComponent();
            LoadSavedPath();
        }

        private void LoadSavedPath()
        {
            string savedPath = Properties.Settings.Default.HTMLPath;
            if (!string.IsNullOrEmpty(savedPath) && Directory.Exists(savedPath))
            {
                lblSelectedPath.Text = $"Current Path: {savedPath}";
                ScanProjectFiles(savedPath);
            }
            else
            {
                lblSelectedPath.Text = "Current Path: Not Set (Click 'Select Project Root')";
            }
        }

        private void BtnNewCi4Project_Click(object sender, EventArgs e)
        {
            string hostingPath = Properties.Settings.Default.HTMLPath;
            if (string.IsNullOrWhiteSpace(hostingPath) || !Directory.Exists(hostingPath))
            {
                hostingPath = @"D:\html";
            }
            string hostIp = Properties.Settings.Default.IP;
            if (string.IsNullOrWhiteSpace(hostIp)) hostIp = "192.168.32.43";

            using (var dlg = new Dialogs.CreateCi4ProjectDialog(hostingPath, hostIp))
            {
                if (dlg.ShowDialog(this) == DialogResult.OK || !string.IsNullOrEmpty(dlg.CreatedProjectPath))
                {
                    if (Directory.Exists(dlg.CreatedProjectPath))
                    {
                        Properties.Settings.Default.HTMLPath = dlg.CreatedProjectPath;
                        Properties.Settings.Default.Save();
                        lblSelectedPath.Text = $"Current Path: {dlg.CreatedProjectPath}";
                        ScanProjectFiles(dlg.CreatedProjectPath);
                    }
                    else if (Directory.Exists(hostingPath))
                    {
                        ScanProjectFiles(hostingPath);
                    }
                }
            }
        }

        private void BtnSelectFolder_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderBrowserDialog.SelectedPath;

                Properties.Settings.Default.HTMLPath = selectedPath;
                Properties.Settings.Default.Save();

                lblSelectedPath.Text = $"Current Path: {selectedPath}";
                ScanProjectFiles(selectedPath);
            }
        }

        private void BtnRefreshFiles_Click(object sender, EventArgs e)
        {
            string currentPath = Properties.Settings.Default.HTMLPath;
            if (!string.IsNullOrEmpty(currentPath) && Directory.Exists(currentPath))
            {
                ScanProjectFiles(currentPath);
            }
        }

        private void BtnOpenExplorer_Click(object sender, EventArgs e)
        {
            string path = Properties.Settings.Default.HTMLPath;
            if (!string.IsNullOrEmpty(path) && Directory.Exists(path))
            {
                Process.Start("explorer.exe", path);
            }
        }

        #region Tree View File Discovery & Filtering

        private void ScanProjectFiles(string rootDir)
        {
            fileTreeView.Nodes.Clear();

            try
            {
                if (!Directory.Exists(rootDir)) return;

                DirectoryInfo rootDirInfo = new DirectoryInfo(rootDir);
                TreeNode rootNode = new TreeNode(rootDirInfo.Name) { Tag = rootDirInfo.FullName };

                PopulateTreeRecursive(rootDirInfo, rootNode, txtSearchFile.Text.Trim());
                fileTreeView.Nodes.Add(rootNode);

                if (!string.IsNullOrEmpty(txtSearchFile.Text.Trim()) && txtSearchFile.Text != "🔍 Search views...")
                {
                    fileTreeView.ExpandAll();
                }
                else
                {
                    rootNode.Expand();
                }

                lblFileListHeader.Text = $"  Project: {rootDirInfo.Name}";
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error scanning directory: " + ex.Message, "Scan Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool PopulateTreeRecursive(DirectoryInfo dirInfo, TreeNode node, string searchQuery)
        {
            bool hasMatchingChild = false;

            try
            {
                foreach (var subDir in dirInfo.GetDirectories())
                {
                    if (subDir.Name.Equals("vendor", StringComparison.OrdinalIgnoreCase) ||
                        subDir.Name.Equals("writable", StringComparison.OrdinalIgnoreCase) ||
                        subDir.Name.Equals("system", StringComparison.OrdinalIgnoreCase))
                        continue;

                    TreeNode subNode = new TreeNode(subDir.Name) { Tag = subDir.FullName };
                    bool subDirHasMatch = PopulateTreeRecursive(subDir, subNode, searchQuery);

                    if (subDirHasMatch || string.IsNullOrEmpty(searchQuery) || searchQuery == "🔍 Search views...")
                    {
                        node.Nodes.Add(subNode);
                        hasMatchingChild = true;
                    }
                }

                foreach (var file in dirInfo.GetFiles())
                {
                    string ext = file.Extension.ToLower();
                    if (ext == ".php" || ext == ".html" || ext == ".htm" || ext == ".cshtml" || ext == ".css" || ext == ".js")
                    {
                        bool matchesSearch = string.IsNullOrEmpty(searchQuery) ||
                                             searchQuery == "🔍 Search views..." ||
                                             file.Name.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) >= 0;

                        if (matchesSearch)
                        {
                            TreeNode fileNode = new TreeNode(file.Name) { Tag = file.FullName };
                            node.Nodes.Add(fileNode);
                            hasMatchingChild = true;
                        }
                    }
                }
            }
            catch { /* Skip unreadable directories */ }

            return hasMatchingChild;
        }

        private void TxtSearchFile_TextChanged(object sender, EventArgs e)
        {
            if (txtSearchFile.ForeColor != Color.Gray)
            {
                string currentPath = Properties.Settings.Default.HTMLPath;
                if (!string.IsNullOrEmpty(currentPath) && Directory.Exists(currentPath))
                {
                    ScanProjectFiles(currentPath);
                }
            }
        }

        private void TxtSearchFile_Enter(object sender, EventArgs e)
        {
            if (txtSearchFile.Text == "🔍 Search views...")
            {
                txtSearchFile.Text = "";
                txtSearchFile.ForeColor = Color.White;
            }
        }

        private void TxtSearchFile_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearchFile.Text))
            {
                txtSearchFile.Text = "🔍 Search views...";
                txtSearchFile.ForeColor = Color.Gray;
                string currentPath = Properties.Settings.Default.HTMLPath;
                if (!string.IsNullOrEmpty(currentPath) && Directory.Exists(currentPath))
                {
                    ScanProjectFiles(currentPath);
                }
            }
        }

        #endregion

        #region Editor Selection & Syntax Highlighting

        private void FileTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node != null && e.Node.Tag is string fullPath && File.Exists(fullPath))
            {
                if (_isDirty)
                {
                    var result = MessageBox.Show("You have unsaved changes! Save before switching files?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        SaveCurrentFile();
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        return;
                    }
                }

                try
                {
                    _activeFilePath = fullPath;
                    _isProgrammaticChange = true;

                    textEditor.BeginUpdate();

                    // Ensure brushes are safe
                    if (textEditor.BackBrush == null)
                        textEditor.BackBrush = new SolidBrush(textEditor.BackColor);

                    // Bypass built-in lexer parsers that crash inside the DLL
                    textEditor.Language = Language.Custom;

                    // Read file content safely
                    string fileContent;
                    using (var fs = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    using (var reader = new StreamReader(fs))
                    {
                        fileContent = reader.ReadToEnd();
                    }

                    textEditor.Text = fileContent;
                    textEditor.EndUpdate();

                    _isDirty = false;
                    _isProgrammaticChange = false;

                    lblCurrentFile.Text = $"Editing: {e.Node.Text}";
                    lblCurrentFile.ForeColor = Color.Cyan;
                    UpdateStats();
                }
                catch (Exception ex)
                {
                    textEditor.EndUpdate();
                    _isProgrammaticChange = false;
                    MessageBox.Show("Failed to open file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private void TextEditor_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_isProgrammaticChange && !_isDirty && !string.IsNullOrEmpty(_activeFilePath))
            {
                _isDirty = true;
                lblCurrentFile.Text = "Editing: " + Path.GetFileName(_activeFilePath) + " *";
                lblCurrentFile.ForeColor = Color.Orange;
            }
            UpdateStats();
        }

        private void UpdateStats()
        {
            int lines = textEditor.LinesCount;
            int chars = textEditor.TextLength;
            lblEditorStats.Text = $"Lines: {lines} | Characters: {chars} | Status: {(_isDirty ? "Modified *" : "Saved")}";
        }

        #endregion

        #region File Actions (Save, Create, Delete, Preview)

        private void BtnSaveFile_Click(object sender, EventArgs e)
        {
            SaveCurrentFile();
        }

        private void SaveCurrentFile()
        {
            if (string.IsNullOrEmpty(_activeFilePath))
            {
                MessageBox.Show("No view file currently selected.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                File.WriteAllText(_activeFilePath, textEditor.Text);
                _isDirty = false;

                if (fileTreeView.SelectedNode != null)
                    lblCurrentFile.Text = $"Editing: {fileTreeView.SelectedNode.Text}";

                lblCurrentFile.ForeColor = Color.Cyan;
                UpdateStats();
                MessageBox.Show("View file saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnNewFile_Click(object sender, EventArgs e)
        {
            string rootDir = Properties.Settings.Default.HTMLPath;
            if (string.IsNullOrEmpty(rootDir) || !Directory.Exists(rootDir))
            {
                MessageBox.Show("Please select a valid project root folder first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string inputName = PromptDialog.Show("Enter new view filename (e.g. index.php or header.html):", "Create New View", "new_view.php");
            if (string.IsNullOrWhiteSpace(inputName)) return;

            string targetFolder = rootDir;
            if (fileTreeView.SelectedNode != null && fileTreeView.SelectedNode.Tag is string tagPath)
            {
                if (Directory.Exists(tagPath))
                    targetFolder = tagPath;
                else if (File.Exists(tagPath))
                    targetFolder = Path.GetDirectoryName(tagPath);
            }

            string fullPath = Path.Combine(targetFolder, inputName);

            if (File.Exists(fullPath))
            {
                MessageBox.Show("A file with this name already exists!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                File.WriteAllText(fullPath, "<!-- New View File -->\n");
                ScanProjectFiles(rootDir);
                MessageBox.Show("View created successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create view: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnDeleteFile_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_activeFilePath) || !File.Exists(_activeFilePath)) return;

            var confirm = MessageBox.Show($"Are you sure you want to permanently delete:\n{Path.GetFileName(_activeFilePath)}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirm == DialogResult.Yes)
            {
                try
                {
                    File.Delete(_activeFilePath);
                    _activeFilePath = "";
                    _isDirty = false;
                    textEditor.Clear();
                    lblCurrentFile.Text = "No file selected";

                    ScanProjectFiles(Properties.Settings.Default.HTMLPath);
                    MessageBox.Show("File deleted successfully.", "Deleted", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to delete file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void BtnPreview_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(_activeFilePath) || !File.Exists(_activeFilePath))
            {
                MessageBox.Show("Please select a view file to preview.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string activeLower = _activeFilePath.ToLower();
                string hostingLower = DynamicHostingPath.ToLower();

                if (activeLower.Contains(hostingLower))
                {
                    string relativePath = _activeFilePath.Substring(DynamicHostingPath.Length).Replace('\\', '/');
                    string targetUrl = "http://localhost/" + relativePath.TrimStart('/');
                    Process.Start(targetUrl);
                }
                else
                {
                    Process.Start(_activeFilePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to launch browser preview: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #endregion

        #region C# Native Input Box Dialog Helper

        public static class PromptDialog
        {
            public static string Show(string promptText, string caption, string defaultValue = "")
            {
                Form prompt = new Form()
                {
                    Width = 420,
                    Height = 170,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    Text = caption,
                    StartPosition = FormStartPosition.CenterParent,
                    MaximizeBox = false,
                    MinimizeBox = false,
                    BackColor = Color.FromArgb(45, 45, 48),
                    ForeColor = Color.White
                };

                Label textLabel = new Label() { Left = 15, Top = 15, Text = promptText, AutoSize = true, ForeColor = Color.White };
                TextBox textBox = new TextBox() { Left = 15, Top = 40, Width = 370, Text = defaultValue, BackColor = Color.FromArgb(30, 30, 30), ForeColor = Color.White, BorderStyle = BorderStyle.FixedSingle };

                Button confirmation = new Button() { Text = "OK", Left = 225, Width = 75, Top = 80, DialogResult = DialogResult.OK, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(62, 62, 66), ForeColor = Color.Cyan };
                Button cancel = new Button() { Text = "Cancel", Left = 310, Width = 75, Top = 80, DialogResult = DialogResult.Cancel, FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(62, 62, 66), ForeColor = Color.White };

                confirmation.Click += (sender, e) => { prompt.Close(); };
                cancel.Click += (sender, e) => { prompt.Close(); };

                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(cancel);
                prompt.AcceptButton = confirmation;
                prompt.CancelButton = cancel;

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
            }
        }

        #endregion
    }
}