using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho.Integration.Tests.Handlers
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
            var data = Converter.Convert(@event.Data);
            System.Diagnostics.Trace.WriteLine(data);
        }
    }
}
