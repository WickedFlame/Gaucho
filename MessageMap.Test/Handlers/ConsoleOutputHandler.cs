using System;
using System.Collections.Generic;
using System.Text;

namespace MessageMap.Test
{
    public class ConsoleOutputHandler : IOutputHandler
    {
        public IConverter Converter { get; set; } = new Converter();

        public void Handle(Event @event)
        {
            var data = Converter.Convert(@event.Data as EventData);
            System.Diagnostics.Trace.WriteLine(data);
        }
    }
}
