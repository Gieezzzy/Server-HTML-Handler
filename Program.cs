using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Server_HTML_Handler.Services;

namespace Server_HTML_Handler
{
    internal static class Program
    {
        private static Mutex _singleInstanceMutex;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            // 1. Single-instance enforcement:
            // If already running in tray, signal it to open dashboard, then exit immediately.
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
