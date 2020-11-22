using System;
using System.Collections.Generic;
using Gaucho.Server.Monitoring;

namespace Gaucho.Diagnostics
{
    public class ProcessedEventMetricCounter : ILogWriter<StatisticEvent>
    {
        private int _count = 0;

		public ProcessedEventMetricCounter(StatisticsApi statistic)
        {
			statistic.AddMetricsCounter(new Metric(MetricType.ProcessedEvents, "Processed Events", () => _count));
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
	        if (@event.Metric != StatisticType.ProcessedEvent)
	        {
		        return;
	        }

	        _count += 1;
        }
    }
}
