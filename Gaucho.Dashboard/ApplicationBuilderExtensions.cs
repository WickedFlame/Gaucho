using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Gaucho.Dashboard
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseGauchoDashboard(this IApplicationBuilder app, string pathMatch = "/gaucho")
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

            var routes = app.ApplicationServices.GetRequiredService<RouteCollection>();

            app.Map(new PathString(pathMatch), x => x.UseMiddleware<DashboardMiddleware>(routes));

            return app;
        }

        private static void ThrowIfNotConfigured(IApplicationBuilder app)
        {
            //var configuration = app.ApplicationServices.GetService<IGlobalConfiguration>();
            //if (configuration == null)
            //{
            //    throw new InvalidOperationException("Unable to find the required services. Please add all the required services by calling 'IServiceCollection.AddGaucho' inside the call to 'ConfigureServices(...)' in the application startup code.");
            //}
        }
    }
}
