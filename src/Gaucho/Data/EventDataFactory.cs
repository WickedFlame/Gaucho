using System;
using System.Collections;

namespace Gaucho
{
	/// <summary>
	/// Factory for creating <see cref="EventData"/> from any object
	/// </summary>
	public class EventDataFactory
    {
		/// <summary>
		/// Initializes a new instance of the EventDataFactory
		/// </summary>
	    public EventDataFactory()
	    {
		    IsValueType = (type) => (type.IsValueType || type == typeof(string)) && !type.IsGenericType;
	    }

		/// <summary>
		/// Build <see cref="EventData"/> from a given object
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="input"></param>
		/// <returns></returns>
        public EventData BuildFrom<T>(T input)
        {
            var data = new EventData();

            if (typeof(T).IsGenericType && input is IDictionary dict)
            {
	            foreach (DictionaryEntry entry in dict)
	            {
		            data.Add(entry.Key.ToString(), entry.Value);
	            }

				return data;
			}

            AppendObject(data, input, null);
            
            return data;
        }


        private void AppendObject(EventData data, object input, string parentName)
        {
	        if (input == null)
	        {
		        return;
	        }

	        foreach (var property in input.GetType().GetProperties())
	        {
		        var name = string.IsNullOrEmpty(parentName) ? property.Name : $"{parentName}.{property.Name}";
		        var value = property.GetValue(input);

		        if (value != null && !IsValueType(value.GetType()))
		        {
			        AppendObject(data, value, name);
			        continue;
		        }

		        data.Add(name, value);
	        }
		}

        /// <summary>
        /// Gets the Evaluator used by the Mapper for value types. Value types are simply added to the match with ToString()
        /// </summary>
        public Func<Type, bool> IsValueType { get; internal set; }
	}
}
