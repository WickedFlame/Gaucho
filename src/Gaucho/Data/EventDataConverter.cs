using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Gaucho.Diagnostics;
using Gaucho.Filters;

namespace Gaucho
{
	/// <summary>
	/// Convert configrured values in EventData 
	/// </summary>
	public class EventDataConverter : System.Collections.IEnumerable, IEventDataConverter
    {
        private readonly List<IFilter> _filters;
        private readonly Stopwatch _stopwatch;
        private readonly ILogger _logger;

		/// <summary>
		/// Create a new EventDataConverter
		/// </summary>
        public EventDataConverter()
        {
			_logger = LoggerConfiguration.Setup();
			_filters = new List<IFilter>();
            _stopwatch = Stopwatch.StartNew();
		}

		internal ILogger Logger => _logger;

		/// <summary>
		/// Gets a list of all configured <see cref="IFilter"/>
		/// </summary>
		public IEnumerable<IFilter> Filters => _filters;

		/// <summary>
		/// Add a new <see cref="IFilter"/>
		/// </summary>
		/// <param name="filter"></param>
		public void Add(IFilter filter)
        {
            _filters.Add(filter);
        }

		/// <summary>
		/// Calls all converters and creates a new <see cref="EventData"/> with the converted results
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public EventData Convert(EventData data)
        {
            _stopwatch.Restart();
			var filers = _filters.Where(f => f.FilterType == FilterType.Property).ToList();
            if (!filers.Any())
            {
	            _logger.Write($"Converted object to EventData{Environment.NewLine}{data}", LogLevel.Debug, "EventDataConverter", () => new
                {
                    Duration = _stopwatch.Elapsed.TotalMilliseconds
                });

				return data;
            }

            var result = new EventData();

            foreach (var filter in filers.Where(f => f.FilterType == FilterType.Property))
            {
	            var property = filter.Filter(data);
	            result.Add(property);
            }

            _logger.Write($"Converted object to EventData{Environment.NewLine}{result}", LogLevel.Debug, "EventDataConverter", () => new
            {
                Duration = _stopwatch.Elapsed.TotalMilliseconds
			});

            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _filters.GetEnumerator();
        }
    }
}
