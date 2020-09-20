using Gaucho.Configuration;
using Gaucho.Diagnostics;
using System.Linq;
using System.Text;
using Gaucho.Handlers;

namespace Gaucho.Server
{
    public class PipelineBuilder
    {
        private readonly PluginManager _pluginMgr;
        private readonly IGlobalConfiguration _config;
        private readonly ILogger _logger;

        public PipelineBuilder() : this(GlobalConfiguration.Configuration) { }

        public PipelineBuilder(IGlobalConfiguration config)
        {
            _config = config;
            _pluginMgr = new PluginManager();
			_logger = LoggerConfiguration.Setup();
		}

        public void BuildPipeline(IProcessingServer server, PipelineConfiguration config)
        {
            var rootCtx = _config.Resolve<IActivationContext>() ?? new ActivationContext();

			_logger.Write($"Setup Pipeline:{config}", Category.Log, LogLevel.Debug, "PipelineBuilder");

			server.SetupPipeline(config.Id, s =>
            {
                s.Register(() =>
                {
                    var pipeline = new EventPipeline();
                    foreach (var node in config.OutputHandlers)
                    {
	                    var ctx = rootCtx.ChildContext();
	                    var outputHandler = BuildHandler<IOutputHandler>(ctx, node);
						
	                    var converter = ctx.Resolve<IEventDataConverter>();
	                    if (converter.Filters.Any(f => f.FilterType == Filters.FilterType.Property))
	                    {
		                    outputHandler = new DataConversionDecorator(converter, outputHandler);
	                    }

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
            nodeCtx.Register<IEventDataConverter>(() => node.BuildEventDataConverter());
            nodeCtx.Register<ConfiguredArguments>(() => node.BuildArguments());

            var plugin = _pluginMgr.GetPlugin(typeof(T), node);
            var handler = nodeCtx.Resolve<T>(plugin.Type);

            return handler;
        }
    }
}
