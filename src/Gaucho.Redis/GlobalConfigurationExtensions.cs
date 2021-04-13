using System;
using Gaucho.Configuration;
using Gaucho.Redis;
using Gaucho.Storage;
using StackExchange.Redis;

namespace Gaucho
{
	public static class GlobalConfigurationExtensions
	{
		public static IGlobalConfiguration UseRedisStorage(this IGlobalConfiguration configuration, ConnectionMultiplexer connectionMultiplexer, RedisStorageOptions options = null)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException(nameof(configuration));
			}

			if (connectionMultiplexer == null)
			{
				throw new ArgumentNullException(nameof(connectionMultiplexer));
			}

			var storage = new RedisStorage(connectionMultiplexer, options);
			
			return configuration.Register<IStorage>(storage);
		}

		public static IGlobalConfiguration UseRedisStorage(this IGlobalConfiguration configuration, string connectionString, RedisStorageOptions options = null)
		{
			if (configuration == null)
			{
				throw new ArgumentNullException(nameof(configuration));
			}

			if (string.IsNullOrEmpty(connectionString))
			{
				throw new ArgumentNullException(nameof(connectionString));
			}

			var redisOptions = ConfigurationOptions.Parse(connectionString);
			options = options ?? new RedisStorageOptions
			{
				Db = redisOptions.DefaultDatabase ?? 0
			};

			var connectionMultiplexer = ConnectionMultiplexer.Connect(connectionString);
			var storage = new RedisStorage(connectionMultiplexer, options);

			return configuration.Register<IStorage>(storage);
		}
	}
}
