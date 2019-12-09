using System;
using Gaucho.Configuration;

namespace Gaucho
{
    public static class GlobalConfigurationExtensions
    {
        public static IGlobalConfiguration UseProcessingServer(this IGlobalConfiguration config, Action<PipelineBuilder> setup)
        {
            var builder = new PipelineBuilder();
            setup.Invoke(builder);

            return config;
        }
    }

    public class PipelineBuilder
    {
        private readonly PluginManager _pluginMgr;

        public PipelineBuilder()
        {
            _pluginMgr = new PluginManager();
        }

        public void BuildPipeline(PipelineConfiguration config)
        {
            ProcessingServer.SetupPipeline(config.Id, s =>
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
}
