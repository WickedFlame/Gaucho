using System;
using Gaucho.Server.Monitoring;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Gaucho.Dashboard
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseGauchoDashboard(this IApplicationBuilder app, string pathMatch = "/gaucho", IServerMonitor monitor = null, DashboardOptions options = null)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (pathMatch == null)
            {
                throw new ArgumentNullException(nameof(pathMatch));
            }

            var routes = app.ApplicationServices.GetRequiredService<RouteCollection>();
            options = options ?? app.ApplicationServices.GetService<DashboardOptions>() ?? new DashboardOptions();
            monitor = monitor ?? app.ApplicationServices.GetRequiredService<IServerMonitor>();
            if (monitor == null)
            {
                throw new InvalidOperationException("Unable to find the required services. Please add all the required services by calling 'IServiceCollection.AddGaucho' inside the call to 'ConfigureServices(...)' in the application startup code.");
            }

            app.Map(new PathString(pathMatch), x => x.UseMiddleware<DashboardMiddleware>(routes, monitor, options));

            return app;
        }
    }
}
