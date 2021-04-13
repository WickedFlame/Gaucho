using System;
using Gaucho.Configuration;

namespace Gaucho.Storage
{
	/// <summary>
	/// The key used for the storage
	/// </summary>
	public class StorageKey
	{
		/// <summary>
		/// Creates a new instance of the StorageKey
		/// </summary>
		/// <param name="key"></param>
		public StorageKey(string key)
		{
			Key = key;
		}

		/// <summary>
		/// Creates a new instance of the StorageKey
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="key"></param>
		public StorageKey(string pipelineId, string key)
			: this(pipelineId, key, null)
		{
		}

		/// <summary>
		/// Creates a new instance of the StorageKey
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="key"></param>
		/// <param name="serverName"></param>
		public StorageKey(string pipelineId, string key, string serverName)
		{
			if (string.IsNullOrEmpty(pipelineId))
			{
				throw new ArgumentNullException(pipelineId);
			}

			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException(key);
			}

			PipelineId = pipelineId;
			Key = key;
			ServerName = ValidateServerName(serverName);
		}

		/// <summary>
		/// Gets or sets the ServerName
		/// </summary>
		public string ServerName { get; set; }

		/// <summary>
		/// Gets or sets the Key for the item in the storage
		/// </summary>
		public string Key { get; set; }

		/// <summary>
		/// Gets or sets the PipelineId
		/// </summary>
		public string PipelineId { get; set; }

		/// <inheritdoc/>
		public override string ToString()
		{
			if (string.IsNullOrEmpty(ServerName) && string.IsNullOrEmpty(PipelineId))
			{
				return Key.ToLower();
			}

			return $"{ServerName}:{PipelineId}:{Key}".ToLower();
		}

		private string ValidateServerName(string serverName)
		{
			if (!string.IsNullOrEmpty(serverName))
			{
				return serverName;
			}

			var options = GlobalConfiguration.Configuration.Resolve<Options>();
			return options.ServerName;
		}
	}
}
