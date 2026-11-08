using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Server_HTML_Handler.Services
{
    /// <summary>
    /// The one email account this server sends from (console "mail" commands and the NEXUS-L outbox).
    /// Passwords and tokens are stored DPAPI-encrypted for the current Windows user.
    /// </summary>
    public class MailSettings
    {
        public string Provider { get; set; } = "";        // gmail | smtp | outlook
        public string FromAddress { get; set; } = "";
        public string FromName { get; set; } = "NEXUS-L";
        public string ReplyTo { get; set; } = "";

        public string SmtpHost { get; set; } = "";
        public int SmtpPort { get; set; } = 587;
        public bool SmtpStartTls { get; set; } = true;
        public string SmtpUser { get; set; } = "";
        public string SmtpPasswordEncrypted { get; set; } = "";

        public string OutlookClientId { get; set; } = "";
        public string OutlookTenant { get; set; } = "common";
        public string OutlookAccount { get; set; } = "";
        public string OutlookRefreshTokenEncrypted { get; set; } = "";

        /// <summary>MySQL database holding tbl_email_outbox (NEXUS-L's database).</summary>
        public string OutboxDatabase { get; set; } = "dbnexus";

        [JsonIgnore]
        public bool IsConfigured => !string.IsNullOrWhiteSpace(Provider) && !string.IsNullOrWhiteSpace(FromAddress);
    }

    public static class MailService
    {
        private const string GraphScope = "https://graph.microsoft.com/Mail.Send offline_access User.Read";
        private static readonly object SettingsLock = new object();
        private static readonly HttpClient Http = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };

        private static string SettingsPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ServerHTMLHandler", "mail-settings.json");

        public static MailSettings Load()
        {
            lock (SettingsLock)
            {
                try
                {
                    if (File.Exists(SettingsPath))
                        return JsonConvert.DeserializeObject<MailSettings>(File.ReadAllText(SettingsPath)) ?? new MailSettings();
                }
                catch { }
                return new MailSettings();
            }
        }

        public static void Save(MailSettings settings)
        {
            lock (SettingsLock)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath));
                File.WriteAllText(SettingsPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
            }
        }

        public static void Clear()
        {
            lock (SettingsLock)
            {
                if (File.Exists(SettingsPath)) File.Delete(SettingsPath);
            }
        }

        public static List<string> SplitAddresses(string list)
        {
            return (list ?? "").Split(new[] { ',', ';', ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(a => a.Trim()).Where(a => a.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        public static bool IsValidAddress(string address)
        {
            try { return new MailAddress(address).Address == address; }
            catch { return false; }
        }

        /// <summary>Sends one email with the configured account. Returns null on success, otherwise the error text.</summary>
        public static string Send(string to, string cc, string subject, string body, bool isHtml)
        {
            var settings = Load();
            if (!settings.IsConfigured) return "No email account is set up. Run 'mail setup gmail|outlook|smtp ...' first.";

            var toList = SplitAddresses(to);
            var ccList = SplitAddresses(cc);
            var bad = toList.Concat(ccList).Where(a => !IsValidAddress(a)).ToList();
            if (toList.Count == 0) return "No recipient address.";
            if (bad.Count > 0) return "Invalid address: " + string.Join(", ", bad);

            try
            {
                // pictures written into the HTML as data:image/...;base64 go out as real inline images (most mail programs show no data: pictures)
                var inline = isHtml ? ExtractInlineImages(ref body) : new List<InlineImage>();
                return settings.Provider == "outlook"
                    ? SendViaGraph(settings, toList, ccList, subject, body, isHtml, inline)
                    : SendViaSmtp(settings, toList, ccList, subject, body, isHtml, inline);
            }
            catch (Exception ex)
            {
                return Describe(ex);
            }
        }

        private class InlineImage { public string Cid; public string Mime; public byte[] Bytes; }

        /// <summary>Replaces every src="data:image/png;base64,..." of an HTML body by src="cid:..." and returns the pictures.</summary>
        private static List<InlineImage> ExtractInlineImages(ref string body)
        {
            var list = new List<InlineImage>();
            body = Regex.Replace(body ?? "", "src=\"data:(image/(?:png|jpeg|gif));base64,([A-Za-z0-9+/=]+)\"", m =>
            {
                var img = new InlineImage { Cid = "img" + (list.Count + 1) + "@nexus-l", Mime = m.Groups[1].Value, Bytes = Convert.FromBase64String(m.Groups[2].Value) };
                list.Add(img);
                return "src=\"cid:" + img.Cid + "\"";
            }, RegexOptions.IgnoreCase);
            return list;
        }

        private static string SendViaSmtp(MailSettings s, List<string> to, List<string> cc, string subject, string body, bool isHtml, List<InlineImage> inline = null)
        {
            using (var message = new MailMessage())
            {
                message.From = new MailAddress(s.FromAddress, s.FromName);
                foreach (var a in to) message.To.Add(a);
                foreach (var a in cc) message.CC.Add(a);
                if (!string.IsNullOrWhiteSpace(s.ReplyTo)) message.ReplyToList.Add(s.ReplyTo);
                message.Subject = subject;
                message.SubjectEncoding = Encoding.UTF8;
                message.BodyEncoding = Encoding.UTF8;
                if (isHtml && inline != null && inline.Count > 0)
                {
                    var view = AlternateView.CreateAlternateViewFromString(body, Encoding.UTF8, "text/html");
                    foreach (var img in inline)
                        view.LinkedResources.Add(new LinkedResource(new MemoryStream(img.Bytes), img.Mime) { ContentId = img.Cid, TransferEncoding = System.Net.Mime.TransferEncoding.Base64 });
                    message.AlternateViews.Add(view);
                }
                else
                {
                    message.Body = body;
                    message.IsBodyHtml = isHtml;
                }

                using (var client = new SmtpClient(s.SmtpHost, s.SmtpPort))
                {
                    client.EnableSsl = s.SmtpStartTls;
                    client.Timeout = 20000;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    if (!string.IsNullOrWhiteSpace(s.SmtpUser))
                        client.Credentials = new NetworkCredential(s.SmtpUser, AesEncryptionHelper.Decrypt(s.SmtpPasswordEncrypted));
                    client.Send(message);
                }
            }
            return null;
        }

        private static string SendViaGraph(MailSettings s, List<string> to, List<string> cc, string subject, string body, bool isHtml, List<InlineImage> inline = null)
        {
            string token = GetGraphAccessToken(s, out string tokenError);
            if (token == null) return tokenError;

            JArray Recipients(IEnumerable<string> list) =>
                new JArray(list.Select(a => new JObject { ["emailAddress"] = new JObject { ["address"] = a } }));

            var message = new JObject
            {
                ["subject"] = subject,
                ["body"] = new JObject { ["contentType"] = isHtml ? "HTML" : "Text", ["content"] = body },
                ["toRecipients"] = Recipients(to)
            };
            if (inline != null && inline.Count > 0)
                message["attachments"] = new JArray(inline.Select(i => new JObject
                {
                    ["@odata.type"] = "#microsoft.graph.fileAttachment", ["name"] = i.Cid.Split('@')[0] + (i.Mime.EndsWith("png") ? ".png" : ".img"),
                    ["contentType"] = i.Mime, ["contentBytes"] = Convert.ToBase64String(i.Bytes), ["isInline"] = true, ["contentId"] = i.Cid
                }));
            if (cc.Count > 0) message["ccRecipients"] = Recipients(cc);
            if (!string.IsNullOrWhiteSpace(s.ReplyTo)) message["replyTo"] = Recipients(new[] { s.ReplyTo });
            // Sending as another mailbox (e.g. a shared noreply@) needs "Send As" permission on it
            if (!string.Equals(s.FromAddress, s.OutlookAccount, StringComparison.OrdinalIgnoreCase))
                message["from"] = new JObject { ["emailAddress"] = new JObject { ["address"] = s.FromAddress, ["name"] = s.FromName } };

            var payload = new JObject { ["message"] = message, ["saveToSentItems"] = true };
            using (var request = new HttpRequestMessage(HttpMethod.Post, "https://graph.microsoft.com/v1.0/me/sendMail"))
            {
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                request.Content = new StringContent(payload.ToString(), Encoding.UTF8, "application/json");
                var response = Http.SendAsync(request).GetAwaiter().GetResult();
                if (response.IsSuccessStatusCode) return null;
                string err = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                return $"Microsoft Graph rejected the email ({(int)response.StatusCode}): {GraphErrorMessage(err)}";
            }
        }

        private static string TokenEndpoint(MailSettings s) =>
            $"https://login.microsoftonline.com/{Uri.EscapeDataString(s.OutlookTenant)}/oauth2/v2.0/token";

        private static string GetGraphAccessToken(MailSettings s, out string error)
        {
            error = null;
            string refresh = AesEncryptionHelper.Decrypt(s.OutlookRefreshTokenEncrypted);
            if (string.IsNullOrEmpty(refresh))
            {
                error = "Outlook sign-in is missing or expired. Run 'mail setup outlook ...' again.";
                return null;
            }

            var form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["client_id"] = s.OutlookClientId,
                ["grant_type"] = "refresh_token",
                ["refresh_token"] = refresh,
                ["scope"] = GraphScope
            });
            var response = Http.PostAsync(TokenEndpoint(s), form).GetAwaiter().GetResult();
            var json = JObject.Parse(response.Content.ReadAsStringAsync().GetAwaiter().GetResult());
            if (!response.IsSuccessStatusCode)
            {
                error = "Outlook sign-in could not be renewed: " + (string)json["error_description"] + " Run 'mail setup outlook ...' again.";
                return null;
            }

            // Microsoft rotates refresh tokens: keep the newest one
            string newRefresh = (string)json["refresh_token"];
            if (!string.IsNullOrEmpty(newRefresh) && newRefresh != refresh)
            {
                var current = Load();
                current.OutlookRefreshTokenEncrypted = AesEncryptionHelper.Encrypt(newRefresh);
                Save(current);
            }
            return (string)json["access_token"];
        }

        /// <summary>
        /// Microsoft device-code sign-in: shows a code to enter at microsoft.com/devicelogin, then waits for the user.
        /// On success stores the refresh token and the signed-in account in <paramref name="settings"/> (caller saves).
        /// </summary>
        public static async Task<string> SignInOutlookAsync(MailSettings settings, Action<string> show, CancellationToken cancel)
        {
            var start = await Http.PostAsync(
                $"https://login.microsoftonline.com/{Uri.EscapeDataString(settings.OutlookTenant)}/oauth2/v2.0/devicecode",
                new FormUrlEncodedContent(new Dictionary<string, string> { ["client_id"] = settings.OutlookClientId, ["scope"] = GraphScope }));
            var startJson = JObject.Parse(await start.Content.ReadAsStringAsync());
            if (!start.IsSuccessStatusCode) return "Microsoft refused the sign-in request: " + (string)startJson["error_description"];

            show((string)startJson["message"]);
            int interval = Math.Max(5, (int?)startJson["interval"] ?? 5);
            DateTime expires = DateTime.Now.AddSeconds((int?)startJson["expires_in"] ?? 900);

            while (DateTime.Now < expires)
            {
                await Task.Delay(TimeSpan.FromSeconds(interval), cancel);
                var poll = await Http.PostAsync(TokenEndpoint(settings), new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["client_id"] = settings.OutlookClientId,
                    ["grant_type"] = "urn:ietf:params:oauth:grant-type:device_code",
                    ["device_code"] = (string)startJson["device_code"]
                }));
                var json = JObject.Parse(await poll.Content.ReadAsStringAsync());
                if (poll.IsSuccessStatusCode)
                {
                    settings.OutlookRefreshTokenEncrypted = AesEncryptionHelper.Encrypt((string)json["refresh_token"]);
                    settings.OutlookAccount = await GetSignedInAddressAsync((string)json["access_token"]) ?? settings.FromAddress;
                    return null;
                }

                string code = (string)json["error"];
                if (code == "authorization_pending") continue;
                if (code == "slow_down") { interval += 5; continue; }
                return "Sign-in failed: " + (string)json["error_description"];
            }
            return "Sign-in timed out. Run the setup command again.";
        }

        private static async Task<string> GetSignedInAddressAsync(string accessToken)
        {
            try
            {
                using (var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.microsoft.com/v1.0/me?$select=mail,userPrincipalName"))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                    var response = await Http.SendAsync(request);
                    if (!response.IsSuccessStatusCode) return null;
                    var me = JObject.Parse(await response.Content.ReadAsStringAsync());
                    return (string)me["mail"] ?? (string)me["userPrincipalName"];
                }
            }
            catch { return null; }
        }

        private static string GraphErrorMessage(string raw)
        {
            try { return (string)JObject.Parse(raw)["error"]?["message"] ?? raw; }
            catch { return raw; }
        }

        /// <summary>Turns SMTP exceptions into a message that says what to fix.</summary>
        public static string Describe(Exception ex)
        {
            string all = ex.Message;
            for (var inner = ex.InnerException; inner != null; inner = inner.InnerException) all += " | " + inner.Message;

            if (all.IndexOf("5.7.0", StringComparison.Ordinal) >= 0 && all.IndexOf("STARTTLS", StringComparison.OrdinalIgnoreCase) >= 0)
                return all + " -> Use port 587 with TLS on.";
            if (all.IndexOf("5.7.8", StringComparison.Ordinal) >= 0 || all.IndexOf("Username and Password not accepted", StringComparison.OrdinalIgnoreCase) >= 0)
                return all + " -> Wrong address or password. Gmail needs an App Password (2-Step Verification on), not the normal password.";
            if (all.IndexOf("5.7.139", StringComparison.Ordinal) >= 0 || all.IndexOf("SmtpClientAuthentication is disabled", StringComparison.OrdinalIgnoreCase) >= 0)
                return all + " -> This Microsoft account does not allow password SMTP. Use 'mail setup outlook' (Microsoft sign-in) instead.";
            if (all.IndexOf("actively refused", StringComparison.OrdinalIgnoreCase) >= 0 || all.IndexOf("timed out", StringComparison.OrdinalIgnoreCase) >= 0 || all.IndexOf("Unable to connect", StringComparison.OrdinalIgnoreCase) >= 0)
                return all + " -> The mail server could not be reached. Check the host/port and that the firewall allows it.";
            return all;
        }
    }
}
