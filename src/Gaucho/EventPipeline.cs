using System.Collections;
using System.Collections.Generic;

namespace Gaucho
{
    public class EventPipeline : IEventPipeline
    {
        private readonly IList<IOutputHandler> _handlers;

        public EventPipeline()
        {
            _handlers = new List<IOutputHandler>();
        }

        /// <summary>
        /// gets a list of all outputhandlers
        /// </summary>
		public IEnumerable<IOutputHandler> Handlers => _handlers;

		// the configuration of a input to output stream
		/// <summary>
		/// add a outputhandler to the pipeline
		/// </summary>
		/// <param name="outputHandler"></param>
		public void AddHandler(IOutputHandler outputHandler)
        {
            _handlers.Add(outputHandler);
        }

		/// <summary>
		/// run the event through all registered handlers
		/// </summary>
		/// <param name="event"></param>
        public void Run(Event @event)
        {
            foreach (var handler in _handlers)
            {
                handler.Handle(@event);
            }
        }
    }
}
