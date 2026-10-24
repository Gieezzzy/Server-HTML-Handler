using MySql.Data.MySqlClient;
using Server_HTML_Handler.Helper;
using Server_HTML_Handler.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_HTML_Handler.Database
{
    internal class dbBase
    {
        public static List<DbUserOverview> GetDatabaseUserAccounts()
        {
            var userList = new List<DbUserOverview>();

            // Replace with your active connection string / helper
            string connStr = StorageMetricsHelper.GetConnectionString();

            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string query = @"SELECT User, Host, Grant_priv, Select_priv, Insert_priv, Update_priv, Delete_priv 
                        FROM mysql.user;";

                using (var cmd = new MySqlCommand(query, conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string user = reader["User"].ToString();
                        string host = reader["Host"].ToString();
                        string grant = string.Equals(reader["Grant_priv"].ToString(), "Y", StringComparison.OrdinalIgnoreCase) ? "Yes" : "No";

                        bool isSelect = string.Equals(reader["Select_priv"].ToString(), "Y", StringComparison.OrdinalIgnoreCase);
                        bool isInsert = string.Equals(reader["Insert_priv"].ToString(), "Y", StringComparison.OrdinalIgnoreCase);

                        string globalPrivs = (isSelect && isInsert) ? "ALL PRIVILEGES" : "USAGE";

                        userList.Add(new DbUserOverview
                        {
                            UserName = string.IsNullOrWhiteSpace(user) ? "Any" : user,
                            HostName = host,
                            HasPassword = "Yes",
                            GlobalPrivileges = globalPrivs,
                            GrantPrivilege = grant
                        });
                    }
                }
            }

            return userList;
        }

        public static bool DropDatabaseUser(string username, string host)
        {
            string connStr = StorageMetricsHelper.GetConnectionString();
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string userTarget = (username == "Any") ? "" : username;
                string safeUser = (userTarget ?? "").Replace("'", "''");
                string safeHost = (host ?? "%").Replace("'", "''");
                string query = $"DROP USER '{safeUser}'@'{safeHost}'; FLUSH PRIVILEGES;";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        public static bool ResetDatabaseUserPassword(string username, string host, string newPassword)
        {
            string connStr = StorageMetricsHelper.GetConnectionString();
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string userTarget = (username == "Any") ? "" : username;
                string safeUser = (userTarget ?? "").Replace("'", "''");
                string safeHost = (host ?? "%").Replace("'", "''");
                string safePass = (newPassword ?? "").Replace("'", "''");

                try
                {
                    string query = $"ALTER USER '{safeUser}'@'{safeHost}' IDENTIFIED BY '{safePass}'; FLUSH PRIVILEGES;";
                    using (var cmd = new MySqlCommand(query, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
                catch
                {
                    // Fallback for older MariaDB/MySQL syntax
                    string fallbackQuery = $"SET PASSWORD FOR '{safeUser}'@'{safeHost}' = PASSWORD('{safePass}'); FLUSH PRIVILEGES;";
                    using (var cmd = new MySqlCommand(fallbackQuery, conn))
                    {
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return true;
        }

        public static bool TestDatabaseConnection()
        {
            try
            {
                string connStr = StorageMetricsHelper.GetConnectionString();
                using (var conn = new MySqlConnection(connStr))
                {
                    conn.Open();
                    return conn.State == ConnectionState.Open;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool TestCustomConnection(string host, uint port, string user, string password, out string serverVersion, out string errorMessage)
        {
            serverVersion = string.Empty;
            errorMessage = string.Empty;
            try
            {
                MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder
                {
                    Server = host,
                    Port = port,
                    UserID = user,
                    Password = password,
                    Database = "mysql",
                    SslMode = MySqlSslMode.Disabled,
                    ConnectionTimeout = 4
                };

                using (var conn = new MySqlConnection(builder.ConnectionString))
                {
                    conn.Open();
                    serverVersion = conn.ServerVersion;
                    return conn.State == ConnectionState.Open;
                }
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

    }
}
