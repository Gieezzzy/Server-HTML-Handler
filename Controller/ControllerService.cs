using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server_HTML_Handler.Controller
{
    internal class ControllerService
    {

        public static bool UpdateWindowsNtfsAcl(string folderPath, string accountName, string accessLevel)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(folderPath);
                DirectorySecurity dirSecurity = dirInfo.GetAccessControl();

                NTAccount account = new NTAccount(accountName);

                // 1. Determine new FileSystemRights based on selected dropdown value
                FileSystemRights newRights;
                if (accessLevel == "Read/Write")
                {
                    newRights = FileSystemRights.ReadAndExecute |
                                FileSystemRights.Write |
                                FileSystemRights.Modify |
                                FileSystemRights.ListDirectory;
                }
                else // Read-Only
                {
                    newRights = FileSystemRights.ReadAndExecute |
                                FileSystemRights.ListDirectory;
                }

                // 2. Remove existing explicit rules for this specific account to avoid duplication
                dirSecurity.PurgeAccessRules(account);

             
                FileSystemAccessRule updatedRule = new FileSystemAccessRule(
                    account,
                    newRights,
                    InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, // Parameter 3: InheritanceFlags
                    PropagationFlags.None,                                              // Parameter 4: PropagationFlags
                    AccessControlType.Allow                                             // Parameter 5: AccessControlType
                );

                // 4. Add new rule and apply to directory
                dirSecurity.AddAccessRule(updatedRule);
                dirInfo.SetAccessControl(dirSecurity);

                return true;
            }
            catch (UnauthorizedAccessException)
            {
                MessageBox.Show(
                    "Access Denied: Please run the application as Administrator to modify Windows ACL permissions.",
                    "Permission Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update ACL permissions: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
    }
}
