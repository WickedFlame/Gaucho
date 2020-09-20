
using System.Runtime.CompilerServices;

namespace Gaucho
{
	public class SimpleData : IEventData
	{
		private SimpleData(object value)
		{
			Value = value;
		}

		public object Value { get; }

		public override string ToString()
		{
			return Value.ToString();
		}

		public static implicit operator SimpleData(string value) => From(value);

		public static implicit operator SimpleData(int value) => From(value);

		public static implicit operator SimpleData(double value) => From(value);

		public static SimpleData From(object value) => new SimpleData(value);
	}
}
