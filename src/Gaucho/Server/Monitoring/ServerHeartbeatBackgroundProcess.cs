using System;
using Gaucho.Storage;

namespace Gaucho.Server.Monitoring
{
	/// <summary>
	/// BackgroundProcess that publishes the heartbeat and serverinfo to the storage
	/// </summary>
	public class ServerHeartbeatBackgroundProcess : IBackgroundProcess
	{
		private readonly IStorage _storage;
		private readonly string _serverName;
		private readonly BackgroundProcess _process;

		/// <summary>
		/// Creates a new instance of HeartbeatBackgroundProcess
		/// </summary>
		/// <param name="storage"></param>
		public ServerHeartbeatBackgroundProcess(IStorage storage)
		{
			_storage = storage ?? throw new ArgumentNullException(nameof(storage));

			var options = GlobalConfiguration.Configuration.GetOptions();
			_serverName = options.ServerName;
			var interval = options.HeartbeatInterval;
			if (interval <= 0)
			{
				interval = 120000;
			}

			_process = new BackgroundProcess(Execute, interval);
		}

		private void Execute()
		{
			_storage.Set(new StorageKey($"server:{_serverName}"), new ServerModel
			{
				Name =_serverName, 
				Heartbeat = DateTime.Now.ToString("o")
			});
		}

		/// <summary>
		/// Dispose the processor
		/// </summary>
		public void Dispose()
		{
			_process.Dispose();
		}
	}
}
