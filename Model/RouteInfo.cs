using System;
using System.Windows.Forms;

namespace Server_HTML_Handler.Models
{

    public class DbUserOverview
    {
        public string UserName { get; set; }
        public string HostName { get; set; }
        public string HasPassword { get; set; }
        public string GlobalPrivileges { get; set; }
        public string GrantPrivilege { get; set; }
    }
    public class RouteInfo
    {


        public string RouteName { get; set; }
        public string TargetFile { get; set; }
        public string UrlPath { get; set; }
        public string PhysicalLocation { get; set; }
        public string StatusAction { get; set; }


        public string RouteKey { get; set; }
        public string DisplayName { get; set; }
        public string Category { get; set; }
        public Type ViewType { get; set; }
        public bool RequiresAdmin { get; set; }

        // Delegate factory for lazy-loading UserControl instances
        public Func<UserControl> ViewFactory { get; set; }

        public RouteInfo() { }

        public RouteInfo(string routeKey, string displayName, Type viewType, string category = "System", bool requiresAdmin = false)
        {
            RouteKey = routeKey;
            DisplayName = displayName;
            ViewType = viewType;
            Category = category;
            RequiresAdmin = requiresAdmin;
        }

        public UserControl CreateViewInstance()
        {
            if (ViewFactory != null)
            {
                return ViewFactory();
            }

            if (ViewType != null && typeof(UserControl).IsAssignableFrom(ViewType))
            {
                return (UserControl)Activator.CreateInstance(ViewType);
            }

            throw new InvalidOperationException($"Unable to resolve view instance for route '{RouteKey}'.");
        }
    }
}