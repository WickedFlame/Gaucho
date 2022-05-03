using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gaucho.BackgroundTasks;
using Gaucho.Configuration;
using Gaucho.Diagnostics;
using Gaucho.Diagnostics.MetricCounters;
using Gaucho.Server.Monitoring;
using Gaucho.Storage;

namespace Gaucho
{
	/// <summary>
	/// 
	/// </summary>
    public class EventBus : IAsyncEventBus, IEventBus
    {
        private readonly EventQueue _queue;
        private IPipelineFactory _pipelineFactory;
        private readonly ILogger _logger;
        private bool _isDisposed;

        private readonly EventProcessorList _processors = new EventProcessorList();
        private readonly EventBusContext _eventQueueContext;
        private readonly MetricService _metricService;
        private readonly EventBusPorcessDispatcher _dispatcher;
        private readonly DispatcherLock _cleanupLock;
        private int _minProcessors;

        /// <summary>
        /// Creates a new instance of the eventbus
        /// </summary>
        /// <param name="factory"></param>
        /// <param name="pipelineId"></param>
        public EventBus(Func<IEventPipeline> factory, string pipelineId)
            : this(new PipelineFactory(factory, new PipelineOptions { PipelineId = pipelineId }), pipelineId)
        {
        }

        /// <summary>
		/// Creates a new instance of the eventbus
		/// </summary>
		/// <param name="pipelineFactory"></param>
		/// <param name="pipelineId"></param>
        public EventBus(IPipelineFactory pipelineFactory, string pipelineId)
        {
            _pipelineFactory = pipelineFactory;
            PipelineId = pipelineId;

            var storage = GlobalConfiguration.Configuration.Resolve<IStorage>();
            _metricService = new MetricService(storage, pipelineId);

			_queue = new EventQueue();
            _logger = LoggerConfiguration.Setup
            (
	            s =>
	            {
		            s.AddWriter(new ProcessedEventMetricCounter(_metricService, pipelineId));
		            s.AddWriter(new ActiveWorkersLogWriter(pipelineId));
		            s.AddWriter(new LogEventStatisticWriter(pipelineId));
	            }
            );

            _metricService.SetMetric(new Metric(MetricType.ThreadCount, "Active Workers", _processors.Count));
            _metricService.SetMetric(new Metric(MetricType.QueueSize, "Events in Queue", _queue.Count));
            _metricService.SetPipelineHeartbeat();

            var options = _pipelineFactory.Options.Merge(GlobalConfiguration.Configuration.GetOptions());
            _minProcessors = options.MinProcessors;

            _cleanupLock = new DispatcherLock();
            _dispatcher = new EventBusPorcessDispatcher(_processors, _queue, () => new EventProcessor(new EventPipelineWorker(_queue, () => _pipelineFactory.Setup(), _logger), CleanupProcessors, EndProcessor, _logger), _logger, _metricService, options);
            RunDispatcher();

            _eventQueueContext = new EventBusContext(_queue, _processors, _metricService, _logger)
            {
                MetricsPollingInterval = options.MetricsPollingInterval
            };
            var taskDispatcher = new BackgroundTaskDispatcher();
            taskDispatcher.StartNew(new EventBusMetricCounterTask(options.ServerName, options.PipelineId), _eventQueueContext);
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
            while (_cleanupLock.IsLocked())
            {
                System.Diagnostics.Trace.WriteLine("Wait for processors to cleanup");
                WaitOne(50);
            }

            var cnt = 0;
            while (_queue.Count > 0)
            {
                var queueSize = _queue.Count;
                System.Diagnostics.Trace.WriteLine($"Wait for {queueSize} Events in the queue to be processed");
                WaitOne(500);
                if (cnt > 5)
                {
                    break;
                }

                // we only wait for max 5 turns if the queue size does not change
                // that could mean that there are no porcessors that are working on the queue
                cnt = _queue.Count == queueSize ? cnt + 1 : 0;
            }

            foreach (var processor in _processors)
            {
                processor.Stop();
            }

            var tasks = _processors.GetTasks();
            while (tasks.Any())
            {
                System.Diagnostics.Trace.WriteLine($"Wait for {_queue.Count} Events to be processed");
                Task.WaitAll(tasks, 1000, CancellationToken.None);
                tasks = _processors.GetTasks();

                if (_queue.Count == 0)
                {
                    break;
                }
            }

            CleanupProcessors();
        }

		/// <summary>
		/// The pipeline will be closed after all queued events are processed.
		/// </summary>
        public void Close()
        {
            _minProcessors = 0;
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
            _logger.Write($"Add event {@event.Id} to queue of Pipeline {PipelineId}", LogLevel.Debug, "EventBus", () => new
            {
                PipelineId = PipelineId,
                ServerName = _pipelineFactory.Options.ServerName,
                Event = @event.Id
            });
            _queue.Enqueue(@event);

            if (!_dispatcher.IsRunning)
            {
                RunDispatcher();
            }

            _dispatcher.WaitHandle.Set();
        }

        private void RunDispatcher()
        {
            _logger.Write($"Starting event dispatcher for {PipelineId}", LogLevel.Info, "EventBus", () => new
            {
                PipelineId = PipelineId,
                ServerName = _pipelineFactory.Options.ServerName
            });
            Task.Factory.StartNew(() => _dispatcher.RunDispatcher(), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        private void EndProcessor(EventProcessor processor)
        {
            if (_queue.Count > 0)
            {
                return;
            }

            if (_processors.Count(p => !p.IsEnded) > _minProcessors)
            {
                // only stop the thread if there are more processors than are configured
                processor.Stop();
            }
        }

        private void CleanupProcessors()
        {
            if (_cleanupLock.IsLocked())
            {
                return;
            }

            _cleanupLock.Lock();

            foreach (var processor in _processors.ToList())
            {
                if (processor.IsWorking)
                {
                    continue;
                }

                if (_processors.Count <= _minProcessors)
                {
                    break;
                }

                _processors.Remove(processor);
                processor.Dispose();

                _logger.Write($"Remove Worker from EventBus. Workers: {_processors.Count}", source: "EventBus", metaData: () => new
                {
                    PipelineId = PipelineId,
                    ServerName = _pipelineFactory.Options.ServerName,
                    ProcessorCount = _processors.Count
                });
                _logger.WriteMetric(_processors.Count, StatisticType.WorkersLog);
            }

            _metricService.SetMetric(new Metric(MetricType.ThreadCount, "Active Workers", _processors.Count));

            _cleanupLock.Unlock();
        }

        /// <summary>
        /// Wait on the tread for the given amount of milliseconds
        /// </summary>
        /// <param name="delay"></param>
        /// <returns></returns>
        private void WaitOne(int delay)
        {
            Task.Delay(delay).Wait();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }

            if (disposing)
            {
                _dispatcher.Dispose();
                foreach (var processor in _processors)
                {
                    processor.Dispose();
                }

                _metricService.Dispose();

                // signal the EventQueueMetricCounter to end
                _eventQueueContext.IsRunning = false;
                _eventQueueContext.WaitHandle.Set();
            }

            _isDisposed = true;
        }
    }
}
