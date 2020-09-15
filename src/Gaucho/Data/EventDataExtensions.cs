using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Gaucho
{
	public static class EventDataExtensions
	{
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
	}
}
