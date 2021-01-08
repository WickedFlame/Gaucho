using Gaucho.Diagnostics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gaucho
{
	public class EventProcessor : IDisposable
	{
		private readonly string _id = Guid.NewGuid().ToString();
		private readonly object _syncRoot = new object();
		private readonly Action<IEventPipeline> _action;
		private readonly ILogger _logger;

		private bool _isWorking;
		private readonly Lazy<IEventPipeline> _pipeline;

		public EventProcessor(Func<IEventPipeline> factory, Action<IEventPipeline> action, ILogger logger)
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
