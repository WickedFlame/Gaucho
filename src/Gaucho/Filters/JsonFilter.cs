using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gaucho.Filters
{
	public class JsonFilter : IFilter
	{
		private readonly EventDataConverter _converter;

		public JsonFilter(IEnumerable<PropertyFilter> filters)
		{
			_converter = new EventDataConverter();
			foreach (var filter in filters)
			{
				_converter.Add(filter);
			}
		}

		public string Key => "json";

		public FilterType FilterType => FilterType.Formatter;

		public Property Filter(EventData data)
		{
			data = _converter.Convert(data);
			var sb = new StringBuilder();

			var content = string.Join(",", data.Select(p => $"\"{p.Key}\":\"{p.Value}\""));

			return new Property("json", $"{{{content}}}");
		}
	}
}
