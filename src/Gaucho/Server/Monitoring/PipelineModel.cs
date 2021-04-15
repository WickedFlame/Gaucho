using Gaucho.Storage;

namespace Gaucho.Server.Monitoring
{
	/// <summary>
	/// Model containing info of the Pipeline that is stored in the <see cref="IStorage"/>
	/// </summary>
	public class PipelineModel
	{
		/// <summary>
		/// Gets or sets the ServerName
		/// </summary>
		public string PipelineId { get; set; }

		/// <summary>
		/// Gets or sets the name of the server the pipeline is running in
		/// </summary>
		public string ServerName { get; set; }

		/// <summary>
		/// Gets or sets the Heartbeat timestamp
		/// </summary>
		public string Heartbeat { get; set; }
	}
}
