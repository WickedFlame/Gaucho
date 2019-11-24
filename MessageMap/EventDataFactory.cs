using System;
using System.Collections.Generic;
using System.Text;

namespace MessageMap
{
    public class EventDataFactory
    {
        public EventData BuildFrom<T>(T input)
        {
            var data = new EventData();

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
