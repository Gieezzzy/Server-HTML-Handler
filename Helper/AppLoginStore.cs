using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Server_HTML_Handler.Helper
{
    /// <summary>
    /// The sign-in of the handler itself. It used to live in the per-user settings of each installed copy, so the Visual Studio
    /// (debug) build, every published ClickOnce version and every new install each had their own password: one could accept a
    /// password the others rejected. It is now one file shared by every build on this PC
    /// (%ProgramData%\ServerHTMLHandler\app-login.json). Until the password is changed the built-in default applies everywhere.
    /// </summary>
    public static class AppLoginStore
    {
        public const string DefaultUser = "master";
        public const string DefaultPasswordHash = "2c0825dcdff02c460d52c34520c9383179a4213e65aa4d09fe680cb40c81fe8e";

        private sealed class Record
        {
            public string User { get; set; }
            public string PasswordHash { get; set; }
        }

        private static string FilePath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "ServerHTMLHandler", "app-login.json");

        public static string Hash(string password)
        {
            using (var sha = SHA256.Create())
            {
                var sb = new StringBuilder();
                foreach (byte b in sha.ComputeHash(Encoding.UTF8.GetBytes(password ?? "")))
                    sb.Append(b.ToString("x2"));
                return sb.ToString();
            }
        }

        private static Record Load()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    var r = JsonConvert.DeserializeObject<Record>(File.ReadAllText(FilePath));
                    if (r != null && !string.IsNullOrWhiteSpace(r.User) && !string.IsNullOrWhiteSpace(r.PasswordHash)) return r;
                }
            }
            catch
            {
                // unreadable file: fall back to the default rather than locking everybody out
            }
            return new Record { User = DefaultUser, PasswordHash = DefaultPasswordHash };
        }

        /// <summary>
        /// True when the typed user name and the SHA-256 hash of the typed password match a valid sign-in of this PC: the shared
        /// file (if one was saved), the built-in default, or the per-install "MasterUser/MasterPassword" setting (also a SHA-256 hash).
        /// </summary>
        public static bool Verify(string username, string passwordHash)
        {
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(passwordHash)) return false;

            Record r = Load();
            if (Matches(username, passwordHash, r.User, r.PasswordHash)) return true;
            if (Matches(username, passwordHash, DefaultUser, DefaultPasswordHash)) return true;
            try
            {
                var set = Properties.Settings.Default;
                if (Matches(username, passwordHash, set.MasterUser, set.MasterPassword)) return true;
            }
            catch { /* settings unreadable: the other sources still apply */ }
            return false;
        }

        private static bool Matches(string user, string hash, string okUser, string okHash)
        {
            return !string.IsNullOrWhiteSpace(okUser) && !string.IsNullOrWhiteSpace(okHash) &&
                   string.Equals(user, okUser, StringComparison.Ordinal) &&
                   string.Equals(hash, okHash.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        // ---- "Remember me": user name + password hash, protected with the Windows account (DPAPI), never as plain text.
        private static string RememberPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ServerHTMLHandler", "remember.dat");

        public static void Remember(string username, string passwordHash)
        {
            string dir = Path.GetDirectoryName(RememberPath);
            Directory.CreateDirectory(dir);
            byte[] data = ProtectedData.Protect(Encoding.UTF8.GetBytes(username + "\n" + passwordHash), null, DataProtectionScope.CurrentUser);
            File.WriteAllBytes(RememberPath, data);
        }

        /// <summary>The remembered sign-in, only when it is still valid.</summary>
        public static bool TryGetRemembered(out string username, out string passwordHash)
        {
            username = passwordHash = null;
            try
            {
                if (!File.Exists(RememberPath)) return false;
                string[] parts = Encoding.UTF8.GetString(ProtectedData.Unprotect(File.ReadAllBytes(RememberPath), null, DataProtectionScope.CurrentUser)).Split('\n');
                if (parts.Length != 2 || !Verify(parts[0], parts[1])) return false;
                username = parts[0]; passwordHash = parts[1];
                return true;
            }
            catch { return false; }
        }

        public static void Forget()
        {
            try { if (File.Exists(RememberPath)) File.Delete(RememberPath); } catch { }
        }

        /// <summary>Stores a new sign-in (hash of the password, never the password itself) for every build on this PC.</summary>
        public static void Save(string username, string passwordHash)
        {
            string dir = Path.GetDirectoryName(FilePath);
            Directory.CreateDirectory(dir);
            string tmp = FilePath + ".tmp";
            File.WriteAllText(tmp, JsonConvert.SerializeObject(new Record { User = username, PasswordHash = passwordHash }, Formatting.Indented));
            if (File.Exists(FilePath)) File.Delete(FilePath);
            File.Move(tmp, FilePath);
        }
    }
}
