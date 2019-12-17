
namespace Gaucho
{
    public class EventDispatcher
    {
        private readonly IProcessingServer _server;

        public EventDispatcher() : this(ProcessingServer.Server) { }

        public EventDispatcher(IProcessingServer server)
        {
            _server = server;
        }

        public void Process<T>(string pipelineId, T item)
        {
            var plugin = _server.GetHandler<T>(pipelineId);
            var @event = plugin.ProcessInput(item);

            _server.Publish(@event);
        }
    }
}
