using System;
using System.Linq;
using System.Threading;
using Gaucho.Diagnostics;
using Gaucho.Server.Monitoring;

namespace Gaucho
{
    public class EventBusPorcessDispatcher : IDisposable
    {
        private bool _isRunning = true;
        private readonly EventProcessorList _processors;
        private readonly EventQueue _queue;
        private readonly Func<EventProcessor> _threadFactory;
        private readonly ILogger _logger;
        private readonly MetricService _metricService;

        public EventBusPorcessDispatcher(EventProcessorList processors, EventQueue queue, Func<EventProcessor> threadFactory, ILogger logger, MetricService metricService, int initialThreads)
        {
            WaitHandle = new ManualResetEvent(false);
            _processors = processors;
            _queue = queue;
            _threadFactory = threadFactory;
            _logger = logger;
            _metricService = metricService;

            SetupProcessors(initialThreads);
        }

        public ManualResetEvent WaitHandle { get; }

        public void RunDispatcher()
        {
            while (_isRunning)
            {
                WaitHandle.Reset();

                if (_queue.Count > 0)
                {
                    if (_processors.Count == 0 || _queue.Count / _processors.Count > 10)
                    {
                        _logger.Write($"Items in Queue count: {_queue.Count}", Category.Log, source: "EventBus");
                        SetupProcessors(_processors.Count + 1);
                    }

                    foreach (var process in _processors.ToList())
                    {
                        process.Start();
                    }
                }

                WaitHandle.WaitOne(100);
            }
        }

        private void SetupProcessors(int threadCount)
        {
            if (threadCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(threadCount), "The EventBus requires at least one worker thread.");
            }

            for (var i = _processors.Count; i < threadCount; i++)
            {
                var thread = _threadFactory.Invoke();

                _processors.Add(thread);
                _metricService.SetMetric(new Metric(MetricType.ThreadCount, "Active Workers", _processors.Count));


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

                _metricService.SetMetric(new Metric(MetricType.ThreadCount, "Active Workers", _processors.Count));
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _isRunning = false;
            }
        }
    }
}
