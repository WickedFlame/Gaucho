
namespace Gaucho.Server.Monitoring
{
	/// <summary>
	/// Heartbeat that is published by the <see cref="HeartbeatBackgroundProcess"/>
	/// </summary>
	public class ServerModel
	{
		/// <summary>
		/// Gets or sets the ServerName
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// Gets or sets the Heartbeat timestamp
		/// </summary>
		public string Heartbeat { get; set; }
	}
}
