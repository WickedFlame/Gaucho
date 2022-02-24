using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gaucho.BackgroundTasks
{
    /// <summary>
    /// Dispatch tasks to background threads
    /// </summary>
    public class BackgroundTaskDispatcher : IBackgroundTaskDispatcher
    {
        private readonly List<Task> _threads;

        /// <summary>
        /// Creates a new instance of a BackgroundTaskDispatcher
        /// </summary>
        public BackgroundTaskDispatcher()
        {
            _threads = new List<Task>();
        }

        /// <summary>
        /// Start the task in a new thread
        /// </summary>
        /// <param name="dispatcher"></param>
        /// <param name="context"></param>
        public void StartNew<T>(IBackgroundTask<T> dispatcher, T context) where T : class, ITaskContext
        {
            var thread = Task.Factory.StartNew(() => dispatcher.Execute(context),
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
