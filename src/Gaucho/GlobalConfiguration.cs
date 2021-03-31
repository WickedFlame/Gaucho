using System;
using Gaucho.Configuration;
using System.Collections.Generic;
using Gaucho.Server;
using Gaucho.Storage;

namespace Gaucho
{
    public class GlobalConfiguration : IGlobalConfiguration
    {
	    static GlobalConfiguration()
	    {
		    // setup default configuration
		    Setup(s => { });
		}

        private GlobalConfiguration()
        {
        }

		/// <summary>
		/// Gets the default confgiuration for the global scope
		/// </summary>
        public static IGlobalConfiguration Configuration { get; private set; }

		/// <summary>
		/// The context for the configuration
		/// </summary>
        public Dictionary<string, object> Context { get; } = new Dictionary<string, object>();

		/// <summary>
		/// Setup and configure a new Configuration object. This replaces the existing configuration
		/// </summary>
		/// <param name="setup"></param>
        public static void Setup(Action<IGlobalConfiguration> setup)
        {
	        var config = new GlobalConfiguration()
		        .Register(new Options
		        {
					ServerName = Environment.MachineName
				})
		        .Register<IStorage>(new InmemoryStorage())
		        .Register<IActivationContext>(new ActivationContext());

	        setup(config);

			Configuration = config;
        }
    }
}
