using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gaucho.Server.Monitoring;

namespace Gaucho.Dashboard.Monitoring
{
    public interface IServerMonitor
    {
        IEnumerable<PipelineMetric> GetPipelineMetrics();
        
        IEnumerable<string> GetPipelines();
    }

    public class ServerMonitor : IServerMonitor
    {
        private readonly IProcessingServer _server;

        public ServerMonitor(IProcessingServer server)
        {
            _server = server;
        }

        public IEnumerable<string> GetPipelines()
        {
            return _server.EventBusFactory.Pipelines;
        }

        public IEnumerable<PipelineMetric> GetPipelineMetrics()
        {
            var pipelines = _server.EventBusFactory.Pipelines;

            return pipelines.Select(p => Monitor(p));
        }

        public PipelineMetric Monitor(string pipelineId)
        {
            var statistics = new StatisticsApi(pipelineId);
            
            var metrics = new PipelineMetric(pipelineId)
            {
                //ProcessedCount = (int)(statistics.GetMetricValue(MetricType.ProcessedEvents) ?? 0),
                //Threads = (int)(statistics.GetMetricValue(MetricType.ThreadCount) ?? 0),
                //QueueSize = (int)(statistics.GetMetricValue(MetricType.QueueSize) ?? 0)
            };

            foreach (var metric in statistics)
            {
                metrics.Add(metric.Key.ToString(), metric.Title, metric.Factory.Invoke() ?? 0);
            }

            return metrics;
        }
    }
}
