using System;

namespace Gaucho.Diagnostics.MetricCounters
{
	/// <summary>
	/// 
	/// </summary>
	public class ActiveWorkersLogMessage
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
