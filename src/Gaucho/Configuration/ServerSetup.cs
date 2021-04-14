using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho
{
	/// <summary>
	/// Used for configuring the server through <see cref="IGlobalConfiguration"/> setup
	/// </summary>
	public class ServerSetup
	{
		/// <summary>
		/// Creates a new instance of the ServerSetup
		/// </summary>
		/// <param name="config"></param>
		internal ServerSetup(IGlobalConfiguration config)
		{
			Configuration = config;
		}

		/// <summary>
		/// Gets the Configuration
		/// </summary>
		internal IGlobalConfiguration Configuration { get; }

		/// <summary>
		/// Gets a list of actions that are executed at the end of the setup
		/// </summary>
		internal List<Action> DelayedSetup { get; } = new List<Action>();
	}
}
