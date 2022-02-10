
using Gaucho.Configuration;

namespace Gaucho
{
    /// <summary>
    /// Options that are used per pipeline
    /// </summary>
    public class PipelineOptions
    {
        /// <summary>
        /// Gets the minimal amount of default workers. Workers are added as the queue size gets bigger.
        /// </summary>
        public int MinProcessors { get; set; } = -1;

        /// <summary>
        /// Gets the maximum items that are allowed per thread in the queue
        /// </summary>
        public int MaxItemsInQueue { get; set; } = -1;

        /// <summary>
        /// Gets the maximum amount of <see cref="EventProcessor"/> that are created to work on the queue
        /// </summary>
        public int MaxProcessors { get; set; } = -1;

        /// <summary>
        /// Gets the Servername that the pipeline is running in
        /// </summary>
        public string ServerName { get; set; }

        /// <summary>
        /// Gets the PipelineId
        /// </summary>
        public string PipelineId { get; set; }
    }

    public static class PipelineOptionsExtensions
    {
        public static PipelineOptions Merge(this PipelineOptions po, Options opt)
        {
            if (po.MinProcessors == -1)
            {
                po.MinProcessors = opt.MinProcessors;
            }

            if (po.MaxProcessors == -1)
            {
                po.MaxProcessors = opt.MaxProcessors;
            }

            if (po.MaxItemsInQueue == -1)
            {
                po.MaxItemsInQueue = opt.MaxItemsInQueue;
            }

            po.ServerName = opt.ServerName;

            return po;
        }
    }
}
