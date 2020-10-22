using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho.Diagnostics
{
    public interface ILogEvent
    {
        string Message { get; }

        string Source { get; }
    }

    public interface ILogMessage : ILogEvent
    {
	    DateTime Timestamp { get; }

		LogLevel Level { get; }
	}

    public class LogEvent : ILogMessage
	{
        public LogEvent(string message, LogLevel level, string source)
        {
	        Timestamp = DateTime.Now;
            Message = message;
            Level = level;
            Source = source;
        }

        public DateTime Timestamp { get; }

        public string Message { get; set; }

        public LogLevel Level { get; set; }

        public string Source { get; set; }
    }

    public class StatisticEvent : ILogEvent
    {
        public StatisticEvent(string message, StatisticType metric)
        {
            Message = message;
            Metric = metric;
        }

        public StatisticType Metric { get; set; }

        public string Message { get; set; }
        
        public string Source { get; set; }
    }
}
