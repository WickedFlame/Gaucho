using System;

namespace Gaucho
{
    /// <summary>
    /// 
    /// </summary>
    public static class EventBusFactoryExtensions
    {
        /// <summary>
        /// Register a new <see cref="IEventPipeline"/> factory to the EventBusFactory
        /// </summary>
        /// <param name="busFactory"></param>
        /// <param name="pipelineId"></param>
        /// <param name="factory"></param>
        public static void Register(this IEventBusFactory busFactory, string pipelineId, Func<IEventPipeline> factory)
            => busFactory.Register(pipelineId, factory, new PipelineOptions());
    }
}
