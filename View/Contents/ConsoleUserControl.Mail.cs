using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Server_HTML_Handler.Services;

namespace Server_HTML_Handler.View.Contents
{
    public partial class ConsoleUserControl
    {
        #region Email (Gmail / Outlook / SMTP) Commands

        private void HandleMailCommand(string rawInput)
        {
            var args = TokenizeQuoted(rawInput);
            var options = ExtractOptions(args, "name", "reply-to", "user", "client-id", "tenant", "cc");
            bool html = RemoveFlag(args, "--html");
            bool noTls = RemoveFlag(args, "--no-tls");
            string sub = args.Count > 1 ? args[1].ToLower() : "help";

            switch (sub)
            {
                case "setup":
                    MailSetup(args, options, noTls);
                    break;
                case "test":
                    MailTest(args);
                    break;
                case "send":
                    MailSend(args, options, html);
                    break;
                case "status":
                    MailStatus();
                    break;
                case "queue":
                    MailQueue(args.Count > 2 && int.TryParse(args[2], out int n) ? Math.Min(Math.Max(n, 1), 100) : 15);
                    break;
                case "retry":
                    MailRetry(args);
                    break;
                case "run":
                    RunMailBackground("Delivering queued emails now...", () =>
                    {
                        int sent = EmailOutboxService.Instance.ProcessQueue();
                        AppendConsole($"[OUTBOX] {sent} email(s) sent.", sent > 0 ? Color.Lime : Color.Gray);
                        if (!string.IsNullOrEmpty(EmailOutboxService.Instance.LastError))
                            AppendConsole("Last error: " + EmailOutboxService.Instance.LastError, Color.Orange);
                    });
                    break;
                case "remove":
                    MailService.Clear();
                    AppendConsole("[MAIL] Saved email account removed. Queued emails will wait until a new account is set up.", Color.Yellow);
                    break;
                default:
                    PrintMailGuide();
                    break;
            }
        }

        private void MailSetup(List<string> args, Dictionary<string, string> opt, bool noTls)
        {
            string provider = args.Count > 2 ? args[2].ToLower() : "";
            var s = new MailSettings
            {
                Provider = provider,
                FromName = opt.TryGetValue("name", out string name) ? name : "NEXUS-L",
                ReplyTo = opt.TryGetValue("reply-to", out string replyTo) ? replyTo : "",
                OutboxDatabase = MailService.Load().OutboxDatabase
            };
            if (!string.IsNullOrEmpty(s.ReplyTo) && !MailService.IsValidAddress(s.ReplyTo))
            {
                AppendConsole($"[MAIL] Invalid --reply-to address: {s.ReplyTo}", Color.Red);
                return;
            }

            switch (provider)
            {
                case "gmail":
                    if (args.Count < 4 || !MailService.IsValidAddress(args[3]))
                    {
                        AppendConsole("Usage: mail setup gmail <you@gmail.com> [--name \"NEXUS-L\"] [--reply-to team@company.com]", Color.Orange);
                        return;
                    }
                    s.FromAddress = args[3];
                    s.SmtpHost = "smtp.gmail.com";
                    s.SmtpPort = 587;
                    s.SmtpStartTls = true;
                    s.SmtpUser = args[3];
                    if (!AskSecret(s, $"Gmail App Password for {args[3]}", "16-letter App Password (Google Account > Security > App passwords):")) return;
                    break;

                case "smtp":
                    if (args.Count < 6 || !int.TryParse(args[4], out int port) || !MailService.IsValidAddress(args[5]))
                    {
                        AppendConsole("Usage: mail setup smtp <host> <port> <from@address> [--user login] [--no-tls] [--name \"NEXUS-L\"] [--reply-to x]", Color.Orange);
                        return;
                    }
                    s.SmtpHost = args[3];
                    s.SmtpPort = port;
                    s.SmtpStartTls = !noTls;
                    s.FromAddress = args[5];
                    s.SmtpUser = opt.TryGetValue("user", out string user) ? user : "";
                    if (s.SmtpUser.Length > 0 && !AskSecret(s, $"SMTP password for {s.SmtpUser}", "Password:")) return;
                    break;

                case "outlook":
                    if (args.Count < 4 || !MailService.IsValidAddress(args[3]) || !opt.TryGetValue("client-id", out string clientId))
                    {
                        AppendConsole("Usage: mail setup outlook <you@company.com> --client-id <Azure app id> [--tenant <tenant id or domain>] [--name \"NEXUS-L\"] [--reply-to x]", Color.Orange);
                        AppendConsole("  The client id comes from an Azure app registration with 'Allow public client flows' on and the delegated Mail.Send permission.", Color.Gray);
                        return;
                    }
                    s.FromAddress = args[3];
                    s.OutlookClientId = clientId;
                    s.OutlookTenant = opt.TryGetValue("tenant", out string tenant) ? tenant : "common";
                    AppendConsole("[MAIL] Starting Microsoft sign-in...", Color.Cyan);
                    Task.Run(async () =>
                    {
                        try
                        {
                            string err = await MailService.SignInOutlookAsync(s, msg => AppendConsole("  " + msg, Color.Yellow), CancellationToken.None);
                            if (err != null)
                            {
                                AppendConsole("[MAIL] " + err, Color.Red);
                                return;
                            }
                            MailService.Save(s);
                            AppendConsole($"[SUCCESS] Signed in as {s.OutlookAccount}. Default sender: {s.FromName} <{s.FromAddress}>", Color.Lime);
                            AfterSetup();
                        }
                        catch (Exception ex)
                        {
                            AppendConsole("[MAIL] Sign-in failed: " + ex.Message, Color.Red);
                        }
                    });
                    return;

                default:
                    AppendConsole("Usage: mail setup gmail|outlook|smtp ...   (type 'mail help' for details)", Color.Orange);
                    return;
            }

            MailService.Save(s);
            AppendConsole($"[SUCCESS] Default sender saved: {s.FromName} <{s.FromAddress}> via {s.SmtpHost}:{s.SmtpPort}{(s.SmtpStartTls ? " (TLS)" : "")}", Color.Lime);
            AfterSetup();
        }

        private void AfterSetup()
        {
            AppendConsole("Next: mail test <your@email>   to send a real test email.", Color.LightBlue);
            try { EmailOutboxService.Instance.Start(); } catch { }
        }

        private bool AskSecret(MailSettings s, string title, string prompt)
        {
            string secret = PromptSecret(title, prompt);
            if (string.IsNullOrEmpty(secret))
            {
                AppendConsole("[MAIL] Setup cancelled (no password entered).", Color.Orange);
                return false;
            }
            // App Passwords are shown in groups of 4 with spaces; Google accepts them without
            if (s.SmtpHost == "smtp.gmail.com") secret = secret.Replace(" ", "");
            s.SmtpPasswordEncrypted = AesEncryptionHelper.Encrypt(secret);
            return true;
        }

        private void MailTest(List<string> args)
        {
            if (args.Count < 3)
            {
                AppendConsole("Usage: mail test <to_email>                 (sends with the saved account)", Color.Orange);
                AppendConsole("       mail test <to_email> <host> [port]   (raw test: no login, no TLS)", Color.Orange);
                return;
            }

            string to = args[2];
            if (args.Count > 3)
            {
                RawSmtpTest(to, args[3], args.Count > 4 && int.TryParse(args[4], out int p) ? p : 25);
                return;
            }

            var s = MailService.Load();
            if (!s.IsConfigured)
            {
                AppendConsole("[MAIL] No account set up yet. Run 'mail setup gmail|outlook|smtp ...' first.", Color.Orange);
                return;
            }

            string subject = $"{s.FromName} email test";
            string body = "<p>This is a test email from <b>Server HTML Handler</b>.</p>"
                        + $"<p>Sender: {System.Net.WebUtility.HtmlEncode(s.FromName)} &lt;{s.FromAddress}&gt;<br>Sent: {DateTime.Now:yyyy-MM-dd HH:mm:ss}<br>Machine: {Environment.MachineName}</p>"
                        + "<p>If you can read this, email sending works.</p>";
            SendNowAndRecord(to, subject, body, true, $"Sending test email to {to} as {s.FromName} <{s.FromAddress}> via {Describe(s)}...");
        }

        private void MailSend(List<string> args, Dictionary<string, string> opt, bool html)
        {
            if (args.Count < 5)
            {
                AppendConsole("Usage: mail send <to[,to2]> \"<subject>\" \"<body>\" [--cc x@y.com] [--html]", Color.Orange);
                return;
            }
            if (opt.ContainsKey("cc"))
            {
                // CC goes through the outbox so it is retried like web emails
                RunMailBackground($"Queuing email to {args[2]} (cc {opt["cc"]})...", () =>
                {
                    long id = EmailOutboxService.Enqueue("CLI", args[2], opt["cc"], args[3], args[4], html);
                    EmailOutboxService.Instance.ProcessQueue();
                    MailQueue(1);
                    AppendConsole($"Queued as #{id}. Check with: mail queue", Color.LightBlue);
                });
                return;
            }
            SendNowAndRecord(args[2], args[3], args[4], html, $"Sending to {args[2]}...");
        }

        private void SendNowAndRecord(string to, string subject, string body, bool html, string startMessage)
        {
            RunMailBackground(startMessage, () =>
            {
                string error = MailService.Send(to, null, subject, body, html);
                if (error == null) AppendConsole($"[SUCCESS] Email sent to {to}. Check the inbox (and the spam/junk folder).", Color.Lime);
                else AppendConsole("[FAILED] " + error, Color.Red);

                try { EmailOutboxService.RecordDirect(to, subject, body, html, error); }
                catch (Exception ex) { AppendConsole("(Not recorded in tbl_email_outbox: " + ex.Message + ")", Color.Gray); }
            });
        }

        private void MailStatus()
        {
            var s = MailService.Load();
            AppendConsole("=== EMAIL STATUS ===", Color.Cyan);
            if (s.IsConfigured)
            {
                AppendConsole($"  Default sender : {s.FromName} <{s.FromAddress}>", Color.White);
                AppendConsole($"  Sends via      : {Describe(s)}", Color.White);
                AppendConsole($"  Reply-To       : {(string.IsNullOrEmpty(s.ReplyTo) ? "(none)" : s.ReplyTo)}", Color.White);
                if (s.Provider == "outlook") AppendConsole($"  Signed in as   : {s.OutlookAccount}", Color.White);
            }
            else
            {
                AppendConsole("  No email account set up. Run 'mail setup gmail|outlook|smtp ...'.", Color.Orange);
            }

            var outbox = EmailOutboxService.Instance;
            AppendConsole($"  Outbox sender  : {(outbox.IsRunning ? "running" : "stopped")}, checks every {outbox.PollSeconds}s, last check {(outbox.LastRun?.ToString("HH:mm:ss") ?? "never")}", Color.White);
            RunMailBackground(null, () =>
            {
                var counts = EmailOutboxService.Counts();
                int Get(string k) => counts.TryGetValue(k, out int v) ? v : 0;
                AppendConsole($"  Outbox ({s.OutboxDatabase}.tbl_email_outbox): queued {Get("queued")}, sending {Get("sending")}, sent {Get("sent")}, failed {Get("failed")}", Color.White);
                if (!string.IsNullOrEmpty(outbox.LastError)) AppendConsole("  Last error     : " + outbox.LastError, Color.Orange);
            });
        }

        private void MailQueue(int count)
        {
            RunMailBackground(null, () =>
            {
                var rows = EmailOutboxService.Recent(count);
                if (rows.Count == 0)
                {
                    AppendConsole("[OUTBOX] Empty.", Color.Gray);
                    return;
                }
                AppendConsole($"{"#",-6} {"Status",-8} {"Try",-3} {"Source",-8} {"Created",-16} {"To",-28} Subject", Color.Cyan);
                foreach (var r in rows)
                {
                    Color c = r.Status == "sent" ? Color.Lime : r.Status == "failed" ? Color.Red : Color.Yellow;
                    AppendConsole($"{r.Id,-6} {r.Status,-8} {r.Attempts,-3} {Cut(r.Source, 8),-8} {r.CreatedAt:MM-dd HH:mm:ss}  {Cut(r.To, 28),-28} {Cut(r.Subject, 50)}", c);
                    if (r.Status != "sent" && !string.IsNullOrEmpty(r.LastError)) AppendConsole("       ! " + Cut(r.LastError, 160), Color.Orange);
                }
            });
        }

        private void MailRetry(List<string> args)
        {
            if (args.Count < 3 || !long.TryParse(args[2], out long id))
            {
                AppendConsole("Usage: mail retry <id>   (ids are listed by 'mail queue')", Color.Orange);
                return;
            }
            RunMailBackground(null, () =>
            {
                if (!EmailOutboxService.Retry(id))
                {
                    AppendConsole($"[OUTBOX] #{id} not found or already sent.", Color.Orange);
                    return;
                }
                AppendConsole($"[OUTBOX] #{id} queued again. Sending...", Color.Cyan);
                EmailOutboxService.Instance.ProcessQueue();
                MailQueue(5);
            });
        }

        private void RawSmtpTest(string to, string host, int port)
        {
            RunMailBackground($"Testing SMTP server at {host}:{port} (no login, no TLS)...", () =>
            {
                try
                {
                    using (var message = new MailMessage("noreply@localhost", to))
                    {
                        message.Subject = "XAMPP Console Service Diagnostic Test";
                        message.Body = $"This is an automated test email sent from Server HTML Handler Console at {DateTime.Now}.";
                        using (var client = new SmtpClient(host, port) { Timeout = 10000 })
                        {
                            client.Send(message);
                        }
                    }
                    AppendConsole($"Test email successfully delivered to '{to}'.", Color.Lime);
                }
                catch (Exception ex)
                {
                    AppendConsole("SMTP Mail Diagnostic Failed: " + MailService.Describe(ex), Color.Red);
                }
            });
        }

        private void RunMailBackground(string startMessage, Action work)
        {
            if (!string.IsNullOrEmpty(startMessage)) AppendConsole(startMessage, Color.Cyan);
            Task.Run(() =>
            {
                try { work(); }
                catch (Exception ex) { AppendConsole("[MAIL] " + ex.Message, Color.Red); }
            });
        }

        private static string Describe(MailSettings s) =>
            s.Provider == "outlook"
                ? $"Microsoft Graph (Outlook sign-in, tenant {s.OutlookTenant})"
                : $"{s.SmtpHost}:{s.SmtpPort}{(s.SmtpStartTls ? " TLS" : "")}{(string.IsNullOrEmpty(s.SmtpUser) ? ", no login" : ", login " + s.SmtpUser)}";

        private static string Cut(string text, int max)
        {
            text = (text ?? "").Replace("\r", " ").Replace("\n", " ");
            return text.Length <= max ? text : text.Substring(0, max - 1) + "…";
        }

        /// <summary>Splits a command line on spaces, keeping "quoted text" together.</summary>
        private static List<string> TokenizeQuoted(string input)
        {
            var tokens = new List<string>();
            var current = new StringBuilder();
            bool inQuotes = false, hasToken = false;
            foreach (char c in input ?? "")
            {
                if (c == '"') { inQuotes = !inQuotes; hasToken = true; continue; }
                if (char.IsWhiteSpace(c) && !inQuotes)
                {
                    if (hasToken) { tokens.Add(current.ToString()); current.Clear(); hasToken = false; }
                    continue;
                }
                current.Append(c);
                hasToken = true;
            }
            if (hasToken) tokens.Add(current.ToString());
            return tokens;
        }

        /// <summary>Removes "--key value" pairs from the arguments and returns them.</summary>
        private static Dictionary<string, string> ExtractOptions(List<string> args, params string[] keys)
        {
            var found = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            for (int i = 0; i < args.Count - 1; i++)
            {
                string key = args[i].StartsWith("--") ? args[i].Substring(2) : null;
                if (key == null || !keys.Contains(key, StringComparer.OrdinalIgnoreCase)) continue;
                found[key] = args[i + 1];
                args.RemoveRange(i, 2);
                i--;
            }
            return found;
        }

        private static bool RemoveFlag(List<string> args, string flag)
        {
            int index = args.FindIndex(a => string.Equals(a, flag, StringComparison.OrdinalIgnoreCase));
            if (index < 0) return false;
            args.RemoveAt(index);
            return true;
        }

        /// <summary>Masked password box, so secrets never appear in the console history.</summary>
        private string PromptSecret(string title, string prompt)
        {
            using (var form = new Form())
            {
                form.Text = title;
                form.FormBorderStyle = FormBorderStyle.FixedDialog;
                form.StartPosition = FormStartPosition.CenterParent;
                form.MinimizeBox = form.MaximizeBox = false;
                form.ClientSize = new Size(420, 120);

                var label = new Label { Text = prompt, Left = 12, Top = 12, Width = 396, Height = 32 };
                var box = new TextBox { Left = 12, Top = 48, Width = 396, UseSystemPasswordChar = true };
                var ok = new Button { Text = "Save", Left = 252, Top = 82, Width = 75, DialogResult = DialogResult.OK };
                var cancel = new Button { Text = "Cancel", Left = 333, Top = 82, Width = 75, DialogResult = DialogResult.Cancel };
                form.Controls.AddRange(new Control[] { label, box, ok, cancel });
                form.AcceptButton = ok;
                form.CancelButton = cancel;

                return form.ShowDialog(FindForm()) == DialogResult.OK ? box.Text : null;
            }
        }

        private void PrintMailGuide()
        {
            AppendConsole("=================== EMAIL COMMANDS ===================", Color.Cyan);
            AppendConsole(" SET UP THE DEFAULT SENDER (one account for console + NEXUS-L):", Color.White);
            AppendConsole("   mail setup gmail <you@gmail.com> [--name \"NEXUS-L\"] [--reply-to team@company.com]", Color.Lime);
            AppendConsole("   mail setup outlook <you@company.com> --client-id <Azure app id> [--tenant <tenant>] [--name ...] [--reply-to ...]", Color.Lime);
            AppendConsole("   mail setup smtp <host> <port> <from@address> [--user login] [--no-tls] [--name ...] [--reply-to ...]", Color.Lime);
            AppendConsole(" TEST AND SEND:", Color.White);
            AppendConsole("   mail status                          Account, outbox sender and queue counts", Color.Lime);
            AppendConsole("   mail test <to>                       Send a test email with the saved account", Color.Lime);
            AppendConsole("   mail test <to> <host> [port]         Raw test without login/TLS (old behaviour)", Color.Lime);
            AppendConsole("   mail send <to> \"<subject>\" \"<body>\" [--cc x] [--html]", Color.Lime);
            AppendConsole(" OUTBOX (emails queued by NEXUS-L in tbl_email_outbox):", Color.White);
            AppendConsole("   mail queue [n]                       Last n emails with status (default 15)", Color.Lime);
            AppendConsole("   mail retry <id>                      Send a failed email again", Color.Lime);
            AppendConsole("   mail run                             Deliver queued emails now", Color.Lime);
            AppendConsole("   mail remove                          Forget the saved account", Color.Lime);
            AppendConsole("======================================================", Color.Cyan);
        }

        #endregion
    }
}
