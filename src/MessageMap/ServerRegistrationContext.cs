using System;

namespace MessageMap
{
    public class ServerRegistrationContext
    {
        private readonly ProcessingServer _server;
        private readonly string _pipelineId;

        internal ServerRegistrationContext(string pipelineId, ProcessingServer server)
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
            _server.Register(_pipelineId, eventBus);
        }

        public void Register(IInputHandler plugin)
        {
            _server.Register(_pipelineId, plugin);
        }
    }
}
