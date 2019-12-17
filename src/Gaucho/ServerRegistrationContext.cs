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
            _server.Register(_pipelineId, eventBus);
        }

        public void Register(IInputHandler plugin)
        {
            _server.Register(_pipelineId, plugin);
        }
    }
}
