using System;
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
		private readonly IMetricService _metrics;
		private readonly EventQueue _queue;
		private readonly Lazy<IEventPipeline> _pipeline;

		/// <summary>
		/// Creates a new instance of the EventBusWorker
		/// </summary>
		/// <param name="queue"></param>
		///<param name="factory"></param>
		/// <param name="logger"></param>
		/// <param name="metrics"></param>
		public EventPipelineWorker(EventQueue queue, Func<IEventPipeline> factory,  ILogger logger,  IMetricService metrics)
		{
			_logger = logger ?? throw new ArgumentException(nameof(logger));
			_queue = queue ?? throw new ArgumentException(nameof(queue));
			_metrics = metrics ?? throw new ArgumentException(nameof(metrics));

			_pipeline = new Lazy<IEventPipeline>(factory);
		}

		/// <summary>
		/// Execute the worker
		/// </summary>
		public void Execute()
		{
			_logger.Write("Begin processing events", Category.Log, LogLevel.Debug, "EventBus");

			if (_pipeline.Value == null)
			{
				throw new ArgumentException($"Pipeline does not exist or in not configured. Event could not be sent to any Pipeline.");
			}

			while (_queue.TryDequeue(out var @event))
			{
				try
				{
					_logger.WriteMetric(@event.Id, StatisticType.ProcessedEvent);
					_metrics.SetMetric(new Metric(MetricType.QueueSize, "Events in Queue", _queue.Count));

					_pipeline.Value.Run(@event);
				}
				catch (Exception e)
				{
					_logger.Write($"Error processing eveng{Environment.NewLine}EventId: {@event.Id}{Environment.NewLine}{e.Message}", Category.Log, LogLevel.Error, "EventBus");
				}
			}
		}
	}
}
