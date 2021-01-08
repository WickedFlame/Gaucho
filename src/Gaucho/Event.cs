using System;

namespace Gaucho
{
	/// <summary>
	/// a event containing the data
	/// </summary>
    public class Event
    {
		/// <summary>
		/// creates a new instance of a event
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="data"></param>
		public Event(string pipelineId, SimpleData data)
			: this(pipelineId, (IEventData)data)
		{
		}

		/// <summary>
		/// creates a new instance of a event
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="data"></param>
		public Event(string pipelineId, Func<EventDataFactory, IEventData> data)
			: this(pipelineId, data(new EventDataFactory()))
		{
		}

		/// <summary>
		/// creates a new instance of a event
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="data"></param>
		public Event(string pipelineId, IEventData data)
        {
            PipelineId = pipelineId;
            Data = data;

            Id = Guid.NewGuid().ToString();
        }

		/// <summary>
		/// unique id for the event
		/// </summary>
        public string Id { get; }

		/// <summary>
		/// id of the pipeline
		/// </summary>
        public string PipelineId { get; }

		/// <summary>
		/// the data
		/// </summary>
        public IEventData Data { get; }
    }
}
