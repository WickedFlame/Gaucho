
namespace Gaucho.BackgroundTasks
{
    /// <summary>
    /// BackgroundServerProcess is used to create dispatchers that run in backgroundthreads
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBackgroundTaskDispatcher
    {
        /// <summary>
        /// Start the <see cref="IBackgroundTaskDispatcher"/> in a new thread and run the Execute method
        /// </summary>
        /// <param name="dispatcher"></param>
        void StartNew<T>(IBackgroundTask<T> dispatcher, T context) where T : class, ITaskContext;
    }
}
