
namespace Gaucho.BackgroundTasks
{
    /// <summary>
    /// BackgroundServerProcess is used to create dispatchers that run in backgroundthreads
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBackgroundTaskDispatcher<out T> where T : class, ITaskContext
    {
        /// <summary>
        /// Start the <see cref="IBackgroundTaskDispatcher{T}"/> in a new thread and run the Execute method
        /// </summary>
        /// <param name="dispatcher"></param>
        void StartNew(IBackgroundTask<T> dispatcher);
    }
}
