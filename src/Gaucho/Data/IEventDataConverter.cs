using System.Collections.Generic;
using Gaucho.Filters;

namespace Gaucho
{
	/// <summary>
    /// Convert configrured values in EventData 
    /// </summary>
    public interface IEventDataConverter
    {
        /// <summary>
        /// Gets a list of all configured <see cref="IFilter"/>
        /// </summary>
        IEnumerable<IFilter> Filters { get; }

        /// <summary>
        /// Add a new <see cref="IFilter"/>
        /// </summary>
        /// <param name="filter"></param>
        void Add(IFilter filter);

        /// <summary>
        /// Calls all converters and creates a new <see cref="EventData"/> with the converted results
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        EventData Convert(EventData data);
    }
}
