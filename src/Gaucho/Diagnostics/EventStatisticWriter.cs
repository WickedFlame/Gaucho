
using System.Collections.Generic;

namespace Gaucho.Diagnostics
{
    public class EventStatisticWriter : ILogWriter<StatisticEvent>
    {
        private readonly Dictionary<EventMetric, List<StatisticEvent>> _metrics = new Dictionary<EventMetric, List<StatisticEvent>>();

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
