
namespace Gaucho.Server.Monitoring
{
    public enum MetricType
    {
        ThreadCount,
        QueueSize,
        ProcessedEvents,
        EventLog,

		/// <summary>
		/// Log timeline for active workers
		/// </summary>
        WorkersLog
    }
}
