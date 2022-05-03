using System.Text.RegularExpressions;

namespace Gaucho.Filters
{
	/// <summary>
	/// Represents a filter to format values to a string
	/// </summary>
	public class FormatFilter : IFilter
	{
		private readonly string _format;
		private readonly string _property;
		private readonly Regex _regex;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="property"></param>
		/// <param name="format"></param>
		public FormatFilter(string property, string format)
		{
			_property = property;
			_format = format;

			_regex = new Regex(@"\${(.*?)}", RegexOptions.Compiled);
		}

		/// <summary>
		/// Gets the key of the filter
		/// </summary>
		public string Key => _property;

		/// <summary>
		/// Defaults to FilterType.Formatter
		/// </summary>
		public FilterType FilterType => FilterType.Formatter;

		/// <summary>
		/// Format the data
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		public Property Filter(EventData data)
		{
			var value = _regex.Replace(_format, m =>
			{
				var property = m.Value.Substring(2, m.Value.Length-3);
				string format = null;
				if (property.Contains(":"))
				{
					// allow formating like {date:yyyy/MM/dd}
					var index = property.IndexOf(":", System.StringComparison.InvariantCultureIgnoreCase);
					format = property.Substring(index);
					property = property.Substring(0, index);
				}

				var val = data[property];
				var substitute = format != null && val != null ? string.Format($"{{{0}{format}}}", val) : val?.ToString();

				return string.IsNullOrEmpty(substitute) ? m.Value : substitute;
			});

			return new Property(_property, value);
		}
	}
}
