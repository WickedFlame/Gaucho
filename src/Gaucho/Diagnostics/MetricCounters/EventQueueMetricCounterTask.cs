using System;
using Gaucho.BackgroundTasks;
using Gaucho.Server.Monitoring;

namespace Gaucho.Diagnostics.MetricCounters
{
    /// <summary>
    /// BackgroundTask for writing Metrics and logs on the state of the <see cref="EventQueue"/>
    /// </summary>
    public class EventQueueMetricCounterTask : IBackgroundTask<EventQueueContext>
    {
        private int _lastSize = -1;
        private readonly string _serverName;
        private readonly string _pipelineId;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverName"></param>
        /// <param name="pipelineId"></param>
        public EventQueueMetricCounterTask(string serverName, string pipelineId)
        {
            _serverName = serverName;
            _pipelineId = pipelineId;
        }

        /// <summary>
        /// Execute the task. This should only be run as a separate thread to avoid endlesess loops
        /// </summary>
        /// <param name="context"></param>
        public void Execute(EventQueueContext context)
        {
            //
            // This should only be run as a separate thread to avoid endlesess loops
            //

            while (context.IsRunning)
            {
                context.WaitHandle.Reset();

                if (context.Queue.Count != _lastSize)
                {
                    _lastSize = context.Queue.Count;

                    context.MetricService.SetMetric(new Metric(MetricType.QueueSize, "Events in Queue", _lastSize));
                    context.Logger.Write($"[{_serverName}] [{_pipelineId}] Items in Queue count: {_lastSize}", Category.Log, LogLevel.Debug, source: "EventQueue");
                }
                
                context.WaitHandle.WaitOne(TimeSpan.FromSeconds(5));
            }
        }
    }
}
