using System;
using System.Collections.Generic;

namespace MessageMap
{
    public interface IEventBusFactory
    {
        void Register(string pipelineId, Func<IEventPipeline> factory);

        void Register(string pipelineId, IEventBus eventBus);

        IEventBus GetEventBus(string pipelineId);
    }

    public class EventBusFactory : IEventBusFactory
    {
        private readonly Dictionary<string, IPipelineSetup> _pipelineRegistrations;
        private readonly Dictionary<string, IEventBus> _activeEventBus;

        public EventBusFactory()
        {
            _pipelineRegistrations = new Dictionary<string, IPipelineSetup>();
            _activeEventBus = new Dictionary<string, IEventBus>();
        }

        public void Register(string pipelineId, Func<IEventPipeline> factory)
        {
            _pipelineRegistrations[pipelineId] = new PipelineSetup(factory);
        }

        public void Register(string pipelineId, IEventBus eventBus)
        {
            if (_activeEventBus.ContainsKey(pipelineId))
            {
                throw new Exception($"Pipeline {pipelineId} is already running. The EventBus for the Pipeline {pipelineId} cannot be changed.");
            }

            var factory = _pipelineRegistrations[pipelineId];
            eventBus.SetPipeline(factory);
            eventBus.PipelineId = pipelineId;

            _activeEventBus[pipelineId] = eventBus;
        }

        public IEventBus GetEventBus(string pipelineId)
        {
            if (!_activeEventBus.ContainsKey(pipelineId))
            {
                var factory = _pipelineRegistrations[pipelineId];
                var eventBus = new EventBus(factory)
                {
                    PipelineId = pipelineId
                };

                _activeEventBus[pipelineId] = eventBus;
            }

            return _activeEventBus[pipelineId];
        }
    }
}
