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

        public void Write(string message, Category category, LogLevel level = LogLevel.Info, string source = null)
        {
            foreach(var writer in _writers.Where(w => w.Category == category))
            {
                writer.Write(message, level, source);
            }
        }

        public static Logger Setup()
        {
            var logger = new Logger();
            logger._writers.Add(new TraceLogWriter());

            return logger;
        }
    }
}
