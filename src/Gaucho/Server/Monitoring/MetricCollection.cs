using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gaucho.Server.Monitoring
{
	/// <summary>
	/// A collection of metrics
	/// </summary>
	public class MetricCollection : IEnumerable<IMetric>
	{
		private readonly List<IMetric> _metrics = new List<IMetric>();

		/// <summary>
		/// Add a metfic to the collection
		/// </summary>
		/// <param name="metric"></param>
		public void Add(IMetric metric)
		{
			lock(_metrics)
			{
				_metrics.Add(metric);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="metric"></param>
		public void Remove(IMetric metric)
		{
			lock(_metrics)
			{
				_metrics.Remove(metric);
			}
		}

		/// <summary>
		/// Gets the metric with the belonging MetricType
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IMetric Get(MetricType type)
		{
			lock(_metrics)
			{
				return _metrics.FirstOrDefault(m => m.Key == type);
			}
		}

		/// <summary>
		/// Gets the enumerator for the collection
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
