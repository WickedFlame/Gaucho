using System;
using System.Threading.Tasks;
using Gaucho.Dashboard.Monitoring;
using Microsoft.AspNetCore.Http;

namespace Gaucho.Dashboard
{
    public class DashboardMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RouteCollection _routes;
        private readonly IPipelineMonitor _monitor;
        private readonly DashboardOptions _options;

        public DashboardMiddleware(RequestDelegate next, RouteCollection routes, IPipelineMonitor monitor, DashboardOptions options)
        {
	        _next = next ?? throw new ArgumentNullException(nameof(next));
            _routes = routes ?? throw new ArgumentNullException(nameof(routes));
            _monitor = monitor ?? throw new ArgumentNullException(nameof(monitor));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            var context = new DashboardContext(httpContext, _monitor, _options);
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
