
namespace Gaucho.Server.Monitoring
{
    public class PipelineMonitor
    {
        public PipelineMonitor(string name, IEventBus eventBus)
        {
            Name = name;
            HandlerCount = 0;
            Threads = eventBus.ThreadCount;
            QueueSize = eventBus.QueueSize;
        }

        public string Name { get; }

        public int HandlerCount { get; }

        public int Threads { get; }

        public int QueueSize { get; }

        public int ProcessedCount { get; }
    }
}
