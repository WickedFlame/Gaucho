using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho.Test
{
    public class ConsoleOutputHandler : IOutputHandler
    {
        public ConsoleOutputHandler() 
            : this(new EventDataConverter())
        {
        }

        public ConsoleOutputHandler(IEventDataConverter converter)
        {
            Converter = converter;
        }

        public IEventDataConverter Converter { get; }

        public void Handle(Event @event)
        {
            var data = Converter.Convert(@event.Data as EventData);
            System.Diagnostics.Trace.WriteLine(data);
        }
    }
}
