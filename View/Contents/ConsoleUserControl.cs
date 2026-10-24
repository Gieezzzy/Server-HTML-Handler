using Server_HTML_Handler.Services;
﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using Npgsql;

namespace Server_HTML_Handler.View.Contents
{
    public partial class ConsoleUserControl : UserControl
    {
        private readonly List<string> _commandHistory = new List<string>();
        private int _historyIndex = -1;

        // Expanded Auto-Completion Keyword List
        private readonly string[] _knownCommands = new string[]
        {
            "sql", "db", "user", "acl", "checkuser", "blockip", "unblockip", "listblocked",
            "service", "log", "sysinfo", "ports", "backup", "restore", "export", "cmd", "clear", "help",
            "vhost", "ssl", "fail2ban", "watch", "schedule", "sync", "phpext",
            "killport", "cors", "stress", "gitpull", "env", "git", "mail",
            "lab", "tunnel", "xdebug", "composer", "opcache", "sqlite", "pg", "gitgraph", "stash"
        };

        // File Monitoring & Intrusion Prevention State
        private FileSystemWatcher _accessLogWatcher;
        private FileSystemWatcher _phpErrorLogWatcher;
        private long _lastLogPosition = 0;
        private bool _isWatchingTraffic = false;
        private bool _isFail2BanEnabled = false;
        private readonly ConcurrentDictionary<string, List<DateTime>> _ipFailTracker = new ConcurrentDictionary<string, List<DateTime>>();

        // Dynamic Hosting Directory Property
        private string DynamicHostingPath
        {
            get
            {
                string path = Properties.Settings.Default.HTMLPath;
                return !string.IsNullOrWhiteSpace(path) ? path : @"C:\xampp\htdocs";
            }
        }

        // Standard Default Configuration Paths
        private const string VhostsConfigPath = @"C:\xampp\apache\conf\extra\httpd-vhosts.conf";
        private const string SslConfigPath = @"C:\xampp\apache\conf\extra\httpd-ssl.conf";
        private const string HostsFilePath = @"C:\Windows\System32\drivers\etc\hosts";
        private const string OpenSslExePath = @"C:\xampp\apache\bin\openssl.exe";
        private const string PhpIniPath = @"C:\xampp\php\php.ini";
        private const string AccessLogPath = @"C:\xampp\apache\logs\access.log";
        private const string PhpErrorLogPath = @"C:\xampp\apache\logs\error.log";

        public ConsoleUserControl()
        {
            InitializeComponent();
            WireEvents();
            LoadGuideDocumentation();
            PrintWelcomeBanner();
            InitializeLogWatcher();
            InitializeErrorLogWatcher();
        }

        private void WireEvents()
        {
            btnExecute.Click += BtnExecute_Click;
            btnClearConsole.Click += BtnClearConsole_Click;
            txtConsoleInput.KeyDown += TxtConsoleInput_KeyDown;
        }

        #region Privilege & Elevation Helpers

        private bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                WindowsPrincipal principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        private void ExecuteSystemCmd(string command, bool requireAdmin = false)
        {
            // If administrator access is explicitly required and not present, prompt via UAC
            if (requireAdmin && !IsAdministrator())
            {
                ExecuteElevatedCmd(command);
                return;
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c {command}",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process p = Process.Start(psi))
            {
                string output = p.StandardOutput.ReadToEnd();
                string error = p.StandardError.ReadToEnd();
                p.WaitForExit();

                if (!string.IsNullOrWhiteSpace(output)) AppendConsole(output.Trim(), Color.LightGray);
                if (!string.IsNullOrWhiteSpace(error))
                {
                    // If error indicates privilege failure, automatically prompt for elevation
                    if (error.Contains("Access is denied") || error.Contains("System error 5"))
                    {
                        AppendConsole("Access Denied. Prompting for Administrator elevation...", Color.Orange);
                        ExecuteElevatedCmd(command);
                    }
                    else
                    {
                        AppendConsole(error.Trim(), Color.Red);
                    }
                }
            }
        }

        private void ExecuteElevatedCmd(string command)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = $"/c {command}",
                    UseShellExecute = true,
                    Verb = "runas",
                    WindowStyle = ProcessWindowStyle.Hidden
                };

                using (Process p = Process.Start(psi))
                {
                    p?.WaitForExit();
                }

                AppendConsole($"[ELEVATED COMMAND EXECUTED SUCCESSFULLY]: {command}", Color.Lime);
            }
            catch (Win32Exception)
            {
                AppendConsole("Command aborted: Administrator elevation was denied by user.", Color.Red);
            }
            catch (Exception ex)
            {
                AppendConsole($"Elevation Error: {ex.Message}", Color.Red);
            }
        }

        #endregion

        #region Documentation & Initialization

        private void LoadGuideDocumentation()
        {
            rtbGuideText.Clear();
            AppendGuideText("========================================================================\n", Color.DarkGray, true);
            AppendGuideText("            SERVER HTML HANDLER - EXPANDED CONSOLE SYSTEM               \n", Color.Navy, true);
            AppendGuideText("========================================================================\n\n", Color.DarkGray, true);

            AppendGuideText("1. DATABASE MANAGEMENT & LARGE IMPORTS\n", Color.DarkBlue, true);
            AppendGuideText("   sql <query>                         Executes raw SQL query against active database\n", Color.Black, false);
            AppendGuideText("   sql slow [lines]                    Analyze MariaDB slow query logs for performance\n", Color.Black, false);
            AppendGuideText("   db create <dbname>                  Creates a new database\n", Color.Black, false);
            AppendGuideText("   db drop <dbname>                    Drops an existing database\n", Color.Black, false);
            AppendGuideText("   db diff <source_db> <target_db>     Compare schemas and generate ALTER migration SQL\n", Color.Black, false);
            AppendGuideText("   user add <user> <pass> [host]       Creates a database user\n", Color.Black, false);
            AppendGuideText("   user drop <user> [host]             Drops a database user\n", Color.Black, false);
            AppendGuideText("   restore <sql_file> <target_db>      Chunked streaming SQL restore for multi-GB files\n\n", Color.Black, false);

            AppendGuideText("2. VIRTUAL HOST & LOCAL DOMAIN MANAGER\n", Color.DarkBlue, true);
            AppendGuideText("   vhost create <domain> <folder>      Setup VirtualHost & bind domain in hosts file\n", Color.Black, false);
            AppendGuideText("   vhost list                          List all configured Apache virtual hosts\n", Color.Black, false);
            AppendGuideText("   vhost remove <domain>               Remove VirtualHost and local domain entry\n\n", Color.Black, false);

            AppendGuideText("3. SSL/TLS CERTIFICATE PROVISIONER\n", Color.DarkBlue, true);
            AppendGuideText("   ssl create <domain>                 Generate OpenSSL certs & bind HTTPS in Apache\n\n", Color.Black, false);

            AppendGuideText("4. SECURITY, FIREWALL & INTRUSION PREVENTION (FAIL2BAN)\n", Color.DarkBlue, true);
            AppendGuideText("   blockip <ip_address>                Block incoming IP via Windows Firewall\n", Color.Black, false);
            AppendGuideText("   unblockip <ip_address>              Remove IP block rule from Firewall\n", Color.Black, false);
            AppendGuideText("   listblocked                         Show currently blocked IP firewall rules\n", Color.Black, false);
            AppendGuideText("   fail2ban <start|stop|status>        Toggle automated IP auto-blocking scanner\n", Color.Black, false);
            AppendGuideText("   watch <traffic|stop>                Stream color-coded Apache access log live\n\n", Color.Black, false);

            AppendGuideText("5. SCHEDULED TASKS & REMOTE SYNC\n", Color.DarkBlue, true);
            AppendGuideText("   schedule <db|web> <HH:mm> [target] Register daily Windows Scheduled Backup Task\n", Color.Black, false);
            AppendGuideText("   sync ftp <host> <user> <pass> <file> Upload archive/dump file to remote FTP server\n\n", Color.Black, false);

            AppendGuideText("6. PHP EXTENSION & RUNTIME ENVIRONMENT\n", Color.DarkBlue, true);
            AppendGuideText("   phpext list                         View available and enabled PHP extensions\n", Color.Black, false);
            AppendGuideText("   phpext enable <extension_name>      Enable PHP module in php.ini and restart Apache\n", Color.Black, false);
            AppendGuideText("   phpext disable <extension_name>     Disable PHP module in php.ini and restart Apache\n", Color.Black, false);
            AppendGuideText("   xdebug <enable|disable|mode> [m]    Manage Xdebug step-debugging and profiling\n", Color.Black, false);
            AppendGuideText("   opcache <status|reset>              Check OPcache metrics or flush memory buffers\n\n", Color.Black, false);

            AppendGuideText("7. DEVELOPER PRODUCTIVITY & TUNNELING\n", Color.DarkBlue, true);
            AppendGuideText("   tunnel <ngrok|localtunnel> [port]  Expose local VirtualHost to public HTTPS URL\n", Color.Black, false);
            AppendGuideText("   composer <audit|install|update>    Check CVE vulnerabilities or update vendors\n", Color.Black, false);
            AppendGuideText("   killport <port>                     Identify and force-kill process blocking a port\n", Color.Black, false);
            AppendGuideText("   cors <enable|disable>              Toggle global Apache CORS headers for API testing\n", Color.Black, false);
            AppendGuideText("   stress <url> [requests] [conc]     Run Apache Benchmark load/stress test on local route\n", Color.Black, false);
            AppendGuideText("   env <get|set> <path> [key] [val]   Read or modify key-value pairs in .env files\n", Color.Black, false);
            AppendGuideText("   gitpull <folder_path>              Execute git pull directly inside web directory\n\n", Color.Black, false);

            AppendGuideText("8. GIT VERSION CONTROL PIPELINE\n", Color.DarkBlue, true);
            AppendGuideText("   git status [repo_path]              Check git working tree status\n", Color.Black, false);
            AppendGuideText("   git log [repo_path] [count]         Show last N commit logs\n", Color.Black, false);
            AppendGuideText("   git branch [repo_path]              List local and remote branches\n", Color.Black, false);
            AppendGuideText("   git checkout <repo_path> <branch>  Switch active branch\n\n", Color.Black, false);

            AppendGuideText("9. EMAIL SERVICE DIAGNOSTICS\n", Color.DarkBlue, true);
            AppendGuideText("   mail test <to_email> [host] [port] Test SMTP connection and send verification mail\n\n", Color.Black, false);

            AppendGuideText("10. MULTI-DEVELOPER DEPLOY & STAGING SYSTEM (NO GIT)\n", Color.DarkBlue, true);
            AppendGuideText("   deploy list                         List all projects in D:\\html & current selection\n", Color.Black, false);
            AppendGuideText("   deploy select <project_name>        Select active project (e.g. deploy select NEXUS-L)\n", Color.Black, false);
            AppendGuideText("   deploy init <dev_tag>-<name>        Create Developing/<proj>/<dev_name>/{Update,Private}\n", Color.Black, false);
            AppendGuideText("   deploy devs                         List registered developer staging workspaces\n", Color.Black, false);
            AppendGuideText("   deploy check <dev_tag>-<name>       Verify PHP syntax & show modified/new files in Update\n", Color.Black, false);
            AppendGuideText("   deploy merge <dev_tag>-<name>       Auto-snapshot backup & merge Update folder into live\n", Color.Black, false);
            AppendGuideText("   deploy sync-private <dev_tag>-<name> Copy current live site into dev Private test folder\n", Color.Black, false);
            AppendGuideText("   deploy backups                      List all version snapshot zip backups\n", Color.Black, false);
            AppendGuideText("   deploy rollback [backup_name]       Restore live project to previous version snapshot\n", Color.Black, false);
            AppendGuideText("   deploy help                         Display complete developer workflow guide\n\n", Color.Black, false);

            AppendGuideText("11. SYSTEM DIAGNOSTICS & UTILITIES\n", Color.DarkBlue, true);
            AppendGuideText("   service <apache|mysql> <action>    Start, stop, restart or check server daemons\n", Color.Black, false);
            AppendGuideText("   log <apache|mysql> [lines]         View recent error log entries\n", Color.Black, false);
            AppendGuideText("   sysinfo                             Display CPU, RAM, Uptime, and Disk metrics\n", Color.Black, false);
            AppendGuideText("   ports                               Check status of listening ports 80, 443, 3306\n", Color.Black, false);
            AppendGuideText("   export [filename.txt]              Save current console log output to disk\n", Color.Black, false);
            AppendGuideText("   clear                               Clear the console screen\n", Color.Black, false);
            AppendGuideText("   help                                Display this reference guide\n\n", Color.Black, false);

            AppendGuideText("12. VULNERABLE SECURITY TEST LAB MANAGER\n", Color.DarkBlue, true);
            AppendGuideText("   lab list                            List available vulnerable target applications\n", Color.Black, false);
            AppendGuideText("   lab install <app_name>              Auto-download, extract, and configure test lab\n", Color.Black, false);
            AppendGuideText("   lab start|stop|status|health|logs  Manage and monitor security test lab instances\n\n", Color.Black, false);

            rtbGuideText.SelectionStart = 0;
            rtbGuideText.ScrollToCaret();
        }

        private void PrintWelcomeBanner()
        {
            AppendConsole("Server HTML Handler Advanced Control Console Initialized.", Color.Cyan);
            AppendConsole($"Hosting Path Configured: {DynamicHostingPath}", Color.Cyan);
            AppendConsole($"Elevated Admin Privileges: {(IsAdministrator() ? "YES" : "NO (UAC Auto-Prompt Active)")}", IsAdministrator() ? Color.Lime : Color.Orange);
            AppendConsole("Type 'help' for command syntax or press TAB for auto-completion.\n", Color.Gray);
        }

        private void AppendGuideText(string text, Color color, bool isBold)
        {
            rtbGuideText.SelectionStart = rtbGuideText.TextLength;
            rtbGuideText.SelectionLength = 0;
            rtbGuideText.SelectionColor = color;
            rtbGuideText.SelectionFont = new Font(rtbGuideText.Font, isBold ? FontStyle.Bold : FontStyle.Regular);
            rtbGuideText.AppendText(text);
        }

        #endregion

        #region Console Input Handling & Navigation

        private void BtnExecute_Click(object sender, EventArgs e) => ProcessInput();

        private void BtnClearConsole_Click(object sender, EventArgs e)
        {
            rtbConsoleOutput.Clear();
            PrintWelcomeBanner();
        }

        private void TxtConsoleInput_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                ProcessInput();
            }
            else if (e.KeyCode == Keys.Up)
            {
                e.SuppressKeyPress = true;
                NavigateHistory(-1);
            }
            else if (e.KeyCode == Keys.Down)
            {
                e.SuppressKeyPress = true;
                NavigateHistory(1);
            }
            else if (e.KeyCode == Keys.Tab)
            {
                e.SuppressKeyPress = true;
                HandleTabCompletion();
            }
        }

        private void HandleTabCompletion()
        {
            string currentText = txtConsoleInput.Text.Trim();
            if (string.IsNullOrEmpty(currentText)) return;

            string match = _knownCommands.FirstOrDefault(c => c.StartsWith(currentText, StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                txtConsoleInput.Text = match + " ";
                txtConsoleInput.SelectionStart = txtConsoleInput.Text.Length;
            }
        }

        private void NavigateHistory(int direction)
        {
            if (_commandHistory.Count == 0) return;

            _historyIndex += direction;

            if (_historyIndex < 0)
                _historyIndex = 0;
            else if (_historyIndex >= _commandHistory.Count)
            {
                _historyIndex = _commandHistory.Count;
                txtConsoleInput.Clear();
                return;
            }

            txtConsoleInput.Text = _commandHistory[_historyIndex];
            txtConsoleInput.SelectionStart = txtConsoleInput.Text.Length;
        }

        private void ProcessInput()
        {
            string rawInput = txtConsoleInput.Text.Trim();
            if (string.IsNullOrEmpty(rawInput)) return;

            _commandHistory.Add(rawInput);
            _historyIndex = _commandHistory.Count;

            AppendConsole($"> {rawInput}", Color.Yellow);
            txtConsoleInput.Clear();

            ExecuteCommand(rawInput);
        }

        #endregion

        #region Command Interpreter Engine

        private void ExecuteCommand(string input)
        {
            string[] parts = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length == 0) return;

            string primaryCmd = parts[0].ToLower();

            try
            {
                switch (primaryCmd)
                {
                    case "clear":
                        rtbConsoleOutput.Clear();
                        break;

                    case "help":
                        rtbGuideText.Focus();
                        AppendConsole("Guide documentation referenced above.", Color.Lime);
                        break;

                    case "sql":
                        if (parts.Length > 1 && parts[1].ToLower() == "slow") HandleSlowQueryCommand(parts);
                        else ExecuteSql(input.Length > 3 ? input.Substring(3).Trim() : "");
                        break;

                    case "db":
                        if (parts.Length > 1 && parts[1].ToLower() == "diff") HandleDbDiffCommand(parts);
                        else HandleDatabaseCommand(parts);
                        break;

                    case "user":
                        HandleUserCommand(parts);
                        break;

                    case "acl":
                        HandleAclCommand(parts);
                        break;

                    case "checkuser":
                        if (parts.Length > 1) CheckWindowsUser(parts[1]);
                        else AppendConsole("Usage: checkuser <username>", Color.Orange);
                        break;

                    case "vhost":
                        HandleVHostCommand(parts);
                        break;

                    case "ssl":
                        HandleSslCommand(parts);
                        break;

                    case "blockip":
                        if (parts.Length > 1) ManageIpRule(parts[1], block: true);
                        else AppendConsole("Usage: blockip <ip_address>", Color.Orange);
                        break;

                    case "unblockip":
                        if (parts.Length > 1) ManageIpRule(parts[1], block: false);
                        else AppendConsole("Usage: unblockip <ip_address>", Color.Orange);
                        break;

                    case "listblocked":
                        ListBlockedIps();
                        break;

                    case "fail2ban":
                        HandleFail2BanCommand(parts);
                        break;

                    case "watch":
                        HandleWatchCommand(parts);
                        break;

                    case "schedule":
                        HandleScheduleCommand(parts);
                        break;

                    case "sync":
                        HandleSyncCommand(parts);
                        break;

                    case "phpext":
                        HandlePhpExtCommand(parts);
                        break;

                    case "service":
                        HandleServiceCommand(parts);
                        break;

                    case "log":
                        HandleLogCommand(parts);
                        break;

                    case "sysinfo":
                        DisplaySystemMetrics();
                        break;

                    case "ports":
                        CheckServerPorts();
                        break;

                    case "backup":
                        HandleBackupCommand(parts);
                        break;

                    case "restore":
                        HandleRestoreCommand(parts);
                        break;

                    case "export":
                        ExportConsoleOutput(parts.Length > 1 ? parts[1] : null);
                        break;

                    case "cmd":
                        ExecuteSystemCmd(input.Length > 3 ? input.Substring(3).Trim() : "");
                        break;

                    case "killport":
                        HandleKillPortCommand(parts);
                        break;

                    case "cors":
                        HandleCorsCommand(parts);
                        break;

                    case "stress":
                        HandleStressTestCommand(parts);
                        break;

                    case "gitpull":
                        HandleGitPullCommand(parts);
                        break;

                    case "env":
                        HandleEnvCommand(parts);
                        break;

                    case "deploy":
                        HandleDeployCommand(parts);
                        break;

                    case "git":
                        HandleGitCommand(parts);
                        break;

                    case "mail":
                        HandleMailCommand(parts);
                        break;

                    case "lab":
                        HandleLabCommand(parts);
                        break;

                    case "tunnel":
                        HandleTunnelCommand(parts);
                        break;

                    case "xdebug":
                        HandleXdebugCommand(parts);
                        break;

                    case "composer":
                        HandleComposerCommand(parts);
                        break;

                    case "opcache":
                        HandleOpCacheCommand(parts);
                        break;

                    case "sqlite":
                        HandleSqliteCommand(parts);
                        break;

                    case "pg":
                        HandlePostgreSqlCommand(parts);
                        break;

                    case "gitgraph":
                        HandleGitGraphCommand(parts);
                        break;

                    case "stash":
                        HandleStashCommand(parts);
                        break;

                    default:
                        ExecuteSystemCmd(input);
                        break;
                }
            }
            catch (Exception ex)
            {
                AppendConsole($"[ERROR]: {ex.Message}", Color.Red);
            }
        }

        #endregion

        #region Standard Command Handlers & Thread-Safe Logging

        private void ExecuteSql(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                AppendConsole("Usage: sql <query>", Color.Orange);
                return;
            }

            using (var conn = new MySqlConnection(Helper.StorageMetricsHelper.GetConnectionString()))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(query, conn))
                {
                    if (query.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase) ||
                        query.StartsWith("SHOW", StringComparison.OrdinalIgnoreCase))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            int cols = reader.FieldCount;
                            while (reader.Read())
                            {
                                List<string> row = new List<string>();
                                for (int i = 0; i < cols; i++) row.Add(reader[i].ToString());
                                AppendConsole(string.Join(" | ", row), Color.White);
                            }
                        }
                    }
                    else
                    {
                        int affected = cmd.ExecuteNonQuery();
                        AppendConsole($"Query executed successfully. Rows affected: {affected}", Color.Lime);
                    }
                }
            }
        }

        private void HandleDatabaseCommand(string[] parts)
        {
            if (parts.Length < 3)
            {
                AppendConsole("Usage: db <create|drop> <dbname>", Color.Orange);
                return;
            }

            string action = parts[1].ToLower();
            string dbName = parts[2];

            if (action == "create") ExecuteSql($"CREATE DATABASE `{dbName}`;");
            else if (action == "drop") ExecuteSql($"DROP DATABASE `{dbName}`;");
        }

        private void HandleUserCommand(string[] parts)
        {
            if (parts.Length < 3)
            {
                AppendConsole("Usage: user <add|drop> <username> [password] [host]", Color.Orange);
                return;
            }

            string action = parts[1].ToLower();
            string user = parts[2];
            string pass = parts.Length > 3 ? parts[3] : "";
            string host = parts.Length > 4 ? parts[4] : "%";

            if (action == "add")
            {
                Helper.Helper.CreateUserAndGrantPrivileges(user, pass, host, "*", new List<string> { "SELECT", "INSERT", "UPDATE", "DELETE" });
                AppendConsole($"User '{user}'@'{host}' created successfully.", Color.Lime);
            }
            else if (action == "drop")
            {
                ExecuteSql($"DROP USER '{user}'@'{host}';");
            }
        }

        private void HandleAclCommand(string[] parts)
        {
            if (parts.Length < 4 || parts[1].ToLower() != "grant")
            {
                AppendConsole("Usage: acl grant <path> <user> <read|full>", Color.Orange);
                return;
            }

            string path = parts[2];
            string user = parts[3];
            string rights = parts.Length > 4 && parts[4].ToLower() == "full" ? "(OI)(CI)F" : "(OI)(CI)R";

            ExecuteSystemCmd($"icacls \"{path}\" /grant \"{user}\":{rights}", requireAdmin: true);
        }

        private void CheckWindowsUser(string username)
        {
            if (Helper.Helper.DoesWindowsAccountExist(username, out string resolvedName))
            {
                AppendConsole($"Valid Account Found: {resolvedName}", Color.Lime);
            }
            else
            {
                AppendConsole($"Account '{username}' not found on local or network domain.", Color.Red);
            }
        }

        private void ManageIpRule(string ipAddress, bool block)
        {
            string ruleName = $"BlockIP_{ipAddress}";
            string cmd = block
                ? $"netsh advfirewall firewall add rule name=\"{ruleName}\" dir=in action=block remoteip={ipAddress}"
                : $"netsh advfirewall firewall delete rule name=\"{ruleName}\"";

            ExecuteSystemCmd(cmd, requireAdmin: true);
        }

        private void ListBlockedIps()
        {
            ExecuteSystemCmd("netsh advfirewall firewall show rule name=all | findstr \"BlockIP_\"", requireAdmin: true);
        }

        private void HandleServiceCommand(string[] parts)
        {
            if (parts.Length < 3)
            {
                AppendConsole("Usage: service <apache|mysql> <start|stop|restart|status>", Color.Orange);
                return;
            }

            string daemon = parts[1].ToLower();
            string action = parts[2].ToLower();
            string serviceName = daemon == "apache" ? "Apache2.4" : (daemon == "mysql" ? "mysql" : daemon);

            if (action == "start") ExecuteSystemCmd($"net start {serviceName}", requireAdmin: true);
            else if (action == "stop") ExecuteSystemCmd($"net stop {serviceName}", requireAdmin: true);
            else if (action == "restart") ExecuteSystemCmd($"net stop {serviceName} & net start {serviceName}", requireAdmin: true);
            else if (action == "status") ExecuteSystemCmd($"sc query {serviceName}");
        }

        private void HandleLogCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                AppendConsole("Usage: log <apache|mysql> [lines]", Color.Orange);
                return;
            }

            string target = parts[1].ToLower();
            int lineCount = (parts.Length > 2 && int.TryParse(parts[2], out int l)) ? l : 20;
            string logPath = target == "apache" ? @"C:\xampp\apache\logs\error.log" : @"C:\xampp\mysql\data\mysql_error.log";

            if (!File.Exists(logPath))
            {
                AppendConsole($"Log file not found at: {logPath}", Color.Red);
                return;
            }

            try
            {
                string[] allLines = File.ReadAllLines(logPath);
                var lines = allLines.Skip(Math.Max(0, allLines.Length - lineCount));

                AppendConsole($"--- LAST {lineCount} LINES OF {target.ToUpper()} LOG ---", Color.Cyan);
                foreach (var line in lines)
                {
                    AppendConsole(line, Color.LightGray);
                }
            }
            catch (Exception ex)
            {
                AppendConsole($"Error reading log file: {ex.Message}", Color.Red);
            }
        }

        private void DisplaySystemMetrics()
        {
            try
            {
                DriveInfo cDrive = new DriveInfo("C");
                long freeGB = cDrive.AvailableFreeSpace / (1024 * 1024 * 1024);
                long totalGB = cDrive.TotalSize / (1024 * 1024 * 1024);
                TimeSpan uptime = TimeSpan.FromMilliseconds(Environment.TickCount);

                AppendConsole("--- SYSTEM DIAGNOSTICS ---", Color.Cyan);
                AppendConsole($"Machine Name : {Environment.MachineName}", Color.White);
                AppendConsole($"OS Version   : {Environment.OSVersion}", Color.White);
                AppendConsole($"System Uptime: {uptime.Days}d {uptime.Hours}h {uptime.Minutes}m", Color.White);
                AppendConsole($"Disk Space(C): {freeGB} GB free of {totalGB} GB total", Color.White);
            }
            catch (Exception ex)
            {
                AppendConsole($"Failed to retrieve system metrics: {ex.Message}", Color.Red);
            }
        }

        private void CheckServerPorts()
        {
            AppendConsole("--- LISTENING PORT DIAGNOSTICS ---", Color.Cyan);
            ExecuteSystemCmd("netstat -ano | findstr \"80 443 3306\"");
        }

        private void HandleBackupCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                AppendConsole("Usage: backup <db|web> [dbname/path] [output_path]", Color.Orange);
                return;
            }

            string targetType = parts[1].ToLower();

            if (targetType == "db")
            {
                string dbName = parts.Length > 2 ? parts[2] : "";
                string output = parts.Length > 3 ? parts[3] : $"backup_{dbName}_{DateTime.Now:yyyyMMdd_HHmmss}.sql";
                Helper.ServerBackupHelper.ExportDatabaseDump(dbName, output);
                AppendConsole($"Database dump saved to: {output}", Color.Lime);
            }
            else if (targetType == "web")
            {
                string webPath = parts.Length > 2 ? parts[2] : DynamicHostingPath;
                string output = parts.Length > 3 ? parts[3] : $"web_backup_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
                Helper.ServerBackupHelper.CompressFolderToZip(webPath, output);
                AppendConsole($"Web directory ({webPath}) archived to: {output}", Color.Lime);
            }
        }

        private void ExportConsoleOutput(string customFileName)
        {
            string fileName = string.IsNullOrWhiteSpace(customFileName)
                ? $"console_export_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
                : customFileName;

            File.WriteAllText(fileName, rtbConsoleOutput.Text);
            AppendConsole($"Console output exported successfully to: {fileName}", Color.Lime);
        }

        private void AppendConsole(string text, Color color)
        {
            if (rtbConsoleOutput.InvokeRequired)
            {
                rtbConsoleOutput.Invoke(new Action(() => AppendConsole(text, color)));
                return;
            }

            rtbConsoleOutput.SelectionStart = rtbConsoleOutput.TextLength;
            rtbConsoleOutput.SelectionLength = 0;
            rtbConsoleOutput.SelectionColor = color;
            rtbConsoleOutput.AppendText(text + "\n");
            rtbConsoleOutput.ScrollToCaret();
        }

        #endregion

        #region Feature 1: Apache Virtual Host & Local Domain Manager

        private void HandleVHostCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                AppendConsole("Usage: vhost <create|list|remove> [domain] [folder_path]", Color.Orange);
                return;
            }

            string subAction = parts[1].ToLower();

            if (subAction == "create")
            {
                if (parts.Length < 4)
                {
                    AppendConsole("Usage: vhost create <domain> <folder_path>", Color.Orange);
                    return;
                }

                string domain = parts[2].Trim();
                string folderPath = parts[3].Trim().Replace("\\", "/");

                if (!Directory.Exists(folderPath))
                {
                    AppendConsole($"Directory path does not exist: {folderPath}", Color.Red);
                    return;
                }

                string vhostBlock = $"\n<VirtualHost *:80>\n" +
                                    $"    DocumentRoot \"{folderPath}\"\n" +
                                    $"    ServerName {domain}\n" +
                                    $"    <Directory \"{folderPath}\">\n" +
                                    $"        Options Indexes FollowSymLinks Includes ExecCGI\n" +
                                    $"        AllowOverride All\n" +
                                    $"        Require all granted\n" +
                                    $"    </Directory>\n" +
                                    $"</VirtualHost>\n";

                try
                {
                    File.AppendAllText(VhostsConfigPath, vhostBlock);

                    string hostEntry = $"\n127.0.0.1\t{domain}";
                    try
                    {
                        File.AppendAllText(HostsFilePath, hostEntry);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        AppendConsole("Admin rights needed to edit hosts file. Triggering UAC elevation...", Color.Yellow);
                        string psCmd = $"Add-Content -Path '{HostsFilePath}' -Value '{hostEntry.Trim()}'";
                        ExecuteSystemCmd($"powershell -Command \"{psCmd}\"", requireAdmin: true);
                    }

                    AppendConsole($"VirtualHost and local domain '{domain}' created successfully!", Color.Lime);
                    AppendConsole("Execute 'service apache restart' to apply changes.", Color.Cyan);
                }
                catch (Exception ex)
                {
                    AppendConsole($"VHost Error: {ex.Message}", Color.Red);
                }
            }
            else if (subAction == "list")
            {
                if (!File.Exists(VhostsConfigPath))
                {
                    AppendConsole($"Configuration file not found: {VhostsConfigPath}", Color.Red);
                    return;
                }

                string content = File.ReadAllText(VhostsConfigPath);
                MatchCollection matches = Regex.Matches(content, @"<VirtualHost.*?>\s*DocumentRoot\s*""?(.*?)""?\s*ServerName\s*(.*?)\s*</VirtualHost>", RegexOptions.Singleline | RegexOptions.IgnoreCase);

                AppendConsole("--- CONFIGURED APACHE VIRTUAL HOSTS ---", Color.Cyan);
                foreach (Match m in matches)
                {
                    AppendConsole($"Domain: {m.Groups[2].Value.Trim()} | Path: {m.Groups[1].Value.Trim()}", Color.White);
                }
            }
            else if (subAction == "remove")
            {
                if (parts.Length < 3)
                {
                    AppendConsole("Usage: vhost remove <domain>", Color.Orange);
                    return;
                }

                string domain = parts[2].Trim();

                try
                {
                    if (File.Exists(VhostsConfigPath))
                    {
                        string content = File.ReadAllText(VhostsConfigPath);
                        string pattern = $@"<VirtualHost \*:80>[\s\S]*?ServerName\s+{Regex.Escape(domain)}[\s\S]*?</VirtualHost>\n?";
                        content = Regex.Replace(content, pattern, "", RegexOptions.IgnoreCase);
                        File.WriteAllText(VhostsConfigPath, content);
                    }

                    if (File.Exists(HostsFilePath))
                    {
                        try
                        {
                            var lines = File.ReadAllLines(HostsFilePath).Where(l => !l.Contains(domain)).ToArray();
                            File.WriteAllLines(HostsFilePath, lines);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            AppendConsole("Admin rights needed to update hosts file. Triggering UAC elevation...", Color.Yellow);
                            string psCmd = $"(Get-Content '{HostsFilePath}') | Where-Object {{ $_ -notmatch '{Regex.Escape(domain)}' }} | Set-Content '{HostsFilePath}'";
                            ExecuteSystemCmd($"powershell -Command \"{psCmd}\"", requireAdmin: true);
                        }
                    }

                    AppendConsole($"VirtualHost '{domain}' removed from Apache and Windows hosts file.", Color.Lime);
                }
                catch (Exception ex)
                {
                    AppendConsole($"VHost Removal Error: {ex.Message}", Color.Red);
                }
            }
        }

        #endregion

        #region Feature 2: SSL/TLS Certificate Provisioner

        private void HandleSslCommand(string[] parts)
        {
            if (parts.Length < 3 || parts[1].ToLower() != "create")
            {
                AppendConsole("Usage: ssl create <domain>", Color.Orange);
                return;
            }

            string domain = parts[2].Trim();
            string certDir = @"C:\xampp\apache\conf\ssl.crt\";
            string keyDir = @"C:\xampp\apache\conf\ssl.key\";

            Directory.CreateDirectory(certDir);
            Directory.CreateDirectory(keyDir);

            string crtPath = Path.Combine(certDir, $"{domain}.crt");
            string keyPath = Path.Combine(keyDir, $"{domain}.key");

            if (!File.Exists(OpenSslExePath))
            {
                AppendConsole($"OpenSSL executable not found at: {OpenSslExePath}", Color.Red);
                return;
            }

            string arguments = $"req -x509 -nodes -days 365 -newkey rsa:2048 -keyout \"{keyPath}\" -out \"{crtPath}\" -subj \"/CN={domain}\"";
            ExecuteSystemCmd($"\"{OpenSslExePath}\" {arguments}");

            if (File.Exists(SslConfigPath))
            {
                string sslBlock = $"\n<VirtualHost *:443>\n" +
                                  $"    ServerName {domain}\n" +
                                  $"    SSLEngine on\n" +
                                  $"    SSLCertificateFile \"{crtPath.Replace("\\", "/")}\"\n" +
                                  $"    SSLCertificateKeyFile \"{keyPath.Replace("\\", "/")}\"\n" +
                                  $"</VirtualHost>\n";

                File.AppendAllText(SslConfigPath, sslBlock);
                AppendConsole($"SSL Certificate generated and bound to Apache SSL config for 'https://{domain}'.", Color.Lime);
            }
        }

        #endregion

        #region Feature 3: Automated Intrusion Prevention (Fail2Ban) & Traffic Watcher

        private void InitializeLogWatcher()
        {
            string dir = Path.GetDirectoryName(AccessLogPath);
            if (!Directory.Exists(dir)) return;

            if (File.Exists(AccessLogPath))
            {
                _lastLogPosition = new FileInfo(AccessLogPath).Length;
            }

            _accessLogWatcher = new FileSystemWatcher(dir, Path.GetFileName(AccessLogPath))
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size
            };

            _accessLogWatcher.Changed += OnAccessLogChanged;
            _accessLogWatcher.EnableRaisingEvents = true;
        }

        private void OnAccessLogChanged(object sender, FileSystemEventArgs e)
        {
            if (!_isWatchingTraffic && !_isFail2BanEnabled) return;

            try
            {
                using (var stream = new FileStream(AccessLogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (stream.Length < _lastLogPosition) _lastLogPosition = 0;
                    stream.Seek(_lastLogPosition, SeekOrigin.Begin);

                    using (var reader = new StreamReader(stream))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            ProcessLogLine(line);
                        }
                        _lastLogPosition = stream.Position;
                    }
                }
            }
            catch { }
        }

        private void ProcessLogLine(string line)
        {
            Match m = Regex.Match(line, @"^(\S+)\s+\S+\s+\S+\s+\[.*?\]\s+""(\S+)\s+.*?\'?""\s+(\d{3})");
            if (!m.Success) return;

            string ip = m.Groups[1].Value;
            string method = m.Groups[2].Value;
            int status = int.Parse(m.Groups[3].Value);

            if (_isWatchingTraffic)
            {
                Color statusColor = Color.Lime;
                if (status >= 300 && status < 400) statusColor = Color.Yellow;
                else if (status >= 400) statusColor = Color.Red;

                AppendConsole($"[LIVE TRAFFIC] {ip} | {method} | HTTP {status}", statusColor);
            }

            if (_isFail2BanEnabled && (status == 404 || status == 403 || method == "POST"))
            {
                DateTime now = DateTime.Now;
                var timestamps = _ipFailTracker.GetOrAdd(ip, new List<DateTime>());

                lock (timestamps)
                {
                    timestamps.Add(now);
                    timestamps.RemoveAll(t => (now - t).TotalSeconds > 60);

                    if (timestamps.Count >= 50)
                    {
                        AppendConsole($"[FAIL2BAN DETECTED] Threat threshold reached for IP: {ip}. Auto-blocking...", Color.Red);
                        ManageIpRule(ip, block: true);
                        timestamps.Clear();
                    }
                }
            }
        }

        private void HandleFail2BanCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                AppendConsole("Usage: fail2ban <start|stop|status>", Color.Orange);
                return;
            }

            string action = parts[1].ToLower();
            if (action == "start")
            {
                _isFail2BanEnabled = true;
                AppendConsole("Automated Intrusion Prevention (Fail2Ban) service STARTED.", Color.Lime);
            }
            else if (action == "stop")
            {
                _isFail2BanEnabled = false;
                AppendConsole("Automated Intrusion Prevention (Fail2Ban) service STOPPED.", Color.Yellow);
            }
            else if (action == "status")
            {
                AppendConsole($"Fail2Ban Status: {(_isFail2BanEnabled ? "ACTIVE" : "INACTIVE")}", Color.Cyan);
            }
        }

        private void HandleWatchCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                AppendConsole("Usage: watch <traffic|stop>", Color.Orange);
                return;
            }

            string action = parts[1].ToLower();
            if (action == "traffic")
            {
                _isWatchingTraffic = true;
                AppendConsole("Live Apache Access Traffic Streamer STARTED.", Color.Lime);
            }
            else if (action == "stop")
            {
                _isWatchingTraffic = false;
                AppendConsole("Live Traffic Streamer STOPPED.", Color.Yellow);
            }
        }

        #endregion

        #region Feature 4: Scheduled Backups & Remote Storage Sync

        private void HandleScheduleCommand(string[] parts)
        {
            if (parts.Length < 3)
            {
                AppendConsole("Usage: schedule <db|web> <HH:mm> [target_dbname_or_path]", Color.Orange);
                return;
            }

            string type = parts[1].ToLower();
            string time = parts[2];
            string appExePath = Application.ExecutablePath;
            string taskName = $"ServerBackup_{type.ToUpper()}_{time.Replace(":", "")}";

            string cmd = $"schtasks /create /tn \"{taskName}\" /tr \"\"{appExePath}\" --backup-{type}\" /sc daily /st {time} /f";
            ExecuteSystemCmd(cmd, requireAdmin: true);

            AppendConsole($"Daily Scheduled Task '{taskName}' registered for {time}.", Color.Lime);
        }

        private void HandleSyncCommand(string[] parts)
        {
            if (parts.Length < 5 || parts[1].ToLower() != "ftp")
            {
                AppendConsole("Usage: sync ftp <host> <username> <password> <local_file_path>", Color.Orange);
                return;
            }

            string host = parts[2];
            string user = parts[3];
            string pass = parts[4];
            string filePath = parts[5];

            if (!File.Exists(filePath))
            {
                AppendConsole($"Local file not found: {filePath}", Color.Red);
                return;
            }

            Task.Run(() =>
            {
                try
                {
                    AppendConsole($"Starting FTP upload for '{Path.GetFileName(filePath)}'...", Color.Cyan);
                    string fileName = Path.GetFileName(filePath);
                    FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"ftp://{host}/{fileName}");
                    request.Method = WebRequestMethods.Ftp.UploadFile;
                    request.Credentials = new NetworkCredential(user, pass);

                    byte[] fileContents = File.ReadAllBytes(filePath);
                    request.ContentLength = fileContents.Length;

                    using (Stream requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(fileContents, 0, fileContents.Length);
                    }

                    using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                    {
                        AppendConsole($"FTP Sync Complete. Status: {response.StatusDescription.Trim()}", Color.Lime);
                    }
                }
                catch (Exception ex)
                {
                    AppendConsole($"FTP Sync Failed: {ex.Message}", Color.Red);
                }
            });
        }

        #endregion

        #region Feature 5: PHP Extension Manager

        private void HandlePhpExtCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                AppendConsole("Usage: phpext <list|enable|disable> [extension_name]", Color.Orange);
                return;
            }

            if (!File.Exists(PhpIniPath))
            {
                AppendConsole($"php.ini file not found at: {PhpIniPath}", Color.Red);
                return;
            }

            string action = parts[1].ToLower();
            string content = File.ReadAllText(PhpIniPath);

            if (action == "list")
            {
                MatchCollection matches = Regex.Matches(content, @"^\s*(;?\s*extension\s*=\s*(\S+))", RegexOptions.Multiline);
                AppendConsole("--- PHP EXTENSIONS IN PHP.INI ---", Color.Cyan);
                foreach (Match m in matches)
                {
                    bool isCommented = m.Groups[1].Value.Trim().StartsWith(";");
                    string extName = m.Groups[2].Value;
                    AppendConsole($"Module: {extName,-20} | Status: {(isCommented ? "DISABLED" : "ENABLED")}", isCommented ? Color.Gray : Color.Lime);
                }
            }
            else if (action == "enable" || action == "disable")
            {
                if (parts.Length < 3)
                {
                    AppendConsole($"Usage: phpext {action} <extension_name>", Color.Orange);
                    return;
                }

                string extName = parts[2].Trim();
                bool enable = action == "enable";

                string pattern = $@"^\s*;?\s*extension\s*=\s*{Regex.Escape(extName)}";
                string replacement = enable ? $"extension={extName}" : $";extension={extName}";

                if (Regex.IsMatch(content, pattern, RegexOptions.Multiline))
                {
                    content = Regex.Replace(content, pattern, replacement, RegexOptions.Multiline);
                    File.WriteAllText(PhpIniPath, content);
                    AppendConsole($"PHP module '{extName}' is now {(enable ? "ENABLED" : "DISABLED")}.", Color.Lime);

                    HandleServiceCommand(new[] { "service", "apache", "restart" });
                }
                else
                {
                    AppendConsole($"Extension '{extName}' directive was not found in php.ini.", Color.Red);
                }
            }
        }

        #endregion

        #region Feature 6: Large Streaming SQL Import Pipeline

        private void HandleRestoreCommand(string[] parts)
        {
            if (parts.Length < 3)
            {
                AppendConsole("Usage: restore <sql_filepath> <target_database>", Color.Orange);
                return;
            }

            string sqlPath = parts[1];
            string dbName = parts[2];

            if (!File.Exists(sqlPath))
            {
                AppendConsole($"SQL dump file not found: {sqlPath}", Color.Red);
                return;
            }

            Task.Run(() => StreamChunkedSqlImport(sqlPath, dbName));
        }

        private void StreamChunkedSqlImport(string sqlPath, string dbName)
        {
            FileInfo fileInfo = new FileInfo(sqlPath);
            long totalBytes = fileInfo.Length;
            long bytesRead = 0;

            AppendConsole($"Starting Chunked SQL Streaming Import ({totalBytes / (1024 * 1024)} MB)...", Color.Cyan);

            try
            {
                using (var conn = new MySqlConnection(Helper.StorageMetricsHelper.GetConnectionString(dbName)))
                {
                    conn.Open();
                    using (StreamReader reader = new StreamReader(sqlPath, Encoding.UTF8))
                    {
                        StringBuilder queryBuffer = new StringBuilder();
                        string line;
                        int lastProgress = 0;

                        while ((line = reader.ReadLine()) != null)
                        {
                            bytesRead += Encoding.UTF8.GetByteCount(line) + 2;

                            string trimmed = line.Trim();
                            if (trimmed.StartsWith("--") || trimmed.StartsWith("/*") || string.IsNullOrEmpty(trimmed))
                                continue;

                            queryBuffer.AppendLine(line);

                            if (trimmed.EndsWith(";"))
                            {
                                using (var cmd = new MySqlCommand(queryBuffer.ToString(), conn))
                                {
                                    cmd.CommandTimeout = 600;
                                    cmd.ExecuteNonQuery();
                                }
                                queryBuffer.Clear();
                            }

                            int progress = (int)((bytesRead * 100) / totalBytes);
                            if (progress >= lastProgress + 10)
                            {
                                lastProgress = progress;
                                AppendConsole($"[RESTORE PROGRESS]: {progress}% completed...", Color.Yellow);
                            }
                        }

                        if (queryBuffer.Length > 0)
                        {
                            using (var cmd = new MySqlCommand(queryBuffer.ToString(), conn))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
                AppendConsole($"Database '{dbName}' successfully restored from '{sqlPath}'.", Color.Lime);
            }
            catch (Exception ex)
            {
                AppendConsole($"Streaming Restore Failed: {ex.Message}", Color.Red);
            }
        }

        #endregion

        #region Developer Productivity Handlers

        private void HandleKillPortCommand(string[] parts)
        {
            if (parts.Length < 2 || !int.TryParse(parts[1], out int port))
            {
                AppendConsole("Usage: killport <port_number> (e.g., killport 80)", Color.Orange);
                return;
            }

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c netstat -ano | findstr :{port}",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process p = Process.Start(psi))
            {
                string output = p.StandardOutput.ReadToEnd();
                p.WaitForExit();

                if (string.IsNullOrWhiteSpace(output))
                {
                    AppendConsole($"No active process found holding port {port}.", Color.Yellow);
                    return;
                }

                var lines = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                HashSet<string> pidsToKill = new HashSet<string>();

                foreach (var line in lines)
                {
                    var tokens = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length >= 5 && tokens[1].EndsWith($":{port}"))
                    {
                        pidsToKill.Add(tokens[tokens.Length - 1]);
                    }
                }

                foreach (string pid in pidsToKill)
                {
                    if (pid == "0" || pid == "4")
                    {
                        AppendConsole($"Port {port} is held by Windows System (PID {pid}). Stop 'World Wide Web Publishing Service' via services.msc.", Color.Orange);
                    }
                    else
                    {
                        ExecuteSystemCmd($"taskkill /F /PID {pid}", requireAdmin: true);
                        AppendConsole($"Terminated process PID {pid} blocking port {port}.", Color.Lime);
                    }
                }
            }
        }

        private void HandleCorsCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                AppendConsole("Usage: cors <enable|disable>", Color.Orange);
                return;
            }

            string apacheConfig = @"C:\xampp\apache\conf\httpd.conf";
            if (!File.Exists(apacheConfig))
            {
                AppendConsole($"httpd.conf not found at: {apacheConfig}", Color.Red);
                return;
            }

            string action = parts[1].ToLower();
            string content = File.ReadAllText(apacheConfig);
            string corsHeader = "Header set Access-Control-Allow-Origin \"*\"";

            if (action == "enable")
            {
                content = Regex.Replace(content, @"^\s*;\s*LoadModule headers_module", "LoadModule headers_module", RegexOptions.Multiline);
                content = Regex.Replace(content, @"^\s*#\s*LoadModule headers_module", "LoadModule headers_module", RegexOptions.Multiline);

                if (!content.Contains(corsHeader))
                {
                    content += $"\n{corsHeader}\nHeader set Access-Control-Allow-Headers \"*\"\nHeader set Access-Control-Allow-Methods \"*\"\n";
                    File.WriteAllText(apacheConfig, content);
                }
                AppendConsole("CORS enabled globally in Apache. Restart Apache to apply.", Color.Lime);
            }
            else if (action == "disable")
            {
                content = content.Replace(corsHeader, "")
                                 .Replace("Header set Access-Control-Allow-Headers \"*\"", "")
                                 .Replace("Header set Access-Control-Allow-Methods \"*\"", "");
                File.WriteAllText(apacheConfig, content);
                AppendConsole("CORS disabled in Apache. Restart Apache to apply.", Color.Yellow);
            }
        }

        private void HandleStressTestCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                AppendConsole("Usage: stress <url> [total_requests] [concurrency] (e.g., stress http://localhost/ 100 10)", Color.Orange);
                return;
            }

            string url = parts[1];
            int requests = (parts.Length > 2 && int.TryParse(parts[2], out int r)) ? r : 100;
            int concurrency = (parts.Length > 3 && int.TryParse(parts[3], out int c)) ? c : 10;

            string abPath = @"C:\xampp\apache\bin\ab.exe";
            if (!File.Exists(abPath))
            {
                AppendConsole($"Apache Benchmark executable not found at: {abPath}", Color.Red);
                return;
            }

            AppendConsole($"Running stress test on '{url}' ({requests} requests, {concurrency} concurrent)...", Color.Cyan);
            ExecuteSystemCmd($"\"{abPath}\" -n {requests} -c {concurrency} {url}");
        }

        private void HandleGitPullCommand(string[] parts)
        {
            if (parts.Length < 2 && string.IsNullOrEmpty(DynamicHostingPath))
            {
                AppendConsole("Usage: gitpull <target_directory>", Color.Orange);
                return;
            }

            string repoPath = parts.Length > 1 ? parts[1] : DynamicHostingPath;
            if (!Directory.Exists(repoPath))
            {
                AppendConsole($"Target directory does not exist: {repoPath}", Color.Red);
                return;
            }

            ExecuteSystemCmd($"cd /d \"{repoPath}\" && git pull");
        }

        private void HandleEnvCommand(string[] parts)
        {
            if (parts.Length < 3)
            {
                AppendConsole("Usage: env <get|set> <env_filepath> [key] [value]", Color.Orange);
                return;
            }

            string action = parts[1].ToLower();
            string envPath = parts[2];

            if (!File.Exists(envPath))
            {
                AppendConsole($"Environment file not found at: {envPath}", Color.Red);
                return;
            }

            string[] lines = File.ReadAllLines(envPath);

            if (action == "get")
            {
                string searchKey = parts.Length > 3 ? parts[3] : null;
                AppendConsole($"--- READING {Path.GetFileName(envPath)} ---", Color.Cyan);

                foreach (var line in lines)
                {
                    if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#")) continue;

                    if (searchKey == null || line.StartsWith(searchKey, StringComparison.OrdinalIgnoreCase))
                    {
                        AppendConsole(line, Color.White);
                    }
                }
            }
            else if (action == "set")
            {
                if (parts.Length < 5)
                {
                    AppendConsole("Usage: env set <env_filepath> <key> <value>", Color.Orange);
                    return;
                }

                string key = parts[3];
                string value = parts[4];
                bool keyFound = false;

                for (int i = 0; i < lines.Length; i++)
                {
                    if (lines[i].StartsWith($"{key}=", StringComparison.OrdinalIgnoreCase))
                    {
                        lines[i] = $"{key}={value}";
                        keyFound = true;
                        break;
                    }
                }

                if (keyFound)
                {
                    File.WriteAllLines(envPath, lines);
                    AppendConsole($"Updated {key}={value} in {envPath}", Color.Lime);
                }
                else
                {
                    File.AppendAllText(envPath, $"\n{key}={value}");
                    AppendConsole($"Appended {key}={value} to {envPath}", Color.Lime);
                }
            }
        }

        #endregion

        #region Expanded Git & Mail Diagnostic Handlers

        private void HandleGitCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                AppendConsole("Usage: git <status|log|branch|checkout|fetch|pull> [repo_path] [args]", Color.Orange);
                return;
            }

            string subCmd = parts[1].ToLower();
            string targetPath = (parts.Length > 2 && Directory.Exists(parts[2])) ? parts[2] : DynamicHostingPath;
            string extraArgs = parts.Length > 3 ? string.Join(" ", parts.Skip(3)) : "";

            switch (subCmd)
            {
                case "status":
                    ExecuteSystemCmd($"cd /d \"{targetPath}\" && git status");
                    break;

                case "log":
                    int limit = (parts.Length > 3 && int.TryParse(parts[3], out int l)) ? l : 5;
                    ExecuteSystemCmd($"cd /d \"{targetPath}\" && git log -n {limit} --oneline");
                    break;

                case "branch":
                    ExecuteSystemCmd($"cd /d \"{targetPath}\" && git branch -a");
                    break;

                case "checkout":
                    if (parts.Length < 4)
                    {
                        AppendConsole("Usage: git checkout <repo_path> <branch_name>", Color.Orange);
                        return;
                    }
                    ExecuteSystemCmd($"cd /d \"{targetPath}\" && git checkout {parts[3]}");
                    break;

                default:
                    ExecuteSystemCmd($"cd /d \"{targetPath}\" && git {subCmd} {extraArgs}");
                    break;
            }
        }

        private void HandleMailCommand(string[] parts)
        {
            if (parts.Length < 3 || parts[1].ToLower() != "test")
            {
                AppendConsole("Usage: mail test <to_email> [smtp_host] [port]", Color.Orange);
                return;
            }

            string toEmail = parts[2];
            string smtpHost = parts.Length > 3 ? parts[3] : "127.0.0.1";
            int smtpPort = (parts.Length > 4 && int.TryParse(parts[4], out int p)) ? p : 25;

            Task.Run(() =>
            {
                try
                {
                    AppendConsole($"Testing SMTP server at {smtpHost}:{smtpPort}...", Color.Cyan);

                    using (var message = new MailMessage("noreply@localhost", toEmail))
                    {
                        message.Subject = "XAMPP Console Service Diagnostic Test";
                        message.Body = $"This is an automated test email sent from Server HTML Handler Console at {DateTime.Now}.";

                        using (var client = new SmtpClient(smtpHost, smtpPort))
                        {
                            client.Timeout = 10000;
                            client.Send(message);
                        }
                    }

                    AppendConsole($"Test email successfully delivered to '{toEmail}'.", Color.Lime);
                }
                catch (Exception ex)
                {
                    AppendConsole($"SMTP Mail Diagnostic Failed: {ex.Message}", Color.Red);
                }
            });
        }

        #endregion

        #region Test Lab Manager Handler

        private void HandleLabCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                AppendConsole("Usage: lab <list|install|start|stop|status|health|reset|logs|difficulty|uninstall> [app_name] [options]", Color.Orange);
                return;
            }

            string action = parts[1].ToLower();

            if (action == "list")
            {
                AppendConsole("--- AVAILABLE TEST LAB ENVIRONMENTS ---", Color.Cyan);
                AppendConsole("1. dvwa       : Damn Vulnerable Web Application (PHP/MySQL)", Color.White);
                AppendConsole("2. xvwa       : Xtreme Vulnerable Web Application (PHP/MySQL) [UNAVAILABLE / ARCHIVED]", Color.Gray);
                AppendConsole("3. mutillidae : OWASP Mutillidae II (PHP/MySQL)", Color.White);
                AppendConsole("4. bwapp      : Buggy Web Application (PHP/MySQL)", Color.White);
                AppendConsole("5. juiceshop  : OWASP Juice Shop (Node.js/Docker)", Color.White);
                AppendConsole("6. webgoat    : OWASP WebGoat (Java/Docker)", Color.White);
            }
            else if (action == "install")
            {
                if (parts.Length < 3)
                {
                    AppendConsole("Usage: lab install <dvwa|mutillidae|bwapp|juiceshop|webgoat>", Color.Orange);
                    return;
                }

                string app = parts[2].ToLower();
                Task.Run(() =>
                {
                    try
                    {
                        if (app == "dvwa")
                        {
                            Helper.TestLabHelper.InstallDvwa(msg => AppendConsole(msg, Color.Lime));
                        }
                        else if (app == "xvwa")
                        {
                            AppendConsole("XVWA is unavailable because the repository has been archived by the owner.", Color.Red);
                        }
                        else if (app == "mutillidae")
                        {
                            Helper.TestLabHelper.InstallMutillidae(msg => AppendConsole(msg, Color.Lime));
                        }
                        else if (app == "bwapp")
                        {
                            Helper.TestLabHelper.InstallBwapp(msg => AppendConsole(msg, Color.Lime));
                        }
                        else if (app == "juiceshop")
                        {
                            AppendConsole("Provisioning OWASP Juice Shop via Docker...", Color.Cyan);
                            ExecuteSystemCmd("docker rm -f lab_juiceshop");
                            ExecuteSystemCmd("docker run -d --name lab_juiceshop -p 3000:3000 bkimminich/juice-shop");
                            AppendConsole("Juice Shop container started! Access at http://localhost:3000", Color.Lime);
                        }
                        else if (app == "webgoat")
                        {
                            AppendConsole("Provisioning OWASP WebGoat via Docker...", Color.Cyan);
                            ExecuteSystemCmd("docker rm -f lab_webgoat");
                            ExecuteSystemCmd("docker run -d --name lab_webgoat -p 8080:8080 webgoat/webgoat");
                            AppendConsole("WebGoat container started! Access at http://localhost:8080/WebGoat", Color.Lime);
                        }
                        else
                        {
                            AppendConsole($"Unknown lab target: '{app}'. Use 'lab list' to view options.", Color.Orange);
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendConsole($"Lab Installation Failed: {ex.Message}", Color.Red);
                    }
                });
            }
            else if (action == "start" || action == "open")
            {
                if (parts.Length < 3)
                {
                    AppendConsole("Usage: lab start <dvwa|mutillidae|bwapp|juiceshop|webgoat>", Color.Orange);
                    return;
                }

                string app = parts[2].ToLower();
                string targetUrl = "";

                switch (app)
                {
                    case "dvwa":
                        targetUrl = "http://localhost/dvwa/setup.php";
                        break;
                    case "xvwa":
                        AppendConsole("XVWA is unavailable (archived).", Color.Red);
                        return;
                    case "mutillidae":
                        targetUrl = "http://localhost/mutillidae/";
                        break;
                    case "bwapp":
                        targetUrl = "http://localhost/bwapp/install.php";
                        break;
                    case "juiceshop":
                        targetUrl = "http://localhost:3000/";
                        break;
                    case "webgoat":
                        targetUrl = "http://localhost:8080/WebGoat";
                        break;
                    default:
                        AppendConsole($"Unknown lab target: '{app}'. Use 'lab list' to view options.", Color.Orange);
                        return;
                }

                try
                {
                    Process.Start(new ProcessStartInfo { FileName = targetUrl, UseShellExecute = true });
                    AppendConsole($"Opened {app.ToUpper()} at {targetUrl}", Color.Lime);
                }
                catch (Exception ex)
                {
                    AppendConsole($"Failed to open browser: {ex.Message}", Color.Red);
                }
            }
            else if (action == "stop")
            {
                if (parts.Length < 3)
                {
                    AppendConsole("Usage: lab stop <juiceshop|webgoat>", Color.Orange);
                    return;
                }

                string app = parts[2].ToLower();
                if (app == "juiceshop")
                {
                    ExecuteSystemCmd("docker stop lab_juiceshop");
                    AppendConsole("Stopped OWASP Juice Shop Docker container.", Color.Yellow);
                }
                else if (app == "webgoat")
                {
                    ExecuteSystemCmd("docker stop lab_webgoat");
                    AppendConsole("Stopped OWASP WebGoat Docker container.", Color.Yellow);
                }
                else
                {
                    AppendConsole($"Container stopping is only applicable to Docker labs (juiceshop, webgoat).", Color.Orange);
                }
            }
            else if (action == "status")
            {
                AppendConsole("--- CHECKING ACTIVE DOCKER LAB CONTAINERS ---", Color.Cyan);
                ExecuteSystemCmd("docker ps --filter name=lab_");
            }
            else if (action == "health")
            {
                AppendConsole("--- CHECKING LAB ENDPOINT RESPONSIVENESS ---", Color.Cyan);
                string[] endpoints = new[]
                {
                    "DVWA (PHP)|http://localhost/dvwa/",
                    "Mutillidae (PHP)|http://localhost/mutillidae/",
                    "bWAPP (PHP)|http://localhost/bwapp/",
                    "Juice Shop (Node)|http://localhost:3000/",
                    "WebGoat (Java)|http://localhost:8080/WebGoat"
                };

                Task.Run(() =>
                {
                    foreach (var ep in endpoints)
                    {
                        var partsEp = ep.Split('|');
                        string name = partsEp[0];
                        string url = partsEp[1];

                        try
                        {
                            var request = (HttpWebRequest)WebRequest.Create(url);
                            request.Timeout = 3000;
                            request.Method = "HEAD";
                            using (var response = (HttpWebResponse)request.GetResponse())
                            {
                                AppendConsole($"[ONLINE] {name,-20} -> HTTP {(int)response.StatusCode}", Color.Lime);
                            }
                        }
                        catch
                        {
                            AppendConsole($"[OFFLINE] {name,-19} -> Endpoint Unreachable", Color.Red);
                        }
                    }
                });
            }
            else if (action == "reset")
            {
                if (parts.Length < 3)
                {
                    AppendConsole("Usage: lab reset <dvwa|mutillidae|bwapp>", Color.Orange);
                    return;
                }

                string app = parts[2].ToLower();
                Task.Run(() =>
                {
                    try
                    {
                        if (app == "dvwa")
                        {
                            ExecuteSql("DROP DATABASE IF EXISTS `dvwa`; CREATE DATABASE `dvwa`;");
                            using (var wc = new WebClient())
                            {
                                wc.DownloadString("http://localhost/dvwa/setup.php");
                            }
                            AppendConsole("DVWA database re-seeded and reset to factory state.", Color.Lime);
                        }
                        else if (app == "mutillidae")
                        {
                            ExecuteSql("DROP DATABASE IF EXISTS `mutillidae`; CREATE DATABASE `mutillidae`;");
                            AppendConsole("Mutillidae database dropped and re-created.", Color.Lime);
                        }
                        else if (app == "bwapp")
                        {
                            ExecuteSql("DROP DATABASE IF EXISTS `bWAPP`; CREATE DATABASE `bWAPP`;");
                            AppendConsole("bWAPP database dropped and re-created.", Color.Lime);
                        }
                        else if (app == "xvwa")
                        {
                            AppendConsole("XVWA is unavailable.", Color.Orange);
                        }
                        else
                        {
                            AppendConsole($"Database reset is only supported for MySQL labs (dvwa, mutillidae, bwapp).", Color.Orange);
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendConsole($"Reset failed: {ex.Message}", Color.Red);
                    }
                });
            }
            else if (action == "logs")
            {
                if (parts.Length < 3)
                {
                    AppendConsole("Usage: lab logs <dvwa|mutillidae|bwapp>", Color.Orange);
                    return;
                }

                string app = parts[2].ToLower();
                string accessLog = @"C:\xampp\apache\logs\access.log";

                if (!File.Exists(accessLog))
                {
                    AppendConsole("Apache access.log file not found.", Color.Red);
                    return;
                }

                try
                {
                    var matchedLines = File.ReadAllLines(accessLog)
                                         .Where(l => l.IndexOf($"/{app}/", StringComparison.OrdinalIgnoreCase) >= 0)
                                         .ToList();

                    var recentLines = matchedLines.Skip(Math.Max(0, matchedLines.Count - 20));

                    AppendConsole($"--- RECENT HTTP REQUESTS TO /{app.ToUpper()}/ ---", Color.Cyan);
                    foreach (var l in recentLines)
                    {
                        AppendConsole(l, Color.LightGray);
                    }
                }
                catch (Exception ex)
                {
                    AppendConsole($"Error reading log file: {ex.Message}", Color.Red);
                }
            }
            else if (action == "difficulty")
            {
                if (parts.Length < 4 || parts[2].ToLower() != "dvwa")
                {
                    AppendConsole("Usage: lab difficulty dvwa <low|medium|high|impossible>", Color.Orange);
                    return;
                }

                string level = parts[3].ToLower();
                if (level == "low" || level == "medium" || level == "high" || level == "impossible")
                {
                    string query = $"UPDATE dvwa.dvwa_security SET security = '{level}';";
                    try
                    {
                        ExecuteSql(query);
                        AppendConsole($"DVWA Security Level changed to '{level.ToUpper()}'.", Color.Lime);
                    }
                    catch (Exception ex)
                    {
                        AppendConsole($"Failed to update DVWA difficulty: {ex.Message}", Color.Red);
                    }
                }
                else
                {
                    AppendConsole("Invalid level. Choose: low, medium, high, impossible", Color.Orange);
                }
            }
            else if (action == "uninstall")
            {
                if (parts.Length < 3)
                {
                    AppendConsole("Usage: lab uninstall <dvwa|mutillidae|bwapp|juiceshop|webgoat|xvwa>", Color.Orange);
                    return;
                }

                string app = parts[2].ToLower();
                if (app == "xvwa")
                {
                    AppendConsole("XVWA is archived and unavailable.", Color.Orange);
                    return;
                }

                if (app == "juiceshop" || app == "webgoat")
                {
                    string containerName = app == "juiceshop" ? "lab_juiceshop" : "lab_webgoat";
                    ExecuteSystemCmd($"docker rm -f {containerName}");
                    AppendConsole($"Removed Docker container: {containerName}", Color.Lime);
                    return;
                }

                string targetPath = Path.Combine(Properties.Settings.Default.HTMLPath, app);

                try
                {
                    if (Directory.Exists(targetPath))
                    {
                        Directory.Delete(targetPath, true);
                        AppendConsole($"Removed folder: {targetPath}", Color.Lime);
                    }

                    string dbName = app == "bwapp" ? "bWAPP" : app;
                    ExecuteSql($"DROP DATABASE IF EXISTS `{dbName}`;");
                    AppendConsole($"Dropped database '{dbName}' from MariaDB.", Color.Lime);
                }
                catch (Exception ex)
                {
                    AppendConsole($"Teardown error: {ex.Message}", Color.Red);
                }
            }
            else
            {
                AppendConsole($"Unknown action '{action}'. Usage: lab <list|install|start|stop|status|health|reset|logs|difficulty|uninstall> [app_name]", Color.Orange);
            }
        }

        #endregion

        #region New Advanced Developer Tooling Handlers

        private void HandleTunnelCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                AppendConsole("Usage: tunnel <ngrok|localtunnel> [port_or_domain]", Color.Orange);
                return;
            }

            string tool = parts[1].ToLower();
            string target = parts.Length > 2 ? parts[2] : "80";

            Task.Run(() =>
            {
                if (tool == "ngrok")
                {
                    AppendConsole($"Starting ngrok tunnel targeted at port/host '{target}'...", Color.Cyan);
                    ExecuteSystemCmd($"ngrok http {target}");
                }
                else if (tool == "localtunnel" || tool == "lt")
                {
                    AppendConsole($"Starting localtunnel targeted at port '{target}'...", Color.Cyan);
                    ExecuteSystemCmd($"lt --port {target}");
                }
                else
                {
                    AppendConsole("Supported tunneling drivers: 'ngrok' or 'localtunnel'", Color.Orange);
                }
            });
        }

        private void HandleXdebugCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                AppendConsole("Usage: xdebug <enable|disable|mode> [debug|profile|trace|off]", Color.Orange);
                return;
            }

            if (!File.Exists(PhpIniPath))
            {
                AppendConsole($"php.ini not found at: {PhpIniPath}", Color.Red);
                return;
            }

            string action = parts[1].ToLower();
            string content = File.ReadAllText(PhpIniPath);

            if (action == "enable" || action == "disable")
            {
                bool enable = action == "enable";
                string pattern = @"^\s*;?\s*zend_extension\s*=\s*xdebug";
                string replacement = enable ? "zend_extension=xdebug" : ";zend_extension=xdebug";

                if (Regex.IsMatch(content, pattern, RegexOptions.Multiline))
                {
                    content = Regex.Replace(content, pattern, replacement, RegexOptions.Multiline);
                    File.WriteAllText(PhpIniPath, content);
                    AppendConsole($"Xdebug extension {(enable ? "ENABLED" : "DISABLED")} in php.ini.", Color.Lime);
                    HandleServiceCommand(new[] { "service", "apache", "restart" });
                }
                else
                {
                    AppendConsole("zend_extension=xdebug directive was not found in php.ini.", Color.Red);
                }
            }
            else if (action == "mode")
            {
                if (parts.Length < 3)
                {
                    AppendConsole("Usage: xdebug mode <debug|profile|trace|off>", Color.Orange);
                    return;
                }

                string mode = parts[2].ToLower();
                string pattern = @"^\s*;?\s*xdebug\.mode\s*=\s*.*";
                string replacement = $"xdebug.mode={mode}\nxdebug.client_host=127.0.0.1\nxdebug.client_port=9003";

                if (Regex.IsMatch(content, pattern, RegexOptions.Multiline))
                {
                    content = Regex.Replace(content, pattern, replacement, RegexOptions.Multiline);
                }
                else
                {
                    content += $"\n[Xdebug]\nxdebug.mode={mode}\nxdebug.client_host=127.0.0.1\nxdebug.client_port=9003\n";
                }

                File.WriteAllText(PhpIniPath, content);
                AppendConsole($"Xdebug mode set to '{mode}' with IDE port 9003 (127.0.0.1).", Color.Lime);
                HandleServiceCommand(new[] { "service", "apache", "restart" });
            }
        }

        private void HandleComposerCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                AppendConsole("Usage: composer <audit|install|update> [target_directory]", Color.Orange);
                return;
            }

            string action = parts[1].ToLower();
            string targetPath = parts.Length > 2 ? parts[2] : DynamicHostingPath;

            if (!Directory.Exists(targetPath))
            {
                AppendConsole($"Target directory does not exist: {targetPath}", Color.Red);
                return;
            }

            Task.Run(() =>
            {
                AppendConsole($"Running 'composer {action}' inside {targetPath}...", Color.Cyan);
                ExecuteSystemCmd($"cd /d \"{targetPath}\" && composer {action}");
            });
        }

        private void HandleDbDiffCommand(string[] parts)
        {
            if (parts.Length < 4)
            {
                AppendConsole("Usage: db diff <source_db> <target_db>", Color.Orange);
                return;
            }

            string sourceDb = parts[2];
            string targetDb = parts[3];

            Task.Run(() =>
            {
                try
                {
                    AppendConsole($"Comparing Schemas: Source ('{sourceDb}') vs Target ('{targetDb}')...", Color.Cyan);

                    Dictionary<string, List<string>> sourceSchema = GetDatabaseSchema(sourceDb);
                    Dictionary<string, List<string>> targetSchema = GetDatabaseSchema(targetDb);

                    List<string> migrationSql = new List<string>();

                    foreach (var table in sourceSchema)
                    {
                        if (!targetSchema.ContainsKey(table.Key))
                        {
                            migrationSql.Add($"-- Missing Table in {targetDb}:\nCREATE TABLE `{targetDb}`.`{table.Key}` LIKE `{sourceDb}`.`{table.Key}`;");
                        }
                        else
                        {
                            var sourceCols = table.Value;
                            var targetCols = targetSchema[table.Key];

                            foreach (var col in sourceCols)
                            {
                                string colName = col.Split(' ')[0];
                                if (!targetCols.Any(c => c.StartsWith(colName + " ")))
                                {
                                    migrationSql.Add($"ALTER TABLE `{targetDb}`.`{table.Key}` ADD COLUMN {col};");
                                }
                            }
                        }
                    }

                    if (migrationSql.Count == 0)
                    {
                        AppendConsole("[PASS] Database schemas are fully synchronized.", Color.Lime);
                    }
                    else
                    {
                        AppendConsole("--- GENERATED MIGRATION SQL SCRIPT ---", Color.Yellow);
                        foreach (var sql in migrationSql)
                        {
                            AppendConsole(sql, Color.White);
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppendConsole($"Schema Diff Error: {ex.Message}", Color.Red);
                }
            });
        }

        private Dictionary<string, List<string>> GetDatabaseSchema(string dbName)
        {
            var schema = new Dictionary<string, List<string>>();
            using (var conn = new MySqlConnection(Helper.StorageMetricsHelper.GetConnectionString(dbName)))
            {
                conn.Open();
                using (var cmd = new MySqlCommand($"SELECT TABLE_NAME, COLUMN_NAME, COLUMN_TYPE, IS_NULLABLE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '{dbName}';", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string table = reader.GetString(0);
                        string colDef = $"`{reader.GetString(1)}` {reader.GetString(2)} {(reader.GetString(3) == "NO" ? "NOT NULL" : "NULL")}";

                        if (!schema.ContainsKey(table)) schema[table] = new List<string>();
                        schema[table].Add(colDef);
                    }
                }
            }
            return schema;
        }

        private void HandleSlowQueryCommand(string[] parts)
        {
            int lineCount = (parts.Length > 2 && int.TryParse(parts[2], out int l)) ? l : 20;
            string mysqlDataDir = @"C:\xampp\mysql\data\";
            string slowLogFile = Directory.GetFiles(mysqlDataDir, "*-slow.log").FirstOrDefault() ?? Path.Combine(mysqlDataDir, "mysql-slow.log");

            if (!File.Exists(slowLogFile))
            {
                AppendConsole("Slow query log file not found. Enabling MariaDB slow query logging...", Color.Yellow);
                ExecuteSql("SET GLOBAL slow_query_log = 'ON'; SET GLOBAL long_query_time = 1;");
                AppendConsole("Slow query logging is now ACTIVE (threshold: 1 sec). Re-run command after queries execute.", Color.Lime);
                return;
            }

            try
            {
                string[] allLines = File.ReadAllLines(slowLogFile);
                var recentLines = allLines.Skip(Math.Max(0, allLines.Length - lineCount));

                AppendConsole($"--- RECENT ENTRIES FROM SLOW QUERY LOG ({slowLogFile}) ---", Color.Cyan);
                foreach (var line in recentLines)
                {
                    AppendConsole(line, Color.LightGray);
                }
            }
            catch (Exception ex)
            {
                AppendConsole($"Error reading slow query log: {ex.Message}", Color.Red);
            }
        }

        private void HandleOpCacheCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                AppendConsole("Usage: opcache <status|reset>", Color.Orange);
                return;
            }

            string action = parts[1].ToLower();

            if (action == "reset")
            {
                string scriptPath = Path.Combine(DynamicHostingPath, "opcache_reset_internal.php");
                File.WriteAllText(scriptPath, "<?php if (function_exists('opcache_reset')) { opcache_reset(); echo 'OPCACHE_CLEARED'; } else { echo 'NO_OPCACHE'; } ?>");

                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        string response = wc.DownloadString("http://localhost/opcache_reset_internal.php");
                        if (response.Contains("OPCACHE_CLEARED")) AppendConsole("PHP OPcache memory buffers flushed successfully.", Color.Lime);
                        else AppendConsole("OPcache extension is not currently loaded in PHP.", Color.Yellow);
                    }
                }
                catch (Exception ex)
                {
                    AppendConsole($"OPcache flush failed: {ex.Message}", Color.Red);
                }
                finally
                {
                    if (File.Exists(scriptPath)) File.Delete(scriptPath);
                }
            }
            else if (action == "status")
            {
                string scriptPath = Path.Combine(DynamicHostingPath, "opcache_status_internal.php");
                File.WriteAllText(scriptPath, "<?php echo function_exists('opcache_get_status') ? json_encode(opcache_get_status(false)) : 'DISABLED'; ?>");

                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        string response = wc.DownloadString("http://localhost/opcache_status_internal.php");
                        AppendConsole("--- OPCACHE SYSTEM METRICS ---", Color.Cyan);
                        AppendConsole(response, Color.White);
                    }
                }
                catch (Exception ex)
                {
                    AppendConsole($"OPcache status check failed: {ex.Message}", Color.Red);
                }
                finally
                {
                    if (File.Exists(scriptPath)) File.Delete(scriptPath);
                }
            }
        }

        private void HandleSqliteCommand(string[] parts)
        {
            if (parts.Length < 3)
            {
                AppendConsole("Usage: sqlite <sqlite_db_file_path> <sql_query>", Color.Orange);
                return;
            }

            string dbPath = parts[2];
            string query = string.Join(" ", parts.Skip(3));

            if (!File.Exists(dbPath))
            {
                AppendConsole($"SQLite database file not found: {dbPath}", Color.Red);
                return;
            }

            try
            {
                string connString = $"Data Source={dbPath};Version=3;";
                using (var conn = new SQLiteConnection(connString))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        if (query.StartsWith("SELECT", StringComparison.OrdinalIgnoreCase))
                        {
                            using (var reader = cmd.ExecuteReader())
                            {
                                int cols = reader.FieldCount;
                                while (reader.Read())
                                {
                                    List<string> row = new List<string>();
                                    for (int i = 0; i < cols; i++) row.Add(reader[i].ToString());
                                    AppendConsole(string.Join(" | ", row), Color.White);
                                }
                            }
                        }
                        else
                        {
                            int affected = cmd.ExecuteNonQuery();
                            AppendConsole($"SQLite execution successful. Rows affected: {affected}", Color.Lime);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendConsole($"SQLite Error: {ex.Message}", Color.Red);
            }
        }

        private void HandlePostgreSqlCommand(string[] parts)
        {
            if (parts.Length < 3)
            {
                AppendConsole("Usage: pg <connection_string> <sql_query>", Color.Orange);
                return;
            }

            string connStr = parts[2];
            string query = string.Join(" ", parts.Skip(3));

            try
            {
                using (var conn = new NpgsqlConnection(connStr))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand(query, conn))
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            int cols = reader.FieldCount;
                            while (reader.Read())
                            {
                                List<string> row = new List<string>();
                                for (int i = 0; i < cols; i++) row.Add(reader[i]?.ToString() ?? "NULL");
                                AppendConsole(string.Join(" | ", row), Color.White);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendConsole($"PostgreSQL Error: {ex.Message}", Color.Red);
            }
        }

        private void InitializeErrorLogWatcher()
        {
            string dir = Path.GetDirectoryName(PhpErrorLogPath);
            if (!Directory.Exists(dir)) return;

            _phpErrorLogWatcher = new FileSystemWatcher(dir, Path.GetFileName(PhpErrorLogPath))
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size,
                EnableRaisingEvents = true
            };

            long lastPos = File.Exists(PhpErrorLogPath) ? new FileInfo(PhpErrorLogPath).Length : 0;

            _phpErrorLogWatcher.Changed += (sender, e) =>
            {
                try
                {
                    using (var stream = new FileStream(PhpErrorLogPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        if (stream.Length < lastPos) lastPos = 0;
                        stream.Seek(lastPos, SeekOrigin.Begin);

                        using (var reader = new StreamReader(stream))
                        {
                            string line;
                            while ((line = reader.ReadLine()) != null)
                            {
                                if (Regex.IsMatch(line, @"(Fatal error|Parse error|Uncaught Exception)", RegexOptions.IgnoreCase))
                                {
                                    AppendConsole($"[FATAL ERROR DETECTED]: {line}", Color.Red);
                                    TriggerDesktopNotification("PHP Fatal Error Detected", line);
                                }
                            }
                            lastPos = stream.Position;
                        }
                    }
                }
                catch { }
            };
        }

        private void TriggerDesktopNotification(string title, string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => TriggerDesktopNotification(title, message)));
                return;
            }

            NotifyIcon notifyIcon = new NotifyIcon
            {
                Visible = true,
                Icon = SystemIcons.Error,
                BalloonTipTitle = title,
                BalloonTipText = message.Length > 60 ? message.Substring(0, 60) + "..." : message,
                BalloonTipIcon = ToolTipIcon.Error
            };
            notifyIcon.ShowBalloonTip(5000);
        }

        private void HandleGitGraphCommand(string[] parts)
        {
            string targetPath = (parts.Length > 1 && Directory.Exists(parts[1])) ? parts[1] : DynamicHostingPath;
            AppendConsole($"--- GIT BRANCH GRAPH & HISTORY ({targetPath}) ---", Color.Cyan);
            ExecuteSystemCmd($"cd /d \"{targetPath}\" && git log --graph --oneline --all -n 15");
        }

        private void HandleStashCommand(string[] parts)
        {
            if (parts.Length < 2)
            {
                AppendConsole("Usage: stash <list|save|pop|drop> [repo_path] [message]", Color.Orange);
                return;
            }

            string action = parts[1].ToLower();
            string targetPath = (parts.Length > 2 && Directory.Exists(parts[2])) ? parts[2] : DynamicHostingPath;
            string msg = parts.Length > 3 ? string.Join(" ", parts.Skip(3)) : "Panel Auto-Stash";

            switch (action)
            {
                case "list":
                    ExecuteSystemCmd($"cd /d \"{targetPath}\" && git stash list");
                    break;
                case "save":
                    ExecuteSystemCmd($"cd /d \"{targetPath}\" && git stash push -m \"{msg}\"");
                    break;
                case "pop":
                    ExecuteSystemCmd($"cd /d \"{targetPath}\" && git stash pop");
                    break;
                case "drop":
                    ExecuteSystemCmd($"cd /d \"{targetPath}\" && git stash drop");
                    break;
                default:
                    AppendConsole("Unknown stash action. Use: list, save, pop, drop", Color.Orange);
                    break;
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _accessLogWatcher?.Dispose();
                _phpErrorLogWatcher?.Dispose();
                components?.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}