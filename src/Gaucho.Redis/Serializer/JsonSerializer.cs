using System;
using System.Linq;
using System.Reflection;

namespace Gaucho.Redis.Serializer
{
	/// <summary>
	/// A simple json serializer
	/// </summary>
	public class JsonSerializer : ISerializer
	{
		/// <summary>
		/// Serialize the object to a json string
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public string Serialize(object item)
		{
			if (item.IsPrimitiveType())
			{
				return item.ToString();
			}

			var nodes = item.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).Select(propInfo => new JsonNode
			{
				Name = propInfo.Name,
				Value = propInfo.GetValue(item, null)?.ToString()
			});

			var value = $"{{{string.Join(", ", nodes.Where(n => !string.IsNullOrEmpty(n.Value)).Select(n => $"{n.Name}: '{n.Value}'"))}}}";
			return value;
		}

		/// <summary>
		/// Deserialize a json string to an object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="json"></param>
		/// <returns></returns>
		public T Deserialize<T>(string json) where T : class, new()
		{
			json = json.Substring(1, json.Length - 2);
			var nodes = json.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).Select(v =>
			{
				var split = v.IndexOf(':');
				var key = v.Substring(0, split);

				var value = v.Substring(v.IndexOf("'", split + 1, StringComparison.OrdinalIgnoreCase) + 1).Trim();
				value = value.Substring(0, value.LastIndexOf("'", StringComparison.OrdinalIgnoreCase));

				return new JsonNode
				{
					Name = key.Trim(),
					Value = value.Trim()
				};
			});

			var item = new T();
			var someObjectType = item.GetType();

			foreach (var node in nodes)
			{
				var property = someObjectType.GetProperty(node.Name);
				if (property == null)
				{
					continue;
				}

				var value = TypeConverter.Convert(property.PropertyType, node.Value);
				property.SetValue(item, value, null);
			}

			return item;
		}
	}
}
