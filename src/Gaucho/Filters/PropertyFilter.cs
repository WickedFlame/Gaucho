using System.Linq;
using System.Text;
using Gaucho.Diagnostics;

namespace Gaucho.Filters
{
	/// <summary>
	/// A PropertyFilter
	/// </summary>
    public class PropertyFilter : IFilter
    {
        private readonly string _source;
        private readonly string _destination;
        private ILogger _logger;

		/// <summary>
		/// Creates a new instance of the PropertyFilter
		/// </summary>
		/// <param name="property"></param>
		public PropertyFilter(string property)
        {
            _source = property;
            _destination = property;
        }

		/// <summary>
		/// Creates a new instance of the PropertyFilter
		/// </summary>
		/// <param name="source"></param>
		/// <param name="destination"></param>
		public PropertyFilter(string source, string destination)
        {
            _source = source;
            _destination = destination;
        }

		/// <summary>
		/// Gets the Key of the PropertyFilter
		/// </summary>
        public string Key => _source;

		/// <summary>
		/// Gets the FilterType
		/// </summary>
        public FilterType FilterType => FilterType.Property;

		/// <summary>
		/// Filters the data and returns a new property node
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public Property Filter(EventData data)
        {
            var value = data[_source] ?? data[_destination];

            if (value == null)
            {
	            if (_logger == null)
	            {
		            _logger = LoggerConfiguration.Setup();
	            }

	            var msg = new StringBuilder()
		            .AppendLine($"Could not find property '{_source}' or '{_destination}'");

	            var candidates = data.Where(p => p.Key.ToLower() == _source.ToLower());
	            if (candidates.Any())
	            {
		            msg.AppendLine($"   Possible candidates are:");
		            foreach (var candidate in candidates)
		            {
			            msg.AppendLine($"   - {candidate.Key}");
		            }
	            }

	            _logger.Write(msg.ToString(), Category.Log, LogLevel.Error, "EventData");
			}

            return new Property(_destination, value);
        }
    }
}
