using System;
using System.Collections;
using System.Collections.Generic;
using Gaucho.Configuration;
using Gaucho.Storage;

namespace Gaucho.Server.Monitoring
{
	/// <summary>
	/// API for statistics
	/// </summary>
	public class StatisticsApi : IEnumerable<IMetric>
    {
        private readonly MetricCollection _metrics;
        private IStorage _storage;

		/// <summary>
		/// Creates a new instance of the StatisticsApi
		/// </summary>
		/// <param name="pipelineId"></param>
		public StatisticsApi(string pipelineId) : this(GlobalConfiguration.Configuration.Resolve<Options>().ServerName, pipelineId)
        {
        }

		/// <summary>
		/// Creates a new instance of the StatisticsApi
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="storage"></param>
		public StatisticsApi(string pipelineId, IStorage storage) : this(pipelineId)
		{
			_storage = storage;
		}

		/// <summary>
		/// Creates a new instance of the StatisticsApi
		/// </summary>
		/// <param name="serverName"></param>
		/// <param name="pipelineId"></param>
		public StatisticsApi(string serverName, string pipelineId)
        {
			PipelineId = pipelineId;

			_metrics = new MetricCollection();

			var storage = GetStorage();
			var keys = storage.GetKeys(new StorageKey(PipelineId, $"metric:", serverName));
			foreach (var key in keys)
			{
				var metric = storage.Get<Metric>(new StorageKey(key));
				_metrics.Add(metric);
			}
        }

		/// <summary>
		/// Gets the pipelineId of the metrics
		/// </summary>
        public string PipelineId { get; }
		
		/// <summary>
		/// Set a metric. Overwrites existing metrics
		/// </summary>
		/// <param name="metric"></param>
        public void SetMetric(IMetric metric)
        {
			
				var saved = _metrics.Get(metric.Key);
				if (saved != null)
				{
					_metrics.Remove(saved);
				}

				_metrics.Add(metric);

			var storage = GetStorage();
	        storage.Set(new StorageKey(PipelineId, $"metric:{metric.Key}"), metric);
        }

		/// <summary>
		/// Gets the metric associated with the MetricType
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
        public IMetric GetMetric(MetricType type)
        {
				return _metrics.Get(type);
        }

		/// <summary>
		/// Gets the metric value associated with the MetricType
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public object GetMetricValue(MetricType type)
        {
			lock(_metrics)
			{
				var metric = _metrics.Get(type);
				return metric?.Value;
			}
        }

		/// <summary>
		/// Gets the enumerator of the collection
		/// </summary>
		/// <returns></returns>
        public IEnumerator<IMetric> GetEnumerator()
        {
            return _metrics.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private IStorage GetStorage()
        {
	        if (_storage == null)
	        {
		        _storage = GlobalConfiguration.Configuration.Resolve<IStorage>();
	        }

			return _storage;
		}
    }
}
