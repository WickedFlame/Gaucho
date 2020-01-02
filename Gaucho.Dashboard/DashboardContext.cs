using System.Text.RegularExpressions;

namespace Gaucho.Dashboard
{
    public class DashboardContext
    {
        public Match UriMatch { get; set; }

        public DashboardResponse Response { get; protected set; }

        public DashboardRequest Request { get; protected set; }
    }
}
