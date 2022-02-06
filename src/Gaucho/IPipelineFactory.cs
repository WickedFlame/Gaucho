
namespace Gaucho
{
	/// <summary>
	/// Factory for EventPipelines
	/// </summary>
	public interface IPipelineFactory
	{
		/// <summary>
		/// Creates and returnes the <see cref="IEventPipeline"/>
		/// </summary>
		/// <returns></returns>
		IEventPipeline Setup();

        /// <summary>
        /// Gets or sets the <see cref="PipelineOptions"/> that contain configurations for each pipeline
        /// </summary>
		PipelineOptions Options { get; }
	}
}
