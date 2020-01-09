
using System.Collections.Generic;

namespace Gaucho.Dashboard.Monitoring
{
    public class PipelineMetric
    {
        private readonly List<DashboardMetric> _metrics = new List<DashboardMetric>();

        public PipelineMetric(string pipelineId)
        {
            PipelineId = pipelineId;
        }

        public string PipelineId { get; }

        public IEnumerable<DashboardMetric> Metrics => _metrics;

        internal void Add(string key, string title, object value)
        {
            _metrics.Add(new DashboardMetric {Key = key,  Title = title, Value = value});
        }
    }
}
