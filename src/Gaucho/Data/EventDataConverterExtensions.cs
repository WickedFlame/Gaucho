using System.Linq;
using System.Text;
using Gaucho.Diagnostics;
using Gaucho.Filters;

namespace Gaucho
{
	/// <summary>
	/// 
	/// </summary>
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
            var logger = converter is EventDataConverter conv ? conv.Logger : LoggerConfiguration.Setup();

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

				logger.Write(msg.ToString(), LogLevel.Error, "EventData");

				return data.ToString();
			}

			var value  = formatter.Filter(data);

            logger.Write($"Formatted Property {value.Key} to value {value.Value}", LogLevel.Debug, "IEventDataConverter", metaData: () => new { Property = value.Key, Value = value.Value });

			return value.Value as string;
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
