using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Gaucho.Diagnostics;

namespace Gaucho
{
    public interface IEventBus : IDisposable
    {
        string PipelineId { get; set; }

        IPipelineSetup PipelineSetup { get; }

        void SetPipeline(IPipelineSetup factory);

        void Publish(Event @event);
    }

    public interface IAsyncEventBus : IEventBus
    {
        void WaitAll();
    }

    public class EventBus : IAsyncEventBus, IEventBus
    {
        private readonly object _syncRoot = new object();

        private readonly EventQueue _queue;
        private IPipelineSetup _pipelineFactory;
        private readonly ILogger _logger;
        private bool _isDisposed;

        private readonly List<WorkerThread> _threads = new List<WorkerThread>();

        public EventBus(Func<IEventPipeline> factory):this(new PipelineSetup(factory))
        {

        }

        public EventBus(IPipelineSetup pipelineFactory)
            : this()
        {
            _pipelineFactory = pipelineFactory;
        }

        public EventBus()
        {
            _queue = new EventQueue();
            _logger = LoggerConfiguration.Setup();
            SetupWorkers(1);
        }

        public string PipelineId { get; set; }

        public IPipelineSetup PipelineSetup => _pipelineFactory;

        public void WaitAll()
        {
            var tasks = _threads.Select(t => t.Task)
                .Where(t => t != null/* && t.Status == TaskStatus.Running*/)
                .ToList();

            if(tasks.Any())
            {
                Task.WaitAll(tasks.ToArray());
            }
        }

        public void SetPipeline(IPipelineSetup factory)
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

        public void Process()
        {
            var pipeline = _pipelineFactory.Setup();
            if (pipeline == null)
            {
                _logger.Write($"Pipeline with the Id {PipelineId} does not exist. Event could not be sent to any Pipeline.", Category.Log, LogLevel.Error, "EventBus");
                return;
            }

            while (_queue.TryDequeue(out var @event))
            {
                pipeline.Run(@event);
            }

            RemoveEndedWorkers();
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
                    var thread = new WorkerThread(() => Process(), _logger);

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

        public void RemoveEndedWorkers()
        {
            lock (_syncRoot)
            {
                foreach (var thread in _threads.ToList())
                {
                    if (thread.IsWorking)
                    {
                        continue;
                    }

                    if (_threads.Count == 1)
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
            private readonly object _syncRoot = new object();
            private readonly Action _action;
            private readonly ILogger _logger;

            private bool _isWorking;

            public WorkerThread(Action action, ILogger logger)
            {
                _action = action;
                _logger = logger;
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

                _logger.Write("Start working on Thread", Category.Log, LogLevel.Debug, "EventBus");

                Task = Task.Factory.StartNew(() =>
                {
                    _action();

                    lock (_syncRoot)
                    {
                        _isWorking = false;
                    }
                }, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }
        }
    }

    public class SimpleEventBus : IEventBus
    {
        private readonly EventQueue _queue;
        private readonly ILogger _logger;
        private IPipelineSetup _pipelineFactory;
        private IEventPipeline _pipeline;

        public SimpleEventBus(IPipelineSetup pipelineFactory)
            : this()
        {
            _pipelineFactory = pipelineFactory;
        }

        public SimpleEventBus()
        {
            _queue = new EventQueue();

            _logger = LoggerConfiguration.Setup();
        }

        public string PipelineId { get; set; }

        public IPipelineSetup PipelineSetup => _pipelineFactory;

        public void SetPipeline(IPipelineSetup factory)
        {
            _pipelineFactory = factory;
        }

        public void Publish(Event @event)
        {
            _queue.Enqueue(@event);
            Process();
        }

        public void Process()
        {
            var pipeline = GetPipeline();
            if (pipeline == null)
            {
                _logger.Write($"Pipeline with the Id {PipelineId} does not exist. Event could not be sent to any Pipeline.", Category.Log, LogLevel.Error, "SimpleEventBus");
                return;
            }

            while (_queue.TryDequeue(out var @event))
            {
                pipeline.Run(@event);
            }
        }

        private IEventPipeline GetPipeline()
        {
            if (_pipeline == null)
            {
                _pipeline = _pipelineFactory.Setup();
            }

            return _pipeline;
        }

        public void Dispose()
        {
        }
    }
}
