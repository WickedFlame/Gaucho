
namespace Gaucho.Diagnostics
{
    /// <summary>
    /// Interface for logging
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Add a logwriter
        /// </summary>
        /// <param name="writer"></param>
        void AddWriter(ILogWriter writer);

        /// <summary>
        /// Write a log
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="event"></param>
        /// <param name="category"></param>
        void Write<T>(T @event, Category category) where T : ILogEvent;
    }
}
