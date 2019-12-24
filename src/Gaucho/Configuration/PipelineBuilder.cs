using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Server;

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
            var rootCtx = _config.Resolve<IActivationContext>() ?? new ActivationContext();
            
            server.SetupPipeline(config.Id, s =>
            {
                s.Register(() =>
                {
                    var pipeline = new EventPipeline();
                    foreach (var node in config.OutputHandlers)
                    {
                        var outputHandler = BuildHandler<IOutputHandler>(rootCtx.ChildContext(), node);
                        pipeline.AddHandler(outputHandler);
                    }

                    return pipeline;
                });

                var inputHandler = BuildHandler<IInputHandler>(rootCtx.ChildContext(), config.InputHandler);
                s.Register(inputHandler);
            });
        }

        public T BuildHandler<T>(IActivationContext nodeCtx, HandlerNode node)
        {
            nodeCtx.Register<IEventDataConverter>(() => node.BuildEventData());
            nodeCtx.Register<ConfiguredArgumentsCollection>(() => node.BuildArguments());

            var plugin = _pluginMgr.GetPlugin(typeof(T), node);
            var handler = nodeCtx.Resolve<T>(plugin.Type);

            return handler;
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
