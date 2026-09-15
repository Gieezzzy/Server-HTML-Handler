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
        public LoginForm()
        {
            InitializeComponent();
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

        
            string hashedInputPassword;
            using (System.Security.Cryptography.SHA256 sha256 = System.Security.Cryptography.SHA256.Create())
            {
                byte[] bytes = System.Text.Encoding.UTF8.GetBytes(password);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                foreach (byte b in hashBytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                hashedInputPassword = sb.ToString();
            }

            // 3. Fetch Master credentials from application settings
            string masterUser = Properties.Settings.Default.MasterUser;
            string masterPasswordHash = Properties.Settings.Default.MasterPassword;

            // 4. Validate inserted details against the settings
            if (username == masterUser && hashedInputPassword.Equals(masterPasswordHash, System.StringComparison.OrdinalIgnoreCase))
            {
                System.Windows.Forms.MessageBox.Show("Login Successful!", "Success", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

                // Open the Dashboard Panel and hide the Login form
                Server_HTML_Handler.View.Panel.DashboardPanel dashboard = new Server_HTML_Handler.View.Panel.DashboardPanel();
                dashboard.Show();
                this.Hide();
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
