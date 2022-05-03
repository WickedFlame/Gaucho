using System;
using System.Collections.Generic;

namespace Gaucho.Diagnostics
{
    /// <summary>
    /// Configuration for the logger
    /// </summary>
    public static class LoggerConfiguration
    {
        private static List<ILogWriter> _defaultWriters = new List<ILogWriter>();

        /// <summary>
        /// Setup the logger
        /// </summary>
        /// <returns></returns>
        public static ILogger Setup()
        {
            var logger = new Logger
            {
                MinLogLevel = GlobalConfiguration.Configuration.GetOptions().LogLevel
            };

            foreach (var writer in _defaultWriters)
            {
                logger.Writers.Add(writer);
            }

            return logger;
        }

        /// <summary>
        /// Setup the logger
        /// </summary>
        /// <param name="factories"></param>
        /// <returns></returns>
        public static ILogger Setup(params Action<LoggerSetup>[] factories)
        {
            var logger = Setup();

            var setup = new LoggerSetup(logger);
            foreach (var factory in factories)
            {
                factory.Invoke(setup);
            }

            return logger;
        }

        /// <summary>
        /// Add a writer to the logger
        /// </summary>
        /// <param name="writer"></param>
        public static void AddLogWriter(ILogWriter writer)
        {
            _defaultWriters.Add(writer);
        }
    }

    /// <summary>
    /// Setupclass for the logger
    /// </summary>
    public class LoggerSetup
    {
        private readonly ILogger _logger;

        internal LoggerSetup(ILogger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Add a writer to the logger
        /// </summary>
        /// <param name="writer"></param>
        public void AddWriter(ILogWriter writer)
        {
            _logger.AddWriter(writer);
        }
    }
}
