using System;
using System.Linq;
using System.Linq.Expressions;

namespace Gaucho
{
	/// <summary>
	/// Extensions for <see cref="EventData"/>
	/// </summary>
	public static class EventDataExtensions
	{
		/// <summary>
		/// Add a new value to the data. The name of the field is taken from the propertyname
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="exp"></param>
		/// <returns></returns>
		public static EventData Add(this EventData eventData, Expression<Func<object>> exp)
		{
			var name = GetName(exp);
			var value = exp.Compile().Invoke()?.ToString();

			if (name == null || value == null)
			{
				//_logger = LoggerConfiguration.Setup();
				return eventData;
			}

			return eventData.Add(name, value);
		}

		private static string GetName(Expression<Func<object>> exp)
		{
			if (exp.Body is MemberExpression member)
			{
				return member.Member.Name;
			}

			if (exp.Body is UnaryExpression unary)
			{
				member = unary.Operand as MemberExpression;
				if (member != null)
				{
					return member.Member.Name;
				}
			}

			return null;
		}

		/// <summary>
		/// Add a new item to the data
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public static EventData Add(this EventData eventData, string key, object value)
		{
			eventData.Add(new Property(key, value));

			return eventData;
		}

		/// <summary>
		/// Check if the key is already contained in the data
		/// </summary>
		/// <param name="eventData"></param>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool Contains(this EventData eventData, string key)
		{
			return eventData.Any(d => d.Key == key);
		}
	}
}
