using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho.Diagnostics.MetricCounters
{
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class WorkerCountMetric
	{
		/// <summary>
		/// 
		/// </summary>
		public string PipelineId { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public DateTime Timestamp { get; set; }

		/// <summary>
		/// 
		/// </summary>
		public int ActiveWorkers { get; set; }
	}
}
