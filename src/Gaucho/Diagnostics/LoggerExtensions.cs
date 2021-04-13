
namespace Gaucho.Diagnostics
{
	/// <summary>
	/// Extension methods for the logger
	/// </summary>
	public static class LoggerExtensions
	{
		/// <summary>
		/// Write a message to the log
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="logger"></param>
		/// <param name="message"></param>
		/// <param name="category"></param>
		/// <param name="level"></param>
		/// <param name="source"></param>
		public static void Write<T>(this ILogger logger, T message, Category category, LogLevel level = LogLevel.Info, string source = null)
			=> Write(logger, message.ToString(), category, level, source);


		/// <summary>
		/// Write a message to the log
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="message"></param>
		/// <param name="category"></param>
		/// <param name="level"></param>
		/// <param name="source"></param>
		public static void Write(this ILogger logger, string message, Category category, LogLevel level = LogLevel.Info, string source = null)
		{
			switch (category)
			{
				case Category.Log:
					logger.Write(new LogEvent(message, level, source), category);
					break;
			}
		}

		/// <summary>
		/// Writes a message that can be collected by the dashboard
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="message"></param>
		/// <param name="metric"></param>
		public static void WriteMetric<T>(this ILogger logger, T message, StatisticType metric)
		{
			logger.Write(new StatisticEvent<T>(message, metric), Category.EventStatistic);
		}
	}
}
