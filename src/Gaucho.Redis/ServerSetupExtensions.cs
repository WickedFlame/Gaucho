using System;
using Gaucho.Configuration;
using Gaucho.Redis;
using Gaucho.Storage;
using StackExchange.Redis;

namespace Gaucho
{
	/// <summary>
	/// 
	/// </summary>
	public static class ServerSetupExtensions
	{
		/// <summary>
		/// Use redis as a storage for metrics
		/// </summary>
		/// <param name="setup"></param>
		/// <param name="connectionMultiplexer"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static ServerSetup UseRedisStorage(this ServerSetup setup, ConnectionMultiplexer connectionMultiplexer, RedisStorageOptions options = null)
		{
			if (setup == null)
			{
				throw new ArgumentNullException(nameof(setup));
			}

			if (connectionMultiplexer == null)
			{
				throw new ArgumentNullException(nameof(connectionMultiplexer));
			}

			var storage = new RedisStorage(connectionMultiplexer, options);
			
			setup.Register<IStorage>(storage);

			return setup;
		}

		/// <summary>
		/// Use redis as a storage for metrics
		/// </summary>
		/// <param name="setup"></param>
		/// <param name="connectionString"></param>
		/// <param name="options"></param>
		/// <returns></returns>
		/// <exception cref="ArgumentNullException"></exception>
		public static ServerSetup UseRedisStorage(this ServerSetup setup, string connectionString, RedisStorageOptions options = null)
		{
			if (setup == null)
			{
				throw new ArgumentNullException(nameof(setup));
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

			setup.Register<IStorage>(storage);

			return setup;
		}
	}
}
