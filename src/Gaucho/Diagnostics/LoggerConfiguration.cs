using System.Collections.Generic;

namespace Gaucho.Diagnostics
{
    public class LoggerConfiguration
    {
        private static List<ILogWriter> _defaultWriters = new List<ILogWriter>
        {
            new TraceLogWriter()
        };

        public static ILogger Setup()
        {
            var logger = new Logger();

            foreach(var writer in _defaultWriters)
            {
                logger.Writers.Add(writer);
            }

            return logger;
        }

        public static void AddLogWriter(ILogWriter writer)
        {
            _defaultWriters.Add(writer);
        }
    }
}
