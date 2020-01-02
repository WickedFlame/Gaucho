using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Gaucho.Dashboard
{
    public class AspNetCoreDashboardMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RouteCollection _routes;

        public AspNetCoreDashboardMiddleware(RequestDelegate next, RouteCollection routes)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }

            if (routes == null)
            {
                throw new ArgumentNullException(nameof(routes));
            }

            _next = next;
            _routes = routes;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var context = new AspNetCoreDashboardContext(httpContext);
            var findResult = _routes.FindDispatcher(httpContext.Request.Path.Value);

            if (findResult == null)
            {
                await _next.Invoke(httpContext);
                return;
            }

            //foreach (var filter in _options.Authorization)
            //{
            //    if (!filter.Authorize(context))
            //    {
            //        var isAuthenticated = httpContext.User?.Identity?.IsAuthenticated;

            //        httpContext.Response.StatusCode = isAuthenticated == true
            //            ? (int)HttpStatusCode.Forbidden
            //            : (int)HttpStatusCode.Unauthorized;

            //        return;
            //    }

            //    var antiforgery = httpContext.RequestServices.GetService<IAntiforgery>();

            //    if (antiforgery != null)
            //    {
            //        var requestValid = await antiforgery.IsRequestValidAsync(httpContext);

            //        if (!requestValid)
            //        {
            //            // Invalid or missing CSRF token
            //            httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
            //            return;
            //        }
            //    }
            //}

            context.UriMatch = findResult.Item2;

            await findResult.Item1.Dispatch(context);
        }
    }
}
