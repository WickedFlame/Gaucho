using System;
using Gaucho.Configuration;
using WickedFlame.Yaml;

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

        public void BuildPipeline(string filename)
        {
            var reader = new YamlReader();
            var config = reader.Read(filename);

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
