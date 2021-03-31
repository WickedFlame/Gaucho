using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gaucho.Diagnostics;
using Gaucho.Diagnostics.MetricCounters;
using Gaucho.Server.Monitoring;

namespace Gaucho
{
	/// <summary>
	/// 
	/// </summary>
    public class EventBus : IAsyncEventBus, IEventBus
    {
        private readonly object _syncRoot = new object();

        private readonly EventQueue _queue;
        private IPipelineFactory _pipelineFactory;
        private readonly ILogger _logger;
        private bool _isDisposed;

        private readonly List<EventProcessor> _processors = new List<EventProcessor>();
        private int _minThreads = 1;

		/// <summary>
		/// Creates a new instance of the eventbus
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="pipelineId"></param>
		public EventBus(Func<IEventPipeline> factory, string pipelineId)
            : this(new PipelineFactory(factory), pipelineId)
        {
        }

		/// <summary>
		/// Creates a new instance of the eventbus
		/// </summary>
		/// <param name="pipelineFactory"></param>
		/// <param name="pipelineId"></param>
        public EventBus(IPipelineFactory pipelineFactory, string pipelineId)
            : this(pipelineId)
        {
            _pipelineFactory = pipelineFactory;
        }

        private EventBus(string pipelineId)
        {
            PipelineId = pipelineId;

            var statistic = new StatisticsApi(pipelineId);
            statistic.AddMetricsCounter(new Metric(MetricType.ThreadCount, "Active Workers", () => _processors.Count));
            statistic.AddMetricsCounter(new Metric(MetricType.QueueSize, "Events in Queue", () => _queue.Count));

            _queue = new EventQueue();
            _logger = LoggerConfiguration.Setup
            (
	            s =>
	            {
		            s.AddWriter(new ProcessedEventMetricCounter(statistic));
		            s.AddWriter(new WorkersLogMetricCounter(statistic));
		            s.AddWriter(new LogEventStatisticWriter(statistic));
	            }
            );
            SetupProcessors(_minThreads);
        }

		/// <summary>
		/// Gets the PipelineId
		/// </summary>
        public string PipelineId { get; }

		/// <summary>
		/// Gets the pipelinefactory that creates the pipeline for this eventbus
		/// </summary>
        public IPipelineFactory PipelineFactory => _pipelineFactory;

		/// <summary>
		/// Waits for all processors to end the work
		/// </summary>
		public void WaitAll()
        {
            var tasks = _processors.Select(t => t.Task)
                .Where(t => t != null)
                .ToArray();

            if(tasks.Any())
            {
                Task.WaitAll(tasks, -1, CancellationToken.None);
            }
        }

		/// <summary>
		/// The pipeline will be closed after all queued events are processed.
		/// </summary>
        public void Close()
        {
	        _minThreads = 0;
        }

		/// <summary>
		/// Set the pipelinefactory
		/// </summary>
		/// <param name="factory"></param>
        public void SetPipeline(IPipelineFactory factory)
        {
            _pipelineFactory = factory;
        }

		/// <summary>
		/// Publish an event to the processingqueue
		/// </summary>
		/// <param name="event"></param>
        public void Publish(Event @event)
        {
            _queue.Enqueue(@event);

            if (_queue.Count / _processors.Count > 10)
            {
	            _logger.Write($"Items in Queue count: {_queue.Count}", Category.Log, source: "EventBus");
				SetupProcessors(_processors.Count + 1);
            }

            foreach (var process in _processors.ToList())
            {
                process.Start();
            }
        }

		/// <summary>
		/// Start porcessing the pipeline
		/// </summary>
		/// <param name="pipeline"></param>
        public void Process(IEventPipeline pipeline)
        {
	        _logger.Write("Begin processing events", Category.Log, LogLevel.Debug, "EventBus");

			if (pipeline == null)
            {
                _logger.Write($"Pipeline with the Id {PipelineId} does not exist. Event could not be sent to any Pipeline.", Category.Log, LogLevel.Error, "EventBus");
                
                return;
            }

            while (_queue.TryDequeue(out var @event))
            {
	            try
	            {
		            _logger.WriteMetric(@event.Id, StatisticType.ProcessedEvent);
					pipeline.Run(@event);
	            }
	            catch (Exception e)
	            {
		            _logger.Write($"Error processing eveng{Environment.NewLine}EventId: {@event.Id}{Environment.NewLine}Pipeline: {PipelineId}{Environment.NewLine}{e.Message}", Category.Log, LogLevel.Error, "EventBus");
	            }
            }

            CleanupProcessors();
        }

        private void SetupProcessors(int threadCount)
        {
            if (threadCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(threadCount), "The EventBus requires at least one worker thread.");
            }

            lock (_syncRoot)
            {
                for (var i = _processors.Count; i < threadCount; i++)
                {
                    var thread = new EventProcessor(() => _pipelineFactory.Setup(), p => Process(p), _logger);

                    _processors.Add(thread);
                    _logger.Write($"Add Worker to EventBus. Active Workers: {i + 1}", Category.Log, source: "EventBus");
                    _logger.WriteMetric(i + 1, StatisticType.WorkersLog);
                }

                var toRemove = _processors.Count - threadCount;

                if (toRemove > 0)
                {
	                while (toRemove > 0)
                    {
                        var processor = _processors[_processors.Count - 1];

                        _processors.Remove(processor);
                        processor.Dispose();

						toRemove--;

						_logger.Write($"Removed Worker from EventBus. Active Workers: {toRemove}", Category.Log, source: "EventBus");
						_logger.WriteMetric(toRemove, StatisticType.WorkersLog);
                    }
                }
            }
        }

        private void CleanupProcessors()
        {
            lock (_syncRoot)
            {
                foreach (var processor in _processors.ToList())
                {
                    if (processor.IsWorking)
                    {
                        continue;
                    }

                    if (_processors.Count == _minThreads)
                    {
                        return;
                    }

                    _processors.Remove(processor);
                    processor.Dispose();

                    _logger.Write($"Remove Worker from EventBus. Workers: {_processors.Count}", Category.Log, source: "EventBus");
                    _logger.WriteMetric(_processors.Count, StatisticType.WorkersLog);
				}
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                foreach (var processor in _processors)
                {
                    processor.Dispose();
                }
            }

            _isDisposed = true;
        }
    }
}
