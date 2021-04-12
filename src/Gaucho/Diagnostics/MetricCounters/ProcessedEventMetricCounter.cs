using Gaucho.Server.Monitoring;
using Gaucho.Storage;

namespace Gaucho.Diagnostics.MetricCounters
{
	/// <summary>
	/// Writes Metrics to the Statistics API
	/// </summary>
	public class ProcessedEventMetricCounter : ILogWriter<StatisticEvent<string>>
    {
        private long _count = -1;
        private readonly string _pipelineId;
		private readonly StatisticsApi _statistic;

		/// <summary>
		/// Creates a new instance of ProcessedEventMetricCounter
		/// </summary>
		/// <param name="statistic"></param>
		public ProcessedEventMetricCounter(StatisticsApi statistic)
        {
	        _statistic = statistic;
	        _pipelineId = statistic.PipelineId;

			_count = statistic.GetMetricValue<long>(MetricType.ProcessedEvents);
			if(_count == 0)
			{
				statistic.SetMetric(new Metric(MetricType.ProcessedEvents, "Processed Events", _count));
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

	        lock (_pipelineId)
	        {
		        _count += 1;
		        _statistic.SetMetric(new Metric(MetricType.ProcessedEvents, "Processed Events", _count));
	        }
        }
    }
}
