using System.Collections.Generic;

namespace Gaucho.Dashboard.Monitoring
{
	/// <summary>
	/// Monitoring object for pipelines
	/// </summary>
	public interface IPipelineMonitor
	{
		/// <summary>
		/// Gets all Metrics
		/// </summary>
		/// <returns></returns>
		IEnumerable<PipelineMetric> GetMetrics();
	}
}
