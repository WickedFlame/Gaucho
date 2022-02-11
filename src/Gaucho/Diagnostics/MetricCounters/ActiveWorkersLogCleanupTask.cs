using System;
using System.Linq;
using Gaucho.BackgroundTasks;
using Gaucho.Storage;

namespace Gaucho.Diagnostics.MetricCounters
{
    /// <summary>
    /// Cleanup for the <see cref="ActiveWorkersLogWriter"/>. Reduces the log size in the <see cref="IStorage"/>
    /// </summary>
    public class ActiveWorkersLogCleanupTask : IBackgroundTask<StorageContext>
    {
        private readonly StorageKey _key;

        /// <summary>
        /// Creates a new instance of a BackgroundTaskDispatcher
        /// </summary>
        /// <param name="key"></param>
        public ActiveWorkersLogCleanupTask(StorageKey key)
        {
            _key = key;
        }

        /// <summary>
        /// Execute the cleanup
        /// </summary>
        /// <param name="context"></param>
        public void Execute(StorageContext context)
        {
            var items = context.Storage.GetList<ActiveWorkersLogMessage>(_key);
            if (items != null && items.Count() > 20)
            {
                context.Storage.RemoveRangeFromList(_key, items.Count() - 10);
            }

            context.DispatcherLock.Unlock();
        }
    }
}
