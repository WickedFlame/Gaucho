using System;

namespace Gaucho
{
    public class ServerRegistrationContext
    {
        private readonly IProcessingServer _server;
        private readonly string _pipelineId;

        internal ServerRegistrationContext(string pipelineId, IProcessingServer server)
        {
            _pipelineId = pipelineId;
            _server = server;
        }

        public void Register(Func<EventPipeline> factory)
        {
            _server.Register(_pipelineId, factory);
        }

        public void Register(IEventBus eventBus)
        {
            if (_pipelineId != eventBus.PipelineId)
            {
                throw new Exception($"The EventBus with PipelineId {eventBus.PipelineId} cannot be registered to the pipeline {_pipelineId}");
            }

            _server.Register(_pipelineId, eventBus);
        }

        public void Register(IInputHandler plugin)
        {
            if (plugin == null)
            {
                return;
            }

            _server.Register(_pipelineId, plugin);
        }
    }
}
