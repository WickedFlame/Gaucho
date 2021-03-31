using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gaucho
{
	/// <summary>
	/// The EventData
	/// </summary>
	public class EventData : IEventData, IEnumerable<Property>
	{
		private readonly List<Property> _properties;
		
		/// <summary>
		/// Creates a new instance of EventData
		/// </summary>
		public EventData()
		{
			_properties = new List<Property>();
		}

		/// <summary>
		/// Add a property node to the EventData collection
		/// </summary>
		/// <param name="property"></param>
		/// <returns></returns>
		public EventData Add(Property property)
		{
			_properties.Add(property);

			return this;
		}

		/// <summary>
		/// Gets the enumerator of the collection
		/// </summary>
		/// <returns></returns>
		public IEnumerator<Property> GetEnumerator()
		{
			return _properties.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Returns the string representation of the EventData
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var sb = new StringBuilder();
			sb.AppendLine("{");
			foreach (var node in _properties)
			{
				sb.AppendLine(node.ToString());
			}

			sb.Append("}");

			return sb.ToString();
		}

		/// <summary>
		/// Gets the value of the property node
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
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
