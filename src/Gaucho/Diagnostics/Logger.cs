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
}
