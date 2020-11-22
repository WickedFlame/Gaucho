using System;
using System.Collections.Generic;
using Gaucho.Server.Monitoring;
using Gaucho.Storage;

namespace Gaucho.Diagnostics
{
    public class ProcessedEventMetricCounter : ILogWriter<StatisticEvent>
    {
        private int _count = -1;
        private readonly string _pipelineId;
		private IStorage _storage;

        public ProcessedEventMetricCounter(StatisticsApi statistic)
        {
			statistic.AddMetricsCounter(new Metric(MetricType.ProcessedEvents, "Processed Events", () => _count));
			_pipelineId = statistic.PipelineId;
        }

        public Category Category => Category.EventStatistic;

        public void Write(ILogEvent @event)
        {
            if (@event is StatisticEvent e)
            {
                Write(e);
            }
        }

        public void Write(StatisticEvent @event)
        {
	        if (_storage == null)
	        {
		        _storage = GlobalConfiguration.Configuration.Resolve<IStorage>();
		        _count = _storage.Get<int>(_pipelineId, "ProcessedEventsMetric");
	        }

	        if (@event.Metric != StatisticType.ProcessedEvent)
	        {
		        return;
	        }

	        _count += 1;
	        _storage.Set(_pipelineId, "ProcessedEventsMetric", _count);

        }
    }
}
