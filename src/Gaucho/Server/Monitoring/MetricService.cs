using System;
using Gaucho.Storage;

namespace Gaucho.Server.Monitoring
{
	/// <summary>
	/// Service class for metrics
	/// </summary>
	public class MetricService : IMetricService
	{
		private readonly IStorage _storage;

		/// <summary>
		/// Creates a new instacen of MetricService
		/// </summary>
		/// <param name="storage"></param>
		/// <param name="pipelineId"></param>
		public MetricService(IStorage storage, string pipelineId)
		{
			_storage = storage;
			PipelineId = pipelineId;
		}

		/// <summary>
		/// Gets the associated PipelineId
		/// </summary>
		public string PipelineId { get; }

		/// <summary>
		/// Gets the metric associated with the MetricType
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IMetric GetMetric(MetricType type)
		{
			var metric = _storage.Get<Metric>(new StorageKey(PipelineId, $"metric:{type}"));
			return metric;
		}

		/// <summary>
		/// Set a metric. Overwrites existing metrics
		/// </summary>
		/// <param name="metric"></param>
		public void SetMetric(IMetric metric)
		{
			_storage.Set(new StorageKey(PipelineId, $"metric:{metric.Key}"), metric);
		}
	}

	/// <summary>
	/// Extensions for MetricService
	/// </summary>
	public static class MetricServiceExtensions
	{
		/// <summary>
		/// Get the value of a MetricType
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="metrics"></param>
		/// <param name="type"></param>
		/// <returns></returns>
		public static T GetMetricValue<T>(this IMetricService metrics, MetricType type)
		{
			var metric = metrics.GetMetric(type);
			if (metric == null)
			{
				return default(T);
			}

			if (metric.Value.GetType() != typeof(T))
			{
				return (T) Convert.ChangeType(metric.Value, typeof(T));
			}

			return (T)metric.Value;
		}
	}
}
