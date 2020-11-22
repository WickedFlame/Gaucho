using System;
using System.Collections.Generic;
using System.Linq;

namespace Gaucho.Diagnostics
{
    public class Logger : ILogger
    {
        private readonly List<ILogWriter> _writers;
        
        public Logger()
        {
            _writers = new List<ILogWriter>();
        }

        public List<ILogWriter> Writers => _writers;

        public void Write<T>(T @event, Category category) where T : ILogEvent
        {
            foreach(var writer in _writers.Where(w => w.Category == category))
            {
				//var w = (ILogWriter<T>)Convert.ChangeType(writer, typeof(ILogWriter<T>), null);
				//w.Write(@event);
				try
				{
					writer.Write(@event);
				}
				catch (Exception)
				{
					// do nothing...
				}
            }
        }
    }

    public static class LoggerExtensions
    {
	    public static void Write<T>(this ILogger logger, T message, Category category, LogLevel level = LogLevel.Info, string source = null)
		    => Write(logger, message.ToString(), category, level, source);


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
        public static void Write(this ILogger logger, string message, StatisticType metric)
        {
            logger.Write(new StatisticEvent(message, metric), Category.EventStatistic);
        }
    }
}
