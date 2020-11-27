using System.Collections;
using System.Collections.Generic;

namespace Gaucho.Server.Monitoring
{
	public class StatisticsApi : IEnumerable<IMetric>
    {
        private static readonly StatisticsCollection _statistics = new StatisticsCollection();

        private readonly MetricCollection _metrics;

        public StatisticsApi(string pipelineId)
        {
			lock(_statistics)
			{
				PipelineId = pipelineId;

				if (!_statistics.Contains(pipelineId))
				{
					_statistics.Add(pipelineId, new MetricCollection());
				}

				_metrics = _statistics.Get(pipelineId);
			}
        }

        public string PipelineId { get; }

        public void AddMetricsCounter(IMetric metric)
        {
            _metrics.Add(metric);
        }

        public IMetric GetMetric(MetricType type)
        {
	        return _metrics.Get(type);
        }

        public object GetMetricValue(MetricType type)
        {
            var metric = _metrics.Get(type);
            if(metric == null)
            {
                return null;
            }

            return metric.Factory.Invoke();
        }

        public IEnumerator<IMetric> GetEnumerator()
        {
            return _metrics.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
