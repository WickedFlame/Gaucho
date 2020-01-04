using System;

namespace Gaucho.Server.Monitoring
{
    public interface IMetric
    {
        MetricType Type { get; }

        string Title { get; }

        Func<object> Factory { get; }
    }
}
