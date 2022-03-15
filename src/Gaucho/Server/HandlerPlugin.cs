using System;

namespace Gaucho.Server
{
	/// <summary>
	/// defines a handler
	/// </summary>
	public class HandlerPlugin
	{
		/// <summary>
		/// Name of the handler
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets the type of the handler
		/// </summary>
		public Type Type { get; set; }
	}
}
