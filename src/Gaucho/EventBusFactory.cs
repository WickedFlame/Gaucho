using System;
using System.Collections.Generic;
using Gaucho.Server.Monitoring;

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

        void Register(string pipelineId, Func<IEventPipeline> factory);

        void Register(string pipelineId, IEventBus eventBus);

		/// <summary>
		/// get the eventbus associated with the pipeline
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <returns></returns>
        IEventBus GetEventBus(string pipelineId);
    }

    public class EventBusFactory : IEventBusFactory
    {
	    private static object _lock = new object();

        private readonly Dictionary<string, IPipelineFactory> _pipelineRegistrations;
        private readonly Dictionary<string, IEventBus> _activeEventBus;

        public EventBusFactory()
        {
            _pipelineRegistrations = new Dictionary<string, IPipelineFactory>();
            _activeEventBus = new Dictionary<string, IEventBus>();
        }

		/// <summary>
		/// Gets a list of all registered pipelines in the factory
		/// </summary>
		public IEnumerable<string> Pipelines => _pipelineRegistrations.Keys;

        public void Register(string pipelineId, Func<IEventPipeline> factory)
        {
			lock(_lock)
			{
				_pipelineRegistrations[pipelineId] = new PipelineFactory(factory);

				if (_activeEventBus.ContainsKey(pipelineId))
				{
					// close active pipeline
					// the next Push event will get the new pipeline
					var activeEventBus = _activeEventBus[pipelineId];
					_activeEventBus.Remove(pipelineId);
					activeEventBus.Close();
				}
			}
        }

        public void Register(string pipelineId, IEventBus eventBus)
        {
			lock(_lock)
			{
				if (_activeEventBus.ContainsKey(pipelineId))
				{
					throw new Exception($"Pipeline {pipelineId} is already running. The EventBus for the Pipeline {pipelineId} cannot be changed.");
				}

				if (pipelineId != eventBus.PipelineId)
				{
					throw new Exception($"The EventBus with PipelineId {eventBus.PipelineId} cannot be registered to the pipeline {pipelineId}");
				}

				var factory = _pipelineRegistrations[pipelineId];
				eventBus.SetPipeline(factory);

				_activeEventBus[pipelineId] = eventBus;
			}
        }

        /// <summary>
        /// get the eventbus associated with the pipeline
        /// </summary>
        /// <param name="pipelineId"></param>
        /// <returns></returns>
		public IEventBus GetEventBus(string pipelineId)
        {
			lock(_lock)
			{
				if (!_activeEventBus.ContainsKey(pipelineId))
				{
					var factory = _pipelineRegistrations[pipelineId];
					var eventBus = new EventBus(factory, pipelineId);

					_activeEventBus[pipelineId] = eventBus;
				}

				return _activeEventBus[pipelineId];
			}
        }
    }
}
