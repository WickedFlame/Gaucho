using System.Collections.Generic;

namespace Gaucho.Server
{
	public class HandlerRegistrationContext
	{
		private readonly List<HandlerPlugin> _plugins = new List<HandlerPlugin>();

		internal IEnumerable<HandlerPlugin> Plugins => _plugins;

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
