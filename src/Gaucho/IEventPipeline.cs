using System.Collections.Generic;

namespace Gaucho
{
	/// <summary>
	/// The interface for EventPipelines
	/// </summary>
	public interface IEventPipeline
	{
		/// <summary>
		/// gets a list of all outputhandlers
		/// </summary>
		IEnumerable<IOutputHandler> Handlers { get; }

		/// <summary>
		/// add a outputhandler to the pipeline
		/// </summary>
		/// <param name="outputHandler"></param>
		void AddHandler(IOutputHandler outputHandler);

		/// <summary>
		/// run the event through all registered handlers
		/// </summary>
		/// <param name="event"></param>
		void Run(Event @event);
	}
}
