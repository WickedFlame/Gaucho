using System.Collections;
using System.Collections.Generic;

namespace Gaucho
{
    public interface IEventPipeline
    {
        string Id { get; set; }

        IEnumerable<IOutputHandler> Handlers { get; }

        void AddHandler(IOutputHandler outputHandler);

        void Run(Event @event);
    }

    public class EventPipeline : IEventPipeline
    {
        private readonly IList<IOutputHandler> _handlers;

        public EventPipeline()
        {
            _handlers = new List<IOutputHandler>();
        }

        public string Id { get; set; }

        public IEnumerable<IOutputHandler> Handlers => _handlers;

        // the configuration of a input to output stream
        public void AddHandler(IOutputHandler outputHandler)
        {
            _handlers.Add(outputHandler);
        }

        public void Run(Event @event)
        {
            foreach (var handler in _handlers)
            {
                handler.Handle(@event);
            }
        }
    }
}
