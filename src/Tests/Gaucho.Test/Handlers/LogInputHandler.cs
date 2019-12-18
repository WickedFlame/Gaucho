using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho.Test
{
    public class LogInputHandler : IInputHandler<LogMessage>
    {
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
