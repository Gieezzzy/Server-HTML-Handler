using MySql.Data.MySqlClient;
using Server_HTML_Handler.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Server_HTML_Handler.Helper
{
    internal class Helper
    {


        public static string ConnectionString
        { 
            get
            {
                return StorageMetricsHelper.GetConnectionString();
            }
        }

        public static bool DoesWindowsAccountExist(string accountName, out string resolvedName)
        {
            resolvedName = accountName;

            if (string.IsNullOrWhiteSpace(accountName))
                return false;

            try
            {
                // Translate user/group name to its Windows Security Identifier (SID)
                NTAccount ntAccount = new NTAccount(accountName);
                SecurityIdentifier sid = (SecurityIdentifier)ntAccount.Translate(typeof(SecurityIdentifier));

                // Resolve SID back to canonical NTAccount format (e.g. DOMAIN\Username or COMPUTER\Group)
                NTAccount resolvedAccount = (NTAccount)sid.Translate(typeof(NTAccount));
                resolvedName = resolvedAccount.Value;

                return sid != null;
            }
            catch (IdentityNotMappedException)
            {
                // Account does not exist on local SAM or network Active Directory domain
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }


        public static bool CreateUserAndGrantPrivileges(string username, string password, string host, string targetDb, List<string> privileges)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(host))
            {
                throw new ArgumentException("Username and host must not be empty.");
            }

            // Administrative privileges that MySQL/MariaDB strictly requires to be granted ON *.*
            var globalOnlyPrivileges = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "FILE", "SUPER", "PROCESS", "RELOAD", "SHUTDOWN", "SHOW DATABASES", "CREATE USER"
    };

            using (var conn = new MySqlConnection(ConnectionString))
            {
                conn.Open();

                string safeUser = username.Replace("'", "''");
                string safeHost = host.Replace("'", "''");

                // 1. Create User using parameterized password to allow special characters safely
                string createUserSql = $"CREATE USER IF NOT EXISTS '{safeUser}'@'{safeHost}' IDENTIFIED BY @password;";
                using (var cmd = new MySqlCommand(createUserSql, conn))
                {
                    cmd.Parameters.AddWithValue("@password", password ?? string.Empty);
                    cmd.ExecuteNonQuery();
                }

                // Determine if target is global (* or empty) or a specific database
                bool isGlobalTarget = string.IsNullOrWhiteSpace(targetDb) || targetDb.Trim() == "*" || targetDb.Trim() == "*.*";
                string targetDbSpecifier = isGlobalTarget ? "*.*" : $"`{targetDb.Trim().Replace("`", "``")}`.*";

                bool withGrantOption = false;
                List<string> globalPrivs = new List<string>();
                List<string> dbPrivs = new List<string>();

                if (privileges != null && privileges.Count > 0)
                {
                    foreach (var priv in privileges)
                    {
                        string cleanPriv = priv.Trim().Replace(";", "");

                        // GRANT OPTION belongs in WITH GRANT OPTION clause, not privilege list
                        if (string.Equals(cleanPriv, "GRANT OPTION", StringComparison.OrdinalIgnoreCase))
                        {
                            withGrantOption = true;
                            continue;
                        }

                        if (isGlobalTarget || globalOnlyPrivileges.Contains(cleanPriv))
                        {
                            globalPrivs.Add(cleanPriv);
                        }
                        else
                        {
                            dbPrivs.Add(cleanPriv);
                        }
                    }
                }
                else
                {
                    if (isGlobalTarget)
                        globalPrivs.Add("ALL PRIVILEGES");
                    else
                        dbPrivs.Add("ALL PRIVILEGES");
                }

                string grantOptionClause = withGrantOption ? " WITH GRANT OPTION" : "";

                // 2. Grant Global Privileges ON *.*
                if (globalPrivs.Count > 0)
                {
                    string globalGrantSql = $"GRANT {string.Join(", ", globalPrivs)} ON *.* TO '{safeUser}'@'{safeHost}'{grantOptionClause};";
                    using (var cmd = new MySqlCommand(globalGrantSql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                // 3. Grant Database Privileges ON `targetDb`.*
                if (!isGlobalTarget && dbPrivs.Count > 0)
                {
                    string dbGrantSql = $"GRANT {string.Join(", ", dbPrivs)} ON {targetDbSpecifier} TO '{safeUser}'@'{safeHost}'{grantOptionClause};";
                    using (var cmd = new MySqlCommand(dbGrantSql, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }

                // 4. Flush Privileges
                using (var cmd = new MySqlCommand("FLUSH PRIVILEGES;", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            return true;
        }

        public static void SetFolderPermissions(string folderPath, string accountName, bool isWriteAccess)
        {
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
            DirectorySecurity dirSecurity = dirInfo.GetAccessControl();

            FileSystemRights rights = isWriteAccess
                ? FileSystemRights.Modify | FileSystemRights.Synchronize
                : FileSystemRights.ReadAndExecute | FileSystemRights.Synchronize;

            FileSystemAccessRule accessRule = new FileSystemAccessRule(
                accountName,
                rights,
                InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit,
                PropagationFlags.None,
                AccessControlType.Allow
            );

            dirSecurity.AddAccessRule(accessRule);
            dirInfo.SetAccessControl(dirSecurity);
        }

        public static void CreateStorageFolderWithWriteAccess(string storagePath)
        {
            SetFolderPermissions(storagePath, "Users", true);
        }
    }

    public static class ServerBackupHelper
    {
        private const string MysqlDumpExePath = @"C:\xampp\mysql\bin\mysqldump.exe";
        private const string ApacheConfigPath = @"C:\xampp\apache\conf\httpd.conf";
        private const string PhpIniPath = @"C:\xampp\php\php.ini";
        private const string XamppApacheExe = @"C:\xampp\apache\bin\httpd.exe";
        private static readonly string[] ApacheServiceNames = { "Apache2.4", "apache", "Apache2", "xamppapache" };

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern IntPtr OpenProcess(uint access, bool inherit, int pid);
        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool CloseHandle(IntPtr handle);
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool QueryFullProcessImageName(IntPtr process, int flags, StringBuilder exeName, ref int size);

        /// <summary>Full path of a process' executable (works for any process, 32/64-bit). Null when it cannot be read.</summary>
        public static string GetProcessImagePath(int pid)
        {
            IntPtr h = OpenProcess(0x1000 /* PROCESS_QUERY_LIMITED_INFORMATION */, false, pid);
            if (h == IntPtr.Zero) return null;
            try
            {
                var sb = new StringBuilder(1024);
                int size = sb.Capacity;
                return QueryFullProcessImageName(h, 0, sb, ref size) ? sb.ToString(0, size) : null;
            }
            finally
            {
                CloseHandle(h);
            }
        }

        /// <summary>
        /// The httpd.exe processes of the XAMPP Apache only. Other products run their own httpd.exe (for example the PostgreSQL
        /// "PEMHTTPD" web server on port 8080); counting those made the handler think Apache was running when it was not.
        /// </summary>
        public static List<Process> GetXamppApacheProcesses()
        {
            string expected = Path.GetFullPath(XamppApacheExe);
            var list = new List<Process>();
            foreach (Process p in Process.GetProcessesByName("httpd"))
            {
                string path = null;
                try { path = GetProcessImagePath(p.Id); } catch { }
                if (path != null && string.Equals(Path.GetFullPath(path), expected, StringComparison.OrdinalIgnoreCase)) list.Add(p);
            }
            return list;
        }

        /// <summary>True when the XAMPP Apache is up: its httpd.exe is running, or its Windows service is running.</summary>
        public static bool IsXamppApacheRunning()
        {
            if (GetXamppApacheProcesses().Count > 0) return true;
            foreach (string name in ApacheServiceNames)
            {
                try
                {
                    using (ServiceController sc = new ServiceController(name))
                    {
                        sc.Refresh();
                        if (sc.Status == ServiceControllerStatus.Running) return true;
                    }
                }
                catch { }
            }
            return false;
        }

        public static void ExportDatabaseDump(string dbName, string outputPath)
        {
            string targetDatabase = string.IsNullOrWhiteSpace(dbName) ? "mysql" : dbName.Trim();

            using (var conn = new MySqlConnection(StorageMetricsHelper.GetConnectionString(targetDatabase)))
            {
                conn.Open();
                StringBuilder dumpBuilder = new StringBuilder();
                dumpBuilder.AppendLine($"-- Database Backup: {targetDatabase}");
                dumpBuilder.AppendLine($"-- Date: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n");

                List<string> tables = new List<string>();

                // 1. Retrieve tables and filter out orphaned temporary objects (#mysql50#)
                using (var cmd = new MySqlCommand("SHOW TABLES;", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string tableName = reader.GetString(0);
                        if (!tableName.StartsWith("#mysql50#", StringComparison.OrdinalIgnoreCase) && !tableName.StartsWith("#"))
                        {
                            tables.Add(tableName);
                        }
                    }
                }

                // 2. Iterate through valid tables with fault tolerance
                foreach (string table in tables)
                {
                    try
                    {
                        // Dump Table Structure
                        using (var cmd = new MySqlCommand($"SHOW CREATE TABLE `{table}`;", conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                dumpBuilder.AppendLine($"DROP TABLE IF EXISTS `{table}`;");
                                dumpBuilder.AppendLine(reader.GetString(1) + ";\n");
                            }
                        }

                        // Dump Table Data
                        using (var cmd = new MySqlCommand($"SELECT * FROM `{table}`;", conn))
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                List<string> values = new List<string>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    if (reader.IsDBNull(i))
                                        values.Add("NULL");
                                    else
                                        values.Add($"'{reader.GetValue(i).ToString().Replace("'", "''")}'");
                                }
                                dumpBuilder.AppendLine($"INSERT INTO `{table}` VALUES ({string.Join(", ", values)});");
                            }
                        }
                        dumpBuilder.AppendLine();
                    }
                    catch (Exception ex)
                    {
                        // Record the skipped object in the dump file without halting execution
                        dumpBuilder.AppendLine($"-- WARNING: Skipped table `{table}` due to engine error: {ex.Message}\n");
                    }
                }

                File.WriteAllText(outputPath, dumpBuilder.ToString());
            }
        }

        public static int GetCurrentPhpUploadLimit()
        {
            if (!File.Exists(PhpIniPath))
            {
                return 64; // Default fallback if php.ini is missing
            }

            try
            {
                string content = File.ReadAllText(PhpIniPath);

                // Find upload_max_filesize value (e.g. 64M, 128M)
                Match match = Regex.Match(content, @"^\s*upload_max_filesize\s*=\s*(\d+)", RegexOptions.Multiline | RegexOptions.IgnoreCase);

                if (match.Success && int.TryParse(match.Groups[1].Value, out int limit))
                {
                    return limit;
                }
            }
            catch
            {
                // Fallback to default in case of I/O error
            }

            return 64;
        }

        public static void CompressFolderToZip(string sourcePath, string zipPath)
        {
            if (!Directory.Exists(sourcePath))
            {
                throw new DirectoryNotFoundException($"Source web path does not exist: {sourcePath}");
            }

            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }

            ZipFile.CreateFromDirectory(sourcePath, zipPath, CompressionLevel.Optimal, includeBaseDirectory: false);
        }

        public static void UpdateApacheDocumentRoot(string newPath)
        {
            if (!File.Exists(ApacheConfigPath))
            {
                throw new FileNotFoundException($"httpd.conf file not found at: {ApacheConfigPath}");
            }

            // Normalize slashes for Apache httpd.conf
            string normalizedPath = newPath.Trim().Replace("\\", "/");
            if (normalizedPath.EndsWith("/"))
            {
                normalizedPath = normalizedPath.TrimEnd('/');
            }

            string content = File.ReadAllText(ApacheConfigPath);

            // 1. Update DocumentRoot "path"
            string documentRootPattern = @"(?i)^(\s*DocumentRoot\s+)""[^""]*""";
            content = Regex.Replace(content, documentRootPattern, $"DocumentRoot \"{normalizedPath}\"", RegexOptions.Multiline);

            // 2. Update the main <Directory "path"> block that directly follows DocumentRoot
            string mainDirPattern = @"(?i)(DocumentRoot\s+""[^""]*""\s*[\r\n]+\s*<Directory\s+"")([^""]*)("")";
            content = Regex.Replace(content, mainDirPattern, $"DocumentRoot \"{normalizedPath}\"\r\n<Directory \"{normalizedPath}\"", RegexOptions.Multiline);

            File.WriteAllText(ApacheConfigPath, content);
        }

        public static void UpdatePhpUploadLimits(int maxMb)
        {
            if (!File.Exists(PhpIniPath))
            {
                throw new FileNotFoundException($"php.ini file not found at: {PhpIniPath}");
            }

            string content = File.ReadAllText(PhpIniPath);

            // post_max_size must be slightly larger than upload_max_filesize to account for POST payload overhead
            int postMaxMb = maxMb + 8;

            // Flexible regex matching for numbers followed by optional units (K, M, G)
            string uploadPattern = @"^(\s*upload_max_filesize\s*=\s*)\d+[KMGkmg]?";
            string postPattern = @"^(\s*post_max_size\s*=\s*)\d+[KMGkmg]?";

            content = Regex.Replace(content, uploadPattern, $"upload_max_filesize = {maxMb}M", RegexOptions.Multiline);
            content = Regex.Replace(content, postPattern, $"post_max_size = {postMaxMb}M", RegexOptions.Multiline);

            File.WriteAllText(PhpIniPath, content);
        }

        public static bool StartApacheService(out string errorMessage, string preferredServiceName = null)
        {
            errorMessage = string.Empty;

            if (IsXamppApacheRunning())
            {
                return true;
            }

            List<string> candidateNames = new List<string>();
            if (!string.IsNullOrWhiteSpace(preferredServiceName))
            {
                candidateNames.Add(preferredServiceName);
            }
            candidateNames.AddRange(new[] { "Apache2.4", "apache", "Apache2", "xamppapache" });

            foreach (string serviceName in candidateNames.Distinct())
            {
                try
                {
                    using (ServiceController sc = new ServiceController(serviceName))
                    {
                        sc.Refresh();
                        if (sc.Status == ServiceControllerStatus.Running)
                        {
                            return true;
                        }

                        if (sc.Status == ServiceControllerStatus.Stopped || sc.Status == ServiceControllerStatus.Paused)
                        {
                            sc.Start();
                            sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(5));
                            return true;
                        }
                    }
                }
                catch
                {
                    // Fallback to elevated command prompt (Removed Hidden style so UAC prompt can appear)
                    try
                    {
                        ProcessStartInfo psi = new ProcessStartInfo
                        {
                            FileName = "cmd.exe",
                            Arguments = $"/c net start \"{serviceName}\"",
                            Verb = "runas",
                            UseShellExecute = true,
                            WindowStyle = ProcessWindowStyle.Normal // Must be Normal to allow UAC prompt
                        };

                        using (Process p = Process.Start(psi))
                        {
                            p.WaitForExit();
                        }

                        if (IsXamppApacheRunning())
                        {
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        errorMessage = ex.Message;
                    }
                }
            }

            errorMessage = "Apache service could not be started. Please ensure you accept the Administrator prompt.";
            return false;
        }

        public static bool StopApacheService(out string errorMessage, string preferredServiceName = null)
        {
            errorMessage = string.Empty;

            List<string> candidateNames = new List<string>();
            if (!string.IsNullOrWhiteSpace(preferredServiceName))
            {
                candidateNames.Add(preferredServiceName);
            }
            candidateNames.AddRange(ApacheServiceNames);

            foreach (string serviceName in candidateNames.Distinct())
            {
                try
                {
                    using (ServiceController sc = new ServiceController(serviceName))
                    {
                        sc.Refresh();
                        if (sc.Status == ServiceControllerStatus.Running || sc.Status == ServiceControllerStatus.Paused)
                        {
                            sc.Stop();
                            sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(3));
                        }
                    }
                }
                catch
                {
                }
            }

            try
            {
                // Only the XAMPP Apache is stopped. (taskkill /IM httpd.exe used to kill every httpd.exe on the PC,
                // including the PostgreSQL PEM web server.)
                var ours = GetXamppApacheProcesses();
                if (ours.Count > 0)
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = "taskkill.exe",
                        Arguments = "/F /T " + string.Join(" ", ours.Select(x => "/PID " + x.Id)),
                        Verb = "runas",
                        UseShellExecute = true,
                        WindowStyle = ProcessWindowStyle.Normal
                    };

                    using (Process p = Process.Start(psi))
                    {
                        p?.WaitForExit();
                    }
                }

                // Apache that runs as a Windows service under SYSTEM cannot be seen or stopped without elevation.
                if (IsXamppApacheRunning())
                {
                    foreach (string serviceName in candidateNames.Distinct())
                    {
                        try
                        {
                            using (ServiceController sc = new ServiceController(serviceName))
                            {
                                sc.Refresh();
                                if (sc.Status != ServiceControllerStatus.Running) continue;
                            }

                            ProcessStartInfo psi = new ProcessStartInfo
                            {
                                FileName = "cmd.exe",
                                Arguments = $"/c net stop \"{serviceName}\"",
                                Verb = "runas",
                                UseShellExecute = true,
                                WindowStyle = ProcessWindowStyle.Normal
                            };
                            using (Process p = Process.Start(psi))
                            {
                                p?.WaitForExit();
                            }
                        }
                        catch { }
                    }
                }
            }
            catch (Exception ex)
            {
                errorMessage = $"Stopping Apache failed: {ex.Message}";
                return false;
            }

            System.Threading.Thread.Sleep(500);

            if (IsXamppApacheRunning())
            {
                errorMessage = "Apache process is still active. The Administrator UAC prompt was likely canceled.";
                return false;
            }

            return true;
        }
        /// <summary>
        /// Configures the Apache2.4 Windows Service to start automatically on system boot (even before user signs into Windows).
        /// </summary>
        public static bool ConfigureApacheServiceAutoStart(out string message)
        {
            try
            {
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = "cmd.exe",
                    Arguments = "/c sc.exe config Apache2.4 start= auto & net start Apache2.4",
                    Verb = "runas",
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Normal
                };

                using (Process p = Process.Start(psi))
                {
                    p?.WaitForExit();
                }

                message = "Apache2.4 service has been configured to start automatically on Windows boot.";
                return true;
            }
            catch (Exception ex)
            {
                message = "Failed to configure Apache service: " + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// Checks whether Server HTML Handler is configured to auto-start with Windows (via Startup shortcut or Registry Run).
        /// </summary>
        public static bool IsWindowsStartupEnabled()
        {
            try
            {
                if (File.Exists(GetStartupShortcutPath())) return true;

                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", false))
                {
                    return key?.GetValue("ServerHTMLHandler") != null;
                }
            }
            catch { return false; }
        }

        /// <summary>
        /// Configures Server HTML Handler to auto-start into the notification tray with Windows.
        /// Prefers the ClickOnce shortcut, which keeps working after updates; the exe path changes with every version.
        /// </summary>
        public static void SetWindowsStartup(bool enable)
        {
            try
            {
                string shortcutFile = GetStartupShortcutPath();
                string installedShortcut = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Programs), "Eigeman Go", "Server Management System.appref-ms");
                bool useShortcut = enable && File.Exists(installedShortcut);

                if (useShortcut)
                {
                    File.Copy(installedShortcut, shortcutFile, true);
                }
                else if (File.Exists(shortcutFile))
                {
                    File.Delete(shortcutFile);
                }

                using (var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (key != null)
                    {
                        if (enable && !useShortcut)
                        {
                            key.SetValue("ServerHTMLHandler", "\"" + Application.ExecutablePath + "\" /tray");
                        }
                        else
                        {
                            key.DeleteValue("ServerHTMLHandler", false);
                        }
                    }
                }
            }
            catch { }
        }

        private static string GetStartupShortcutPath()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Startup), "Server Management System.appref-ms");
        }

        public static List<RouteInfo> GetRoutes(string rootPath)
        {
            var routes = new List<RouteInfo>();

            if (string.IsNullOrEmpty(rootPath) || !Directory.Exists(rootPath))
                return routes;

            DirectoryInfo rootDir = new DirectoryInfo(rootPath);

            foreach (DirectoryInfo subDir in rootDir.GetDirectories())
            {
                // Skip system or internal folders
                if (subDir.Name.StartsWith(".") || subDir.Name.Equals("xampp", StringComparison.OrdinalIgnoreCase))
                    continue;

                string routeName = subDir.Name;
                string physicalLocation = subDir.FullName;
                string targetFile = "index.php";
                string urlPath = $"/{routeName}/";

                // --- CI4 & MODERN FRAMEWORK DETECTION ---
                string publicFolderPath = Path.Combine(subDir.FullName, "public");

                if (File.Exists(Path.Combine(publicFolderPath, "index.php")))
                {
                    targetFile = "public/index.php";
                    // If root has .htaccess rewriting to public/, route URL is directly /{routeName}/
                    string rootHtaccess = Path.Combine(subDir.FullName, ".htaccess");
                    if (File.Exists(rootHtaccess) && File.ReadAllText(rootHtaccess).Contains("public/"))
                    {
                        urlPath = $"/{routeName}/";
                    }
                    else
                    {
                        urlPath = $"/{routeName}/public/";
                    }
                }
                else if (File.Exists(Path.Combine(publicFolderPath, "index.html")))
                {
                    targetFile = "public/index.html";
                    urlPath = $"/{routeName}/public/";
                }
                // --- STANDARD ROOT ENTRY DETECTION ---
                else if (File.Exists(Path.Combine(subDir.FullName, "index.html")))
                {
                    targetFile = "index.html";
                    urlPath = $"/{routeName}/";
                }
                else if (File.Exists(Path.Combine(subDir.FullName, "index.php")))
                {
                    targetFile = "index.php";
                    urlPath = $"/{routeName}/";
                }
                else
                {
                    var firstPhp = subDir.GetFiles("*.php").FirstOrDefault();
                    if (firstPhp != null)
                    {
                        targetFile = firstPhp.Name;
                    }
                }

                string statusAction = GetRouteStatus(subDir.FullName);

                routes.Add(new RouteInfo
                {
                    RouteName = routeName,
                    TargetFile = targetFile,
                    UrlPath = urlPath,
                    PhysicalLocation = physicalLocation,
                    StatusAction = statusAction
                });
            }

            return routes;
        }

        /// <summary>
        /// Checks the .htaccess file in the given directory to determine route status.
        /// </summary>
        public static string GetRouteStatus(string directoryPath)
        {
            string htaccessPath = Path.Combine(directoryPath, ".htaccess");
            if (File.Exists(htaccessPath))
            {
                string htContent = File.ReadAllText(htaccessPath);
                if (htContent.Contains("Require all denied") || htContent.Contains("Deny from all"))
                {
                    return "Offline";
                }
                if (htContent.Contains("ROUTE STATUS START") || htContent.Contains("MAINTENANCE MODE START"))
                {
                    return "Maintenance";
                }
            }
            return "Online";
        }

        public static bool SetRouteStatus(string physicalPath, string newStatus, out string errorMessage)
        {
            errorMessage = string.Empty;

            if (string.IsNullOrEmpty(physicalPath) || !Directory.Exists(physicalPath))
            {
                errorMessage = "Directory path does not exist.";
                return false;
            }

            string htaccessPath = Path.Combine(physicalPath, ".htaccess");

            try
            {
                string content = "";

                if (File.Exists(htaccessPath))
                {
                    // Unset Hidden attribute if active to enable file overwriting
                    FileAttributes attributes = File.GetAttributes(htaccessPath);
                    if ((attributes & FileAttributes.Hidden) == FileAttributes.Hidden)
                    {
                        File.SetAttributes(htaccessPath, attributes & ~FileAttributes.Hidden);
                    }

                    content = File.ReadAllText(htaccessPath);
                }

                // Strip out pre-existing status rule blocks
                content = Regex.Replace(
                    content,
                    @"# --- ROUTE STATUS START ---[\s\S]*?# --- ROUTE STATUS END ---",
                    ""
                ).Trim();

                string newRule = "";
                if (string.Equals(newStatus, "Offline", StringComparison.OrdinalIgnoreCase))
                {
                    newRule = "\n\n# --- ROUTE STATUS START ---\nRequire all denied\n# --- ROUTE STATUS END ---\n";
                }
                else if (string.Equals(newStatus, "Maintenance", StringComparison.OrdinalIgnoreCase))
                {
                    newRule = "\n\n# --- ROUTE STATUS START ---\nRewriteEngine On\nRewriteRule ^ - [R=503,L]\nErrorDocument 503 \"Site under maintenance.\"\nHeader set Retry-After \"3600\"\n# --- ROUTE STATUS END ---\n";
                }

                File.WriteAllText(htaccessPath, content + newRule);
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        public static string GetPhpVersion()
        {
            try
            {
                string phpPath = @"C:\xampp\php\php.exe";
                if (!File.Exists(phpPath))
                {
                    phpPath = "php";
                }

                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = phpPath,
                    Arguments = "-v",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process proc = Process.Start(psi))
                {
                    string output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit();

                    if (!string.IsNullOrEmpty(output))
                    {
                        var match = Regex.Match(output, @"PHP\s+([0-9\.]+)");
                        if (match.Success)
                        {
                            return match.Groups[1].Value + "+";
                        }
                    }
                }
            }
            catch
            {
                // Fallthrough on execution error
            }

            return "Unknown";
        }

        /// <summary>
        /// Checks the status of the MySQL Windows Service.
        /// </summary>
        public static string GetMySqlStatus(string serviceName)
        {
            try
            {
                using (ServiceController mysqlSc = new ServiceController(serviceName))
                {
                    // Force refresh to get the accurate current state from Windows
                    mysqlSc.Refresh();

                    if (mysqlSc.Status == ServiceControllerStatus.Running)
                    {
                        return "Running (MySQL)";
                    }
                    else
                    {
                        return $"Stopped ({mysqlSc.Status})";
                    }
                }
            }
            catch
            {
                return "Stopped / Not Found";
            }
        }

        /// <summary>
        /// Calculates total directory size and formats it as MB or GB.
        /// </summary>
        public static string GetDirectorySizeFormatted(string path)
        {
            if (string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                return "Path not set";
            }

            try
            {
                long totalBytes = GetDirectorySize(new DirectoryInfo(path));
                double totalMb = totalBytes / (1024.0 * 1024.0);

                if (totalMb > 1024)
                {
                    return $"{totalMb / 1024.0:F2} GB";
                }

                return $"{totalMb:F2} MB";
            }
            catch
            {
                return "Error calculating";
            }
        }

        /// <summary>
        /// Recursively calculates total byte count for a directory structure while ignoring access exceptions.
        /// </summary>
        public static long GetDirectorySize(DirectoryInfo dir)
        {
            if (dir == null || !dir.Exists)
            {
                return 0;
            }

            long size = 0;

            try
            {
                foreach (FileInfo file in dir.EnumerateFiles("*", SearchOption.TopDirectoryOnly))
                {
                    size += file.Length;
                }

                foreach (DirectoryInfo subDir in dir.EnumerateDirectories("*", SearchOption.TopDirectoryOnly))
                {
                    try
                    {
                        size += GetDirectorySize(subDir);
                    }
                    catch (UnauthorizedAccessException)
                    {
                        // Ignore unauthorized subdirectories
                    }
                }
            }
            catch (UnauthorizedAccessException)
            {
                // Ignore unauthorized root directory
            }

            return size;
        }

        public class ConnectedClientInfo
        {
            public string IP { get; set; }
            public string Hostname { get; set; }
            public string ConnectedSince { get; set; }
            public string Status { get; set; }
            public string UserAgent { get; set; }
        }

        /// <summary>
        /// Performs a reverse DNS lookup to resolve an IP address into a Hostname/PC name.
        /// </summary>
        public static string ResolveHostname(string ipAddress)
        {
            if (ipAddress == "127.0.0.1" || ipAddress == "::1")
                return "localhost";

            try
            {
                IPHostEntry hostEntry = Dns.GetHostEntry(ipAddress);
                return string.IsNullOrEmpty(hostEntry.HostName) ? "Unknown PC" : hostEntry.HostName;
            }
            catch
            {
                return "Unknown PC";
            }
        }

        /// <summary>
        /// Reads and parses the Apache access log safely while Apache holds an active lock.
        /// </summary>
        public static List<ConnectedClientInfo> GetConnectedClients(string logPath = @"C:\xampp\apache\logs\access.log", int lineCount = 100)
        {
            var clients = new List<ConnectedClientInfo>();

            if (!File.Exists(logPath))
                return clients;

            var lineQueue = new Queue<string>(lineCount);

            using (FileStream fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(fs))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (lineQueue.Count >= lineCount)
                        {
                            lineQueue.Dequeue();
                        }
                        lineQueue.Enqueue(line);
                    }
                }
            }

            Regex logRegex = new Regex(
                @"^(\S+)\s+\S+\s+\S+\s+\[([^\]]+)\]\s+""([^""]*)""\s+(\d{3})\s+(?:\d+|-)(?:\s+""[^""]*""\s+""([^""]*)""|)",
                RegexOptions.Compiled
            );

            var recentClients = lineQueue
                .Select(line => logRegex.Match(line))
                .Where(match => match.Success && !string.IsNullOrEmpty(match.Groups[1].Value))
                .Select(match => new
                {
                    IP = match.Groups[1].Value,
                    ConnectedSince = match.Groups[2].Value,
                    Request = match.Groups[3].Value,
                    StatusCode = match.Groups[4].Value,
                    UserAgent = match.Groups[5].Success && !string.IsNullOrEmpty(match.Groups[5].Value)
                        ? match.Groups[5].Value
                        : "Standard Client"
                })
                .GroupBy(x => x.IP)
                .Select(g => g.Last()) // Retain latest connection per IP
                .Reverse();

            foreach (var client in recentClients)
            {
                clients.Add(new ConnectedClientInfo
                {
                    IP = client.IP,
                    Hostname = ResolveHostname(client.IP),
                    ConnectedSince = client.ConnectedSince,
                    Status = $"{client.StatusCode} ({client.Request})",
                    UserAgent = client.UserAgent
                });
            }

            return clients;
        }

        /// <summary>
        /// Reads and filters log contents using stream tailing without triggering memory spikes or process lock exceptions.
        /// </summary>
        public static string ReadAndFilterLogs(string logPath, string searchTerm, int maxLines = 200)
        {
            if (!File.Exists(logPath))
            {
                return "[Error] Log file not found.";
            }

            try
            {
                var lineQueue = new Queue<string>(maxLines);

                using (FileStream fs = new FileStream(logPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (StreamReader reader = new StreamReader(fs))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (string.IsNullOrWhiteSpace(searchTerm) || line.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            if (lineQueue.Count >= maxLines)
                            {
                                lineQueue.Dequeue();
                            }
                            lineQueue.Enqueue(line);
                        }
                    }
                }

                return string.Join(Environment.NewLine, lineQueue);
            }
            catch (Exception ex)
            {
                return $"[Error] Failed to read log: {ex.Message}";
            }
        }

        public class SystemMetricsInfo
        {
            public string DbPoolStatus { get; set; }
            public string CpuLoad { get; set; }
            public string RamUtil { get; set; }
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        private static readonly PerformanceCounter CpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");

        /// <summary>
        /// Fetches system CPU load, RAM utilization, and active MySQL connection count.
        /// </summary>
        public static SystemMetricsInfo GetSystemMetrics()
        {
            // 1. CPU Load
            float cpuPercent = 0;
            try
            {
                cpuPercent = CpuCounter.NextValue();
                if (cpuPercent == 0)
                {
                    System.Threading.Thread.Sleep(50);
                    cpuPercent = CpuCounter.NextValue();
                }
            }
            catch { }

            // 2. RAM Utilization
            string ramText = "RAM Utilized: Unknown";
            try
            {
                using (var availableRamCounter = new PerformanceCounter("Memory", "Available MBytes"))
                {
                    float availableMb = availableRamCounter.NextValue();

                    if (GetPhysicallyInstalledSystemMemory(out long totalMemoryKb))
                    {
                        double totalMb = totalMemoryKb / 1024.0;
                        double usedMb = totalMb - availableMb;

                        ramText = usedMb > 1024
                            ? $"RAM Utilized: {usedMb / 1024.0:F1} GB"
                            : $"RAM Utilized: {usedMb:F0} MB";
                    }
                }
            }
            catch { }

            // 3. MySQL Pool / Active Connections
            string dbStatus = "MySQL Pool Connections: Offline";
            try
            {
                string mysqlAdminPath = @"C:\xampp\mysql\bin\mysqladmin.exe";
                if (File.Exists(mysqlAdminPath))
                {
                    ProcessStartInfo psi = new ProcessStartInfo
                    {
                        FileName = mysqlAdminPath,
                        Arguments = "-u root status",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    };

                    using (Process p = Process.Start(psi))
                    {
                        string output = p.StandardOutput.ReadToEnd();
                        p.WaitForExit();

                        var match = Regex.Match(output, @"Threads:\s*(\d+)");
                        if (match.Success)
                        {
                            int activeThreads = int.Parse(match.Groups[1].Value);
                            dbStatus = $"MySQL Pool Connections: Active ({activeThreads}/10)";
                        }
                    }
                }
            }
            catch { }

            return new SystemMetricsInfo
            {
                DbPoolStatus = dbStatus,
                CpuLoad = $"CPU Load: {cpuPercent:F1}%",
                RamUtil = ramText
            };
        }

        public class FolderAclEntry
        {
            public string AccountName { get; set; }
            public string AccessType { get; set; } // Allow / Deny
            public string AccessLevel { get; set; } // Read-Only or Read/Write
        }
        public class AclPermissionRule
        {
            public string AccountName { get; set; }  // Binds to "colAccount"
            public string AccessType { get; set; }   // Binds to "colAccessType" (Allow / Deny)
            public string AccessLevel { get; set; }  // Binds to "colAccessLevel" (Read-Only / Read/Write)
        }


        public static List<AclPermissionRule> GetFolderAclRules(string folderPath)
        {
            var rules = new List<AclPermissionRule>();

            if (string.IsNullOrWhiteSpace(folderPath) || !Directory.Exists(folderPath))
                return rules;

            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(folderPath);

                // Fetch NTFS Access Control Security rules
                DirectorySecurity dirSecurity = dirInfo.GetAccessControl();
                AuthorizationRuleCollection aclRules = dirSecurity.GetAccessRules(
                    includeExplicit: true,
                    includeInherited: true,
                    targetType: typeof(NTAccount)
                );

                foreach (FileSystemAccessRule rule in aclRules)
                {
                    // Skip system/internal SIDs if unnecessary
                    string account = rule.IdentityReference.Value;

                    string type = rule.AccessControlType.ToString(); // Allow or Deny

                    // Determine friendly Access Level string based on FileSystemRights bitmask
                    string level = "Read-Only";
                    if ((rule.FileSystemRights & FileSystemRights.Write) == FileSystemRights.Write ||
                        (rule.FileSystemRights & FileSystemRights.Modify) == FileSystemRights.Modify ||
                        (rule.FileSystemRights & FileSystemRights.FullControl) == FileSystemRights.FullControl)
                    {
                        level = "Read/Write";
                    }

                    rules.Add(new AclPermissionRule
                    {
                        AccountName = account,
                        AccessType = type,
                        AccessLevel = level
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to read ACLs: {ex.Message}");
            }

            return rules;
        }

        public static void ImportDatabaseDump(string targetDb, string sqlFilePath)
        {
            if (!File.Exists(sqlFilePath))
            {
                throw new FileNotFoundException("SQL dump file not found.", sqlFilePath);
            }

            string mysqlExecutable = @"C:\xampp\mysql\bin\mysql.exe";
            if (!File.Exists(mysqlExecutable))
            {
                mysqlExecutable = "mysql.exe"; // Fallback to system PATH
            }

            string dbArg = string.IsNullOrWhiteSpace(targetDb) ? "" : $"`{targetDb}`";
            string formattedFilePath = sqlFilePath.Replace("\\", "/");

            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = mysqlExecutable,
                Arguments = $"-u root {dbArg} -e \"source {formattedFilePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = Process.Start(psi))
            {
                string errorOutput = process.StandardError.ReadToEnd();
                process.WaitForExit();

                if (process.ExitCode != 0 && !string.IsNullOrWhiteSpace(errorOutput))
                {
                    throw new Exception($"MySQL Restore Failed: {errorOutput}");
                }
            }
        }

        /// <summary>
        /// Reads log file content safely without locking active processes.
        /// </summary>
        public static string ReadServerLogFile(string selectedLog)
        {
            string logFilePath = selectedLog switch
            {
                "Apache Error Log" => @"C:\xampp\apache\logs\error.log",
                "Apache Access Log" => @"C:\xampp\apache\logs\access.log",
                "MySQL Error Log" => @"C:\xampp\mysql\data\mysql_error.log",
                _ => selectedLog
            };

            if (!File.Exists(logFilePath))
            {
                return $"[Log file not found at: {logFilePath}]";
            }

            // Use FileShare.ReadWrite so Apache/MySQL active locks don't block reading
            using (var fs = new FileStream(logFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs))
            {
                return sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Restarts the Apache server service or httpd process.
        /// </summary>
        public static void RestartApacheService()
        {
            // 1. Try restarting via Windows Service (if installed as Apache service)
            try
            {
                using (ServiceController sc = new ServiceController("Apache2.4"))
                {
                    if (sc.Status == ServiceControllerStatus.Running || sc.Status == ServiceControllerStatus.Paused)
                    {
                        sc.Stop();
                        sc.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromSeconds(10));
                        sc.Start();
                        sc.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromSeconds(10));
                        return;
                    }
                }
            }
            catch
            {
                // Fallback to standalone XAMPP httpd.exe process kill/start
            }

            // 2. Kill the running XAMPP httpd.exe instances (other products' httpd.exe are left alone)
            foreach (var proc in GetXamppApacheProcesses())
            {
                try
                {
                    proc.Kill();
                    proc.WaitForExit(3000);
                }
                catch { }
            }

            // 3. Relaunch httpd.exe
            string httpdPath = XamppApacheExe;
            if (File.Exists(httpdPath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = httpdPath,
                    UseShellExecute = true,
                    WindowStyle = ProcessWindowStyle.Hidden
                });
            }
        }


    }


    public class FolderSizeDetail
    {
        public string FolderName { get; set; }
        public string FullPath { get; set; }
        public long SizeBytes { get; set; }
        public string FormattedSize { get; set; }
    }

    public static class StorageMetricsHelper
    {
        /// <summary>
        /// Gets available drive space and total directory consumption.
        /// </summary>
        public static void GetDriveAndDirectoryMetrics(string path, out string availableDriveSpace, out string totalConsumedSpace)
        {
            availableDriveSpace = "N/A";
            totalConsumedSpace = "0.00 MB";

            if (string.IsNullOrWhiteSpace(path)) return;

            try
            {
                // 1. Calculate Drive Free Space
                string root = Path.GetPathRoot(Path.GetFullPath(path));
                if (!string.IsNullOrEmpty(root) && Directory.Exists(root))
                {
                    DriveInfo drive = new DriveInfo(root);
                    double freeGb = drive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);
                    availableDriveSpace = $"{freeGb:F2} GB";
                }

                // 2. Calculate Directory Consumed Space
                if (Directory.Exists(path))
                {
                    long dirBytes = ServerBackupHelper.GetDirectorySize(new DirectoryInfo(path));
                    totalConsumedSpace = FormatSizeBytes(dirBytes);
                }
            }
            catch { }
        }

        /// <summary>
        /// Scans top-level subdirectories, calculates size, and returns them ordered biggest to smallest.
        /// </summary>
        public static List<FolderSizeDetail> GetSubfolderSizesSorted(string parentPath)
        {
            var result = new List<FolderSizeDetail>();

            if (string.IsNullOrWhiteSpace(parentPath) || !Directory.Exists(parentPath))
                return result;

            DirectoryInfo parentDir = new DirectoryInfo(parentPath);

            foreach (DirectoryInfo subDir in parentDir.GetDirectories())
            {
                try
                {
                    long size = ServerBackupHelper.GetDirectorySize(subDir);
                    result.Add(new FolderSizeDetail
                    {
                        FolderName = subDir.Name,
                        FullPath = subDir.FullName,
                        SizeBytes = size,
                        FormattedSize = FormatSizeBytes(size)
                    });
                }
                catch (UnauthorizedAccessException) { }
            }

            // Sort descending: largest folders on top
            return result.OrderByDescending(x => x.SizeBytes).ToList();
        }

        public static string FormatSizeBytes(long bytes)
        {
            double mb = bytes / (1024.0 * 1024.0);
            if (mb >= 1024)
            {
                return $"{mb / 1024.0:F2} GB";
            }
            return $"{mb:F2} MB";
        }

        public static string GetConnectionString(string database = "mysql")
        {
            string server = string.IsNullOrWhiteSpace(Properties.Settings.Default.DbHost)
                ? "127.0.0.1"
                : Properties.Settings.Default.DbHost;

            uint port = uint.TryParse(Properties.Settings.Default.DbPort, out uint parsedPort)
                ? parsedPort
                : 3306;

            string user = string.IsNullOrWhiteSpace(Properties.Settings.Default.MasterUser)
                ? "root"
                : Properties.Settings.Default.MasterUser;

            // Check if DBmaster setting is empty; encrypt and store default if needed
            if (string.IsNullOrWhiteSpace(Properties.Settings.Default.DBmaster))
            {
                string rawPassword = "M@$t3r"; // Set your actual MariaDB password here

                Properties.Settings.Default.DBmaster = AesEncryptionHelper.Encrypt(rawPassword);
                Properties.Settings.Default.Save();
            }

            // Retrieve encrypted setting and decrypt via DPAPI
            string rawEncryptedPassword = Properties.Settings.Default.DBmaster;
            string decryptedPassword = AesEncryptionHelper.Decrypt(rawEncryptedPassword);

            MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
            {
                Server = server,
                Port = port,
                UserID = user,
                Password = decryptedPassword,
                Database = database,
                SslMode = MySqlSslMode.Disabled,
                AllowUserVariables = true,
                ConvertZeroDateTime = true,
                ConnectionTimeout = 5
            };

            return builder.ConnectionString;
        }


    }

}
