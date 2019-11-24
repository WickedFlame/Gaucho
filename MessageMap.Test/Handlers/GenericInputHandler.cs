using System;
using System.Collections.Generic;
using System.Text;

namespace MessageMap.Test
{
    public class GenericInputHandler<T> : IInputHandler<T>
    {
        public Event ProcessInput(T input)
        {
            var factory = new EventDataFactory();
            var data = factory.BuildFrom(input);

            return new Event(PipelineId, data);
        }

        public string PipelineId { get; set; }
    }
}
