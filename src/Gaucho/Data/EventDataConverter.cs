using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Gaucho.Diagnostics;
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

    public static class EventDataConverterExtensions
    {
		/// <summary>
		/// Convert the data according to the filters
		/// </summary>
		/// <param name="converter"></param>
		/// <param name="data"></param>
		/// <returns></returns>
	    public static EventData Convert(this IEventDataConverter converter, IEventData data)
	    {
		    var eventData = data as EventData;
		    if (eventData == null)
		    {
				return null;
		    }

		    return converter.Convert(eventData);
	    }

		/// <summary>
		/// Format the property
		/// </summary>
		/// <param name="converter"></param>
		/// <param name="key"></param>
		/// <param name="data"></param>
		/// <returns></returns>
	    public static string Format(this EventDataConverter converter, string key, EventData data)
		    => Format(converter as IEventDataConverter, key, data);

		/// <summary>
		/// Format the property
		/// </summary>
		/// <param name="converter"></param>
		/// <param name="key"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public static string Format(this IEventDataConverter converter, string key, IEventData data)
	    {
		    var eventData = data as EventData;
		    if (eventData == null)
		    {
			    return null;
		    }

		    return converter.Format(key, eventData);
		}

		/// <summary>
		/// Format the property
		/// </summary>
		/// <param name="converter"></param>
		/// <param name="key"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public static string Format(this IEventDataConverter converter, string key, EventData data)
	    {
		    var formatter = converter.Filters.FirstOrDefault(f => f.FilterType == FilterType.Formatter && f.Key == key);
		    if (formatter == null)
		    {
			    var logger = converter is EventDataConverter conv ? conv.Logger : LoggerConfiguration.Setup();
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

			    logger.Write(msg.ToString(), LogLevel.Error, "EventData");

				return data.ToString();
		    }

		    return formatter.Filter(data).Value as string;
	    }

		/// <summary>
		/// Format the property
		/// </summary>
		/// <param name="converter"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public static IEventDataConverter AppendFormated(this IEventDataConverter converter, IEventData data)
			=> AppendFormated(converter, data as EventData);

		/// <summary>
		/// Format the property
		/// </summary>
		/// <param name="converter"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public static IEventDataConverter AppendFormated(this IEventDataConverter converter, EventData data)
		{
			if (data == null)
			{
				return converter;
			}

			foreach (var filter in converter.Filters.Where(f => f.FilterType == Filters.FilterType.Formatter))
			{
				data.Add(filter.Key, converter.Format(filter.Key, data));
			}

			return converter;
		}
    }
}
