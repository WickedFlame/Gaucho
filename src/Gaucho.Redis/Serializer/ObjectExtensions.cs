using Gaucho.Redis.Serializer;
using StackExchange.Redis;
using System;
using System.Linq;

namespace Gaucho.Redis.Serializer
{
	/// <summary>
	/// Extensions for object
	/// </summary>
	public static class ObjectExtensions
	{
		/// <summary>
		/// Serialize objects to HashEntries
		/// </summary>
		/// <param name="obj"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Deserialize Redis hashEntries to Objects
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="hashEntries"></param>
		/// <returns></returns>
		public static T DeserializeRedis<T>(this HashEntry[] hashEntries)
		{
			if (!hashEntries.Any())
			{
				return default(T);
			}

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

				property.SetValue(obj, TypeConverter.Convert(property.PropertyType, entry.Value.ToString()));
			}

			return (T)obj;
		}
	}
}
