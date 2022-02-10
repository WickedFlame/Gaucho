using Gaucho.Server.Monitoring;

namespace Gaucho.Diagnostics.MetricCounters
{
	/// <summary>
	/// Writes Metrics to the Statistics API
	/// </summary>
	public class ProcessedEventMetricCounter : ILogWriter<StatisticEvent<string>>
    {
        private long _count = -1;
        private readonly string _pipelineId;
		private readonly IMetricService _metrics;

		/// <summary>
		/// Creates a new instance of ProcessedEventMetricCounter. This counts the total amount of events that have been processed
		/// </summary>
		/// <param name="metrics"></param>
		/// <param name="pipelineId"></param>
		public ProcessedEventMetricCounter(IMetricService metrics, string pipelineId)
        {
	        _metrics = metrics;
	        _pipelineId = pipelineId;

			_count = metrics.GetMetricValue<long>(MetricType.ProcessedEvents);
			if(_count == 0)
			{
				metrics.SetMetric(new Metric(MetricType.ProcessedEvents, "Processed Events", _count));
			}
        }

		/// <summary>
		/// The loggercategory
		/// </summary>
		public Category Category => Category.EventStatistic;

		/// <summary>
		/// Write the ILogEvent to the Logs
		/// </summary>
		/// <param name="event"></param>
		public void Write(ILogEvent @event)
        {
			if (@event is StatisticEvent<string> e)
            {
                Write(e);
			}
        }

		/// <summary>
		/// Write the StatisticEvent to the Logs
		/// </summary>
		/// <param name="event"></param>
		public void Write(StatisticEvent<string> @event)
        {
	        if (@event.Metric != StatisticType.ProcessedEvent)
	        {
		        return;
	        }

			// count the amount of events that have been processed

	        lock (_pipelineId)
	        {
		        _count += 1;
		        _metrics.SetMetric(new Metric(MetricType.ProcessedEvents, "Processed Events", _count));
	        }
        }
    }
}
