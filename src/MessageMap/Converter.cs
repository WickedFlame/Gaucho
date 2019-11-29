using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageMap
{
    public interface IConverter
    {
        IEnumerable<IFilter> Filters { get; }

        void Add(IFilter filter);

        EventData Convert(EventData data);
    }

    public class Converter : System.Collections.IEnumerable, IConverter
    {
        private readonly List<IFilter> _filters;

        public Converter()
        {
            _filters = new List<IFilter>();
        }

        public IEnumerable<IFilter> Filters => _filters;

        public void Add(IFilter filter)
        {
            _filters.Add(filter);

            //return this;
        }

        public EventData Convert(EventData data)
        {
            if (!_filters.Any())
            {
                return data;
            }

            var result = new EventData();

            foreach (var filter in _filters)
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
}
