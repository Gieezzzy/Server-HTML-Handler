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
            string connStr = Helper.StorageMetricsHelper.GetConnectionString();

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
            string connStr = Helper.StorageMetricsHelper.GetConnectionString();
            using (var conn = new MySqlConnection(connStr))
            {
                conn.Open();
                string userTarget = (username == "Any") ? "" : username;
                string query = $"DROP USER '{userTarget}'@'{host}'; FLUSH PRIVILEGES;";

                using (var cmd = new MySqlCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
            return true;
        }

        public static bool TestDatabaseConnection()
        {
            try
            {
                string connStr = Helper.StorageMetricsHelper.GetConnectionString();
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

    }
}
