using System;

namespace Gaucho.Server.Monitoring
{
    public class Metric : IMetric
    {
        public Metric(MetricType type, string title, Func<object> func)
        {
            Type = type;
            Title = title;

            Factory = func;
        }

        public MetricType Type { get; }

        public string Title { get; }

        public Func<object> Factory { get; }
    }
}
