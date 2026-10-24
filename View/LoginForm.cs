using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server_HTML_Handler.View
{
    public partial class LoginForm : System.Windows.Forms.Form
    {
        /// <summary>Set by Logout so the sign-in screen is not skipped again straight away.</summary>
        public static bool SkipAutoLogin;

        public LoginForm()
        {
            InitializeComponent();
            chkSavePassword.Text = "Remember me (sign in automatically next time)";
            chkSavePassword.AutoSize = true;
            this.Shown += LoginForm_Shown;
        }

        private void LoginForm_Shown(object sender, EventArgs e)
        {
            if (SkipAutoLogin) { SkipAutoLogin = false; return; }
            if (string.IsNullOrEmpty(Properties.Settings.Default.HTMLPath)) return; // first run still needs the folder question
            if (Helper.AppLoginStore.TryGetRemembered(out string user, out string hash))
            {
                chkSavePassword.Checked = true;
                OpenDashboard();
            }
        }

        private void OpenDashboard()
        {
            Server_HTML_Handler.View.Panel.DashboardPanel dashboard = new Server_HTML_Handler.View.Panel.DashboardPanel();
            dashboard.Show();
            this.Hide();
        }

        private void btnLogin_Click(object sender, System.EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text.Trim();

            // 1. Check if HTMLPath setting is configured and not null/empty
            string htmlPath = Properties.Settings.Default.HTMLPath;
            if (string.IsNullOrEmpty(htmlPath))
            {
                System.Windows.Forms.MessageBox.Show("HTML Path is not configured. Please select your shared website folder.", "Setup Required", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

                using (System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog())
                {
                    folderDialog.Description = "Select the CodeIgniter Web Hosting Folder";
                    if (folderDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                    {
                        Properties.Settings.Default.HTMLPath = folderDialog.SelectedPath;
                        Properties.Settings.Default.Save();
                        htmlPath = folderDialog.SelectedPath;
                    }
                    else
                    {
                        return;
                    }
                }
            }

        
            // The sign-in is shared by every build on this PC (debug, published, new installs): see AppLoginStore.
            string hashedInputPassword = Helper.AppLoginStore.Hash(password);

            // Validate the inserted details
            if (Helper.AppLoginStore.Verify(username, hashedInputPassword))
            {
                try
                {
                    if (chkSavePassword.Checked) Helper.AppLoginStore.Remember(username, hashedInputPassword);
                    else Helper.AppLoginStore.Forget();
                }
                catch { /* remembering is optional: never block the sign-in */ }

                OpenDashboard();
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("Invalid username or password.", "Access Denied", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
