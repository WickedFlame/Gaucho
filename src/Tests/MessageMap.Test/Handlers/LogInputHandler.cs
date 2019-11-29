using System;
using System.Collections.Generic;
using System.Text;

namespace MessageMap.Test
{
    public class LogInputHandler : IInputHandler<LogMessage>
    {
        public LogInputHandler()
        {
        }

        public IConverter Converter { get; set; }

        public string PipelineId { get; set; }

        public Event ProcessInput(LogMessage message)
        {
            var data = new EventData
            {
                {"msg", message.Message}
            };

            // transform data
            var @event = new Event(PipelineId, data);

            return @event;
        }
    }
}
