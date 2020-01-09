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
            var defaultkeys = new List<MetricType> {MetricType.ThreadCount, MetricType.QueueSize, MetricType.ProcessedEvents};

            var statistics = new StatisticsApi(pipelineId);
            var metrics = new PipelineMetric(pipelineId);

            foreach (var key in defaultkeys)
            {
                var metric = statistics.FirstOrDefault(s => s.Key == key);
                metrics.Add(key.ToString(), metric?.Title, metric?.Factory.Invoke());
            }

            foreach (var metric in statistics)
            {
                if (defaultkeys.Contains(metric.Key))
                {
                    continue;
                }

                metrics.Add(metric.Key.ToString(), metric.Title, metric.Factory.Invoke() ?? 0);
            }

            return metrics;
        }
    }
}
