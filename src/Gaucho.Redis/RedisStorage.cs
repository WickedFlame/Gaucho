using Gaucho.Configuration;
using Gaucho.Storage;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using Gaucho.Redis.Serializer;

namespace Gaucho.Redis
{
	/// <summary>
	/// Storageprovider for storing data in Redis
	/// </summary>
	public class RedisStorage : IStorage
	{
		private readonly RedisStorageOptions _options;
		private readonly IDatabase _database;
		private string _serverName;

		/// <summary>
		/// Creates a new instance of the RedisStorage
		/// </summary>
		/// <param name="connectionMultiplexer"></param>
		/// <param name="options"></param>
		public RedisStorage(IConnectionMultiplexer connectionMultiplexer, RedisStorageOptions options = null)
		{
			if (connectionMultiplexer == null)
			{
				throw new ArgumentNullException(nameof(connectionMultiplexer));
			}

			_options = options ?? new RedisStorageOptions();
			_database = connectionMultiplexer.GetDatabase(_options.Db);
		}

		/// <inheritdoc/>
		public void AddToList<T>(string pipelineId, string key, T value)
		{
			_database.ListRightPushAsync(RedisKey(pipelineId, key), value.SerializeToJson());
		}

		/// <inheritdoc/>
		public IEnumerable<T> GetList<T>(string pipelineId, string key) where T : class, new()
		{
			var list = _database.ListRange(RedisKey(pipelineId, key));
			var items = list.Select(l => l.DeserializeJson<T>());

			return items;
		}

		/// <inheritdoc/>
		public void RemoveRangeFromList(string pipelineId, string key, int count)
		{
			for (var i = 0; i < count; i++)
			{
				_database.ListLeftPopAsync(RedisKey(pipelineId, key));
			}
		}

		/// <inheritdoc/>
		public void Set<T>(string pipelineId, string key, T value)
		{
			_database.HashSetAsync(RedisKey(pipelineId, key), value.SerializeToRedis());
		}

		/// <inheritdoc/>
		public T Get<T>(string pipelineId, string key)
		{
			var hash = _database.HashGetAll(RedisKey(pipelineId, key));

			return hash.DeserializeRedis<T>();
		}

		private string ServerName()
		{
			if (string.IsNullOrEmpty(_serverName))
			{
				var options = GlobalConfiguration.Configuration.Resolve<Options>();
				_serverName = options.ServerName;
			}

			return _serverName;
		}

		private string RedisKey(string pipelineId, string key) => $"{_options.Prefix}:{ServerName()}:{pipelineId}:{key}".ToLower();
	}
}
