using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Dashboard.Monitoring;
using Gaucho.Server.Monitoring;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Gaucho.Dashboard
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGauchoDashboard(this IServiceCollection services, IProcessingServer server)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            // ===== Configurable services =====

            //services.TryAddSingletonChecked(_ => JobStorage.Current);
            //services.TryAddSingletonChecked(_ => JobActivator.Current);
            services.TryAddSingletonChecked<IServerMonitor>(_ => new ServerMonitor(server));
            services.TryAddSingletonChecked(_ => DashboardRoutes.Routes);
            //services.TryAddSingletonChecked<IJobFilterProvider>(_ => JobFilterProviders.Providers);


            // ===== Internal services =====

            // NOTE: these are not required to be checked, because they only depend on already checked configurables,
            //       are not accessed directly, and can't be affected by customizations made from configuration block.



            //services.AddSingleton<IGlobalConfiguration>(serviceProvider =>
            //{
            //    var configurationInstance = GlobalConfiguration.Configuration;

            //    // init defaults for log provider and job activator
            //    // they may be overwritten by the configuration callback later

            //    var loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            //    if (loggerFactory != null)
            //    {
            //        configurationInstance.UseLogProvider(new AspNetCoreLogProvider(loggerFactory));
            //    }

            //    var scopeFactory = serviceProvider.GetService<IServiceScopeFactory>();
            //    if (scopeFactory != null)
            //    {
            //        configurationInstance.UseActivator(new AspNetCoreJobActivator(scopeFactory));
            //    }

            //    // do configuration inside callback

            //    configuration(configurationInstance);

            //    return configurationInstance;
            //});

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
