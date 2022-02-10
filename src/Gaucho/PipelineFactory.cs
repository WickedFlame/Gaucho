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
		/// <param name="options"></param>
        public PipelineFactory(Func<IEventPipeline> factory, PipelineOptions options)
        {
            _factory = factory;
			Options = options;
        }

		/// <summary>
		/// Creates and returnes the <see cref="IEventPipeline"/>
		/// </summary>
		/// <returns></returns>
		public IEventPipeline Setup()
        {
            return _factory.Invoke();
        }

        /// <summary>
        /// Gets or sets the <see cref="PipelineOptions"/> that contain configurations for each pipeline
        /// </summary>
		public PipelineOptions Options { get; }
    }
}
