using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace Server_HTML_Handler.Services
{
    public static class DeploymentStagingService
    {
        private static string _selectedProject = "";
        public static string SelectedProject => _selectedProject;

        public static string GetHostingPath()
        {
            string path = Properties.Settings.Default.HTMLPath;
            return (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path)) ? path : @"D:\html";
        }

        public static string GetDevelopingPath() => Path.Combine(GetHostingPath(), "Developing");
        public static string GetBackupsPath() => Path.Combine(GetHostingPath(), "Developing", "_Backups");

        public static List<string> GetAvailableProjects()
        {
            string hosting = GetHostingPath();
            if (!Directory.Exists(hosting)) return new List<string>();

            return Directory.GetDirectories(hosting)
                .Select(Path.GetFileName)
                .Where(d => !d.Equals("Developing", StringComparison.OrdinalIgnoreCase) && !d.Equals("agora", StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        public static bool SelectProject(string projectName, out string message)
        {
            string target = Path.Combine(GetHostingPath(), projectName);
            if (!Directory.Exists(target))
            {
                message = $"Project folder does not exist: {target}";
                return false;
            }

            _selectedProject = projectName;
            string devProjectDir = Path.Combine(GetDevelopingPath(), _selectedProject);
            if (!Directory.Exists(devProjectDir))
            {
                Directory.CreateDirectory(devProjectDir);
            }

            message = $"Active project set to: {_selectedProject} ({target})";
            return true;
        }

        public static bool InitDeveloper(string devTag, out string updatePath, out string privatePath, out string error)
        {
            updatePath = "";
            privatePath = "";
            error = "";

            if (string.IsNullOrWhiteSpace(_selectedProject))
            {
                error = "No project selected. Please run 'deploy select <project_name>' first.";
                return false;
            }

            try
            {
                string devDir = Path.Combine(GetDevelopingPath(), _selectedProject, devTag);
                updatePath = Path.Combine(devDir, "Update");
                privatePath = Path.Combine(devDir, "Private");

                Directory.CreateDirectory(updatePath);
                Directory.CreateDirectory(privatePath);

                string readme = Path.Combine(updatePath, "README_INSTRUCTIONS.txt");
                if (!File.Exists(readme))
                {
                    string info = $"DEVELOPER WORKSPACE [{devTag}] - Project [{_selectedProject}]\r\n"
                                + "========================================================================\r\n"
                                + "1. Put ONLY modified or new files here with their relative directory structure.\r\n"
                                + "   Example:\r\n"
                                + "      Update\\app\\Controllers\\Auth.php\r\n"
                                + "      Update\\app\\Views\\auth\\login.php\r\n\r\n"
                                + "2. In Console, run: deploy check " + devTag + "\r\n"
                                + "3. If checks pass, run: deploy merge " + devTag + "\r\n";
                    File.WriteAllText(readme, info);
                }

                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        public static List<string> GetDevelopers(out string error)
        {
            error = "";
            if (string.IsNullOrWhiteSpace(_selectedProject))
            {
                error = "No project selected. Run 'deploy select <project>' first.";
                return new List<string>();
            }

            string projectDevDir = Path.Combine(GetDevelopingPath(), _selectedProject);
            if (!Directory.Exists(projectDevDir)) return new List<string>();

            return Directory.GetDirectories(projectDevDir)
                .Select(Path.GetFileName)
                .Where(d => !d.StartsWith("_"))
                .ToList();
        }

        public static bool CheckUpdate(string devTag, Action<string, Color> logger, out int fileCount, out int errorCount)
        {
            fileCount = 0;
            errorCount = 0;

            if (string.IsNullOrWhiteSpace(_selectedProject))
            {
                logger("[!] No project selected. Run 'deploy select <project>' first.", Color.Orange);
                return false;
            }

            string updateDir = Path.Combine(GetDevelopingPath(), _selectedProject, devTag, "Update");
            if (!Directory.Exists(updateDir))
            {
                logger($"[ERROR] Developer workspace not found at: {updateDir}", Color.Red);
                return false;
            }

            var files = Directory.GetFiles(updateDir, "*.*", SearchOption.AllDirectories)
                .Where(f => !Path.GetFileName(f).StartsWith("README_INSTRUCTIONS"))
                .ToList();

            fileCount = files.Count;
            if (fileCount == 0)
            {
                logger($"[INFO] Update folder is empty for '{devTag}'. Place your modified files in:\n{updateDir}", Color.Orange);
                return true;
            }

            string liveProjDir = Path.Combine(GetHostingPath(), _selectedProject);

            foreach (var file in files)
            {
                string relPath = file.Substring(updateDir.Length).TrimStart('\\', '/');
                string liveFile = Path.Combine(liveProjDir, relPath);

                if (file.EndsWith(".php", StringComparison.OrdinalIgnoreCase))
                {
                    string lintOutput = RunPhpLint(file);
                    if (!string.IsNullOrEmpty(lintOutput) && !lintOutput.Contains("No syntax errors detected"))
                    {
                        errorCount++;
                        logger($"  [SYNTAX ERROR] {relPath}: {lintOutput.Trim()}", Color.Red);
                        continue;
                    }
                }

                if (File.Exists(liveFile))
                {
                    logger($"  [MODIFIED] {relPath} (Will overwrite live file)", Color.Yellow);
                }
                else
                {
                    logger($"  [NEW]      {relPath} (New file to be created)", Color.Lime);
                }
            }

            return errorCount == 0;
        }

        public static bool MergeUpdate(string devTag, Action<string, Color> logger, out string backupFile)
        {
            backupFile = "";

            if (string.IsNullOrWhiteSpace(_selectedProject))
            {
                logger("[!] No project selected. Run 'deploy select <project>' first.", Color.Orange);
                return false;
            }

            string updateDir = Path.Combine(GetDevelopingPath(), _selectedProject, devTag, "Update");
            if (!Directory.Exists(updateDir))
            {
                logger($"[ERROR] Developer workspace not found: {updateDir}", Color.Red);
                return false;
            }

            var files = Directory.GetFiles(updateDir, "*.*", SearchOption.AllDirectories)
                .Where(f => !Path.GetFileName(f).StartsWith("README_INSTRUCTIONS"))
                .ToList();

            if (files.Count == 0)
            {
                logger("[INFO] Update folder is empty. Nothing to merge.", Color.Orange);
                return false;
            }

            // 1. Lint check
            foreach (var file in files)
            {
                if (file.EndsWith(".php", StringComparison.OrdinalIgnoreCase))
                {
                    string lint = RunPhpLint(file);
                    if (!string.IsNullOrEmpty(lint) && !lint.Contains("No syntax errors detected"))
                    {
                        logger($"[ABORTED] PHP Syntax Error in {Path.GetFileName(file)}:\n{lint}", Color.Red);
                        return false;
                    }
                }
            }

            // 2. Automated Safety Snapshot
            string liveProjDir = Path.Combine(GetHostingPath(), _selectedProject);
            string backupName = $"{_selectedProject}_{DateTime.Now:yyyyMMdd_HHmmss}_{devTag}.zip";
            string backupZipPath = Path.Combine(GetBackupsPath(), backupName);
            backupFile = backupName;

            try
            {
                Directory.CreateDirectory(GetBackupsPath());
                if (File.Exists(backupZipPath)) File.Delete(backupZipPath);
                ZipFile.CreateFromDirectory(liveProjDir, backupZipPath);
                logger($"[+] Auto-snapshot created: Developing\\_Backups\\{backupName}", Color.Lime);
            }
            catch (Exception ex)
            {
                logger($"[FATAL] Failed to create pre-merge backup: {ex.Message}. Merge aborted.", Color.Red);
                return false;
            }

            // 3. Copy files to Live Site
            int copied = 0;
            try
            {
                foreach (var file in files)
                {
                    string relPath = file.Substring(updateDir.Length).TrimStart('\\', '/');
                    string targetFile = Path.Combine(liveProjDir, relPath);
                    string targetFolder = Path.GetDirectoryName(targetFile);

                    if (!Directory.Exists(targetFolder)) Directory.CreateDirectory(targetFolder);
                    File.Copy(file, targetFile, true);
                    copied++;
                }
            }
            catch (Exception ex)
            {
                logger($"[ERROR] File copy failed: {ex.Message}. Initiating rollback...", Color.Red);
                Rollback(backupName, logger);
                return false;
            }

            // 4. Move processed files to History
            try
            {
                string historyDir = Path.Combine(GetDevelopingPath(), _selectedProject, devTag, "History", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                Directory.CreateDirectory(historyDir);

                foreach (var file in files)
                {
                    string relPath = file.Substring(updateDir.Length).TrimStart('\\', '/');
                    string dest = Path.Combine(historyDir, relPath);
                    string destFolder = Path.GetDirectoryName(dest);
                    if (!Directory.Exists(destFolder)) Directory.CreateDirectory(destFolder);
                    File.Move(file, dest);
                }
            }
            catch { }

            logger($"[SUCCESS] {copied} file(s) from '{devTag}' deployed into '{_selectedProject}'!", Color.Lime);
            return true;
        }

        public static bool SyncLiveToPrivate(string devTag, Action<string, Color> logger)
        {
            if (string.IsNullOrWhiteSpace(_selectedProject))
            {
                logger("[!] No project selected. Run 'deploy select <project>' first.", Color.Orange);
                return false;
            }

            string liveProjDir = Path.Combine(GetHostingPath(), _selectedProject);
            string privateDir = Path.Combine(GetDevelopingPath(), _selectedProject, devTag, "Private");

            try
            {
                Directory.CreateDirectory(privateDir);
                CopyDirectoryRecursiveExcludingDeveloping(liveProjDir, privateDir);
                logger($"[SUCCESS] Private folder for '{devTag}' is synchronized with live project '{_selectedProject}'!", Color.Lime);
                return true;
            }
            catch (Exception ex)
            {
                logger($"[ERROR] Sync failed: {ex.Message}", Color.Red);
                return false;
            }
        }

        public static bool Rollback(string backupName, Action<string, Color> logger)
        {
            if (string.IsNullOrWhiteSpace(_selectedProject))
            {
                logger("[!] No project selected.", Color.Orange);
                return false;
            }

            string backupsPath = GetBackupsPath();
            if (!Directory.Exists(backupsPath))
            {
                logger("[ERROR] No backups found in Developing\\_Backups\\", Color.Red);
                return false;
            }

            string targetZip = "";
            if (string.IsNullOrWhiteSpace(backupName))
            {
                var latest = Directory.GetFiles(backupsPath, $"{_selectedProject}_*.zip")
                    .Select(f => new FileInfo(f))
                    .OrderByDescending(f => f.CreationTime)
                    .FirstOrDefault();

                if (latest == null)
                {
                    latest = Directory.GetFiles(backupsPath, "*.zip")
                        .Select(f => new FileInfo(f))
                        .OrderByDescending(f => f.CreationTime)
                        .FirstOrDefault();
                }

                if (latest == null)
                {
                    logger($"[ERROR] No snapshot backups found for project '{_selectedProject}'.", Color.Red);
                    return false;
                }
                targetZip = latest.FullName;
            }
            else
            {
                targetZip = Path.Combine(backupsPath, backupName);
                if (!File.Exists(targetZip))
                {
                    logger($"[ERROR] Backup file not found: {targetZip}", Color.Red);
                    return false;
                }
            }

            string liveProjDir = Path.Combine(GetHostingPath(), _selectedProject);
            logger($"[ROLLBACK] Restoring live site from snapshot '{Path.GetFileName(targetZip)}'...", Color.Yellow);

            try
            {
                using (ZipArchive archive = ZipFile.OpenRead(targetZip))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        if (string.IsNullOrEmpty(entry.Name)) continue; // Directory entry
                        string destPath = Path.Combine(liveProjDir, entry.FullName);
                        string destDir = Path.GetDirectoryName(destPath);
                        if (!Directory.Exists(destDir)) Directory.CreateDirectory(destDir);
                        entry.ExtractToFile(destPath, true);
                    }
                }
                logger("[SUCCESS] Rollback complete! The live project has been restored.", Color.Lime);
                return true;
            }
            catch (Exception ex)
            {
                logger($"[ERROR] Rollback failed: {ex.Message}", Color.Red);
                return false;
            }
        }

        public static List<FileInfo> GetBackupsList()
        {
            string backupsPath = GetBackupsPath();
            if (!Directory.Exists(backupsPath)) return new List<FileInfo>();

            return Directory.GetFiles(backupsPath, "*.zip")
                .Select(f => new FileInfo(f))
                .OrderByDescending(f => f.CreationTime)
                .ToList();
        }

        private static void CopyDirectoryRecursiveExcludingDeveloping(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string targetFilePath = Path.Combine(destinationDir, Path.GetFileName(file));
                try
                {
                    File.Copy(file, targetFilePath, true);
                }
                catch (FileNotFoundException) { } // File removed by another process after enumeration
                catch (DirectoryNotFoundException) { }
            }

            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(subDir);
                if (dirName.Equals("Developing", StringComparison.OrdinalIgnoreCase) ||
                    dirName.Equals(".git", StringComparison.OrdinalIgnoreCase) ||
                    dirName.Equals("graphify-out", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string newDestinationDir = Path.Combine(destinationDir, dirName);
                try
                {
                    CopyDirectoryRecursiveExcludingDeveloping(subDir, newDestinationDir);
                }
                catch (DirectoryNotFoundException) { } // Folder removed by another process after enumeration
            }
        }

        public static string RunPhpLint(string phpFilePath)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "php",
                    Arguments = $"-l \"{phpFilePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (var proc = Process.Start(psi))
                {
                    string output = proc.StandardOutput.ReadToEnd();
                    string error = proc.StandardError.ReadToEnd();
                    proc.WaitForExit();
                    return string.IsNullOrWhiteSpace(error) ? output : error;
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
