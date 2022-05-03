
namespace Gaucho.Filters
{
    /// <summary>
    /// Defines a filter
    /// </summary>
    public interface IFilter
    {
		//https://www.elastic.co/guide/en/logstash/current/filter-plugins.html
		/*
         filter {
          json {
            source => "message"
          }
        }
         *
         */

        /// <summary>
        /// Gets the key of the filter
        /// </summary>
		string Key { get; }

        /// <summary>
        /// Gets the type of filter
        /// </summary>
		FilterType FilterType { get; }
			
        /// <summary>
        /// Filter the value
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
		Property Filter(EventData data);
    }
}
