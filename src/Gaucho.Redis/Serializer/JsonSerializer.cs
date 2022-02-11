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
				Value = ConvertToString(propInfo, item)
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
			var nodes = json.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries).Select(v => StringToJsonNode(v));

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

        private string ConvertToString(PropertyInfo propInfo, object item)
        {
            var value = propInfo.GetValue(item, null);
            if (value == null)
            {
				return null;
            }

            if (propInfo.PropertyType == typeof(DateTime))
            {
                return ((DateTime)value).ToString("o");
            }

            return value.ToString();
        }

        private JsonNode StringToJsonNode(string v)
        {
            try
            {
                var split = v.IndexOf(':');
                if (split < 0)
                {
                    return new JsonNode
                    {
                        Name = string.Empty,
                        Value = v
                    };
                }

                var key = v.Substring(0, split);

                var value = v.Substring(v.IndexOf("'", split + 1, StringComparison.OrdinalIgnoreCase) + 1).Trim();

                var index = value.LastIndexOf("'", StringComparison.OrdinalIgnoreCase);
                value = index > 0 ? value.Substring(0, index) : value;

                return new JsonNode
                {
                    Name = key.Trim(),
                    Value = value.Trim()
                };
            }
            catch (Exception ex)
            {
                return new JsonNode
                {
                    Name = "Error",
                    Value = v
                };
            }
        }
	}
}
