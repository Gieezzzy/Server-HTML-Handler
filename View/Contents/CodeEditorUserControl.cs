using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace Server_HTML_Handler.View.Contents
{
    public partial class CodeEditorUserControl : UserControl
    {
        private string _currentFilePath = "";
        private string _activeRootDirectory = "";
        private bool _isDirty = false;
        private bool _isProgrammaticChange = false;

        private const string DummyNodeTag = "DUMMY_EXPAND_NODE";

        public CodeEditorUserControl()
        {
            InitializeComponent();
            InitializeDefaultPaths();
        }

        // ==========================================
        // Path Auto-Detection
        // ==========================================
        private void InitializeDefaultPaths()
        {
            string savedPath = Properties.Settings.Default.HTMLPath;
            if (!string.IsNullOrEmpty(savedPath) && Directory.Exists(savedPath))
            {
                _activeRootDirectory = savedPath;
            }
            else
            {
                _activeRootDirectory = DetectHtdocsPath();
                Properties.Settings.Default.HTMLPath = _activeRootDirectory;
                Properties.Settings.Default.Save();
            }
            LoadDirectoryLazy(_activeRootDirectory);
        }

        private string DetectHtdocsPath()
        {
            string[] possiblePaths = {
                @"C:\xampp\htdocs",
                @"D:\xampp\htdocs",
                @"E:\xampp\htdocs",
                @"C:\wamp64\www"
            };

            foreach (var path in possiblePaths)
            {
                if (Directory.Exists(path)) return path;
            }

            return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        }

        private string GetXamppRoot()
        {
            if (string.IsNullOrEmpty(_activeRootDirectory)) return @"C:\xampp";
            DirectoryInfo dir = new DirectoryInfo(_activeRootDirectory);
            return dir.Parent != null ? dir.Parent.FullName : @"C:\xampp";
        }

        // ==========================================
        // Lazy Loading TreeView Engine (Zero UI Lag)
        // ==========================================
        private void LoadDirectoryLazy(string dirPath)
        {
            if (!Directory.Exists(dirPath)) return;

            fileTree.BeginUpdate();
            fileTree.Nodes.Clear();

            DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
            TreeNode rootNode = new TreeNode(dirInfo.Name)
            {
                Tag = dirInfo.FullName,
                ForeColor = Color.Orange,
                NodeFont = new Font(fileTree.Font, FontStyle.Bold) // Changed Font to NodeFont
            };


            PopulateSubDirectoriesAndFiles(rootNode, dirInfo.FullName);
            fileTree.Nodes.Add(rootNode);
            rootNode.Expand();

            fileTree.EndUpdate();
        }

        private void PopulateSubDirectoriesAndFiles(TreeNode parentNode, string dirPath)
        {
            parentNode.Nodes.Clear();

            try
            {
                // Load Child Directories
                foreach (string d in Directory.GetDirectories(dirPath))
                {
                    DirectoryInfo subDir = new DirectoryInfo(d);
                    if ((subDir.Attributes & FileAttributes.Hidden) != 0) continue;

                    TreeNode dirNode = new TreeNode(subDir.Name)
                    {
                        Tag = subDir.FullName,
                        ForeColor = Color.Orange
                    };

                    // Insert dummy node to show [+] expand arrow
                    dirNode.Nodes.Add(new TreeNode("") { Tag = DummyNodeTag });
                    parentNode.Nodes.Add(dirNode);
                }

                // Load Child Files
                foreach (string f in Directory.GetFiles(dirPath))
                {
                    FileInfo fileInfo = new FileInfo(f);
                    if ((fileInfo.Attributes & FileAttributes.Hidden) != 0) continue;

                    TreeNode fileNode = new TreeNode(fileInfo.Name)
                    {
                        Tag = fileInfo.FullName,
                        ForeColor = Color.LightGray
                    };
                    parentNode.Nodes.Add(fileNode);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading folder: " + ex.Message);
            }
        }

        private void FileTree_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (e.Node == null || e.Node.Tag == null) return;

            // Lazy Expand: Replace dummy node with real folder contents
            if (e.Node.Nodes.Count == 1 && e.Node.Nodes[0].Tag?.ToString() == DummyNodeTag)
            {
                fileTree.BeginUpdate();
                string fullPath = e.Node.Tag.ToString();
                PopulateSubDirectoriesAndFiles(e.Node, fullPath);
                fileTree.EndUpdate();
            }
        }

        // ==========================================
        // File Opening & Unsaved Protection
        // ==========================================
        private void FileTree_NodeDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Node?.Tag == null) return;
            string path = e.Node.Tag.ToString();

            if (File.Exists(path))
            {
                OpenFileWithCheck(path);
            }
        }

        private void FileTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                fileTree.SelectedNode = e.Node;
            }
        }

        private void OpenFileWithCheck(string path)
        {
            if (_isDirty)
            {
                var result = MessageBox.Show("You have unsaved changes! Do you want to save before opening a new file?", "Unsaved Changes", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
                if (result == DialogResult.Yes)
                {
                    SaveCurrentFile();
                }
                else if (result == DialogResult.Cancel)
                {
                    return;
                }
            }

            OpenFile(path);
        }

        private void OpenFile(string path)
        {
            if (!File.Exists(path))
            {
                MessageBox.Show("File not found: " + path, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                _currentFilePath = path;
                _isProgrammaticChange = true;

                textEditor.Text = File.ReadAllText(path);
                _isDirty = false;
                _isProgrammaticChange = false;

                lblCurrentFile.Text = $"Editing: {path}";
                lblCurrentFile.ForeColor = Color.Cyan;
                UpdateStats();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not read file: " + ex.Message, "Read Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TextEditor_TextChanged(object sender, EventArgs e)
        {
            if (!_isProgrammaticChange && !_isDirty && !string.IsNullOrEmpty(_currentFilePath))
            {
                _isDirty = true;
                lblCurrentFile.Text = $"Editing: {Path.GetFileName(_currentFilePath)} *";
                lblCurrentFile.ForeColor = Color.Orange;
            }
            UpdateStats();
        }

        private void UpdateStats()
        {
            int lines = textEditor.Lines.Length;
            int chars = textEditor.TextLength;
            lblEditorStats.Text = $"Lines: {lines} | Characters: {chars} | Status: {(_isDirty ? "Modified *" : "Saved")}";
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            SaveCurrentFile();
        }

        private void SaveCurrentFile()
        {
            if (string.IsNullOrEmpty(_currentFilePath))
            {
                MessageBox.Show("No file currently selected for saving.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                File.WriteAllText(_currentFilePath, textEditor.Text);
                _isDirty = false;
                lblCurrentFile.Text = $"Editing: {_currentFilePath}";
                lblCurrentFile.ForeColor = Color.Cyan;
                UpdateStats();
                MessageBox.Show("File saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show("Access Denied! You may need Administrator privileges to edit system files like hosts.", "Permission Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Save failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // ==========================================
        // Top Toolbar Config Actions
        // ==========================================
        private void BtnChangeRoot_Click(object sender, EventArgs e)
        {
            folderBrowserDialog.SelectedPath = _activeRootDirectory;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                _activeRootDirectory = folderBrowserDialog.SelectedPath;
                Properties.Settings.Default.HTMLPath = _activeRootDirectory;
                Properties.Settings.Default.Save();
                LoadDirectoryLazy(_activeRootDirectory);
            }
        }

        private void BtnVhosts_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(GetXamppRoot(), @"apache\conf\extra\httpd-vhosts.conf");
            OpenFileWithCheck(path);
        }

        private void BtnPhpIni_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(GetXamppRoot(), @"php\php.ini");
            OpenFileWithCheck(path);
        }

        private void BtnHttpdConf_Click(object sender, EventArgs e)
        {
            string path = Path.Combine(GetXamppRoot(), @"apache\conf\httpd.conf");
            OpenFileWithCheck(path);
        }

        private void BtnHostsFile_Click(object sender, EventArgs e)
        {
            string path = @"C:\Windows\System32\drivers\etc\hosts";
            OpenFileWithCheck(path);
        }

        // ==========================================
        // TreeView Context Menu Actions
        // ==========================================
        private void TsmiNewFile_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = fileTree.SelectedNode;
            if (selectedNode == null) return;

            string targetDir = Directory.Exists(selectedNode.Tag.ToString())
                ? selectedNode.Tag.ToString()
                : Path.GetDirectoryName(selectedNode.Tag.ToString());

            string inputName = PromptDialog.Show("Enter new filename (e.g., config.php or script.js):", "Create New File", "new_file.php");
            if (string.IsNullOrWhiteSpace(inputName)) return;

            string fullPath = Path.Combine(targetDir, inputName);
            if (File.Exists(fullPath))
            {
                MessageBox.Show("A file with this name already exists!", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                File.WriteAllText(fullPath, "");
                LoadDirectoryLazy(_activeRootDirectory);
                OpenFileWithCheck(fullPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not create file: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TsmiDelete_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = fileTree.SelectedNode;
            if (selectedNode == null || selectedNode.Tag == null) return;

            string path = selectedNode.Tag.ToString();
            if (File.Exists(path))
            {
                var confirm = MessageBox.Show($"Are you sure you want to delete:\n{Path.GetFileName(path)}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    try
                    {
                        File.Delete(path);
                        if (_currentFilePath == path)
                        {
                            _currentFilePath = "";
                            textEditor.Clear();
                            lblCurrentFile.Text = "No file selected";
                            _isDirty = false;
                        }
                        LoadDirectoryLazy(_activeRootDirectory);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Delete failed: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void TsmiOpenExplorer_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = fileTree.SelectedNode;
            if (selectedNode == null || selectedNode.Tag == null) return;

            string path = selectedNode.Tag.ToString();
            string folderToOpen = Directory.Exists(path) ? path : Path.GetDirectoryName(path);

            if (Directory.Exists(folderToOpen))
            {
                Process.Start("explorer.exe", folderToOpen);
            }
        }

        private void TsmiRefresh_Click(object sender, EventArgs e)
        {
            LoadDirectoryLazy(_activeRootDirectory);
        }

        // ==========================================
        // Pure C# Input Box Helper
        // ==========================================
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

                confirmation.Click += (s, e) => prompt.Close();
                cancel.Click += (s, e) => prompt.Close();

                prompt.Controls.Add(textLabel);
                prompt.Controls.Add(textBox);
                prompt.Controls.Add(confirmation);
                prompt.Controls.Add(cancel);
                prompt.AcceptButton = confirmation;
                prompt.CancelButton = cancel;

                return prompt.ShowDialog() == DialogResult.OK ? textBox.Text : "";
            }
        }
    }
}