using System;
using System.Text.RegularExpressions;
using Gaucho.Dashboard.Dispatchers;

namespace Gaucho.Dashboard
{
    public static class RouteCollectionExtensions
    {
        public static void AddRazorPage(this RouteCollection routes, string pathTemplate, Func<Match, RazorPage> pageFunc)
        {
            if (routes == null)
            {
                throw new ArgumentNullException(nameof(routes));
            }

            if (pathTemplate == null)
            {
                throw new ArgumentNullException(nameof(pathTemplate));
            }

            if (pageFunc == null)
            {
                throw new ArgumentNullException(nameof(pageFunc));
            }

            routes.Add(pathTemplate, new RazorPageDispatcher(pageFunc));
        }
    }
}
