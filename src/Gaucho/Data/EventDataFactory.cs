using System;
using System.Collections;
using System.Collections.Generic;

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

            AppendObject(data, input, null);
            
            return data;
        }
        
        private void AppendObject(EventData data, object input, string parentName)
        {
	        if (input == null)
	        {
		        return;
	        }

            if (AppendDictionary(data, input, parentName))
            {
                return;
            }

            if (AppendArray(data, input, parentName))
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

        private bool AppendDictionary(EventData data, object input, string parentName)
        {
            if (!input.GetType().IsGenericType || !(input is IDictionary dict))
            {
                return false;
            }

            foreach (DictionaryEntry entry in dict)
            {
                var name = string.IsNullOrEmpty(parentName) ? entry.Key.ToString() : $"{parentName}.{entry.Key}";
                if (entry.Value is IDictionary sub)
                {
                    AppendObject(data, sub, name);
                }
                else
                {
                    data.Add(name, entry.Value);
                }
            }

            return true;
        }

        private bool AppendArray(EventData data, object input, string parentName)
        {
            if (!(input is IEnumerable enumerable))
            {
                return false;
            }

            var lst = new List<EventData>();
            foreach (var item in enumerable)
            {
                var tmp = new EventData();
                AppendObject(tmp, item, null);
                lst.Add(tmp);
            }

            data.Add(parentName, lst.ToArray());

            return true;
        }

        /// <summary>
        /// Gets the Evaluator used by the Mapper for value types. Value types are simply added to the match with ToString()
        /// </summary>
        public Func<Type, bool> IsValueType { get; internal set; }
	}
}
