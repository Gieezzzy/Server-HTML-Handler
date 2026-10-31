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
            return (!string.IsNullOrWhiteSpace(path) && Directory.Exists(path)) ? path : @"E:\html";
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
                                + "1. Put modified or new files here with their relative directory structure\r\n"
                                + "   (uploading the whole project is fine: only the lines you changed are merged).\r\n"
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

        #region Line-Level Merge Planning

        // Never merged from a developer upload: environment config, runtime data and generated/tool output
        private static readonly HashSet<string> ProtectedDirNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Developing", ".git", "graphify-out", "writable", "node_modules"
        };

        private static readonly HashSet<string> ProtectedRootFiles = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            ".env"
        };

        private enum MergeAction
        {
            New,        // File does not exist on live: copied as-is
            Merged,     // Developer's changed lines applied into the live file
            Resolved,   // Conflict resolved manually via the Conflicts folder
            Unchanged,  // Developer's file matches live, or the developer did not change it
            Protected,  // Skipped by policy (.env, writable/, node_modules/, ...)
            Conflict    // Developer and live changed the same lines differently
        }

        private class FileMergePlan
        {
            public string RelPath;
            public string SourceFile;
            public MergeAction Action;
            public byte[] Output;
            public int DevChanges;
            public int LiveChangesKept;
            public string Note = "";
        }

        private static string GetDevDir(string devTag) => Path.Combine(GetDevelopingPath(), _selectedProject, devTag);

        private static bool IsProtected(string relPath)
        {
            var segments = relPath.Split(new[] { '\\', '/' }, StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length == 1 && ProtectedRootFiles.Contains(segments[0])) return true;
            return segments.Take(segments.Length - 1).Any(s => ProtectedDirNames.Contains(s));
        }

        private static bool BytesEqual(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] != b[i]) return false;
            }
            return true;
        }

        private static byte[] ReadIfExists(string path) => File.Exists(path) ? File.ReadAllBytes(path) : null;

        /// <summary>
        /// Works out, file by file, what merging the developer's Update folder into live would do.
        /// Uses the developer's Base snapshot (live at sync time) as the common ancestor; falls back to Private.
        /// </summary>
        private static List<FileMergePlan> BuildMergePlan(string devTag, Action<string, Color> logger, out string baseRoot)
        {
            string devDir = GetDevDir(devTag);
            string updateDir = Path.Combine(devDir, "Update");
            string conflictsDir = Path.Combine(devDir, "Conflicts");
            string liveProjDir = Path.Combine(GetHostingPath(), _selectedProject);

            baseRoot = Path.Combine(devDir, "Base");
            if (!Directory.Exists(baseRoot))
            {
                string privateDir = Path.Combine(devDir, "Private");
                if (Directory.Exists(privateDir) && Directory.EnumerateFileSystemEntries(privateDir).Any())
                {
                    logger($"[WARN] No Base snapshot for '{devTag}'. Using Private\\ as the merge base (assumes it was not edited).", Color.Orange);
                    baseRoot = privateDir;
                }
                else
                {
                    logger($"[WARN] No Base or Private snapshot for '{devTag}'. Every changed existing file will be reported as a conflict.", Color.Orange);
                    baseRoot = null;
                }
            }

            var plans = new List<FileMergePlan>();
            var files = Directory.GetFiles(updateDir, "*.*", SearchOption.AllDirectories)
                .Where(f => !Path.GetFileName(f).StartsWith("README_INSTRUCTIONS"));

            foreach (var file in files)
            {
                string relPath = file.Substring(updateDir.Length).TrimStart('\\', '/');
                var plan = new FileMergePlan { RelPath = relPath, SourceFile = file };
                plans.Add(plan);

                if (IsProtected(relPath))
                {
                    plan.Action = MergeAction.Protected;
                    continue;
                }

                byte[] dev = File.ReadAllBytes(file);
                byte[] live = ReadIfExists(Path.Combine(liveProjDir, relPath));
                byte[] baseBytes = baseRoot == null ? null : ReadIfExists(Path.Combine(baseRoot, relPath));

                PlanFile(plan, dev, live, baseBytes, devTag);

                if (plan.Action == MergeAction.Conflict)
                {
                    ApplyManualResolution(plan, Path.Combine(conflictsDir, relPath));
                }
            }

            return plans;
        }

        private static void PlanFile(FileMergePlan plan, byte[] dev, byte[] live, byte[] baseBytes, string devTag)
        {
            if (live == null)
            {
                if (baseBytes == null)
                {
                    plan.Action = MergeAction.New;
                    plan.Output = dev;
                }
                else if (BytesEqual(dev, baseBytes))
                {
                    plan.Action = MergeAction.Unchanged;
                    plan.Note = "deleted on live since sync; developer did not change it, not restored";
                }
                else
                {
                    plan.Action = MergeAction.Conflict;
                    plan.Note = "deleted on live since sync, but changed by developer";
                }
                return;
            }

            if (BytesEqual(dev, live))
            {
                plan.Action = MergeAction.Unchanged;
                return;
            }

            var devText = ThreeWayMerge.TryParseText(dev);
            var liveText = ThreeWayMerge.TryParseText(live);
            var baseText = baseBytes == null ? null : ThreeWayMerge.TryParseText(baseBytes);

            if (devText != null && liveText != null && ThreeWayMerge.SameLines(devText.Lines, liveText.Lines))
            {
                plan.Action = MergeAction.Unchanged; // Only line endings / BOM differ
                return;
            }

            if (baseBytes == null)
            {
                plan.Action = MergeAction.Conflict;
                plan.Note = "exists on live but not in the base snapshot, so the developer's changes cannot be isolated";
                return;
            }

            if (devText == null || liveText == null || baseText == null)
            {
                // Binary (images, fonts, archives): whole-file decision
                if (BytesEqual(baseBytes, live))
                {
                    plan.Action = MergeAction.Merged;
                    plan.Output = dev;
                    plan.DevChanges = 1;
                    plan.Note = "binary file replaced";
                }
                else if (BytesEqual(baseBytes, dev))
                {
                    plan.Action = MergeAction.Unchanged;
                }
                else
                {
                    plan.Action = MergeAction.Conflict;
                    plan.Note = "binary file changed both on live and by developer";
                }
                return;
            }

            var result = ThreeWayMerge.Merge(baseText.Lines, liveText.Lines, devText.Lines, devTag);
            plan.DevChanges = result.DevChanges;
            plan.LiveChangesKept = result.LiveChangesKept;

            if (result.Conflicts > 0)
            {
                plan.Action = MergeAction.Conflict;
                plan.Note = $"{result.Conflicts} overlapping change(s)";
                plan.Output = ThreeWayMerge.ToBytes(result.Lines, liveText); // Marker file for manual resolution
            }
            else if (result.DevChanges == 0)
            {
                plan.Action = MergeAction.Unchanged;
            }
            else
            {
                plan.Action = MergeAction.Merged;
                plan.Output = ThreeWayMerge.ToBytes(result.Lines, liveText);
            }
        }

        /// <summary>A file placed in Conflicts\ without conflict markers is taken as the final version.</summary>
        private static void ApplyManualResolution(FileMergePlan plan, string resolvedPath)
        {
            if (!File.Exists(resolvedPath))
            {
                if (plan.Output != null)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(resolvedPath));
                    File.WriteAllBytes(resolvedPath, plan.Output);
                }
                plan.Output = null;
                return;
            }

            byte[] resolved = File.ReadAllBytes(resolvedPath);
            var text = ThreeWayMerge.TryParseText(resolved);
            if (text != null && ThreeWayMerge.HasConflictMarkers(text.Lines))
            {
                plan.Output = null;
                plan.Note += " (Conflicts file still has markers)";
                return;
            }

            plan.Action = MergeAction.Resolved;
            plan.Output = resolved;
            plan.Note = "using manually resolved file from Conflicts\\";
        }

        private static void LogPlan(List<FileMergePlan> plans, string devTag, Action<string, Color> logger)
        {
            foreach (var p in plans.OrderBy(p => p.Action).ThenBy(p => p.RelPath))
            {
                string note = string.IsNullOrEmpty(p.Note) ? "" : $" - {p.Note}";
                switch (p.Action)
                {
                    case MergeAction.New:
                        logger($"  [NEW]      {p.RelPath}", Color.Lime);
                        break;
                    case MergeAction.Merged:
                        string kept = p.LiveChangesKept > 0 ? $", {p.LiveChangesKept} live change(s) kept" : "";
                        logger($"  [MERGE]    {p.RelPath} ({p.DevChanges} developer change(s) applied{kept}){note}", Color.Yellow);
                        break;
                    case MergeAction.Resolved:
                        logger($"  [RESOLVED] {p.RelPath}{note}", Color.Yellow);
                        break;
                    case MergeAction.Conflict:
                        logger($"  [CONFLICT] {p.RelPath}{note}", Color.Red);
                        break;
                    case MergeAction.Unchanged:
                        if (!string.IsNullOrEmpty(p.Note)) logger($"  [SKIP]     {p.RelPath}{note}", Color.Gray);
                        break;
                }
            }

            int unchanged = plans.Count(p => p.Action == MergeAction.Unchanged);
            int protectedCount = plans.Count(p => p.Action == MergeAction.Protected);
            if (unchanged > 0) logger($"  ({unchanged} file(s) unchanged - already identical to live or not modified by {devTag})", Color.Gray);
            if (protectedCount > 0) logger($"  ({protectedCount} file(s) skipped - protected: .env, writable\\, node_modules\\, graphify-out\\)", Color.Gray);

            int conflicts = plans.Count(p => p.Action == MergeAction.Conflict);
            if (conflicts > 0)
            {
                string conflictsDir = Path.Combine(GetDevDir(devTag), "Conflicts");
                logger($"[!] {conflicts} conflict(s). Edit the files in {conflictsDir}", Color.Orange);
                logger("    (remove the <<<<<<< / ||||||| / ======= / >>>>>>> markers, keeping the lines you want), then run merge again.", Color.Orange);
                logger("    For binary or deleted files, place the final version at the same path in Conflicts\\.", Color.Orange);
            }
        }

        private static string LintOutput(FileMergePlan plan)
        {
            string temp = Path.Combine(Path.GetTempPath(), $"deploy_lint_{Guid.NewGuid():N}.php");
            try
            {
                File.WriteAllBytes(temp, plan.Output);
                return RunPhpLint(temp).Replace(temp, plan.RelPath);
            }
            finally
            {
                try { File.Delete(temp); } catch { }
            }
        }

        private static List<FileMergePlan> GetPhpLintFailures(IEnumerable<FileMergePlan> writes, Action<string, Color> logger)
        {
            var failures = new List<FileMergePlan>();
            foreach (var plan in writes.Where(p => p.RelPath.EndsWith(".php", StringComparison.OrdinalIgnoreCase)))
            {
                string lint = LintOutput(plan);
                if (!string.IsNullOrEmpty(lint) && !lint.Contains("No syntax errors detected"))
                {
                    failures.Add(plan);
                    logger($"  [SYNTAX ERROR] {plan.RelPath} (after merge): {lint.Trim()}", Color.Red);
                }
            }
            return failures;
        }

        #endregion

        public static bool CheckUpdate(string devTag, Action<string, Color> logger, out int fileCount, out int errorCount)
        {
            fileCount = 0;
            errorCount = 0;

            if (string.IsNullOrWhiteSpace(_selectedProject))
            {
                logger("[!] No project selected. Run 'deploy select <project>' first.", Color.Orange);
                return false;
            }

            string updateDir = Path.Combine(GetDevDir(devTag), "Update");
            if (!Directory.Exists(updateDir))
            {
                logger($"[ERROR] Developer workspace not found at: {updateDir}", Color.Red);
                return false;
            }

            var plans = BuildMergePlan(devTag, logger, out _);
            if (plans.Count == 0)
            {
                logger($"[INFO] Update folder is empty for '{devTag}'. Place your modified files in:\n{updateDir}", Color.Orange);
                return true;
            }

            LogPlan(plans, devTag, logger);

            var writes = plans.Where(p => p.Output != null && p.Action != MergeAction.Conflict).ToList();
            fileCount = writes.Count;
            errorCount = plans.Count(p => p.Action == MergeAction.Conflict) + GetPhpLintFailures(writes, logger).Count;

            if (fileCount == 0 && errorCount == 0)
            {
                logger("[INFO] Nothing to deploy: no file in Update differs from live in a way the developer changed.", Color.Orange);
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

            string devDir = GetDevDir(devTag);
            string updateDir = Path.Combine(devDir, "Update");
            if (!Directory.Exists(updateDir))
            {
                logger($"[ERROR] Developer workspace not found: {updateDir}", Color.Red);
                return false;
            }

            // 1. Plan the line-level merge
            var plans = BuildMergePlan(devTag, logger, out string baseRoot);
            if (plans.Count == 0)
            {
                logger("[INFO] Update folder is empty. Nothing to merge.", Color.Orange);
                return false;
            }

            LogPlan(plans, devTag, logger);

            if (plans.Any(p => p.Action == MergeAction.Conflict))
            {
                logger("[ABORTED] Merge stopped because of conflicts. Nothing was changed on live.", Color.Red);
                return false;
            }

            var writes = plans.Where(p => p.Output != null).ToList();
            if (writes.Count == 0)
            {
                logger("[INFO] Nothing to deploy: no file in Update differs from live in a way the developer changed.", Color.Orange);
                return false;
            }

            // 2. Lint the merged result, not just the developer's copy
            if (GetPhpLintFailures(writes, logger).Count > 0)
            {
                logger("[ABORTED] PHP syntax errors in the merged result. Nothing was changed on live.", Color.Red);
                return false;
            }

            // 3. Automated Safety Snapshot
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

            // 4. Write merged files to Live Site
            int written = 0;
            try
            {
                foreach (var plan in writes)
                {
                    string targetFile = Path.Combine(liveProjDir, plan.RelPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(targetFile));
                    File.WriteAllBytes(targetFile, plan.Output);
                    written++;
                }
            }
            catch (Exception ex)
            {
                logger($"[ERROR] File write failed: {ex.Message}. Initiating rollback...", Color.Red);
                Rollback(backupName, logger);
                return false;
            }

            // 5. Advance the Base snapshot so the next upload is compared against what is now live
            string baseDir = Path.Combine(devDir, "Base");
            if (string.Equals(baseRoot, baseDir, StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    foreach (var plan in writes)
                    {
                        string baseFile = Path.Combine(baseDir, plan.RelPath);
                        Directory.CreateDirectory(Path.GetDirectoryName(baseFile));
                        File.WriteAllBytes(baseFile, plan.Output);
                    }
                }
                catch (Exception ex)
                {
                    logger($"[WARN] Could not update Base snapshot: {ex.Message}", Color.Orange);
                }
            }

            // 6. Move processed files (and resolved conflicts) to History
            try
            {
                string historyDir = Path.Combine(devDir, "History", DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                Directory.CreateDirectory(historyDir);

                foreach (var plan in plans)
                {
                    string dest = Path.Combine(historyDir, plan.RelPath);
                    Directory.CreateDirectory(Path.GetDirectoryName(dest));
                    File.Move(plan.SourceFile, dest);
                }

                string conflictsDir = Path.Combine(devDir, "Conflicts");
                if (Directory.Exists(conflictsDir))
                {
                    Directory.Move(conflictsDir, Path.Combine(historyDir, "_Conflicts"));
                }
            }
            catch { }

            int merged = writes.Count(p => p.Action != MergeAction.New);
            int added = writes.Count(p => p.Action == MergeAction.New);
            logger($"[SUCCESS] {written} file(s) from '{devTag}' deployed into '{_selectedProject}' ({merged} merged line-by-line, {added} new)!", Color.Lime);
            return true;
        }

        public static bool SyncLiveToPrivate(string devTag, Action<string, Color> logger)
        {
            if (string.IsNullOrWhiteSpace(_selectedProject))
            {
                logger("[!] No project selected. Run 'deploy select <project>' first.", Color.Orange);
                return false;
            }

            string devDir = GetDevDir(devTag);
            string liveProjDir = Path.Combine(GetHostingPath(), _selectedProject);
            string privateDir = Path.Combine(devDir, "Private");
            string baseDir = Path.Combine(devDir, "Base");
            string updateDir = Path.Combine(devDir, "Update");

            try
            {
                Directory.CreateDirectory(privateDir);
                CopyDirectoryRecursiveExcludingDeveloping(liveProjDir, privateDir);
                logger($"[SUCCESS] Private folder for '{devTag}' is synchronized with live project '{_selectedProject}'!", Color.Lime);
            }
            catch (Exception ex)
            {
                logger($"[ERROR] Sync failed: {ex.Message}", Color.Red);
                return false;
            }

            // Refreshing Base while work is pending would make the pending files look like they undo newer live changes
            bool hasPendingUpdate = Directory.Exists(updateDir) &&
                Directory.EnumerateFiles(updateDir, "*.*", SearchOption.AllDirectories)
                    .Any(f => !Path.GetFileName(f).StartsWith("README_INSTRUCTIONS"));
            if (hasPendingUpdate)
            {
                logger("[WARN] Update folder has files waiting to be merged, so the Base snapshot was NOT refreshed. Merge them first, then sync again.", Color.Orange);
                return true;
            }

            try
            {
                if (Directory.Exists(baseDir)) DeleteDirectoryForce(baseDir);
                CopyDirectoryRecursiveExcludingDeveloping(liveProjDir, baseDir, ProtectedDirNames);
                logger("[+] Base snapshot refreshed (used by 'deploy merge' to apply only the developer's changed lines).", Color.Lime);
                return true;
            }
            catch (Exception ex)
            {
                logger($"[ERROR] Base snapshot failed: {ex.Message}", Color.Red);
                return false;
            }
        }

        public static bool SyncLiveToAllDevelopers(Action<string, Color> logger, out int successCount, out int failCount)
        {
            successCount = 0;
            failCount = 0;

            if (string.IsNullOrWhiteSpace(_selectedProject))
            {
                logger("[!] No project selected. Run 'deploy select <project>' first.", Color.Orange);
                return false;
            }

            var devs = GetDevelopers(out string err);
            if (!string.IsNullOrEmpty(err))
            {
                logger($"[ERROR] Failed to list developers: {err}", Color.Red);
                return false;
            }

            if (devs.Count == 0)
            {
                logger($"[INFO] No registered developer workspaces found in project '{_selectedProject}'.", Color.Orange);
                logger("Tip: Run 'deploy init <dev_tag>-<name>' to register a developer.", Color.Gray);
                return false;
            }

            logger($"[SYNC-ALL] Synchronizing live site '{_selectedProject}' across {devs.Count} developer workspace(s)...", Color.Cyan);

            foreach (var dev in devs)
            {
                logger("--------------------------------------------------", Color.DarkGray);
                logger($"▶ Synchronizing developer: '{dev}'", Color.White);
                bool ok = SyncLiveToPrivate(dev, logger);
                if (ok) successCount++;
                else failCount++;
            }

            logger("==================================================", Color.DarkGray);
            if (failCount == 0)
            {
                logger($"[SUCCESS] All {successCount} developer workspace(s) in '{_selectedProject}' synchronized successfully with live!", Color.Lime);
            }
            else
            {
                logger($"[SUMMARY] Sync completed: {successCount} succeeded, {failCount} failed. Check log above for details.", Color.Orange);
            }

            return failCount == 0;
        }

        private static void DeleteDirectoryForce(string dir)
        {
            foreach (var file in Directory.GetFiles(dir, "*", SearchOption.AllDirectories))
            {
                File.SetAttributes(file, FileAttributes.Normal);
            }
            Directory.Delete(dir, true);
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

        private static void CopyDirectoryRecursiveExcludingDeveloping(string sourceDir, string destinationDir, ISet<string> extraExcludedDirs = null)
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
                    dirName.Equals("graphify-out", StringComparison.OrdinalIgnoreCase) ||
                    (extraExcludedDirs != null && extraExcludedDirs.Contains(dirName)))
                {
                    continue;
                }

                string newDestinationDir = Path.Combine(destinationDir, dirName);
                try
                {
                    CopyDirectoryRecursiveExcludingDeveloping(subDir, newDestinationDir, extraExcludedDirs);
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
