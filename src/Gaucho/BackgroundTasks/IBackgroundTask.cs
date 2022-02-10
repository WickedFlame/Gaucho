
namespace Gaucho.BackgroundTasks
{
    /// <summary>
    /// Task that is executed in a background thread
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBackgroundTask<in T> where T : ITaskContext
    {
        /// <summary>
        /// Execute the task
        /// </summary>
        /// <param name="context"></param>
        void Execute(T context);
    }
}
