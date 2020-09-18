using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Gaucho
{
    public class EventDataFactory
    {
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

            foreach (var property in typeof(T).GetProperties())
            {
                var name = property.Name;
                var value = property.GetValue(input);

                data.Add(name, value);
            }

            return data;
        }
    }
}
