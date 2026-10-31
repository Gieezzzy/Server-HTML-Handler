using System;
using System.Drawing;
using Server_HTML_Handler.Services;

namespace Server_HTML_Handler.View.Contents
{
    public partial class ConsoleUserControl
    {
        #region Multi-Developer Staging & Deployment System (No Git)

        private void HandleDeployCommand(string[] parts)
        {
            if (parts.Length < 2 || parts[1].ToLower() == "help")
            {
                PrintDeployGuide();
                return;
            }

            string subCmd = parts[1].ToLower();

            switch (subCmd)
            {
                case "list":
                    ListDeployProjects();
                    break;

                case "select":
                    if (parts.Length < 3)
                    {
                        AppendConsole("Usage: deploy select <project_name>", Color.Orange);
                        AppendConsole("Type 'deploy list' to see available projects.", Color.Gray);
                        return;
                    }
                    if (DeploymentStagingService.SelectProject(parts[2].Trim(), out string selectMsg))
                    {
                        AppendConsole($"[SUCCESS] {selectMsg}", Color.Lime);
                    }
                    else
                    {
                        AppendConsole($"[ERROR] {selectMsg}", Color.Red);
                    }
                    break;

                case "init":
                    if (parts.Length < 3)
                    {
                        AppendConsole("Usage: deploy init <dev_tag>-<dev_name>   (e.g. deploy init Dev1-Marc)", Color.Orange);
                        return;
                    }
                    string devTag = parts[2].Trim();
                    if (DeploymentStagingService.InitDeveloper(devTag, out string upPath, out string privPath, out string initErr))
                    {
                        AppendConsole($"[SUCCESS] Workspace created for '{devTag}' in project '{DeploymentStagingService.SelectedProject}'!", Color.Lime);
                        AppendConsole($"  [Update Folder] : {upPath}", Color.White);
                        AppendConsole($"  [Private Folder]: {privPath}", Color.White);
                        AppendConsole($"Tip: Run 'deploy sync-private {devTag}' to copy current live files into Private/ folder.", Color.LightBlue);
                    }
                    else
                    {
                        AppendConsole($"[ERROR] {initErr}", Color.Red);
                    }
                    break;

                case "devs":
                    var devs = DeploymentStagingService.GetDevelopers(out string devsErr);
                    if (!string.IsNullOrEmpty(devsErr))
                    {
                        AppendConsole($"[ERROR] {devsErr}", Color.Red);
                        return;
                    }
                    AppendConsole($"Developers in project [{DeploymentStagingService.SelectedProject}]:", Color.Cyan);
                    if (devs.Count == 0)
                    {
                        AppendConsole("  (No developers initialized yet. Run: deploy init Dev1-Marc)", Color.Gray);
                    }
                    else
                    {
                        foreach (var d in devs)
                        {
                            AppendConsole($"  • {d}", Color.White);
                        }
                    }
                    break;

                case "check":
                    if (parts.Length < 3)
                    {
                        AppendConsole("Usage: deploy check <dev_tag>-<dev_name>   (e.g. deploy check Dev1-Marc)", Color.Orange);
                        return;
                    }
                    string checkTag = parts[2].Trim();
                    AppendConsole($"[CHECK] Inspecting Update folder for '{checkTag}' in '{DeploymentStagingService.SelectedProject}'...", Color.Cyan);
                    bool checkOk = DeploymentStagingService.CheckUpdate(checkTag, (msg, col) => AppendConsole(msg, col), out int fCount, out int errCount);
                    if (checkOk && fCount > 0 && errCount == 0)
                    {
                        AppendConsole($"[PASSED] All {fCount} file(s) valid! Run 'deploy merge {checkTag}' to deploy live.", Color.Lime);
                    }
                    break;

                case "merge":
                    if (parts.Length < 3)
                    {
                        AppendConsole("Usage: deploy merge <dev_tag>-<dev_name>   (e.g. deploy merge Dev1-Marc)", Color.Orange);
                        return;
                    }
                    string mergeTag = parts[2].Trim();
                    AppendConsole($"[MERGE] Merging '{mergeTag}' into '{DeploymentStagingService.SelectedProject}'...", Color.Cyan);
                    if (DeploymentStagingService.MergeUpdate(mergeTag, (msg, col) => AppendConsole(msg, col), out string backupZip))
                    {
                        AppendConsole($"[SUCCESS] Deployed! Backup snapshot saved as: {backupZip}", Color.Lime);
                        AppendConsole("To undo this deployment at any time, run: deploy rollback", Color.Yellow);
                    }
                    break;

                case "sync-all":
                    DeploymentStagingService.SyncLiveToAllDevelopers((msg, col) => AppendConsole(msg, col), out _, out _);
                    break;

                case "sync-private":
                case "sync":
                    if (parts.Length >= 3 && string.Equals(parts[2].Trim(), "all", StringComparison.OrdinalIgnoreCase))
                    {
                        DeploymentStagingService.SyncLiveToAllDevelopers((msg, col) => AppendConsole(msg, col), out _, out _);
                        break;
                    }
                    if (parts.Length < 3)
                    {
                        AppendConsole("Usage: deploy sync <dev_tag>-<dev_name>   (e.g. deploy sync Dev1-Marc)", Color.Orange);
                        AppendConsole("   or: deploy sync-all                   (Syncs all developers at once)", Color.Cyan);
                        return;
                    }
                    string syncTag = parts[2].Trim();
                    DeploymentStagingService.SyncLiveToPrivate(syncTag, (msg, col) => AppendConsole(msg, col));
                    break;

                case "backups":
                    var backups = DeploymentStagingService.GetBackupsList();
                    AppendConsole($"Version Snapshot Backups ({backups.Count} total):", Color.Cyan);
                    if (backups.Count == 0)
                    {
                        AppendConsole("  (No backups found in Developing\\_Backups\\)", Color.Gray);
                    }
                    else
                    {
                        foreach (var b in backups)
                        {
                            double mb = b.Length / (1024.0 * 1024.0);
                            AppendConsole($"  • {b.Name}  [{mb:F2} MB]  ({b.CreationTime:yyyy-MM-dd HH:mm:ss})", Color.White);
                        }
                    }
                    break;

                case "rollback":
                    string specificBackup = parts.Length > 2 ? parts[2].Trim() : "";
                    DeploymentStagingService.Rollback(specificBackup, (msg, col) => AppendConsole(msg, col));
                    break;

                default:
                    if (parts[1].Contains("-"))
                    {
                        AppendConsole($"[INFO] Target developer workspace: '{parts[1]}'.", Color.Cyan);
                        AppendConsole($"Run 'deploy check {parts[1]}' to verify, or 'deploy merge {parts[1]}' to deploy.", Color.LightBlue);
                    }
                    else
                    {
                        AppendConsole($"Unknown deploy command: '{parts[1]}'. Type 'deploy help' for guide.", Color.Orange);
                    }
                    break;
            }
        }

        private void PrintDeployGuide()
        {
            AppendConsole("==================================================================", Color.Cyan);
            AppendConsole("      MULTI-DEVELOPER WORKSPACE & ZERO-GIT DEPLOYMENT GUIDE       ", Color.Yellow);
            AppendConsole("==================================================================", Color.Cyan);
            AppendConsole("1. WORKSPACE STRUCTURE ON DISK:", Color.White);
            AppendConsole("   D:\\html\\Developing\\<project_name>\\<dev_tag>-<name>\\", Color.Gray);
            AppendConsole("      ├── Update/    <- Modified/new files, or the developer's WHOLE project", Color.Gray);
            AppendConsole("      ├── Private/   <- Developer's full private test copy of the site", Color.Gray);
            AppendConsole("      ├── Base/      <- Untouched snapshot of live taken at sync (merge base)", Color.Gray);
            AppendConsole("      └── Conflicts/ <- Files where live and developer changed the same lines", Color.Gray);
            AppendConsole("", Color.White);
            AppendConsole("2. STEP-BY-STEP WORKFLOW:", Color.White);
            AppendConsole("   Step 1: deploy list                  (See all projects in D:\\html)", Color.Lime);
            AppendConsole("   Step 2: deploy select NEXUS-L        (Select active project)", Color.Lime);
            AppendConsole("   Step 3: deploy init Dev1-Marc        (Creates Developing\\NEXUS-L\\Dev1-Marc\\{Update,Private})", Color.Lime);
            AppendConsole("   Step 4: deploy sync Dev1-Marc        (or: deploy sync-all to update all developers at once)", Color.Lime);
            AppendConsole("   Step 5: [Developer works in Private, puts finished files into Update/ folder]", Color.Yellow);
            AppendConsole("   Step 6: deploy check Dev1-Marc       (Previews NEW / MERGE / CONFLICT per file, PHP lint)", Color.Lime);
            AppendConsole("   Step 7: deploy merge Dev1-Marc       (ZIP backup, then applies ONLY the developer's changed lines)", Color.Lime);
            AppendConsole("   Step 8: deploy rollback              (If bug found, restores previous version in 2s)", Color.Lime);
            AppendConsole("==================================================================", Color.Cyan);
        }

        private void ListDeployProjects()
        {
            var projects = DeploymentStagingService.GetAvailableProjects();
            AppendConsole($"Available Projects in {DeploymentStagingService.GetHostingPath()}:", Color.Cyan);
            if (projects.Count == 0)
            {
                AppendConsole("  (No projects found)", Color.Gray);
            }
            else
            {
                foreach (var p in projects)
                {
                    bool isCurrent = string.Equals(DeploymentStagingService.SelectedProject, p, StringComparison.OrdinalIgnoreCase);
                    string marker = isCurrent ? " [ACTIVE SELECTED *]" : "";
                    Color col = isCurrent ? Color.Lime : Color.White;
                    AppendConsole($"  • {p}{marker}", col);
                }
            }

            if (string.IsNullOrWhiteSpace(DeploymentStagingService.SelectedProject))
            {
                AppendConsole("Tip: Select a project using: deploy select <project_name>", Color.Orange);
            }
            else
            {
                AppendConsole($"Current Active Project: {DeploymentStagingService.SelectedProject}", Color.Lime);
            }
        }

        #endregion
    }
}
