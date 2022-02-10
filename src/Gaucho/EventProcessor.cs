using Gaucho.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;
using Gaucho.BackgroundTasks;

namespace Gaucho
{
	/// <summary>
	/// EventProcessor
	/// </summary>
	public class EventProcessor : IDisposable
	{
		private readonly string _id = Guid.NewGuid().ToString();
		private readonly object _syncRoot = new object();
		private readonly Action _continuation;
		private readonly ILogger _logger;
		private readonly IWorker _worker;

		private DispatcherLock _lock;
		

		/// <summary>
		/// Creates a new instance of EventProcessor
		/// </summary>
		/// <param name="worker"></param>
		/// <param name="continuation">Task to continue with after the worker is done</param>
		/// <param name="logger"></param>
		public EventProcessor(IWorker worker, Action continuation, ILogger logger)
		{
			logger.Write($"Created new WorkerThread with Id {_id}", Category.Log, LogLevel.Debug, "EventBus");
            
            _lock = new DispatcherLock();
			_worker = worker ?? throw new ArgumentNullException(nameof(worker));
			_continuation = continuation ?? throw new ArgumentNullException(nameof(continuation));
			_logger = logger;
		}

		/// <summary>
		/// The Task that the processor runs in
		/// </summary>
		public Task Task { get; private set; }

		/// <summary>
		/// Gets if the processor is working
		/// </summary>
		public bool IsWorking => _lock.IsLocked();

        /// <summary>
		/// Dispose the processor
		/// </summary>
		public void Dispose()
		{
			_lock.Unlock();
            _logger.Write($"Disposed WorkerThread with Id {_id}", Category.Log, LogLevel.Debug, "EventBus");
		}

		/// <summary>
		/// Start processing events
		/// </summary>
		public void Start()
		{
            if (_lock.IsLocked())
            {
                return;
            }

			_lock.Lock();

			_logger.Write($"Start working on Thread {_id}", Category.Log, LogLevel.Debug, "EventBus");

			Task = Task.Factory.StartNew(() =>
			{
				try
				{
					_worker.Execute();
				}
				catch (Exception e)
				{
					_logger.Write(e.Message, Category.Log, LogLevel.Error, "EventProcessor");
				}
				finally
                {
                    _continuation();
					_lock.Unlock();
                }
			}, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
		}
	}
}
