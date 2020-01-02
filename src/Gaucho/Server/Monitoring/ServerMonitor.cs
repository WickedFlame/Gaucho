using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

            return pipelines.Select(p =>
            {
                return _server.Monitor(p);
            });
        }
    }

    public class PipelineMonitor
    {
        public PipelineMonitor(string name, int threads, int queued, int handlerCount)
        {
            Name = name;
            HandlerCount = handlerCount;
            Threads = threads;
            QueueCount = queued;
        }

        public string Name { get; }

        public int HandlerCount { get; }

        public int Threads { get; }

        public int QueueCount { get; }

        public int ProcessedCount { get; }
    }
}
