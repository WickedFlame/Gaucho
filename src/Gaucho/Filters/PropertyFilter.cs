using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho.Filters
{
    public class PropertyFilter : IFilter
    {
        private readonly string _source;
        private readonly string _destination;

        public PropertyFilter(string property)
        {
            _source = property;
            _destination = property;
        }

        public PropertyFilter(string source, string destination)
        {
            _source = source;
            _destination = destination;
        }

        public FilterType FilterType => FilterType.Property;

		public Property Filter(EventData data)
        {
            var value = data[_source];
            return new Property(_destination, value);
        }
    }
}
