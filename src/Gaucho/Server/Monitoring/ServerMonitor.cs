using System.Collections.Generic;
using System.Linq;

namespace Gaucho.Server.Monitoring
{
    public interface IServerMonitor
    {
        IEnumerable<PipelineMonitor> GetPipelines();
    }

    public class ServerMonitor : IServerMonitor
    {
        private readonly IProcessingServer _server;

        public ServerMonitor(IProcessingServer server)
        {
            _server = server;
        }

        public IEnumerable<PipelineMonitor> GetPipelines()
        {
            var pipelines = _server.EventBusFactory.Pipelines;

            return pipelines.Select(p => _server.Monitor(p));
        }
    }
}
