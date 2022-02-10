using System;
using System.Collections.Generic;
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
        private readonly List<Task> _threads;

        /// <summary>
        /// Creates a new instance of a BackgroundTaskDispatcher
        /// </summary>
        /// <param name="context"></param>
        public BackgroundTaskDispatcher(StorageContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _threads = new List<Task>();
        }

        /// <summary>
        /// Start the task in a new thread
        /// </summary>
        /// <param name="dispatcher"></param>
        public void StartNew(IBackgroundTask<StorageContext> dispatcher)
        {
            var thread = Task.Factory.StartNew(() => dispatcher.Execute(_context),
                CancellationToken.None,
                TaskCreationOptions.None,
                TaskScheduler.Default);

            _threads.Add(thread);
            thread.ContinueWith(t =>
            {
                if (_threads.Contains(t))
                {
                    _threads.Remove(t);
                }
            });
        }

        /// <summary>
        /// Wait for all the tasks to complete
        /// </summary>
        public void WaitAll()
        {
            Task.WaitAll(_threads.ToArray(), -1, CancellationToken.None);
        }
    }
}
