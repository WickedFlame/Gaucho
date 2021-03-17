using Gaucho.Configuration;
using Gaucho.Diagnostics;
using System.Linq;
using System.Text;
using Gaucho.Handlers;

namespace Gaucho.Server
{
    public class PipelineBuilder
    {
        private readonly HandlerPluginManager _pluginManager;
        private readonly ILogger _logger;

        public PipelineBuilder()
        {
            _pluginManager = new HandlerPluginManager();
			_logger = LoggerConfiguration.Setup();
		}

        public void BuildPipeline(IProcessingServer server, PipelineConfiguration config)
        {
			_logger.Write($"Setup Pipeline:{config}", Category.Log, LogLevel.Debug, "PipelineBuilder");

			server.SetupPipeline(config.Id, s =>
            {
	            var rootCtx = GlobalConfiguration.Configuration.Resolve<IActivationContext>();

				s.Register(() =>
                {
                    var pipeline = new EventPipeline();
                    foreach (var node in config.OutputHandlers)
                    {
	                    var ctx = rootCtx.ChildContext();
	                    var outputHandler = BuildHandler<IOutputHandler>(ctx, node);
						
	                    var converter = node.BuildDataFilter();
	                    if (converter.Filters.Any(f => f.FilterType == Filters.FilterType.Property))
	                    {
		                    outputHandler = new DataFilterDecorator(converter, outputHandler);
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
            nodeCtx.Register<IEventDataConverter>(node.BuildDataFilter);
            nodeCtx.Register<ConfiguredArguments>(node.BuildArguments);

            var plugin = _pluginManager.GetPlugin(typeof(T), node);
            var handler = nodeCtx.Resolve<T>(plugin.Type);

            return handler;
        }
    }
}
