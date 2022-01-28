using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gaucho.BackgroundTasks;
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
        private int _minThreads = 1;
        private readonly MetricService _metricService;
        private readonly EventBusPorcessDispatcher _dispatcher;
        private readonly DispatcherLock _cleanupLock;

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

            _cleanupLock = new DispatcherLock();
            _dispatcher = new EventBusPorcessDispatcher(_processors, _queue, () => new EventProcessor(new EventPipelineWorker(_queue, () => _pipelineFactory.Setup(), _logger, _metricService), CleanupProcessors, _logger), _logger, _metricService, _minThreads);
            Task.Factory.StartNew(() => _dispatcher.RunDispatcher(), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
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

            var tasks = _processors.GetTasks();

            var cnt = 0;
            while (_queue.Count > 0)
            {
                System.Diagnostics.Trace.WriteLine("Wait for queue to be processed");
                WaitOne(500);
                if (cnt > 5)
                {
                    break;
                }

                cnt += 1;
            }

            while (tasks.Any())
            {
                Task.WaitAll(tasks, 1000, CancellationToken.None);
                tasks = _processors.GetTasks();
            }

            CleanupProcessors();
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
            _metricService.SetMetric(new Metric(MetricType.QueueSize, "Events in Queue", _queue.Count));

            _dispatcher.WaitHandle.Set();
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

                if (_processors.Count == _minThreads)
                {
                    break;
                }

                _processors.Remove(processor);
                processor.Dispose();

                _logger.Write($"Remove Worker from EventBus. Workers: {_processors.Count}", Category.Log, source: "EventBus");
                _logger.WriteMetric(_processors.Count, StatisticType.WorkersLog);
            }

            _cleanupLock.Unlock();

            _metricService.SetMetric(new Metric(MetricType.ThreadCount, "Active Workers", _processors.Count));
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
            }

            _isDisposed = true;
        }
    }
}
