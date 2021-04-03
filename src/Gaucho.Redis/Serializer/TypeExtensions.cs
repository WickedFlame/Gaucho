using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho.Redis.Serializer
{
	/// <summary>
	/// Extensions for Type
	/// </summary>
	public static class TypeExtensions
	{
		/// <summary>
		/// Checks if the object is a primitive type
		/// </summary>
		/// <param name="item"></param>
		/// <returns></returns>
		public static bool IsPrimitiveType(this object item)
		{
			var type = item.GetType();
			return type.IsPrimitive || type == typeof(string) || type == typeof(Guid);
		}
	}
}
