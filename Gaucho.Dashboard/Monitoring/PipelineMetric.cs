
namespace Gaucho.Server.Monitoring
{
    public class PipelineMetric
    {
        public PipelineMetric(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public int HandlerCount { get; set; }

        public int Threads { get; set; }

        public int QueueSize { get; set; }

        public int ProcessedCount { get; set; }
    }
}
