using System;

namespace Gaucho
{
    public interface IPipelineSetup
    {
        IEventPipeline Setup();
    }

    public class PipelineSetup : IPipelineSetup
    {
        private readonly Func<IEventPipeline> _factory;

        public PipelineSetup(Func<IEventPipeline> factory)
        {
            _factory = factory;
        }

        public IEventPipeline Setup()
        {
            return _factory.Invoke();
        }
    }
}
