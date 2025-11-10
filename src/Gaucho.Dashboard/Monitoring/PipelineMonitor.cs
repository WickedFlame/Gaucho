using Gaucho.Diagnostics;
using Gaucho.Diagnostics.MetricCounters;
using Gaucho.Server.Monitoring;
using Gaucho.Storage;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gaucho.Dashboard.Monitoring
{
	/// <summary>
	/// Monitor object for all pipelines
	/// </summary>
	public class PipelineMonitor : IPipelineMonitor
    {
		/// <summary>
		/// Get or set the <see cref="DashboardOptions"/>
		/// </summary>
		public DashboardOptions Options { get; set; } = new DashboardOptions();

		/// <summary>
		/// Gets all Metrics
		/// </summary>
		/// <returns></returns>
		public IEnumerable<PipelineMetric> GetMetrics()
        {
	        var storage = GlobalConfiguration.Configuration.Resolve<IStorage>();

	        var pipelines = new List<PipelineModel>();

	        var keys = storage.GetKeys(new StorageKey("server:"));
	        var servers = keys.Select(key => storage.Get<ServerModel>(new StorageKey(key)));
	        foreach (var server in servers.OrderByDescending(s => s.Heartbeat))
	        {
		        keys = storage.GetKeys(new StorageKey($"{server.Name}:pipeline:"));
		        pipelines.AddRange(keys.Select(key => storage.Get<PipelineModel>(new StorageKey(key))));
	        }

            return pipelines.Select(p => Monitor(p, storage)).OrderByDescending(m => m.Hearbeat);
        }
		
        private PipelineMetric Monitor(PipelineModel pipeline, IStorage storage)
        {
			var defaultMetrics = new List<MetricType> {MetricType.ThreadCount, MetricType.QueueSize, MetricType.ProcessedEvents};
			var server = pipeline.ServerName ?? Environment.MachineName;

			var heartbeat = storage.Get<ServerModel>(new StorageKey($"server:{server.ToLower()}"));

			var statistics = new StatisticsApi(server, pipeline.PipelineId, storage);
            var metrics = new PipelineMetric(pipeline.PipelineId, server, heartbeat != null ? DateTime.Parse(heartbeat.Heartbeat) : (DateTime?)null);

            metrics.AddMetric("Server", "Server", server);
            metrics.AddMetric("Heartbeat", "Heartbeat", heartbeat != null ? DateTime.Parse(heartbeat.Heartbeat).ToString("yyyy.MM.dd HH:mm:ss") : "");

            foreach (var key in defaultMetrics)
            {
                var metric = statistics.FirstOrDefault(s => s.Key == key);
                if (metric == null)
                {
	                metric = new Metric(key, "", "");

                }
                metrics.AddMetric(key.ToString(), metric?.Title, metric?.Value);
            }
			
            foreach (var metric in statistics)
            {
                if (defaultMetrics.Contains(metric.Key))
                {
                    continue;
                }

                switch (metric.Key)
                {
					case MetricType.EventLog:
						//LogEventStatisticWriter.cs
						if (metric.Value is IEnumerable<LogEvent> logs)
						{
							foreach (var log in logs.OrderByDescending(l => l.Timestamp).Take(Options.LogCount))
							{
								metrics.AddElement(metric.Key.ToString(), metric.Title, new DashboardLog
								{
									Timestamp = log.Timestamp,
									Source = log.Source,
									Level = log.Level.ToString(),
									Message = log.Message
								});
							}
						}
						break;

					case MetricType.WorkersLog:
						//WorkersLogMetricCounter.cs
						if (metric.Value is IEnumerable<ActiveWorkersLogMessage> workers)
						{
							foreach (var worker in workers.OrderByDescending(w => w.Timestamp).Take(Options.WorkersLogCount))
							{
								metrics.AddElement(metric.Key.ToString(), metric.Title, new TimelineLog<int>
								{
									Timestamp = worker.Timestamp,
									Value = worker.ActiveWorkers
								});
							}
						}
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
