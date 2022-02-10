using System;
using System.Threading;
using System.Threading.Tasks;

namespace Gaucho.BackgroundTasks
{
    /// <summary>
    /// Dispatch tasks to background threads
    /// </summary>
    public class BackgroundTaskDispatcher : IBackgroundTaskDispatcher<StorageContext>
    {
        private readonly StorageContext _context;

        /// <summary>
        /// Creates a new instance of a BackgroundTaskDispatcher
        /// </summary>
        /// <param name="context"></param>
        public BackgroundTaskDispatcher(StorageContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Start the task in a new thread
        /// </summary>
        /// <param name="dispatcher"></param>
        public void StartNew(IBackgroundTask<StorageContext> dispatcher)
        {
            Task.Factory.StartNew(() => dispatcher.Execute(_context),
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.Default);
        }
    }
}
