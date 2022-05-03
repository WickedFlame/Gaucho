using System;

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

        /// <summary>
        /// Object containing some metrics to log
        /// </summary>
        object MetaData { get; set; }
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
		/// <param name="metaData"></param>
		public LogEvent(string message, LogLevel level, string source, object metaData)
        {
	        Timestamp = DateTime.Now;
            Message = message;
            Level = level;
            Source = source;
			MetaData = metaData;
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

		/// <summary>
		/// Object containing some metrics to log
		/// </summary>
		public object MetaData { get; set; }
    }

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
    public class StatisticEvent<T> : ILogEvent
    {
		/// <summary>
		/// 
		/// </summary>
		/// <param name="value"></param>
		/// <param name="metric"></param>
        public StatisticEvent(T value, StatisticType metric)
        {
	        Timestamp = DateTime.Now;
			Message = value.ToString();
			Value = value;
            Metric = metric;
        }

		/// <summary>
		/// The metric type
		/// </summary>
        public StatisticType Metric { get; set; }

		/// <summary>
		/// The timestamp
		/// </summary>
        public DateTime Timestamp { get; }

		/// <summary>
		/// The logmessage
		/// </summary>
		public string Message { get; set; }
        
		/// <summary>
		/// The value to log
		/// </summary>
		public T Value { get; }

		/// <summary>
		/// The source this log originated from
		/// </summary>
        public string Source { get; set; }
    }
}
