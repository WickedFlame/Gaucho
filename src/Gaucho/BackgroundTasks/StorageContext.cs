using Gaucho.Storage;

namespace Gaucho.BackgroundTasks
{
    /// <summary>
    /// Context for background processing
    /// </summary>
    public class StorageContext : ITaskContext
    {
        /// <summary>
        /// Create a new StorageContext
        /// </summary>
        /// <param name="storage"></param>
        public StorageContext(IStorage storage)
        {
            Storage = storage;
        }

        /// <summary>
        /// Gets the <see cref="IStorage"/>
        /// </summary>
        public IStorage Storage { get; }
    }
}
