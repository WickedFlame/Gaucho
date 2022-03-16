using System;
using System.Collections.Generic;
using System.Linq;

namespace Gaucho.Diagnostics
{
    /// <summary>
    /// Default logger
    /// </summary>
    public class Logger : ILogger
    {
        private readonly List<ILogWriter> _writers;
        
        /// <summary>
        /// 
        /// </summary>
        public Logger()
        {
            _writers = new List<ILogWriter>();
        }

        /// <summary>
        /// Gets a list of <see cref="ILogWriter"/>
        /// </summary>
        public List<ILogWriter> Writers => _writers;
        
        /// <summary>
        /// Gets the minimal loglevel
        /// </summary>
        public LogLevel MinLogLevel { get; set; } = LogLevel.Info;

        /// <summary>
        /// Write the event to the logwriters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <param name="category"></param>
        public void Write<T>(T @event, Category category) where T : ILogEvent
        {
            if (@event is ILogMessage msgEvent && msgEvent.Level < MinLogLevel)
            {
                return;
            }

            foreach(var writer in _writers.Where(w => w.Category == category))
            {
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
}
