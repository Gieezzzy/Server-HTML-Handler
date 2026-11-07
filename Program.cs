using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;
using Server_HTML_Handler.Services;

namespace Server_HTML_Handler
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        private const int SW_RESTORE = 9;

        private static Mutex _singleInstanceMutex;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // 1. Process-level single instance check:
            // If another instance of Server HTML Handler is already running, restore its window and exit immediately,
            // unless this is a newer (just updated) version: then the older copy is closed and this one takes over.
            Process current = Process.GetCurrentProcess();
            Process other = Process.GetProcessesByName(current.ProcessName)
                                   .FirstOrDefault(p => p.Id != current.Id && p.SessionId == current.SessionId);
            if (other != null && IsNewerThan(other) && ReplaceOlderInstance(other))
            {
                other = null;
            }
            if (other != null)
            {
                ServerTrayContext.SignalExistingInstanceToOpenDashboard();
                try
                {
                    if (other.MainWindowHandle != IntPtr.Zero)
                    {
                        ShowWindow(other.MainWindowHandle, SW_RESTORE);
                        SetForegroundWindow(other.MainWindowHandle);
                    }
                }
                catch { }
                return;
            }

            // 2. Named Mutex single-instance check:
            string mutexName = @"Local\ServerHTMLHandler_Mutex_" + Environment.UserName;
            _singleInstanceMutex = new Mutex(true, mutexName, out bool createdNew);
            if (!createdNew)
            {
                ServerTrayContext.SignalExistingInstanceToOpenDashboard();
                return;
            }

            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += (s, e) =>
            {
                LogCrash(e.Exception);
            };
            AppDomain.CurrentDomain.UnhandledException += (s, e) =>
            {
                if (e.ExceptionObject is Exception ex) LogCrash(ex);
            };

            try
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // Start directly in system notification tray by default so Apache and background services
                // run immediately without requiring user interaction or login.
                bool startMinimized = true;
                if (args != null && args.Length > 0)
                {
                    if (args.Any(a => string.Equals(a, "/open", StringComparison.OrdinalIgnoreCase) ||
                                      string.Equals(a, "--open", StringComparison.OrdinalIgnoreCase)))
                    {
                        startMinimized = false;
                    }
                }

                // Run the unified ServerTrayContext
                Application.Run(new ServerTrayContext(startMinimized));
            }
            catch (Exception ex)
            {
                LogCrash(ex);
            }
            finally
            {
                if (_singleInstanceMutex != null)
                {
                    try { _singleInstanceMutex.ReleaseMutex(); } catch { }
                    _singleInstanceMutex.Dispose();
                }
            }
        }

        /// <summary>The ClickOnce version of this copy (e.g. 1.0.0.32), or null when not installed through ClickOnce.</summary>
        public static Version AppVersion => ReadDeployedVersion(Application.ExecutablePath);

        /// <summary>Reads the version ClickOnce writes into the .exe.manifest installed next to the exe.</summary>
        private static Version ReadDeployedVersion(string exePath)
        {
            try
            {
                string manifest = exePath + ".manifest";
                if (!File.Exists(manifest)) return null;
                var doc = System.Xml.Linq.XDocument.Load(manifest);
                var identity = doc.Root?.Elements().FirstOrDefault(e => e.Name.LocalName == "assemblyIdentity");
                return Version.TryParse((string)identity?.Attribute("version"), out Version v) ? v : null;
            }
            catch { return null; }
        }

        private static bool IsNewerThan(Process other)
        {
            try
            {
                Version mine = AppVersion;
                Version theirs = ReadDeployedVersion(other.MainModule.FileName);
                return mine != null && theirs != null && mine > theirs;
            }
            catch { return false; }
        }

        /// <summary>Asks the older running copy to exit; copies from before 1.0.0.33 cannot hear that, so they are closed after a wait.</summary>
        private static bool ReplaceOlderInstance(Process older)
        {
            try
            {
                ServerTrayContext.SignalExistingInstanceToExit();
                if (older.WaitForExit(15000)) return true;
                older.Kill();
                return older.WaitForExit(5000);
            }
            catch (Exception ex)
            {
                LogCrash(ex);
                return false;
            }
        }

        public static void LogCrash(Exception ex)
        {
            try
            {
                string logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startup_error.log");
                File.AppendAllText(logFile, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {ex}{Environment.NewLine}");
            }
            catch { }
        }
    }
}
