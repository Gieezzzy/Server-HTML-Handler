using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using MySql.Data.MySqlClient;

namespace Server_HTML_Handler.Services
{
    /// <summary>
    /// Sends the emails that web systems (NEXUS-L) put into tbl_email_outbox, using the account from MailService.
    /// A web page only INSERTs a row; this tray service delivers it, retries failures and records the result.
    /// </summary>
    public class EmailOutboxService
    {
        private static readonly Lazy<EmailOutboxService> _instance = new Lazy<EmailOutboxService>(() => new EmailOutboxService());
        public static EmailOutboxService Instance => _instance.Value;

        public const int MaxAttempts = 5;
        public int PollSeconds { get; set; } = 20;

        public bool IsRunning => _timer != null;
        public DateTime? LastRun { get; private set; }
        public string LastError { get; private set; } = "";

        private Timer _timer;
        private int _busy;

        public class OutboxRow
        {
            public long Id;
            public string Source;
            public string To;
            public string Subject;
            public string Status;
            public int Attempts;
            public string LastError;
            public DateTime CreatedAt;
            public DateTime? SentAt;
        }

        private static string ConnectionString() =>
            Helper.StorageMetricsHelper.GetConnectionString(MailService.Load().OutboxDatabase);

        public static void EnsureTable()
        {
            using (var conn = new MySqlConnection(ConnectionString()))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(@"
                    CREATE TABLE IF NOT EXISTS tbl_email_outbox (
                        id              BIGINT AUTO_INCREMENT PRIMARY KEY,
                        source          VARCHAR(50)  NOT NULL DEFAULT 'NEXUS-L',
                        to_address      TEXT         NOT NULL,
                        cc_address      TEXT         NULL,
                        subject         VARCHAR(255) NOT NULL,
                        body            MEDIUMTEXT   NOT NULL,
                        is_html         TINYINT(1)   NOT NULL DEFAULT 1,
                        status          VARCHAR(10)  NOT NULL DEFAULT 'queued',
                        attempts        INT          NOT NULL DEFAULT 0,
                        last_error      TEXT         NULL,
                        created_at      DATETIME     NOT NULL DEFAULT CURRENT_TIMESTAMP,
                        next_attempt_at DATETIME     NULL,
                        sent_at         DATETIME     NULL,
                        KEY idx_outbox_status (status, next_attempt_at)
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Start()
        {
            if (_timer != null) return;
            try
            {
                EnsureTable();
                // Rows left in 'sending' by a crash or shutdown go back to the queue
                Execute("UPDATE tbl_email_outbox SET status = 'queued' WHERE status = 'sending'", null);
            }
            catch (Exception ex) { LastError = ex.Message; }
            _timer = new Timer(_ => ProcessQueue(), null, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(Math.Max(5, PollSeconds)));
        }

        public void Stop()
        {
            _timer?.Dispose();
            _timer = null;
        }

        /// <summary>Delivers due queued rows. Returns how many were sent.</summary>
        public int ProcessQueue()
        {
            if (Interlocked.Exchange(ref _busy, 1) == 1) return 0;
            int sent = 0;
            try
            {
                LastRun = DateTime.Now;
                if (!MailService.Load().IsConfigured) return 0;

                var due = new List<(long Id, string To, string Cc, string Subject, string Body, bool Html, int Attempts)>();
                using (var conn = new MySqlConnection(ConnectionString()))
                {
                    conn.Open();
                    using (var cmd = new MySqlCommand(@"
                        SELECT id, to_address, cc_address, subject, body, is_html, attempts
                        FROM tbl_email_outbox
                        WHERE status = 'queued' AND (next_attempt_at IS NULL OR next_attempt_at <= NOW())
                        ORDER BY id LIMIT 10", conn))
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            due.Add((r.GetInt64(0), r.GetString(1), r.IsDBNull(2) ? "" : r.GetString(2), r.GetString(3),
                                     r.GetString(4), r.GetBoolean(5), r.GetInt32(6)));
                        }
                    }
                }

                foreach (var row in due)
                {
                    // Claim the row so a second sender can never send it twice
                    if (Execute("UPDATE tbl_email_outbox SET status = 'sending', attempts = attempts + 1 WHERE id = @id AND status = 'queued'",
                                p => p.AddWithValue("@id", row.Id)) == 0) continue;

                    string error = MailService.Send(row.To, row.Cc, row.Subject, row.Body, row.Html);
                    if (error == null)
                    {
                        Execute("UPDATE tbl_email_outbox SET status = 'sent', sent_at = NOW(), last_error = NULL WHERE id = @id",
                                p => p.AddWithValue("@id", row.Id));
                        sent++;
                    }
                    else
                    {
                        int attempts = row.Attempts + 1;
                        bool giveUp = attempts >= MaxAttempts;
                        Execute(@"UPDATE tbl_email_outbox
                                  SET status = @st, last_error = @err, next_attempt_at = DATE_ADD(NOW(), INTERVAL @wait MINUTE)
                                  WHERE id = @id", p =>
                        {
                            p.AddWithValue("@st", giveUp ? "failed" : "queued");
                            p.AddWithValue("@err", error);
                            p.AddWithValue("@wait", attempts * 5);
                            p.AddWithValue("@id", row.Id);
                        });
                        LastError = $"#{row.Id}: {error}";
                    }
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
            }
            finally
            {
                Interlocked.Exchange(ref _busy, 0);
            }
            return sent;
        }

        /// <summary>Adds an email to the outbox (what NEXUS-L does with a plain INSERT).</summary>
        public static long Enqueue(string source, string to, string cc, string subject, string body, bool isHtml)
        {
            EnsureTable();
            using (var conn = new MySqlConnection(ConnectionString()))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(@"
                    INSERT INTO tbl_email_outbox (source, to_address, cc_address, subject, body, is_html)
                    VALUES (@src, @to, @cc, @sub, @body, @html); SELECT LAST_INSERT_ID();", conn))
                {
                    cmd.Parameters.AddWithValue("@src", source);
                    cmd.Parameters.AddWithValue("@to", to);
                    cmd.Parameters.AddWithValue("@cc", string.IsNullOrWhiteSpace(cc) ? (object)DBNull.Value : cc);
                    cmd.Parameters.AddWithValue("@sub", subject);
                    cmd.Parameters.AddWithValue("@body", body);
                    cmd.Parameters.AddWithValue("@html", isHtml ? 1 : 0);
                    return Convert.ToInt64(cmd.ExecuteScalar());
                }
            }
        }

        /// <summary>Records an email the console already sent directly, so it shows up in 'mail queue'.</summary>
        public static void RecordDirect(string to, string subject, string body, bool isHtml, string error)
        {
            bool ok = string.IsNullOrEmpty(error);
            long id = Enqueue("CLI", to, null, subject, body, isHtml);
            Execute(@"UPDATE tbl_email_outbox SET status = @st, attempts = 1, last_error = @err, sent_at = @sent WHERE id = @id", p =>
            {
                p.AddWithValue("@st", ok ? "sent" : "failed");
                p.AddWithValue("@err", ok ? DBNull.Value : (object)error);
                p.AddWithValue("@sent", ok ? (object)DateTime.Now : DBNull.Value);
                p.AddWithValue("@id", id);
            });
        }

        public static bool Retry(long id)
        {
            return Execute(@"UPDATE tbl_email_outbox SET status = 'queued', attempts = 0, next_attempt_at = NULL
                             WHERE id = @id AND status IN ('failed', 'queued')", p => p.AddWithValue("@id", id)) > 0;
        }

        public static List<OutboxRow> Recent(int count)
        {
            var rows = new List<OutboxRow>();
            using (var conn = new MySqlConnection(ConnectionString()))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(@"
                    SELECT id, source, to_address, subject, status, attempts, last_error, created_at, sent_at
                    FROM tbl_email_outbox ORDER BY id DESC LIMIT @n", conn))
                {
                    cmd.Parameters.AddWithValue("@n", count);
                    using (var r = cmd.ExecuteReader())
                    {
                        while (r.Read())
                        {
                            rows.Add(new OutboxRow
                            {
                                Id = r.GetInt64(0), Source = r.GetString(1), To = r.GetString(2), Subject = r.GetString(3),
                                Status = r.GetString(4), Attempts = r.GetInt32(5), LastError = r.IsDBNull(6) ? "" : r.GetString(6),
                                CreatedAt = r.GetDateTime(7), SentAt = r.IsDBNull(8) ? (DateTime?)null : r.GetDateTime(8)
                            });
                        }
                    }
                }
            }
            return rows;
        }

        public static Dictionary<string, int> Counts()
        {
            var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            using (var conn = new MySqlConnection(ConnectionString()))
            {
                conn.Open();
                using (var cmd = new MySqlCommand("SELECT status, COUNT(*) FROM tbl_email_outbox GROUP BY status", conn))
                using (var r = cmd.ExecuteReader())
                {
                    while (r.Read()) counts[r.GetString(0)] = Convert.ToInt32(r.GetValue(1));
                }
            }
            return counts;
        }

        private static int Execute(string sql, Action<MySqlParameterCollection> bind)
        {
            using (var conn = new MySqlConnection(ConnectionString()))
            {
                conn.Open();
                using (var cmd = new MySqlCommand(sql, conn))
                {
                    bind?.Invoke(cmd.Parameters);
                    return cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
