
namespace Gaucho
{
    public class Client
    {
        private readonly ProcessingServer _server;

        public Client() : this(ProcessingServer.Server) { }

        public Client(ProcessingServer server)
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
