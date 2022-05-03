using System;
using System.Diagnostics;
using System.Threading;
using Gaucho.Diagnostics;
using Gaucho.Server.Monitoring;

namespace Gaucho
{
	/// <summary>
	/// The EventBusWorker
	/// </summary>
	public class EventPipelineWorker : IWorker
	{
		private readonly ILogger _logger;
		private readonly EventQueue _queue;
		private readonly Lazy<IEventPipeline> _pipeline;
        private readonly Stopwatch _stopwatch;

        /// <summary>
		/// Creates a new instance of the EventBusWorker
		/// </summary>
		/// <param name="queue"></param>
		///<param name="factory"></param>
		/// <param name="logger"></param>
		public EventPipelineWorker(EventQueue queue, Func<IEventPipeline> factory,  ILogger logger)
		{
			_logger = logger ?? throw new ArgumentException(nameof(logger));
			_queue = queue ?? throw new ArgumentException(nameof(queue));

			_pipeline = new Lazy<IEventPipeline>(factory);

            _stopwatch = Stopwatch.StartNew();
		}

        /// <summary>
        /// Execute the worker
        /// </summary>
        public void Execute()
        {
            if (_pipeline.Value == null)
            {
                throw new ArgumentException($"Pipeline does not exist or in not configured. Event could not be sent to any Pipeline.");
            }

            while (_queue.TryDequeue(out var @event))
            {
                try
                {
                    _stopwatch.Restart();
                    _logger.WriteMetric(@event.Id, StatisticType.ProcessedEvent);

                    _pipeline.Value.Run(@event);
                    var elapsed = _stopwatch.Elapsed.TotalMilliseconds;
                    _logger.Write($"Running event {@event.Id} took {elapsed} ms", LogLevel.Debug, "EventPipelineWorker", metaData: () => new
                    {
                        Event = @event.Id,
                        Duration = elapsed
                    });
                }
                catch (Exception e)
                {
                    _logger.Write($"Error processing event{Environment.NewLine}EventId: {@event.Id}{Environment.NewLine}{e.Message}", LogLevel.Error, "EventPipelineWorker", metaData: () => new
                    {
                        Event = @event.Id
                    });
                }
            }
        }
    }
}
