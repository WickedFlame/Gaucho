using System.Collections.Generic;

namespace Gaucho.Server
{
	/// <summary>
	/// Base for the HandlerRegistration
	/// </summary>
	public abstract class HandlerRegistration
	{
		/// <summary>
		/// Register the handlers
		/// </summary>
		/// <param name="context"></param>
		public abstract void RegisterHandlers(HandlerRegistrationContext context);

		internal IEnumerable<HandlerPlugin> GetPlugins()
		{
			var ctx = new HandlerRegistrationContext();
			this.RegisterHandlers(ctx);

			return ctx.Plugins;
		}
	}
}
