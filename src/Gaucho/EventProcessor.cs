using Gaucho.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;
using Gaucho.BackgroundTasks;

namespace Gaucho
{
    public delegate void OnEndProcessing(EventProcessor processor);

    public delegate void OnEndTask();

	/// <summary>
	/// EventProcessor
	/// </summary>
	public class EventProcessor : IDisposable
	{
		private readonly string _id = Guid.NewGuid().ToString();
        private readonly OnEndProcessing _onEndProcessing;
		private readonly OnEndTask _onEndTask;
		private readonly ILogger _logger;
		private readonly IWorker _worker;
        private readonly ManualResetEvent _waitHandle;
		private bool _isRunning;

		private DispatcherLock _lock;


		/// <summary>
		/// Creates a new instance of EventProcessor
		/// </summary>
		/// <param name="worker"></param>
		/// <param name="onEndTask">Task to continue with after the worker is done</param>
		/// <param name="onEndProcessing"></param>
		/// <param name="logger"></param>
		public EventProcessor(IWorker worker, OnEndTask onEndTask, OnEndProcessing onEndProcessing, ILogger logger)
		{
			logger.Write($"Created new WorkerThread with Id {_id}", LogLevel.Debug, "EventBus");

            _waitHandle = new ManualResetEvent(false);
            _lock = new DispatcherLock();
			_worker = worker ?? throw new ArgumentNullException(nameof(worker));
            _onEndTask = onEndTask ?? throw new ArgumentNullException(nameof(onEndTask));
			_onEndProcessing = onEndProcessing ?? throw new ArgumentNullException(nameof(onEndProcessing));
			_logger = logger;
		}

		/// <summary>
		/// The Task that the processor runs in
		/// </summary>
		public Task Task { get; private set; }

		/// <summary>
		/// Gets if the processor has a thread that is working
		/// </summary>
		public bool IsWorking => _lock.IsLocked();

		/// <summary>
		/// Gets if the threadloop is ended and the process is in wait state
		/// </summary>
        public bool IsEnded => !_isRunning;

		/// <summary>
		/// Start processing events
		/// </summary>
		public void Start()
		{
            if (_lock.IsLocked())
            {
				//
				// worker is still running
				// to be sure it starts processing we signal the worker that it has some work to do
                _waitHandle.Set();

                return;
            }

			_lock.Lock();
            _isRunning = true;

			_logger.Write($"Start working on Thread {_id}", LogLevel.Debug, "EventBus");

			Task = Task.Factory.StartNew(() =>
			{
				try
				{
                    while (_isRunning)
                    {
                        _waitHandle.Reset();
						
                        _worker.Execute();

						// end process if there are too many running
						_onEndProcessing(this);

						_waitHandle.WaitOne(10000);
                    }
				}
				catch (Exception e)
				{
					_logger.Write(e.Message, LogLevel.Error, "EventProcessor");
				}
				finally
                {
                    _lock.Unlock();

					_onEndTask();
                }
			}, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}

		/// <summary>
		/// Stop the Executionloop when all events are processed
		/// </summary>
        public void Stop()
        {
			_isRunning = false;
            _waitHandle.Set();
        }

        /// <summary>
        /// Dispose the processor
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
			GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
			_lock.Unlock();
            _logger.Write($"Disposed WorkerThread with Id {_id}", LogLevel.Debug, "EventBus");

            _isRunning = false;
            _waitHandle.Set();
		}
    }
}
