
namespace Gaucho
{
	public class EventDataInputHandler : IInputHandler<EventData>
	{
		public string PipelineId { get; set; }

		public Event ProcessInput(EventData input)
		{
			return new Event(PipelineId, input);
		}
	}
}
