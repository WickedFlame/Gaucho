using Gaucho.Redis.Serializer;
using StackExchange.Redis;
using System;
using System.Linq;

namespace Gaucho.Redis.Serializer
{
	internal static class ObjectExtensions
	{
		//Serialize in Redis format:
		public static HashEntry[] SerializeToRedis(this object obj)
		{
			var properties = obj.GetType().GetProperties();

			if (!properties.Any() || obj is string)
			{
				return new[] {new HashEntry(obj.GetType().Name, obj.ToString())};
			}

			var hashset = properties
				.Where(x => x.GetValue(obj) != null) // <-- PREVENT NullReferenceException
				.Select(property => new HashEntry(property.Name, property.GetValue(obj)
					.ToString())).ToArray();

			return hashset;
		}

		//Deserialize from Redis format
		public static T DeserializeRedis<T>(this HashEntry[] hashEntries)
		{
			var properties = typeof(T).GetProperties();

			if (!properties.Any())
			{
				var entry = hashEntries.FirstOrDefault();
				if (entry.Equals(new HashEntry()))
				{
					return default(T);
				}

				return (T)TypeConverter.Convert(typeof(T), entry.Value.ToString());
			}


			var obj = Activator.CreateInstance(typeof(T));
			foreach (var property in properties)
			{
				var entry = hashEntries.FirstOrDefault(g => g.Name.ToString().Equals(property.Name));
				if (entry.Equals(new HashEntry()))
				{
					continue;
				}

				property.SetValue(obj, Convert.ChangeType(entry.Value.ToString(), property.PropertyType));
			}

			return (T)obj;
		}
	}
}
