
namespace Gaucho.Server.Monitoring
{
    public class PipelineMonitor
    {
        public PipelineMonitor(string name, int threads, int queued, int handlerCount)
        {
            Name = name;
            HandlerCount = handlerCount;
            Threads = threads;
            QueueCount = queued;
        }

        public string Name { get; }

        public int HandlerCount { get; }

        public int Threads { get; }

        public int QueueCount { get; }

        public int ProcessedCount { get; }
    }
}
