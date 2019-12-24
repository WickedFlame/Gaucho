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

        public static void Register<T>(this IGlobalConfiguration config, T item)
        {
            config.Context.Add(item.GetType().Name, item);
        }
    }

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
