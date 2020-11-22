using System.Collections.Generic;

namespace Gaucho.Server.Monitoring
{
	internal class StatisticsCollection
	{
		private readonly Dictionary<string, MetricCollection> _metrics = new Dictionary<string, MetricCollection>();

		public void Add(string pipelineId, MetricCollection metrics)
		{
			_metrics.Add(pipelineId, metrics);
		}

		public bool Contains(string pipelineId) => _metrics.ContainsKey(pipelineId);

		public MetricCollection Get(string pipelineId) => _metrics[pipelineId];
	}
}
