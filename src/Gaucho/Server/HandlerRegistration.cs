using System.Collections.Generic;

namespace Gaucho.Server
{
	public abstract class HandlerRegistration
	{
		public abstract void RegisterHandlers(HandlerRegistrationContext context);

		internal IEnumerable<HandlerPlugin> GetPlugins()
		{
			var ctx = new HandlerRegistrationContext();
			this.RegisterHandlers(ctx);

			return ctx.Plugins;
		}
	}
}
