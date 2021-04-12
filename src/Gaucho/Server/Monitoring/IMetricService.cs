
namespace Gaucho.Server.Monitoring
{
	/// <summary>
	/// Service for metrics
	/// </summary>
	public interface IMetricService
	{
		/// <summary>
		/// Gets the metric associated with the MetricType
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IMetric GetMetric(MetricType type);

		/// <summary>
		/// Set a metric. Overwrites existing metrics
		/// </summary>
		/// <param name="metric"></param>
		void SetMetric(IMetric metric);
	}
}
