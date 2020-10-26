using System;
using Gaucho.Configuration;
using Gaucho.Server;

namespace Gaucho
{
    public static class ProcessingServerExtensions
    {
		/// <summary>
		/// creates or updates a pipeline based on the registrationcontext
		/// </summary>
		/// <param name="server"></param>
		/// <param name="pipelineId"></param>
		/// <param name="setup"></param>
        public static void SetupPipeline(this IProcessingServer server, string pipelineId, Action<ServerRegistrationContext> setup)
        {
            var context = new ServerRegistrationContext(pipelineId, server);
            setup(context);
        }

		/// <summary>
		/// creates or updates a pipeline based on the given configuration
		/// </summary>
		/// <param name="server"></param>
		/// <param name="pipelineId"></param>
		/// <param name="config"></param>
		/// <returns></returns>
        public static IProcessingServer SetupPipeline(this IProcessingServer server, string pipelineId, PipelineConfiguration config)
        {
	        var builder = new PipelineBuilder(GlobalConfiguration.Configuration);
	        builder.BuildPipeline(server, config);

	        return server;
		}
    }
}
