using System;
using Gaucho.Storage;

namespace Gaucho.Server.Monitoring
{
	/// <summary>
	/// BackgroundProcess that publishes the heartbeat and piplelineinfo to the storage
	/// </summary>
	public class PipelineHeartbeatBackgroundProcess : IBackgroundProcess
	{
		private readonly IStorage _storage;
		private readonly string _serverName;
		private readonly BackgroundProcess _process;
		private readonly string _pipelineId;

		/// <summary>
		/// Creates a new instance of HeartbeatBackgroundProcess
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="storage"></param>
		public PipelineHeartbeatBackgroundProcess(IStorage storage, string pipelineId)
		{
			_storage = storage ?? throw new ArgumentNullException(nameof(storage));
			_pipelineId = pipelineId ?? throw new ArgumentNullException(nameof(pipelineId));

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
			_storage.Set(new StorageKey($"{_serverName}:pipeline:{_pipelineId}"), new PipelineModel
			{
				PipelineId = _pipelineId,
				ServerName = _serverName,
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
