using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Gaucho.Dashboard
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHangfireDashboard(this IApplicationBuilder app, string pathMatch = "/gaucho")
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            if (pathMatch == null)
            {
                throw new ArgumentNullException(nameof(pathMatch));
            }

            ThrowIfNotConfigured(app);

            var services = app.ApplicationServices;

            //storage = storage ?? services.GetRequiredService<JobStorage>();
            //options = options ?? services.GetService<DashboardOptions>() ?? new DashboardOptions();
            var routes = app.ApplicationServices.GetRequiredService<RouteCollection>();

            app.Map(new PathString(pathMatch), x => x.UseMiddleware<AspNetCoreDashboardMiddleware>(routes));

            return app;
        }

        private static void ThrowIfNotConfigured(IApplicationBuilder app)
        {
            //var configuration = app.ApplicationServices.GetService<IGlobalConfiguration>();
            //if (configuration == null)
            //{
            //    throw new InvalidOperationException("Unable to find the required services. Please add all the required services by calling 'IServiceCollection.AddHangfire' inside the call to 'ConfigureServices(...)' in the application startup code.");
            //}
        }
    }
}
