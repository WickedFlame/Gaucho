using System;
using System.Collections;
using System.Collections.Generic;
using Gaucho.Configuration;
using Gaucho.Diagnostics;
using Gaucho.Diagnostics.MetricCounters;
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
		public StatisticsApi(string pipelineId)
			: this(GlobalConfiguration.Configuration.Resolve<Options>().ServerName, pipelineId, GlobalConfiguration.Configuration.Resolve<IStorage>())
        {
        }

		/// <summary>
		/// Creates a new instance of the StatisticsApi
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="storage"></param>
		public StatisticsApi(string pipelineId, IStorage storage) 
			: this(GlobalConfiguration.Configuration.Resolve<Options>().ServerName, pipelineId, storage)
		{
		}

		/// <summary>
		/// Creates a new instance of the StatisticsApi
		/// </summary>
		/// <param name="serverName"></param>
		/// <param name="pipelineId"></param>
		/// <param name="storage"></param>
		public StatisticsApi(string serverName, string pipelineId, IStorage storage)
        {
	        _storage = storage;
			PipelineId = pipelineId;

			_metrics = new MetricCollection();

			var keys = storage.GetKeys(new StorageKey(PipelineId, $"metric:", serverName));
			foreach (var key in keys)
			{
				var metric = storage.Get<Metric>(new StorageKey(key));
				_metrics.Add(metric);
			}

			// add logs
			_metrics.Add(new LogMetric(MetricType.WorkersLog, "Active Workers", () =>
			{
				var logs = storage.GetList<ActiveWorkersLogMessage>(new StorageKey(pipelineId, $"log:{MetricType.WorkersLog}", serverName));
				return logs;
			}));
			_metrics.Add(new LogMetric(MetricType.EventLog, "Active Workers", () =>
			{
				var logs = storage.GetList<LogEvent>(new StorageKey(pipelineId, $"logs", serverName));
				return logs;
			}));
		}

		/// <summary>
		/// Gets the pipelineId of the metrics
		/// </summary>
        public string PipelineId { get; }

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
    }
}
