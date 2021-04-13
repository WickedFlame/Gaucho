using System;
using Gaucho.Diagnostics;
using Gaucho.Server.Monitoring;

namespace Gaucho
{
	/// <summary>
	/// The EventBusWorker
	/// </summary>
	public class EventPipelineWorker : IWorker<IEventPipeline>
	{
		private readonly ILogger _logger;
		private readonly EventQueue _queue;
		private readonly IMetricService _metrics;

		/// <summary>
		/// Creates a new instance of the EventBusWorker
		/// </summary>
		/// <param name="logger"></param>
		/// <param name="queue"></param>
		/// <param name="metrics"></param>
		public EventPipelineWorker(ILogger logger, EventQueue queue, IMetricService metrics)
		{
			_logger = logger ?? throw new ArgumentException(nameof(logger));
			_queue = queue ?? throw new ArgumentException(nameof(queue));
			_metrics = metrics ?? throw new ArgumentException(nameof(metrics));
		}

		/// <summary>
		/// Execute the worker
		/// </summary>
		/// <param name="pipeline"></param>
		public void Execute(IEventPipeline pipeline)
		{
			_logger.Write("Begin processing events", Category.Log, LogLevel.Debug, "EventBus");

			if (pipeline == null)
			{
				throw new ArgumentException(nameof(pipeline), $"Pipeline does not exist or in not configured. Event could not be sent to any Pipeline.");
			}

			while (_queue.TryDequeue(out var @event))
			{
				try
				{
					_logger.WriteMetric(@event.Id, StatisticType.ProcessedEvent);
					_metrics.SetMetric(new Metric(MetricType.QueueSize, "Events in Queue", _queue.Count));

					pipeline.Run(@event);
				}
				catch (Exception e)
				{
					_logger.Write($"Error processing eveng{Environment.NewLine}EventId: {@event.Id}{Environment.NewLine}{e.Message}", Category.Log, LogLevel.Error, "EventBus");
				}
			}
		}
	}
}
