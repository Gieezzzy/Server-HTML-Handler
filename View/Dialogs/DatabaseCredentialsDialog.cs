using System;
using System.Drawing;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server_HTML_Handler.View.Dialogs
{
    public partial class DatabaseCredentialsDialog : Form
    {
        public DatabaseCredentialsDialog(string customMessage = null)
        {
            InitializeComponent();
            LoadCurrentSettings();

            if (!string.IsNullOrWhiteSpace(customMessage))
            {
                lblStatus.ForeColor = Color.FromArgb(241, 196, 15);
                lblStatus.Text = customMessage;
            }
        }

        private void LoadCurrentSettings()
        {
            string host = Properties.Settings.Default.DbHost;
            txtHost.Text = string.IsNullOrWhiteSpace(host) ? "localhost" : host;

            string port = Properties.Settings.Default.DbPort;
            txtPort.Text = string.IsNullOrWhiteSpace(port) ? "3306" : port;

            string user = Properties.Settings.Default.MasterUser;
            txtUsername.Text = string.IsNullOrWhiteSpace(user) ? "master" : user;

            try
            {
                string encryptedPass = Properties.Settings.Default.DBmaster;
                txtPassword.Text = AesEncryptionHelper.Decrypt(encryptedPass);
            }
            catch
            {
                txtPassword.Text = string.Empty;
            }
        }

        private void ChkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        private async void BtnTestConnection_Click(object sender, EventArgs e)
        {
            string host = txtHost.Text.Trim();
            if (!uint.TryParse(txtPort.Text.Trim(), out uint port))
            {
                port = 3306;
            }
            string user = txtUsername.Text.Trim();
            string pass = txtPassword.Text;

            btnTestConnection.Enabled = false;
            btnSave.Enabled = false;
            lblStatus.ForeColor = Color.FromArgb(200, 200, 200);
            lblStatus.Text = "Testing connection to MySQL server...";

            string serverVersion = string.Empty;
            string error = string.Empty;
            bool ok = false;

            await Task.Run(() =>
            {
                ok = Database.dbBase.TestCustomConnection(host, port, user, pass, out serverVersion, out error);
            });

            btnTestConnection.Enabled = true;
            btnSave.Enabled = true;

            if (ok)
            {
                lblStatus.ForeColor = Color.FromArgb(46, 204, 113);
                lblStatus.Text = $"✔ Connection successful! Server: {serverVersion}";
            }
            else
            {
                lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
                lblStatus.Text = $"✖ Connection failed: {error}";
            }
        }

        private async void BtnSave_Click(object sender, EventArgs e)
        {
            string host = txtHost.Text.Trim();
            if (string.IsNullOrWhiteSpace(host))
            {
                MessageBox.Show("Please enter the MySQL Server Host (e.g. localhost or 127.0.0.1).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtHost.Focus();
                return;
            }

            if (!uint.TryParse(txtPort.Text.Trim(), out uint port) || port == 0)
            {
                MessageBox.Show("Please enter a valid port number (default is 3306).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtPort.Focus();
                return;
            }

            string user = txtUsername.Text.Trim();
            if (string.IsNullOrWhiteSpace(user))
            {
                MessageBox.Show("Please enter the MySQL Administrator Username (e.g. master or root).", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsername.Focus();
                return;
            }

            string pass = txtPassword.Text;

            // Test connection first
            btnSave.Enabled = false;
            btnTestConnection.Enabled = false;
            lblStatus.ForeColor = Color.FromArgb(200, 200, 200);
            lblStatus.Text = "Verifying connection before saving...";

            string serverVer = string.Empty;
            string error = string.Empty;
            bool testOk = false;

            await Task.Run(() =>
            {
                testOk = Database.dbBase.TestCustomConnection(host, port, user, pass, out serverVer, out error);
            });

            btnSave.Enabled = true;
            btnTestConnection.Enabled = true;

            if (!testOk)
            {
                DialogResult forceSave = MessageBox.Show(
                    $"MySQL authentication failed with these credentials:\n\n{error}\n\nDo you want to save these credentials anyway?",
                    "Connection Test Failed",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);

                if (forceSave != DialogResult.Yes)
                {
                    lblStatus.ForeColor = Color.FromArgb(231, 76, 60);
                    lblStatus.Text = $"✖ Connection failed: {error}";
                    return;
                }
            }

            try
            {
                Properties.Settings.Default.DbHost = host;
                Properties.Settings.Default.DbPort = port.ToString();
                Properties.Settings.Default.MasterUser = user;
                Properties.Settings.Default.DBmaster = AesEncryptionHelper.Encrypt(pass);

                if (chkSyncAppLogin.Checked)
                {
                    // Stored once for every build on this PC (debug and published), not per installed copy.
                    Helper.AppLoginStore.Save(user, Helper.AppLoginStore.Hash(pass));
                }

                Properties.Settings.Default.Save();
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save credentials: {ex.Message}", "Settings Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
