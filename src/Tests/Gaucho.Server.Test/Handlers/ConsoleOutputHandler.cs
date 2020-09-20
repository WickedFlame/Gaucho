
using Microsoft.Extensions.Logging;

namespace Gaucho.Server.Test.Handlers
{
    public class ConsoleOutputHandler : IOutputHandler
    {
        public void Handle(Event @event)
        {
            System.Console.WriteLine(@event.Data);
        }
    }
}
