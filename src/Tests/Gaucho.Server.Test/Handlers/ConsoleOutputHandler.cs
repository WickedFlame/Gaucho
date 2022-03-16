
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Gaucho.Server.Test.Handlers
{
    public class ConsoleOutputHandler : IOutputHandler
    {
	    private readonly IEventDataConverter _converter;

	    public ConsoleOutputHandler(IEventDataConverter converter)
	    {
		    _converter = converter;
	    }

		public void Handle(Event @event)
		{
			_converter.AppendFormated(@event.Data);

			//System.Diagnostics.Debug.WriteLine(@event.Data);
        }
    }
}
