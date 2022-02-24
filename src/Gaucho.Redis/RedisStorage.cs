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
		private readonly IServer _server;

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
			_server = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints(true).FirstOrDefault());
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

		/// <inheritdoc/>
		public IEnumerable<string> GetKeys(StorageKey key)
		{
            try
            {
                var keys = _server.Keys(_options.Db, $"*{CreateKey(key)}*").ToList();
                return keys.Select(k => k.ToString());
            }
            catch (Exception)
            {
				return Enumerable.Empty<string>();
            }
        }

        /// <inheritdoc/>
		public void Delete(StorageKey key)
        {
            _database.KeyDelete(CreateKey(key));
        }

		private string CreateKey(StorageKey key) => key.Key.StartsWith(_options.Prefix) ? key.ToString() : $"{_options.Prefix}:{key}".ToLower();
	}
}
