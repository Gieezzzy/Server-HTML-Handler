using Server_HTML_Handler.Models;
using Server_HTML_Handler.View.Contents;
using System;
using System.Collections.Generic;
using static Server_HTML_Handler.Helper.Helper;

namespace Server_HTML_Handler.Routes
{
    public static class RouteConfig
    {
        public static Dictionary<string, RouteInfo> Routes { get; set; } = new Dictionary<string, RouteInfo>();

        static RouteConfig()
        {
            RegisterRoutes();
        }

        private static void RegisterRoutes()
        {
            AddRoute(new RouteInfo
            {
                RouteKey = "server_control",
                DisplayName = "Server & Database Control",
                Category = "Administration",
                ViewType = typeof(ServerControlForm),
                RequiresAdmin = true
            });
        }

        public static void AddRoute(RouteInfo route)
        {
            if (!Routes.ContainsKey(route.RouteKey))
            {
                Routes.Add(route.RouteKey, route);
            }
        }

        public static RouteInfo GetRoute(string routeKey)
        {
            if (Routes.TryGetValue(routeKey, out RouteInfo route))
            {
                return route;
            }
            throw new KeyNotFoundException($"Route key '{routeKey}' was not registered in RouteConfig.");
        }
    }
}