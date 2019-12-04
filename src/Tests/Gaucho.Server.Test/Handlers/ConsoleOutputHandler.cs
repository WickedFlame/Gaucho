
using Microsoft.Extensions.Logging;

namespace Gaucho.Server.Test.Handlers
{
    public class ConsoleOutputHandler : IOutputHandler
    {
        static ConsoleOutputHandler()
        {
            
        }

        public IConverter Converter { get; set; } = new Converter();

        public void Handle(Event @event)
        {
            var data = Converter.Convert(@event.Data as EventData);
            System.Console.WriteLine(data);
        }
    }
}
