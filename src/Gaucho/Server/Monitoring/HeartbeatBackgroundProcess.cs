using System;
using System.Threading;
using Gaucho.Configuration;
using Gaucho.Storage;

namespace Gaucho.Server.Monitoring
{
	/// <summary>
	/// BackgroundProcess that publishes the heartbeat and serverinfo to the storage
	/// </summary>
	public class HeartbeatBackgroundProcess : IBackgroundProcess
	{
		private readonly IStorage _storage;
		private readonly string _serverName;
		private readonly Timer _timer;

		/// <summary>
		/// Creates a new instance of HeartbeatBackgroundProcess
		/// </summary>
		public HeartbeatBackgroundProcess()
		{
			_storage = GlobalConfiguration.Configuration.Resolve<IStorage>();
			var options = GlobalConfiguration.Configuration.Resolve<Options>();
			_serverName = options.ServerName;
			var interval = options.HeartbeatInterval;
			if (interval <= 0)
			{
				interval = 120000;
			}

			_timer = new Timer(Execute, null, 0, interval);
		}

		private void Execute(object stateInfo)
		{
			_storage.Set(new StorageKey($"server:{_serverName}"), new HeartbeatModel { Name =_serverName, Heartbeat = DateTime.Now.ToString("o") });
		}

		public void Dispose()
		{
			_timer.Dispose();
		}
	}

	/// <summary>
	/// Heartbeat that is published by the <see cref="HeartbeatBackgroundProcess"/>
	/// </summary>
	public class HeartbeatModel
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
