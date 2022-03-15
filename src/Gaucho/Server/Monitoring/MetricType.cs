
namespace Gaucho.Server.Monitoring
{
    /// <summary>
    /// Type of metric that is written
    /// </summary>
    public enum MetricType
    {
        /// <summary>
        /// Count of threads
        /// </summary>
        ThreadCount,

        /// <summary>
        /// Amount of events in the queue
        /// </summary>
        QueueSize,

        /// <summary>
        /// Amount of events that are processed
        /// </summary>
        ProcessedEvents,

        /// <summary>
        /// Write to the eventlog
        /// </summary>
        EventLog,

		/// <summary>
		/// Log timeline for active workers
		/// </summary>
        WorkersLog
    }
}
