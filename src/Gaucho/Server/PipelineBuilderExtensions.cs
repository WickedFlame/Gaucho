using Gaucho.Configuration;

namespace Gaucho.Server
{
	/// <summary>
	/// Extensions for the PipelineBuilder
	/// </summary>
    public static class PipelineBuilderExtensions
    {
		/// <summary>
		/// Build the Pipeline
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="config"></param>
        public static void BuildPipeline(this PipelineBuilder builder, PipelineConfiguration config)
        {
            builder.BuildPipeline(ProcessingServer.Server, config);
        }
    }
}
