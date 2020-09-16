using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Diagnostics;
using Gaucho.Filters;

namespace Gaucho
{
    public interface IEventDataConverter
    {
        IEnumerable<IFilter> Filters { get; }

        void Add(IFilter filter);

        EventData Convert(EventData data);
    }

    public class EventDataConverter : System.Collections.IEnumerable, IEventDataConverter
    {
        private readonly List<IFilter> _filters;
        private readonly ILogger _logger;

        public EventDataConverter()
        {
			_logger = LoggerConfiguration.Setup();
			_filters = new List<IFilter>();
        }

		internal ILogger Logger => _logger;

        public IEnumerable<IFilter> Filters => _filters;

        public void Add(IFilter filter)
        {
            _filters.Add(filter);
        }

        public EventData Convert(EventData data)
        {
	        var filers = _filters.Where(f => f.FilterType == FilterType.Property).ToList();
            if (!filers.Any())
            {
                return data;
            }

            var result = new EventData();

            foreach (var filter in filers.Where(f => f.FilterType == FilterType.Property))
            {
	            var property = filter.Filter(data);
	            result.Add(property);
            }

            _logger.Write(result.ToString(), Category.Log, LogLevel.Debug, "EventDataConverter");

            return result;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _filters.GetEnumerator();
        }
    }

    public static class EventDataConverterExtensions
    {
	    public static EventData Convert(this IEventDataConverter converter, IEventData data)
	    {
		    var eventData = data as EventData;
		    if (eventData == null)
		    {
				return null;
		    }

		    return converter.Convert(eventData);
	    }

		public static string Format(this EventDataConverter converter, string key, EventData data)
	    {
		    var formatter = converter.Filters.FirstOrDefault(f => f.FilterType == FilterType.Formatter && f.Key == key);
		    if (formatter == null)
		    {
			    var msg = new StringBuilder()
				    .AppendLine($"Could not find the formatter '{key}'");

			    var candidates = converter.Filters.Where(p => p.Key.ToLower() == key.ToLower());
			    if (candidates.Any())
			    {
				    msg.AppendLine($"   Possible candidates are:");
				    foreach (var candidate in candidates)
				    {
					    msg.AppendLine($"   - {candidate.Key}");
				    }
			    }

			    converter.Logger.Write(msg.ToString(), Category.Log, LogLevel.Error, "EventData");

				return data.ToString();
		    }

		    return formatter.Filter(data).Value as string;
	    }
    }
}
