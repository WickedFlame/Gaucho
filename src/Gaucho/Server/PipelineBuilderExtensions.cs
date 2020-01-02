using Gaucho.Configuration;

namespace Gaucho.Server
{
    public static class PipelineBuilderExtensions
    {
        public static void BuildPipeline(this PipelineBuilder builder, PipelineConfiguration config)
        {
            builder.BuildPipeline(ProcessingServer.Server, config);
        }
    }
}
