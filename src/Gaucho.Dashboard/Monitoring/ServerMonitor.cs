using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Gaucho.Configuration;
using Gaucho.Diagnostics;
using Gaucho.Diagnostics.MetricCounters;
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
	        var options = GlobalConfiguration.Configuration.Resolve<Options>();

			var defaultkeys = new List<MetricType> {MetricType.ThreadCount, MetricType.QueueSize, MetricType.ProcessedEvents};

            var statistics = new StatisticsApi(pipelineId);
            var metrics = new PipelineMetric(pipelineId);

            metrics.AddMetric("Server", "Server", options.ServerName ?? Environment.MachineName);

            foreach (var key in defaultkeys)
            {
                var metric = statistics.FirstOrDefault(s => s.Key == key);
                metrics.AddMetric(key.ToString(), metric?.Title, metric?.Value);
            }

            foreach (var metric in statistics)
            {
                if (defaultkeys.Contains(metric.Key))
                {
                    continue;
                }

                switch (metric.Key)
                {
					case MetricType.EventLog:
						//LogEventStatisticWriter.cs
						//if (metric.Factory.Invoke() is List<ILogMessage> logs)
						//{
						//	foreach(var log in logs.OrderByDescending(l => l.Timestamp).Take(20).OrderBy(l => l.Timestamp))
						//	{
						//		metrics.AddElement(metric.Key.ToString(), metric.Title, new DashboardLog
						//		{
						//			Timestamp = log.Timestamp,
						//			Source = log.Source,
						//			Level = log.Level.ToString(),
						//			Message = log.Message
						//		});
						//	}
						//}
						break;

					case MetricType.WorkersLog:
						//WorkersLogMetricCounter.cs
						//if (metric.Factory.Invoke() is IEnumerable<WorkerCountMetric> workers)
						//{
						//	foreach (var worker in workers.OrderBy(w => w.Timestamp).Take(50))
						//	{
						//		metrics.AddElement(metric.Key.ToString(), metric.Title, new TimelineLog<int>
						//		{
						//			Timestamp = worker.Timestamp,
						//			Value = worker.ActiveWorkers
						//		});
						//	}
						//}
						break;

					default:
						metrics.AddMetric(metric.Key.ToString(), metric.Title, metric.Value ?? 0);
						break;
                }
            }

            return metrics;
        }
    }
}
