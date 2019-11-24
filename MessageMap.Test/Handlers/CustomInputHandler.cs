using System;
using System.Collections.Generic;
using System.Text;

namespace MessageMap.Test
{
    public class CustomInputHandler : IInputHandler<LogMessage>
    {
        public string PipelineId { get; set; }

        public Event ProcessInput(LogMessage input)
        {
            var factory = new EventDataFactory();
            var data = factory.BuildFrom(input);

            return new Event(PipelineId, data);
        }
    }

    public class LogMessage
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public string Level { get; set; }
    }
}
