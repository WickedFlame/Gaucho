using System;

namespace Gaucho.Server.Monitoring
{
    public class Metric : IMetric
    {
        public Metric(MetricType key, string title, Func<object> func)
        {
            Key = key;
            Title = title;

            Factory = func;
        }

        public MetricType Key { get; }

        public string Title { get; }

        public Func<object> Factory { get; }
    }
}
