using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gaucho
{
	public class EventData : IEventData, IEnumerable
	{
		private readonly List<Property> _properties;

		public EventData()
		{
			_properties = new List<Property>();
		}

		public IEnumerable<Property> Properties => _properties;

		public EventData Add(string key, object value)
		{
			_properties.Add(new Property(key, value));

			return this;
		}

		public IEnumerator GetEnumerator()
		{
			return _properties.GetEnumerator();
		}

		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine("{");
			foreach (var node in Properties)
			{
				sb.AppendLine(node.ToString());
			}

			sb.Append("}");

			return sb.ToString();
		}

		public object this[string source]
		{
			get
			{
				var property = _properties.FirstOrDefault(p => p.Key == source);
				if (property == null)
				{
					return null;
				}

				return property.Value;
			}
		}
	}
}
