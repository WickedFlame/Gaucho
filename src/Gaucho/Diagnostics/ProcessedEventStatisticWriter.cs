using System;
using System.Collections.Generic;
using Gaucho.Server.Monitoring;

namespace Gaucho.Diagnostics
{
    public class ProcessedEventStatisticWriter : ILogWriter<StatisticEvent>
    {
        //private readonly Dictionary<StatisticType, List<StatisticEvent>> _metrics = new Dictionary<StatisticType, List<StatisticEvent>>();
        //private readonly List<StatisticEvent> _metrics = new List<StatisticEvent>();
        private int _count = 0;

		public ProcessedEventStatisticWriter(StatisticsApi statistic)
        {
			//statistic.AddMetricsCounter(new Metric(MetricType.ProcessedEvents, "Processed Events", () => _metrics[StatisticType.ProcessedEvent].Count));
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

	        //if (!_metrics.ContainsKey(@event.Metric))
	        //{
	        //    _metrics.Add(@event.Metric, new List<StatisticEvent>());
	        //}

	        //_metrics[@event.Metric].Add(@event);
        }
    }
}
