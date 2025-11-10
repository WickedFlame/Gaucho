using System.Collections.Generic;
using System.Diagnostics.Metrics;

namespace Gaucho.Server.Monitoring
{
    public static class TelemetryProvider
    {
        public const string MeterName = "Gaucho";
        private static readonly object _locker = new();

        internal static readonly Meter Meter = new(MeterName, typeof(TelemetryProvider).Assembly.GetName().Version?.ToString());

        private static readonly Dictionary<string, PipelineTelemetryMonitor> _monitors = [];

        internal static void WriteMeter(string pipelineId, IMetric metric)
        {
            var monitor = GetMonitor(pipelineId);

            monitor.WriteMetric(metric);
        }

        private static PipelineTelemetryMonitor GetMonitor(string pipelineId)
        {
            lock (_locker)
            {
                if (!_monitors.TryGetValue(pipelineId, out var monitor))
                {
                    monitor = new PipelineTelemetryMonitor(pipelineId, Meter);
                    _monitors.Add(pipelineId, monitor);
                }

                return monitor;
            }
        }
    }

    public class PipelineTelemetryMonitor
    {
        private int _queueSize;
        private int _threadCount;

        internal readonly Counter<long> ProcessedEventsCounter;
        internal readonly ObservableGauge<int> QueueSize;
        internal readonly ObservableGauge<int> ThreadCount;

        public PipelineTelemetryMonitor(string pipelineId, Meter meter)
        {
            ProcessedEventsCounter = meter.CreateCounter<long>($"{pipelineId}.{MetricType.ProcessedEvents}", unit: "count", description: "Number of processed events");
            QueueSize = meter.CreateObservableGauge<int>($"{pipelineId}.{MetricType.QueueSize}", () => _queueSize, unit: "count", description: "Number of items in the Queue");
            ThreadCount = meter.CreateObservableGauge<int>($"{pipelineId}.{MetricType.ThreadCount}", () => _threadCount, unit: "count", description: "Number of Threads processing the Queue");
        }


        public void WriteMetric(IMetric metric)
        {
            switch (metric.Key)
            {
                case MetricType.ProcessedEvents:
                    ProcessedEventsCounter.Add(1);
                    break;

                case MetricType.QueueSize:
                    _queueSize = metric.Value is int ? (int)metric.Value : int.Parse(metric.Value.ToString());
                    break;

                case MetricType.ThreadCount:
                    _threadCount = metric.Value is int ? (int)metric.Value : int.Parse(metric.Value.ToString());
                    break;

                case MetricType.WorkersLog:
                case MetricType.EventLog:
                    break;
            }
        }
    }
}
