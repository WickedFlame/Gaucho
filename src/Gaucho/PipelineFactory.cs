using System;

namespace Gaucho
{
    public interface IPipelineFactory
    {
        IEventPipeline Setup();
    }

    public class PipelineFactory : IPipelineFactory
    {
        private readonly Func<IEventPipeline> _factory;

        public PipelineFactory(Func<IEventPipeline> factory)
        {
            _factory = factory;
        }

        public IEventPipeline Setup()
        {
            return _factory.Invoke();
        }
    }
}
