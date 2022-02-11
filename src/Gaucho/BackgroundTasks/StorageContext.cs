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
        public StorageContext(IStorage storage, DispatcherLock dispatcherLock)
        {
            Storage = storage;
            DispatcherLock = dispatcherLock;
        }

        /// <summary>
        /// Gets the <see cref="IStorage"/>
        /// </summary>
        public IStorage Storage { get; }

        /// <summary>
        /// Gets the <see cref="DispatcherLock"/> associated with the Processor
        /// </summary>
        public DispatcherLock DispatcherLock { get; }
    }
}
