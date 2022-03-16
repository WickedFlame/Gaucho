using Gaucho.Diagnostics;
using System.Collections.Generic;

namespace Gaucho.Filters
{
	public static class FilterExtensions
	{
		/// <summary>
		/// build a converter based on the filters in the list
		/// </summary>
		/// <param name="filters"></param>
		/// <returns></returns>
		public static IEventDataConverter BuildDataFilter(this IEnumerable<string> filters)
		{
			var converter = new EventDataConverter();
			if (filters == null)
			{
				return converter;
			}

			foreach (var filterString in filters)
			{
				var filter = FilterFactory.BuildFilter(filterString);
				if (filter == null)
				{
					var logger = LoggerConfiguration.Setup();
					logger.Write($"Could not convert '{filterString}' to a Filter", LogLevel.Warning, source: "FilterFactory");
					continue;
				}

				converter.Add(filter);
			}

			return converter;
		}
	}
}
