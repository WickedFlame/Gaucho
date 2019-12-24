using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho.Configuration
{
    public class PipelineBuilder
    {
        private readonly PluginManager _pluginMgr;
        private readonly IGlobalConfiguration _config;

        public PipelineBuilder() : this(GlobalConfiguration.Configuration) { }

        public PipelineBuilder(IGlobalConfiguration config)
        {
            _config = config;
            _pluginMgr = new PluginManager();
        }

        public void BuildPipeline(IProcessingServer server, PipelineConfiguration config)
        {
            server.SetupPipeline(config.Id, s =>
            {
                s.Register(() =>
                {
                    var pipeline = new EventPipeline();
                    foreach (var handler in _pluginMgr.GetOutputHandlers(config))
                    {
                        pipeline.AddHandler(handler);
                    }

                    return pipeline;
                });

                s.Register(_pluginMgr.GetInputHandler(config));
            });
        }
    }

    public static class PipelineBuilderExtensions
    {
        public static void BuildPipeline(this PipelineBuilder builder, PipelineConfiguration config)
        {
            builder.BuildPipeline(ProcessingServer.Server, config);
        }
    }
}
