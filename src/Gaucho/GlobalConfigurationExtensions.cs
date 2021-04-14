using System;
using Gaucho.Configuration;
using Gaucho.Diagnostics;
using Gaucho.Server;

namespace Gaucho
{
	/// <summary>
	/// Extensions for <see cref="IGlobalConfiguration"/>
	/// </summary>
	public static class GlobalConfigurationExtensions
    {
		[Obsolete("Use GlobalConfiguration.Setup(c => {})")]
        public static IGlobalConfiguration UseProcessingServer(this IGlobalConfiguration config, Action<PipelineBuilder> setup)
        {
            var builder = new PipelineBuilder();
            setup.Invoke(builder);

            return config;
        }

		[Obsolete("Use GlobalConfiguration.Setup(c => {})", true)]
		public static IGlobalConfiguration UseOptions(this IGlobalConfiguration config, Options options)
        {
	        var def = config.Resolve<Options>();
	        if (def == null)
	        {
		        config.Register(options);
		        def = options;
	        }

	        def.Merge(options);

	        return config;
        }

		/// <summary>
		/// Add a LogWriter to the Configuration
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="config"></param>
		/// <param name="writer"></param>
		/// <returns></returns>
		public static IGlobalConfiguration AddLogWriter<T>(this IGlobalConfiguration config, ILogWriter<T> writer) where T : ILogMessage
		{
			LoggerConfiguration.AddLogWriter(writer);
			return config;
		}

		/// <summary>
		/// Register a item in the <see cref="IGlobalConfiguration"/> Context
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="config"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static IGlobalConfiguration Register<T>(this IGlobalConfiguration config, T item)
        {
            var key = typeof(T).Name;
            config.Context[key] = item;

			return config;
        }

		/// <summary>
		/// Resolve a item from the <see cref="IGlobalConfiguration"/> Context
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="config"></param>
		/// <returns></returns>
		public static T Resolve<T>(this IGlobalConfiguration config)
        {
            var key = typeof(T).Name;
            if (config.Context.ContainsKey(key))
            {
                return (T)config.Context[key];
            }

            return default(T);
        }

		/// <summary>
		/// Adds a service to the ActivationContext. This is resolved and injected to the instantiation of the handlers
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="config"></param>
		/// <param name="service"></param>
		/// <returns></returns>
        public static IGlobalConfiguration AddService<T>(this IGlobalConfiguration config, Func<T> service)
        {
	        var ctx = config.Resolve<IActivationContext>();
	        if (ctx == null)
	        {
		        ctx = new ActivationContext();
		        config.Register(ctx);
	        }

	        ctx.Register(service);

			return config;
        }

		/// <summary>
		/// Adds a service to the ActivationContext. This is resolved and injected to the instantiation of the handlers
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <typeparam name="TImpl"></typeparam>
		/// <param name="config"></param>
		/// <returns></returns>
		public static IGlobalConfiguration AddService<TService, TImpl>(this IGlobalConfiguration config) where TImpl : TService
        {
	        var ctx = config.Resolve<IActivationContext>();
	        if (ctx == null)
	        {
		        ctx = new ActivationContext();
		        config.Register(ctx);
	        }

	        ctx.Register<TService, TImpl>();

	        return config;
        }
	}
}
