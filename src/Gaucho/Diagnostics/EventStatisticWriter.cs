
using System;
using System.Collections.Generic;
using Gaucho.Server.Monitoring;

namespace Gaucho.Diagnostics
{
    public class EventStatisticWriter : ILogWriter<StatisticEvent>
    {
        private readonly Dictionary<StatisticType, List<StatisticEvent>> _metrics = new Dictionary<StatisticType, List<StatisticEvent>>();

        public EventStatisticWriter(StatisticsApi statistic)
        {
            statistic.AddMetricsCounter(new Metric(MetricType.ProcessedEvents, "Processed Events count", () => _metrics[StatisticType.ProcessedEvent].Count));
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
            if (!_metrics.ContainsKey(@event.Metric))
            {
                _metrics.Add(@event.Metric, new List<StatisticEvent>());
            }

            _metrics[@event.Metric].Add(@event);
        }
    }
}
