using System.Collections.Generic;

namespace Gaucho.Server
{
	/// <summary>
	/// Context for registering handlers
	/// </summary>
	public class HandlerRegistrationContext
	{
		private readonly List<HandlerPlugin> _plugins = new List<HandlerPlugin>();

		internal IEnumerable<HandlerPlugin> Plugins => _plugins;

		/// <summary>
		/// Register a new handler based on the name and the type
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="name"></param>
		public void Register<T>(string name) //where T : IInputHandler
		{
			_plugins.Add(new HandlerPlugin
			{
				Name = name,
				Type = typeof(T)
			});
		}
	}
}
