using System.Collections.Generic;
using System.Linq;
using Gaucho.Server.Monitoring;

namespace Gaucho.Dashboard.Monitoring
{
    public interface IServerMonitor
    {
        IEnumerable<PipelineMetric> GetPipelines();
    }

    public class ServerMonitor : IServerMonitor
    {
        private readonly IProcessingServer _server;

        public ServerMonitor(IProcessingServer server)
        {
            _server = server;
        }

        public IEnumerable<PipelineMetric> GetPipelines()
        {
            var pipelines = _server.EventBusFactory.Pipelines;

            return pipelines.Select(p => Monitor(p));
        }

        public PipelineMetric Monitor(string pipelineId)
        {
            var statistics = new StatisticsApi(pipelineId);
            
            return new PipelineMetric(pipelineId)
            {
                ProcessedCount = (int)(statistics.GetMetricValue(MetricType.ProcessedEvents) ?? 0),
                Threads = (int)(statistics.GetMetricValue(MetricType.ThreadCount) ?? 0),
                QueueSize = (int)(statistics.GetMetricValue(MetricType.QueueSize) ?? 0)
            };
        }
    }
}
