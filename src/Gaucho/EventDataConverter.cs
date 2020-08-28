using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public EventDataConverter()
        {
            _filters = new List<IFilter>();
        }

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

            foreach (var filter in filers)
            {
                var property = filter.Filter(data);
                result.Add(property.Key, property.Value);
            }

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

		public static string Format(this EventDataConverter converter, EventData data)
	    {
		    var formatter = converter.Filters.FirstOrDefault(f => f.FilterType == FilterType.Formatter);
		    if (formatter == null)
		    {
			    return data.ToString();
		    }

		    return formatter.Filter(data).Value as string;
	    }
    }
}
