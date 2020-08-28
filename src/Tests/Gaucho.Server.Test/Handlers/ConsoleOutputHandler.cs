
using Microsoft.Extensions.Logging;

namespace Gaucho.Server.Test.Handlers
{
    public class ConsoleOutputHandler : IOutputHandler
    {
        public ConsoleOutputHandler(IEventDataConverter converter)
        {
            Converter = converter;
        }

        public IEventDataConverter Converter { get; }

        public void Handle(Event @event)
        {
            var data = Converter.Convert(@event.Data);
            System.Console.WriteLine(data);
        }
    }
}
