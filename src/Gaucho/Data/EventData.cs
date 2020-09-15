using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Diagnostics;

namespace Gaucho
{
	public class EventData : IEventData, IEnumerable
	{
		private readonly List<Property> _properties;
		private ILogger _logger;

		public EventData()
		{
			_properties = new List<Property>();
		}

		public IEnumerable<Property> Properties => _properties;

		public EventData Add(Property property)
		{
			_properties.Add(property);

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
					if(_logger== null)
					{
						_logger = LoggerConfiguration.Setup();
					}

					var msg = new StringBuilder()
						.AppendLine($"Could not find property '{source}'");

					var candidates = _properties.Where(p => p.Key.ToLower() == source.ToLower());
					if (candidates.Any())
					{
						msg.AppendLine($"   Possible candidates are:");
						foreach (var candidate in candidates)
						{
							msg.AppendLine($"   - {candidate.Value}");
						}
					}

					_logger.Write(msg.ToString(), Category.Log, LogLevel.Debug, "EventData");

					return null;
				}

				return property.Value;
			}
		}
	}
}
