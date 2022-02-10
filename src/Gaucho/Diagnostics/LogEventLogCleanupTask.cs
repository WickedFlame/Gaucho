using System.Collections.Generic;
using Gaucho.BackgroundTasks;
using Gaucho.Storage;

namespace Gaucho.Diagnostics
{
    /// <summary>
    /// Cleanup for the <see cref="LogEventStatisticWriter"/>
    /// </summary>
    public class LogEventLogCleanupTask : IBackgroundTask<StorageContext>
    {
        private readonly List<ILogMessage> _logQueue;
        private readonly StorageKey _storageKey;
        private readonly int _shrinkSize;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logQueue"></param>
        /// <param name="storageKey"></param>
        /// <param name="shrinkSize"></param>
        public LogEventLogCleanupTask(List<ILogMessage> logQueue, StorageKey storageKey, int shrinkSize)
        {
            _logQueue = logQueue;
            _storageKey = storageKey;
            _shrinkSize = shrinkSize;

        }

        /// <summary>
        /// Execute the cleanup
        /// </summary>
        /// <param name="context"></param>
        public void Execute(StorageContext context)
        {
            if (_logQueue.Count > _shrinkSize)
            {
                _logQueue.RemoveRange(0, _shrinkSize);
            }

            context.Storage.RemoveRangeFromList(_storageKey, _shrinkSize);

            context.DispatcherLock.Unlock();
        }
    }
}
