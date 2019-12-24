using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho
{
    public static class ProcessingServerExtensions
    {
        //public static void SetupPipeline(string pipelineId, Action<ServerRegistrationContext> setup)
        //    => SetupPipeline(pipelineId, Server, setup);

        public static void SetupPipeline(this IProcessingServer server, string pipelineId, Action<ServerRegistrationContext> setup)
        {
            var context = new ServerRegistrationContext(pipelineId, server);
            setup(context);
        }
    }
}
