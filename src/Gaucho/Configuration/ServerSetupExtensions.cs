using Gaucho.Diagnostics;
using Gaucho.Server;
using System;
using Gaucho.Configuration;

namespace Gaucho
{
	/// <summary>
	/// Extensions for the <see cref="ServerSetup"/>
	/// </summary>
	public static class ServerSetupExtensions
	{
		/// <summary>
		/// Setup the ProcessingServer with the help of the PipelineBuilder
		/// </summary>
		/// <param name="setup"></param>
		/// <param name="setupAction"></param>
		/// <returns></returns>
		public static ServerSetup UseProcessingServer(this ServerSetup setup, Action<PipelineBuilder> setupAction)
		{
			setup.DelayedSetup.Add(() =>
			{
				var builder = new PipelineBuilder();
				setupAction.Invoke(builder);
			});

			return setup;
		}

		/// <summary>
		/// Add a LogWriter to the Configuration
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="setup"></param>
		/// <param name="writer"></param>
		/// <returns></returns>
		public static ServerSetup AddLogWriter<T>(this ServerSetup setup, ILogWriter<T> writer) where T : ILogMessage
		{
			LoggerConfiguration.AddLogWriter(writer);
			return setup;
		}

		/// <summary>
		/// Set the options that are used for the Server
		/// </summary>
		/// <param name="setup"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		public static ServerSetup UseOptions(this ServerSetup setup, Options options)
		{
			var def = setup.Resolve<Options>();
			if (def == null)
			{
				setup.Register(options);
				def = options;
			}

			def.Merge(options);

			return setup;
		}
		
		/// <summary>
		/// Register a item in the <see cref="IGlobalConfiguration"/> Context
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="setup"></param>
		/// <param name="item"></param>
		/// <returns></returns>
		public static ServerSetup Register<T>(this ServerSetup setup, T item)
		{
			setup.Configuration.Register<T>(item);

			return setup;
		}

		/// <summary>
		/// Resolve a item from the <see cref="IGlobalConfiguration"/> Context
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="setup"></param>
		/// <returns></returns>
		public static T Resolve<T>(this ServerSetup setup)
		{
			return setup.Configuration.Resolve<T>();
		}

		/// <summary>
		/// Adds a service to the ActivationContext. This is resolved and injected to the instantiation of the handlers
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="setup"></param>
		/// <param name="service"></param>
		/// <returns></returns>
		public static ServerSetup AddService<T>(this ServerSetup setup, Func<T> service)
		{
			setup.Configuration.AddService<T>(service);

			return setup;
		}

		/// <summary>
		/// Adds a service to the ActivationContext. This is resolved and injected to the instantiation of the handlers
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <typeparam name="TImpl"></typeparam>
		/// <param name="setup"></param>
		/// <returns></returns>
		public static ServerSetup AddService<TService, TImpl>(this ServerSetup setup) where TImpl : TService
		{
			setup.Configuration.AddService<TService, TImpl>();

			return setup;
		}
	}
}
