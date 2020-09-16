using System.Text.RegularExpressions;

namespace Gaucho.Filters
{
	public class FormatFilter : IFilter
	{
		private readonly string _format;
		private readonly string _property;
		private readonly Regex _regex;

		public FormatFilter(string property, string format)
		{
			_property = property;
			_format = format;

			_regex = new Regex(@"\${(.*?)}", RegexOptions.Compiled);
		}

		public string Key => _property;

		public FilterType FilterType => FilterType.Formatter;

		public Property Filter(EventData data)
		{
			var value = _regex.Replace(_format, m =>
			{
				var property = m.Value.Substring(2, m.Value.Length-3);
				var substitute = data[property]?.ToString();
				return string.IsNullOrEmpty(substitute) ? m.Value : substitute;
			});

			return new Property(_property, value);
		}
	}
}
