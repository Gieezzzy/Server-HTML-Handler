using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using MySql.Data.MySqlClient;

namespace Server_HTML_Handler.Helper
{
    public static class TestLabHelper
    {
        // Dynamically retrieve the hosting folder path from Settings
        private static string HostingPath => Properties.Settings.Default.HTMLPath;

        public static void InstallDvwa(Action<string> logCallback)
        {
            string targetDir = Path.Combine(HostingPath, "dvwa");
            string zipUrl = "https://github.com/digininja/DVWA/archive/refs/heads/master.zip";
            string tempZip = Path.Combine(Path.GetTempPath(), "dvwa_master.zip");

            logCallback("Downloading DVWA repository from GitHub...");
            using (WebClient wc = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                wc.DownloadFile(zipUrl, tempZip);
            }

            logCallback($"Extracting files to {targetDir}...");
            if (Directory.Exists(targetDir)) Directory.Delete(targetDir, true);

            string extractTemp = Path.Combine(Path.GetTempPath(), "dvwa_extract");
            if (Directory.Exists(extractTemp)) Directory.Delete(extractTemp, true);

            ZipFile.ExtractToDirectory(tempZip, extractTemp);

            string extractedFolder = Path.Combine(extractTemp, "DVWA-master");
            if (!Directory.Exists(extractedFolder))
            {
                extractedFolder = Directory.GetDirectories(extractTemp)[0];
            }

            CopyDirectory(extractedFolder, targetDir);

            // Configure config.inc.php
            logCallback("Configuring DVWA database settings...");
            string configFileDist = Path.Combine(targetDir, @"config\config.inc.php.dist");
            string configFile = Path.Combine(targetDir, @"config\config.inc.php");

            if (File.Exists(configFileDist))
            {
                string configContent = File.ReadAllText(configFileDist);
                configContent = configContent.Replace("$_DVWA[ 'db_password' ] = 'p@ssw0rd';", "$_DVWA[ 'db_password' ] = '';");
                File.WriteAllText(configFile, configContent);
            }

            // Create dvwa database
            logCallback("Creating MariaDB 'dvwa' database...");
            using (var conn = new MySqlConnection(StorageMetricsHelper.GetConnectionString()))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("CREATE DATABASE IF NOT EXISTS `dvwa`;", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            File.Delete(tempZip);
            if (Directory.Exists(extractTemp)) Directory.Delete(extractTemp, true);

            logCallback("DVWA Installed Successfully! Access at: http://localhost/dvwa/setup.php");
        }

        public static void InstallMutillidae(Action<string> logCallback)
        {
            string targetDir = Path.Combine(HostingPath, "mutillidae");
            string zipUrl = "https://github.com/webpwnized/mutillidae/archive/refs/heads/main.zip";
            string tempZip = Path.Combine(Path.GetTempPath(), "mutillidae_master.zip");

            logCallback("Downloading OWASP Mutillidae II repository from GitHub...");
            using (WebClient wc = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                wc.DownloadFile(zipUrl, tempZip);
            }

            logCallback($"Extracting files to {targetDir}...");
            if (Directory.Exists(targetDir)) Directory.Delete(targetDir, true);

            string extractTemp = Path.Combine(Path.GetTempPath(), "mutillidae_extract");
            if (Directory.Exists(extractTemp)) Directory.Delete(extractTemp, true);

            ZipFile.ExtractToDirectory(tempZip, extractTemp);

            string extractedFolder = Path.Combine(extractTemp, "mutillidae-main");
            if (!Directory.Exists(extractedFolder))
            {
                extractedFolder = Directory.GetDirectories(extractTemp)[0];
            }

            CopyDirectory(extractedFolder, targetDir);

            // Create mutillidae database
            logCallback("Creating MariaDB 'mutillidae' database...");
            using (var conn = new MySqlConnection(StorageMetricsHelper.GetConnectionString()))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("CREATE DATABASE IF NOT EXISTS `mutillidae`;", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            File.Delete(tempZip);
            if (Directory.Exists(extractTemp)) Directory.Delete(extractTemp, true);

            logCallback("OWASP Mutillidae II Installed Successfully! Access at: http://localhost/mutillidae/");
        }

        public static void InstallBwapp(Action<string> logCallback)
        {
            string targetDir = Path.Combine(HostingPath, "bwapp");
            string zipUrl = "https://github.com/ajpalok/bWAPP/archive/refs/heads/main.zip";
            string tempZip = Path.Combine(Path.GetTempPath(), "bwapp_master.zip");

            logCallback("Downloading bWAPP repository from GitHub...");
            using (WebClient wc = new WebClient())
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                wc.DownloadFile(zipUrl, tempZip);
            }

            logCallback($"Extracting files to {targetDir}...");
            if (Directory.Exists(targetDir)) Directory.Delete(targetDir, true);

            string extractTemp = Path.Combine(Path.GetTempPath(), "bwapp_extract");
            if (Directory.Exists(extractTemp)) Directory.Delete(extractTemp, true);

            ZipFile.ExtractToDirectory(tempZip, extractTemp);

            string extractedFolder = Path.Combine(extractTemp, "bWAPP-main");
            if (!Directory.Exists(extractedFolder))
            {
                extractedFolder = Directory.GetDirectories(extractTemp)[0];
            }

            CopyDirectory(extractedFolder, targetDir);

            // Create bWAPP database
            logCallback("Creating MariaDB 'bWAPP' database...");
            using (var conn = new MySqlConnection(StorageMetricsHelper.GetConnectionString()))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("CREATE DATABASE IF NOT EXISTS `bWAPP`;", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }

            File.Delete(tempZip);
            if (Directory.Exists(extractTemp)) Directory.Delete(extractTemp, true);

            logCallback("bWAPP Installed Successfully! Access at: http://localhost/bwapp/install.php");
        }

        public static void InstallXvwa(Action<string> logCallback)
        {
            throw new InvalidOperationException("XVWA repository has been archived by the owner and is unavailable.");
        }

        private static void CopyDirectory(string sourceDir, string destinationDir)
        {
            Directory.CreateDirectory(destinationDir);

            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string targetFilePath = Path.Combine(destinationDir, Path.GetFileName(file));
                File.Copy(file, targetFilePath, true);
            }

            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string newDestinationDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
                CopyDirectory(subDir, newDestinationDir);
            }
        }
    }
}