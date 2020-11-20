﻿using System;
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

        private readonly List<WorkerThread> _threads = new List<WorkerThread>();
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
            statistic.AddMetricsCounter(new Metric(MetricType.ThreadCount, "Workers", () => _threads.Count));
            statistic.AddMetricsCounter(new Metric(MetricType.QueueSize, "Events in Queue", () => _queue.Count));

            _queue = new EventQueue();
            _logger = LoggerConfiguration.Setup
            (
	            s =>
	            {
		            s.AddWriter(new ProcessedEventStatisticWriter(statistic));
		            s.AddWriter(new LogEventStatisticWriter(statistic));
	            }
            );
            SetupWorkers(1);
        }

        public string PipelineId { get; }

        public void WaitAll()
        {
            var tasks = _threads.Select(t => t.Task)
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

            if (_queue.Count / _threads.Count > 10)
            {
                SetupWorkers(_threads.Count + 1);
            }

            foreach (var thread in _threads.ToList())
            {
                thread.Start();
            }
        }

        public void Process(IEventPipeline pipeline)
        {
	        _logger.Write("Begin processing events", Category.Log, LogLevel.Debug, "EventBus");
			//var pipeline = _pipelineFactory.Setup();
            if (pipeline == null)
            {
                _logger.Write($"Pipeline with the Id {PipelineId} does not exist. Event could not be sent to any Pipeline.", Category.Log, LogLevel.Error, "EventBus");
                return;
            }

            while (_queue.TryDequeue(out var @event))
            {
                pipeline.Run(@event);
                _logger.Write(@event.Id, StatisticType.ProcessedEvent);
            }

            CleanupWorkers();
        }

        private void SetupWorkers(int threadCount)
        {
            if (threadCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(threadCount), "The EventBus requires at least one worker thread.");
            }

            lock (_syncRoot)
            {
                for (var i = _threads.Count; i < threadCount; i++)
                {
                    var thread = new WorkerThread(() => _pipelineFactory.Setup(), p => Process(p), _logger);

                    _threads.Add(thread);
                    _logger.Write($"Add Worker to EventBus. Workers: {i}", Category.Log, source: "EventBus");
                }

                var toRemove = _threads.Count - threadCount;

                if (toRemove > 0)
                {
                    foreach (var thread in _threads.ToList())
                    {
                        _threads.Remove(thread);

                        toRemove--;
                    }

                    while (toRemove > 0)
                    {
                        var thread = _threads[_threads.Count - 1];

                        _threads.Remove(thread);

                        toRemove--;
                    }
                }
            }
        }

        public void CleanupWorkers()
        {
            lock (_syncRoot)
            {
                foreach (var thread in _threads.ToList())
                {
                    if (thread.IsWorking)
                    {
                        continue;
                    }

                    if (_threads.Count == _minThreads)
                    {
                        return;
                    }

                    _threads.Remove(thread);
                    thread.Dispose();

                    _logger.Write($"Remove Worker from EventBus. Workers: {_threads.Count}", Category.Log, source: "EventBus");
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
                foreach (var thread in _threads)
                {
                    thread.Dispose();
                }
            }

            _isDisposed = true;
        }

        public class WorkerThread : IDisposable
        {
	        private readonly string _id = Guid.NewGuid().ToString();
            private readonly object _syncRoot = new object();
            private readonly Action<IEventPipeline> _action;
            private readonly ILogger _logger;

            private bool _isWorking;
            private Lazy<IEventPipeline> _pipeline;

            public WorkerThread(Func<IEventPipeline> factory, Action<IEventPipeline> action, ILogger logger)
            {
	            logger.Write($"Created new WorkerThread with Id {_id}", Category.Log, LogLevel.Debug, "EventBus");

                _action = action;
                _logger = logger;
                _pipeline = new Lazy<IEventPipeline>(factory);
            }

            public Task Task { get; private set; }

            public bool IsWorking
            {
                get
                {
                    lock (_syncRoot)
                    {
                        return _isWorking;
                    }
                }
            }

            public void Dispose()
            {
                lock (_syncRoot)
                {
                    _isWorking = false;
                    _logger.Write($"Disposed WorkerThread with Id {_id}", Category.Log, LogLevel.Debug, "EventBus");
				}
            }

            public void Start()
            {
                lock (_syncRoot)
                {
                    if (_isWorking)
                    {
                        return;
                    }

                    _isWorking = true;
                }

                _logger.Write($"Start working on Thread {_id}", Category.Log, LogLevel.Debug, "EventBus");

                Task = Task.Factory.StartNew(() =>
                {
                    _action(_pipeline.Value);

                    lock (_syncRoot)
                    {
                        _isWorking = false;
                    }
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }
    }
}
