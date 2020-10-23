using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Diagnostics;

namespace Gaucho.Server.Monitoring
{
    public class StatisticsApi : IEnumerable<IMetric>
    {
        private static readonly StatisticsCollection _statistics = new StatisticsCollection();

        private readonly string _pipelineId;
        private readonly MetricCollection _metrics;

        public StatisticsApi(string pipelineId)
        {
			lock(_statistics)
			{
				_pipelineId = pipelineId;

				if (!_statistics.Contains(pipelineId))
				{
					_statistics.Add(pipelineId, new MetricCollection());
				}

				_metrics = _statistics.Get(pipelineId);
			}
        }

        public void AddMetricsCounter(IMetric metric)
        {
            _metrics.Add(metric);
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

    internal class MetricCollection : IEnumerable<IMetric>
    {
        private readonly List<IMetric> _metrics = new List<IMetric>();

        public void Add(IMetric metric)
        {
            _metrics.Add(metric);
        }

        public IMetric Get(MetricType type)
        {
            return _metrics.FirstOrDefault(m => m.Key == type);
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
