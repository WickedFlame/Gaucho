using System;
using System.Collections.Generic;

namespace Gaucho
{
	/// <summary>
	/// Interface for the ProcessingServer
	/// </summary>
	public interface IProcessingServer
	{
		/// <summary>
		/// Gets the associated EventBusFactory
		/// </summary>
		IEventBusFactory EventBusFactory { get; }

		/// <summary>
		/// gets all registered inputhandlers
		/// </summary>
		IEnumerable<IInputHandler> InputHandlers { get; }

		/// <summary>
		/// Register an EventPipeline to the pipeline
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="factory"></param>
		void Register(string pipelineId, Func<EventPipeline> factory);

		/// <summary>
		/// Register an EventBus to the pipeline
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="eventBus"></param>
		void Register(string pipelineId, IEventBus eventBus);

		/// <summary>
		/// Register an InputHandler to the pipeline
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="plugin"></param>
		void Register(string pipelineId, IInputHandler plugin);

		/// <summary>
		/// Get the InputHandler for the pipeline
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pipelineId"></param>
		/// <returns></returns>
		IInputHandler<T> GetHandler<T>(string pipelineId);

		/// <summary>
		/// Publish an envent to the Pipeline
		/// </summary>
		/// <param name="event"></param>
		void Publish(Event @event);

		/// <summary>
		/// Wait for all events to be handled in the pipeline
		/// </summary>
		/// <param name="pipelineId">The Pipeline to wait for</param>
		void WaitAll(string pipelineId);
	}
}
