using Gaucho.Dashboard.Monitoring;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace Gaucho.Dashboard
{
	/// <summary>
	/// Extensions for <see cref="IServiceCollection"/>
	/// </summary>
	public static class ServiceCollectionExtensions
    {
		[Obsolete("Use AddGauchoServer instead")]
	    public static IServiceCollection AddGauchoDashboard(this IServiceCollection services, IProcessingServer server)
	    {
		    return AddGauchoServer(services, server);
	    }

		/// <summary>
		/// Add the dashbaord API
		/// </summary>
		/// <param name="services"></param>
		/// <param name="server"></param>
		/// <returns></returns>
		public static IServiceCollection AddGauchoServer(this IServiceCollection services, IProcessingServer server)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // ===== Configurable services =====

            //services.TryAddSingletonChecked(_ => JobStorage.Current);
            //services.TryAddSingletonChecked(_ => JobActivator.Current);
            services.TryAddSingletonChecked<IPipelineMonitor>(_ => new PipelineMonitor());
            services.TryAddSingletonChecked(_ => DashboardRoutes.Routes);

            return services;
        }

        private static void TryAddSingletonChecked<T>(this IServiceCollection serviceCollection, Func<IServiceProvider, T> implementationFactory)
            where T : class
        {
            serviceCollection.TryAddSingleton<T>(serviceProvider =>
            {
                if (serviceProvider == null)
                {
                    throw new ArgumentNullException(nameof(serviceProvider));
                }

                // ensure the configuration was performed
                //serviceProvider.GetRequiredService<IGlobalConfiguration>();

                return implementationFactory(serviceProvider);
            });
        }
    }
}
