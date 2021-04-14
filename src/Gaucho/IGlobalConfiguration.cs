using System.Collections.Generic;

namespace Gaucho
{
	/// <summary>
	/// Storage for global configurations
	/// </summary>
	public interface IGlobalConfiguration
    {
		/// <summary>
		/// Gets the storage context for configurations
		/// </summary>
        Dictionary<string, object> Context { get; }
    }
}
