using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho
{
	/// <summary>
	/// Interface fot the EventBusFactory
	/// </summary>
	public interface IEventBusFactory
	{
		/// <summary>
		/// Gets a list of all registered pipelines in the factory
		/// </summary>
		IEnumerable<string> Pipelines { get; }

		/// <summary>
		/// Register a new <see cref="IEventPipeline"/> factory to the EventBusFactory
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="factory"></param>
		/// <param name="options"></param>
		void Register(string pipelineId, Func<IEventPipeline> factory, PipelineOptions options);

		/// <summary>
		/// Register a new <see cref="IEventBus"/> to the EventBusFactory
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="eventBus"></param>
		void Register(string pipelineId, IEventBus eventBus);

		/// <summary>
		/// get the eventbus associated with the pipeline
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <returns></returns>
		IEventBus GetEventBus(string pipelineId);
	}
}
