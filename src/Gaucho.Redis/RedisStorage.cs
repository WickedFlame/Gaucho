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
		public void AddToList<T>(StorageKey key, T value)
		{
			var serializer = _options.Serializer ?? new JsonSerializer();
			_database.ListRightPushAsync(CreateKey(key), serializer.Serialize(value));
		}

		/// <inheritdoc/>
		public IEnumerable<T> GetList<T>(StorageKey key) where T : class, new()
		{
			var serializer = _options.Serializer ?? new JsonSerializer();

			var list = _database.ListRange(CreateKey(key));
			var items = list.Select(l => serializer.Deserialize<T>(l));

			return items;
		}

		/// <inheritdoc/>
		public void RemoveRangeFromList(StorageKey key, int count)
		{
			for (var i = 0; i < count; i++)
			{
				_database.ListLeftPopAsync(CreateKey(key));
			}
		}

		/// <inheritdoc/>
		public void Set<T>(StorageKey key, T value)
		{
			_database.HashSetAsync(CreateKey(key), value.SerializeToRedis());
		}

		/// <inheritdoc/>
		public T Get<T>(StorageKey key)
		{
			var hash = _database.HashGetAll(CreateKey(key));

			return hash.DeserializeRedis<T>();
		}

		private string ServerName(string serverName)
		{
			if (!string.IsNullOrEmpty(serverName))
			{
				return serverName;
			}

			if (string.IsNullOrEmpty(_serverName))
			{
				var options = GlobalConfiguration.Configuration.Resolve<Options>();
				_serverName = options.ServerName;
			}

			return _serverName;
		}

		private string CreateKey(StorageKey key) => $"{_options.Prefix}:{ServerName(key.ServerName)}:{key.PipelineId}:{key.Key}".ToLower();
	}
}
