using System;
using System.Linq;
using System.Threading.Tasks;
using Gaucho.BackgroundTasks;
using Gaucho.Storage;

namespace Gaucho.Diagnostics.MetricCounters
{
	/// <summary>
	/// Logs the count workers for a statistics graph
	/// </summary>
	public class ActiveWorkersLogWriter : ILogWriter<StatisticEvent<int>>
	{
		private readonly string _pipelineId;
		private readonly Lazy<IStorage> _storage;
		private IBackgroundTaskDispatcher<StorageContext> _taskDispatcher;
        private readonly DispatcherLock _dispatcherLock;

        /// <summary>
		/// Creates a workers log metric counter
		/// </summary>
		/// <param name="pipelineId"></param>
		public ActiveWorkersLogWriter(string pipelineId)
		{
			_pipelineId = pipelineId;

			_storage = new Lazy<IStorage>(() => GlobalConfiguration.Configuration.Resolve<IStorage>());
			_dispatcherLock = new DispatcherLock();
		}

		/// <summary>
		/// the category that is loged
		/// </summary>
		public Category Category => Category.EventStatistic;

		/// <summary>
		/// default write method
		/// </summary>
		/// <param name="event"></param>
		public void Write(ILogEvent @event)
		{
			if (@event is StatisticEvent<int> e)
			{
				Write(e);
			}
		}

		/// <summary>
		/// statistic event override
		/// </summary>
		/// <param name="event"></param>
		public void Write(StatisticEvent<int> @event)
		{
			if (@event.Metric != StatisticType.WorkersLog)
			{
				return;
			}

			lock (_pipelineId)
			{
				var metric = new ActiveWorkersLogMessage
				{
					Timestamp = DateTime.Now,
					PipelineId = _pipelineId,
					ActiveWorkers = @event.Value
				};

				_storage.Value.AddToList(new StorageKey(_pipelineId, $"log:{@event.Metric}"), metric);

                if (_dispatcherLock.IsLocked())
                {
                    return;
                }

				_dispatcherLock.Lock();

                if (_taskDispatcher == null)
                {
                    _taskDispatcher = new BackgroundTaskDispatcher(new StorageContext(_storage.Value));
                }

                _taskDispatcher.StartNew(new ActiveWorkersLogCleanupTask(_dispatcherLock, new StorageKey(_pipelineId, $"log:{@event.Metric}")));
            }
		}
	}
}
