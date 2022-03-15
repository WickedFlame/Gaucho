using System;
using Gaucho.BackgroundTasks;
using Gaucho.Server.Monitoring;

namespace Gaucho.Diagnostics.MetricCounters
{
    /// <summary>
    /// BackgroundTask for writing Metrics and logs on the state of the <see cref="EventQueue"/>
    /// Included Metrics:
    /// - QueueSize
    /// - Count of processors
    /// </summary>
    public class EventBusMetricCounterTask : IBackgroundTask<EventBusContext>
    {
        private int _lastQueueSize = -1;
        private int _lastProcessorCount = -1;
        private readonly string _serverName;
        private readonly string _pipelineId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="pipelineId"></param>
        public EventBusMetricCounterTask(string serverName, string pipelineId)
        {
            _serverName = serverName;
            _pipelineId = pipelineId;
        }

        /// <summary>
        /// Execute the task. This should only be run as a separate thread to avoid endlesess loops
        /// </summary>
        /// <param name="context"></param>
        public void Execute(EventBusContext context)
        {
            //
            // This should only be run as a separate thread to avoid endlesess loops
            //

            while (context.IsRunning)
            {
                try
                {
                    context.WaitHandle.Reset();

                    var cnt = context.Queue.Count;

                    if (cnt != _lastQueueSize)
                    {
                        _lastQueueSize = cnt;

                        context.MetricService.SetMetric(new Metric(MetricType.QueueSize, "Events in Queue", _lastQueueSize));
                        context.Logger.Write($"[{_serverName}] [{_pipelineId}] Items in Queue count: {_lastQueueSize}", Category.Log, LogLevel.Info, source: "EventQueue");
                    }


                    cnt = context.Processors.Count;

                    if (cnt != _lastProcessorCount)
                    {
                        _lastProcessorCount = cnt;

                        context.MetricService.SetMetric(new Metric(MetricType.ThreadCount, "Active Workers", _lastProcessorCount));
                        context.Logger.Write($"[{_serverName}] [{_pipelineId}] Active Workers in EventBus: {_lastProcessorCount}", Category.Log, LogLevel.Info, source: "EventProcessor");
                    }

                    context.WaitHandle.WaitOne(TimeSpan.FromSeconds(context.MetricsPollingInterval));
                }
                catch
                {
                    // ignore all exceptions
                }
            }
        }
    }
}
