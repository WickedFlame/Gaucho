using System;

namespace Gaucho
{
	/// <summary>
	/// Factory for EventPipelines
	/// </summary>
	public class PipelineFactory : IPipelineFactory
    {
        private readonly Func<IEventPipeline> _factory;

		/// <summary>
		/// Creates a new instance of a PipelineFactory
		/// </summary>
		/// <param name="factory"></param>
        public PipelineFactory(Func<IEventPipeline> factory)
        {
            _factory = factory;
        }

		/// <summary>
		/// Creates and returnes the <see cref="IEventPipeline"/>
		/// </summary>
		/// <returns></returns>
		public IEventPipeline Setup()
        {
            return _factory.Invoke();
        }
    }
}
