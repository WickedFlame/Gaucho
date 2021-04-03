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
		private IStorage _storage;

		/// <summary>
		/// Creates a new instance of ProcessedEventMetricCounter
		/// </summary>
		/// <param name="statistic"></param>
		public ProcessedEventMetricCounter(StatisticsApi statistic)
        {
			statistic.AddMetricsCounter(new Metric(MetricType.ProcessedEvents, "Processed Events", () => _count));
			_pipelineId = statistic.PipelineId;
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
				if (_storage == null)
				{
					_storage = GlobalConfiguration.Configuration.Resolve<IStorage>();
					_count = _storage.Get<long>(_pipelineId, "ProcessedEventsMetric");
				}
			}

	        lock (_pipelineId)
	        {
		        _count += 1;
		        _storage.Set(_pipelineId, "ProcessedEventsMetric", _count);
	        }
        }
    }
}
