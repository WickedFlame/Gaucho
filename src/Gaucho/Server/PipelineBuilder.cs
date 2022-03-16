using Gaucho.Configuration;
using Gaucho.Diagnostics;
using System.Linq;
using Gaucho.Handlers;

namespace Gaucho.Server
{
	/// <summary>
	/// The PipelineBuilder
	/// </summary>
    public class PipelineBuilder
    {
        private readonly HandlerPluginManager _pluginManager;
        private readonly ILogger _logger;

		/// <summary>
		/// Creates a new instance of the PipelineBuilder
		/// </summary>
        public PipelineBuilder()
        {
            _pluginManager = new HandlerPluginManager();
			_logger = LoggerConfiguration.Setup();
		}

		/// <summary>
		/// Build the pipeline based on the configuration
		/// </summary>
		/// <param name="server"></param>
		/// <param name="config"></param>
        public void BuildPipeline(IProcessingServer server, PipelineConfiguration config)
        {
			_logger.Write($"Setup Pipeline:{config}", LogLevel.Debug, "PipelineBuilder");

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
                }, config.Options);

                var inputHandler = BuildHandler<IInputHandler>(rootCtx.ChildContext(), config.InputHandler);
                s.Register(inputHandler);
            });
        }

		/// <summary>
		/// Build a Handler based on the node configuration
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="nodeCtx"></param>
		/// <param name="node"></param>
		/// <returns></returns>
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
