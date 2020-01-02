using System;
using System.Text.RegularExpressions;
using Gaucho.Server.Monitoring;
using Microsoft.AspNetCore.Http;

namespace Gaucho.Dashboard
{
    public class DashboardContext
    {
        public DashboardContext(HttpContext httpContext, IServerMonitor monitor, DashboardOptions options)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            HttpContext = httpContext;
            Request = new DashboardRequest(httpContext);
            Response = new DashboardResponse(httpContext);

            ServerMonitor = monitor;
            Options = options;

            //var antiforgery = HttpContext.RequestServices.GetService<IAntiforgery>();

            //if (antiforgery != null)
            //{
            //    var tokenSet = antiforgery.GetAndStoreTokens(HttpContext);

            //    AntiforgeryHeader = tokenSet.HeaderName;
            //    AntiforgeryToken = tokenSet.RequestToken;
            //}
        }

        public HttpContext HttpContext { get; }

        public IServerMonitor ServerMonitor { get; }

        public DashboardOptions Options { get; }

        //public override IBackgroundJobClient GetBackgroundJobClient()
        //{
        //    return HttpContext.RequestServices.GetService<IBackgroundJobClient>() ?? base.GetBackgroundJobClient();
        //}

        //public override IRecurringJobManager GetRecurringJobManager()
        //{
        //    return HttpContext.RequestServices.GetService<IRecurringJobManager>() ?? base.GetRecurringJobManager();
        //}

        public Match UriMatch { get; set; }

        public DashboardResponse Response { get; protected set; }

        public DashboardRequest Request { get; protected set; }
    }
}
