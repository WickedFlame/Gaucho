
namespace Gaucho
{
	/// <summary>
	/// passthrough inputhandler for eventdata
	/// </summary>
	public class EventDataInputHandler : IInputHandler<EventData>
	{
		/// <summary>
		/// the id of the pipeline
		/// </summary>
		public string PipelineId { get; set; }

		/// <summary>
		/// process the input
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public Event ProcessInput(EventData input)
		{
			return new Event(PipelineId, (IEventData)input);
		}
	}
}
