using System;

namespace Gaucho.Server.Monitoring
{
    public interface IMetric
    {
        MetricType Key { get; }

        string Title { get; }

        Func<object> Factory { get; }
    }
}
