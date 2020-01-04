using System;
using Gaucho.Server.Monitoring;

namespace Gaucho
{
    public static class ProcessingServerExtensions
    {
        public static void SetupPipeline(this IProcessingServer server, string pipelineId, Action<ServerRegistrationContext> setup)
        {
            var context = new ServerRegistrationContext(pipelineId, server);
            setup(context);
        }
    }
}
