using System;

namespace Gaucho
{
	public interface IEventBus : IDisposable
	{
		/// <summary>
		/// Gets the PipelineId
		/// </summary>
		string PipelineId { get; }

		/// <summary>
		/// Gets the pipelinefactory that creates the pipeline for this eventbus
		/// </summary>
		IPipelineFactory PipelineFactory { get; }

		/// <summary>
		/// Set the pipelinefactory
		/// </summary>
		/// <param name="factory"></param>
		void SetPipeline(IPipelineFactory factory);

		/// <summary>
		/// Publish an event to the processingqueue
		/// </summary>
		/// <param name="event"></param>
		void Publish(Event @event);

		/// <summary>
		/// The pipeline will be closed after all queued events are processed.
		/// </summary>
		void Close();
	}

	public interface IAsyncEventBus : IEventBus
	{
		/// <summary>
		/// Waits for all processors to end the work
		/// </summary>
		void WaitAll();
	}
}
