using System;
using Gaucho.Configuration;
using Gaucho.Server.Monitoring;
using System.Collections.Generic;
using Gaucho.BackgroundTasks;
using Gaucho.Storage;

namespace Gaucho.Diagnostics
{
	/// <summary>
	/// Writes LogEvents to the Statistics API
	/// </summary>
	public class LogEventStatisticWriter : ILogWriter<ILogMessage>
	{
		private List<ILogMessage> _logQueue;
		private readonly LogLevel _minLogLevel;
		private readonly Lazy<IStorage> _storage;
		private readonly string _pipelineId;
        private readonly Lazy<Options> _options;
        private readonly DispatcherLock _dispatcherLock;
        private BackgroundTaskDispatcher _taskDispatcher;

        /// <summary>
		/// Creates a new instance of LogEventStatisticWriter
		/// </summary>
		/// <param name="pipelineId"></param>
		public LogEventStatisticWriter(string pipelineId)
		{
			_pipelineId = pipelineId;
            _dispatcherLock = new DispatcherLock();

			_minLogLevel = GlobalConfiguration.Configuration.GetOptions().LogLevel;
			_storage = new Lazy<IStorage>(() => GlobalConfiguration.Configuration.GetStorage());

            _options = new Lazy<Options>(() => GlobalConfiguration.Configuration.GetOptions());
        }

        /// <summary>
        /// Creates a new instance of LogEventStatisticWriter
        /// </summary>
        /// <param name="pipelineId"></param>
        /// <param name="configuration"></param>
        public LogEventStatisticWriter(string pipelineId, IGlobalConfiguration configuration)
        {
            _pipelineId = pipelineId;
            _dispatcherLock = new DispatcherLock();

			_minLogLevel = GlobalConfiguration.Configuration.GetOptions().LogLevel;
            _storage = new Lazy<IStorage>(() => configuration.GetStorage());

            _options = new Lazy<Options>(() => configuration.GetOptions());
        }

		/// <summary>
		/// The loggercategory
		/// </summary>
		public Category Category => Category.Log;

		/// <summary>
		/// Gets the list of logs
		/// </summary>
        public IEnumerable<ILogMessage> Logs => _logQueue;

		/// <summary>
		/// Write the ILogEvent to the Logs
		/// </summary>
		/// <param name="event"></param>
		public void Write(ILogEvent @event)
		{
			if (@event is ILogMessage e)
			{
				Write(e);
			}
		}

		/// <summary>
		/// Write the ILogMessage to the Logs
		/// </summary>
		/// <param name="event"></param>
		public void Write(ILogMessage @event)
		{
			if (_logQueue == null)
			{
				InitLogQueue();
			}

			if (@event.Level >= _minLogLevel)
			{
				_logQueue.Add(@event);
				_storage.Value.AddToList(new StorageKey(_pipelineId, "logs"), @event);

                if (_dispatcherLock.IsLocked())
                {
                    // only start the cleanup-thread when it is not already running
                    return;
                }

                if (_logQueue.Count <= _options.Value.MaxLogSize)
                {
					// only cleanup if size is larger
                    return;
                }

				_dispatcherLock.Lock();

				if (_taskDispatcher == null)
                {
                    _taskDispatcher = new BackgroundTaskDispatcher(new StorageContext(_storage.Value, _dispatcherLock));
                }

                _taskDispatcher.StartNew(new LogEventLogCleanupTask(_logQueue, new StorageKey(_pipelineId, "logs"), _logQueue.Count - _options.Value.LogShrinkSize));
			}
		}

		/// <summary>
		/// Wait for all cleanup tasks to complete
		/// </summary>
        public void WaitAll()
        {
            _taskDispatcher.WaitAll();
        }

		private void InitLogQueue()
		{
			if (_logQueue != null)
			{
				return;
			}

			_logQueue = new List<ILogMessage>();
			var logs = _storage.Value.GetList<LogEvent>(new StorageKey(_pipelineId, "logs"));
			if (logs != null)
			{
				_logQueue.AddRange(logs);
			}
		}
	}
}
