using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gaucho.Diagnostics;
using Gaucho.Server.Monitoring;

namespace Gaucho
{
    public interface IEventBus : IDisposable
    {
        string PipelineId { get; }

        /// <summary>
        /// gets the pipelinefactory that creates the pipeline for this eventbus
        /// </summary>
		IPipelineFactory PipelineFactory { get; }

        void SetPipeline(IPipelineFactory factory);

        void Publish(Event @event);

        void Close();
    }

    public interface IAsyncEventBus : IEventBus
    {
        void WaitAll();
    }

    public class EventBus : IAsyncEventBus, IEventBus
    {
        private readonly object _syncRoot = new object();

        private readonly EventQueue _queue;
        private IPipelineFactory _pipelineFactory;
        private readonly ILogger _logger;
        private bool _isDisposed;

        private readonly List<EventProcessor> _processors = new List<EventProcessor>();
        private int _minThreads = 1;

        public EventBus(Func<IEventPipeline> factory, string pipelineId)
            : this(new PipelineFactory(factory), pipelineId)
        {
        }

        public EventBus(IPipelineFactory pipelineFactory, string pipelineId)
            : this(pipelineId)
        {
            _pipelineFactory = pipelineFactory;
        }

        private EventBus(string pipelineId)
        {
            PipelineId = pipelineId;

            var statistic = new StatisticsApi(pipelineId);
            statistic.AddMetricsCounter(new Metric(MetricType.ThreadCount, "Workers", () => _processors.Count));
            statistic.AddMetricsCounter(new Metric(MetricType.QueueSize, "Events in Queue", () => _queue.Count));

            _queue = new EventQueue();
            _logger = LoggerConfiguration.Setup
            (
	            s =>
	            {
		            s.AddWriter(new ProcessedEventMetricCounter(statistic));
		            s.AddWriter(new LogEventStatisticWriter(statistic));
	            }
            );
            SetupProcessors(1);
        }

        public string PipelineId { get; }

		/// <summary>
		/// gets the pipelinefactory that creates the pipeline for this eventbus
		/// </summary>
        public IPipelineFactory PipelineFactory => _pipelineFactory;


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

        public void Close()
        {
	        _minThreads = 0;
        }

        public void SetPipeline(IPipelineFactory factory)
        {
            _pipelineFactory = factory;
        }

        public void Publish(Event @event)
        {
            _queue.Enqueue(@event);

            if (_queue.Count / _processors.Count > 10)
            {
                SetupProcessors(_processors.Count + 1);
            }

            foreach (var process in _processors.ToList())
            {
                process.Start();
            }
        }

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
		            _logger.Write(@event.Id, StatisticType.ProcessedEvent);
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
                    _logger.Write($"Add Worker to EventBus. Active Workers: {i}", Category.Log, source: "EventBus");
                }

                var toRemove = _processors.Count - threadCount;

                if (toRemove > 0)
                {
                    foreach (var processor in _processors.ToList())
                    {
                        _processors.Remove(processor);

                        toRemove--;
                    }

                    while (toRemove > 0)
                    {
                        var processor = _processors[_processors.Count - 1];

                        _processors.Remove(processor);

                        toRemove--;
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
