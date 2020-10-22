
using System.Collections.Generic;

namespace Gaucho.Dashboard.Monitoring
{
    public class PipelineMetric
    {
        private readonly List<DashboardMetric> _metrics = new List<DashboardMetric>();
        private Dictionary<string, DashboardElements> _elements;

        public PipelineMetric(string pipelineId)
        {
            PipelineId = pipelineId;
        }

        public string PipelineId { get; }

        public IEnumerable<DashboardMetric> Metrics => _metrics;

        public Dictionary<string, DashboardElements> Elements => _elements ?? (_elements = new Dictionary<string, DashboardElements>());

        internal void AddMetric(string key, string title, object value)
        {
            _metrics.Add(new DashboardMetric {Key = key,  Title = title, Value = value});
        }

        internal void AddElement(string key, string title, object value)
        {
	        if (!Elements.ContainsKey(key))
	        {
		        Elements.Add(key, new DashboardElements {Key = key, Title = title});
	        }

	        Elements[key].Elements.Add(value);
        }
    }
}
