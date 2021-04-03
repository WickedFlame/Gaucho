using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho.Diagnostics
{
	/// <summary>
	/// Interface for a LogEvent
	/// </summary>
    public interface ILogEvent
    {
		/// <summary>
		/// Gets or sets the Message
		/// </summary>
		string Message { get; }

		/// <summary>
		/// Gets or sets the Source
		/// </summary>
		string Source { get; }
    }

	/// <summary>
	/// Interface for a LogMessage
	/// </summary>
	public interface ILogMessage : ILogEvent
    {
		/// <summary>
		/// Gets or sets the Timestamp
		/// </summary>
		DateTime Timestamp { get; }

		/// <summary>
		/// Gets or sets the LogLevel
		/// </summary>
		LogLevel Level { get; }
	}

	/// <summary>
	/// The LogEvent
	/// </summary>
	public class LogEvent : ILogMessage
	{
		/// <summary>
		/// Creates a new instance of the LogEvent
		/// </summary>
		public LogEvent()
		{
		}

		/// <summary>
		/// Creates a new instance of the LogEvent
		/// </summary>
		/// <param name="message"></param>
		/// <param name="level"></param>
		/// <param name="source"></param>
		public LogEvent(string message, LogLevel level, string source)
        {
	        Timestamp = DateTime.Now;
            Message = message;
            Level = level;
            Source = source;
        }

		/// <summary>
		/// Gets or sets the Timestamp
		/// </summary>
        public DateTime Timestamp { get; set; }

		/// <summary>
		/// Gets or sets the Message
		/// </summary>
        public string Message { get; set; }

		/// <summary>
		/// Gets or sets the LogLevel
		/// </summary>
        public LogLevel Level { get; set; }

		/// <summary>
		/// Gets or sets the Source
		/// </summary>
        public string Source { get; set; }
    }

    public class StatisticEvent<T> : ILogEvent
    {
        public StatisticEvent(T value, StatisticType metric)
        {
	        Timestamp = DateTime.Now;
			Message = value.ToString();
			Value = value;
            Metric = metric;
        }

        public StatisticType Metric { get; set; }

        public DateTime Timestamp { get; }

		public string Message { get; set; }
        
		public T Value { get; }

        public string Source { get; set; }
    }
}
