using System;
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

        public static ILogger Setup(params Action<LoggerSetup>[] factories)
        {
            var logger = new Logger();

            foreach (var writer in _defaultWriters)
            {
                logger.Writers.Add(writer);
            }

            var setup = new LoggerSetup(logger);
            foreach (var factory in factories)
            {
                factory.Invoke(setup);
            }

            return logger;
        }

        public static void AddLogWriter(ILogWriter writer)
        {
            _defaultWriters.Add(writer);
        }
    }

    public class LoggerSetup
    {
        private Logger _logger;

        internal LoggerSetup(Logger logger)
        {
            _logger = logger;
        }

        public void AddWriter(ILogWriter writer)
        {
            _logger.Writers.Add(writer);
        }
    }
}
