using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Gaucho.Dashboard
{
    public sealed class AspNetCoreDashboardContext : DashboardContext
    {
        public AspNetCoreDashboardContext( HttpContext httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            HttpContext = httpContext;
            Request = new AspNetCoreDashboardRequest(httpContext);
            Response = new AspNetCoreDashboardResponse(httpContext);

            //var antiforgery = HttpContext.RequestServices.GetService<IAntiforgery>();

            //if (antiforgery != null)
            //{
            //    var tokenSet = antiforgery.GetAndStoreTokens(HttpContext);

            //    AntiforgeryHeader = tokenSet.HeaderName;
            //    AntiforgeryToken = tokenSet.RequestToken;
            //}
        }

        public HttpContext HttpContext { get; }

        //public override IBackgroundJobClient GetBackgroundJobClient()
        //{
        //    return HttpContext.RequestServices.GetService<IBackgroundJobClient>() ?? base.GetBackgroundJobClient();
        //}

        //public override IRecurringJobManager GetRecurringJobManager()
        //{
        //    return HttpContext.RequestServices.GetService<IRecurringJobManager>() ?? base.GetRecurringJobManager();
        //}
    }
}
