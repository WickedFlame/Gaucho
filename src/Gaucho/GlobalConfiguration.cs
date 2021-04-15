using System;
using Gaucho.Configuration;
using System.Collections.Generic;
using Gaucho.Server;
using Gaucho.Storage;

namespace Gaucho
{
	/// <summary>
	/// Storage for global configurations.
	/// Uses a singleton instance that is created and configured with <see cref="Setup(Action{ServerSetup})"/>.
	/// </summary>
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
		/// Setup and configure a new Configuration object. This replaces the existing configuration.
		/// Overwrites all existing configurations previousley created.
		/// </summary>
		/// <param name="setup"></param>
		/// <returns>The new <see cref="IGlobalConfiguration"/></returns>
        public static IGlobalConfiguration Setup(Action<ServerSetup> setup)
        {
	        var config = new GlobalConfiguration()
		        .Register(new Options
		        {
					ServerName = Environment.MachineName
				})
		        .Register<IStorage>(new InmemoryStorage())
		        .Register<IActivationContext>(new ActivationContext());

	        Configuration = config;

			var serverSetup = new ServerSetup(config);

			setup(serverSetup);

			foreach (var delayed in serverSetup.DelayedSetup)
			{
				delayed.Invoke();
			}

			return config;
        }
    }
}
