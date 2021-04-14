using System;
using Gaucho.Configuration;
using Gaucho.Server;

namespace Gaucho
{
	/// <summary>
	/// Extensions for the ProcessingServer
	/// </summary>
    public static class ProcessingServerExtensions
    {
		/// <summary>
		/// creates or updates a pipeline based on the registrationcontext
		/// </summary>
		/// <param name="server"></param>
		/// <param name="pipelineId"></param>
		/// <param name="registration"></param>
		public static void SetupPipeline(this IProcessingServer server, string pipelineId, Action<PipelineRegistration> registration)
        {
			//TODO: add the possibility to delay the setup
			var context = new PipelineRegistration(pipelineId, server);
			registration(context);
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
	        var builder = new PipelineBuilder();
	        builder.BuildPipeline(server, config);

	        return server;
		}
    }
}
