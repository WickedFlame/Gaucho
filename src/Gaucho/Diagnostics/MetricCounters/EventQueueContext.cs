using System.Threading;
using Gaucho.BackgroundTasks;
using Gaucho.Server.Monitoring;

namespace Gaucho.Diagnostics.MetricCounters
{
    /// <summary>
    /// Context for <see cref="IBackgroundTask{T}"/> that use the <see cref="EventQueue"/>
    /// </summary>
    public class EventQueueContext : ITaskContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="queue"></param>
        /// <param name="metricService"></param>
        /// <param name="logger"></param>
        public EventQueueContext(EventQueue queue, MetricService metricService, ILogger logger)
        {
            Queue = queue;
            MetricService = metricService;
            Logger = logger;

            IsRunning = true;
            WaitHandle = new ManualResetEvent(false);
        }

        /// <summary>
        /// Gets the <see cref="WaitHandle"/> used for thread loops
        /// </summary>
        public EventWaitHandle WaitHandle { get; }

        /// <summary>
        /// Defines if the Task is running
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// Gets the EventQueue
        /// </summary>
        public EventQueue Queue { get; }

        /// <summary>
        /// Gets the <see cref="MetricService"/>
        /// </summary>
        public MetricService MetricService { get; }

        /// <summary>
        /// Gets the <see cref="ILogger"/>
        /// </summary>
        public ILogger Logger { get; }
    }
}
