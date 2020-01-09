using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Gaucho.Dashboard.Dispatchers;
using Gaucho.Dashboard.Pages;

namespace Gaucho.Dashboard
{
    public static class DashboardRoutes
    {
        private static readonly string[] Javascripts =
        {
            //"jquery-2.1.4.min.js",
            //"bootstrap.min.js",
            //"moment.min.js",
            //"moment-with-locales.min.js",
            //"d3.min.js",
            //"d3.layout.min.js",
            //"rickshaw.min.js",
            "fetch.min.js",
            "gaucho.js"
        };

        private static readonly string[] Stylesheets =
        {
            //"bootstrap.min.css",
            //"rickshaw.min.css",
            "gaucho.css"
        };

        static DashboardRoutes()
        {
            Routes = new RouteCollection();
            Routes.AddRazorPage("/", x => new DashboardPage());
            Routes.Add("/metrics", new MetricsDispatcher());

            Routes.Add("/js[0-9]+", new CombinedResourceDispatcher(
                "application/javascript",
                GetExecutingAssembly(),
                GetContentFolderNamespace("js"),
                Javascripts));

            Routes.Add("/css[0-9]+", new CombinedResourceDispatcher(
                "text/css",
                GetExecutingAssembly(),
                GetContentFolderNamespace("css"),
                Stylesheets));
        }

        public static RouteCollection Routes { get; set; }

        internal static string GetContentFolderNamespace(string contentFolder)
        {
            return $"{typeof(DashboardRoutes).Namespace}.Content.{contentFolder}";
        }

        private static Assembly GetExecutingAssembly()
        {
            return typeof(DashboardRoutes).GetTypeInfo().Assembly;
        }
    }
}
