
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
	}
}
