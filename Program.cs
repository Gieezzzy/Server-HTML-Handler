using System;
using System.IO;
using System.Windows.Forms;
using Server_HTML_Handler.View;

namespace Server_HTML_Handler
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
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

                // Background duties that must not depend on which screen is open (or whether the window is shown, minimized
                // or hidden in the tray): keep Apache working for the web, and keep the notification watcher running.
                StartBackgroundServices();

                Application.Run(new LoginForm());
            }
            catch (Exception ex)
            {
                LogCrash(ex);
            }
        }

        private static void StartBackgroundServices()
        {
            try { Services.ApacheWatchdogService.Instance.Start(); }
            catch (Exception ex) { LogCrash(ex); }

            try { Services.NotificationDispatcherService.Instance.StartWatcher(); }
            catch (Exception ex) { LogCrash(ex); }
        }

        private static void LogCrash(Exception ex)
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
