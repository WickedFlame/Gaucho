using System;
using Gaucho.Configuration;
using Gaucho.Diagnostics;

namespace Gaucho
{
    //TODO: RENAME TO MessageBroker??
    public class ProcessingServer
    {
        static ProcessingServer _server;
        public static ProcessingServer Server
        {
            get
            {
                if (_server == null)
                {
                    _server = new ProcessingServer(new EventBusFactory());
                }

                return _server;
            }
        }

        public static void SetupPipeline(string pipelineId, Action<ServerRegistrationContext> setup)
            => SetupPipeline(pipelineId, Server, setup);

        public static void SetupPipeline(string pipelineId, ProcessingServer server, Action<ServerRegistrationContext> setup)
        {
            var context = new ServerRegistrationContext(pipelineId, server);
            setup(context);
        }

        public static void SetupPipeline(string pipelineId, PipelineConfiguration config)
        {
            var pluginMgr = new PluginManager();

            SetupPipeline(pipelineId, s =>
            {
                s.Register(() =>
                {
                    var pipeline = new EventPipeline();
                    foreach (var handler in pluginMgr.GetOutputHandlers(config))
                    {
                        pipeline.AddHandler(handler);
                    }

                    return pipeline;
                });

                s.Register(pluginMgr.GetInputHandler(config));
            });
        }

        private readonly IEventBusFactory _pipelineFactory;
        private readonly InputHandlerCollection _inputHandlers;

        private readonly ILogger _logger;

        public ProcessingServer() 
            : this(new EventBusFactory())
        {
        }

        public ProcessingServer(IEventBusFactory factory)
        {
            _pipelineFactory = factory;

            _inputHandlers = new InputHandlerCollection();
            _logger = LoggerConfiguration.Setup();
        }

        public IEventBusFactory EventBusFactory => _pipelineFactory;

        public InputHandlerCollection InputHandlers => _inputHandlers;

        public void Register(string pipelineId, Func<EventPipeline> factory)
        {
            _pipelineFactory.Register(pipelineId, factory);
        }

        public void Register(string pipelineId, IEventBus eventBus)
        {
            _pipelineFactory.Register(pipelineId, eventBus);
        }

        public void Register(string pipelineId, IInputHandler plugin)
        {
            _inputHandlers.Register(pipelineId, plugin);
        }

        public IInputHandler<T> GetHandler<T>(string pipelineId)
        {
            return _inputHandlers.GetHandler<T>(pipelineId);
        }

        public void Publish(Event @event)
        {
            var pipeline = _pipelineFactory.GetEventBus(@event.PipelineId);
            if (pipeline == null)
            {
                _logger.Write($"Pipeline with the Id {@event.PipelineId} does not exist. Event {@event.Id} could not be sent to any Pipeline.", Category.Log, LogLevel.Error, "EventBus");
                return;
            }

            pipeline.Publish(@event);
        }

        /// <summary>
        /// Wait for all events to be handled in the pipeline
        /// </summary>
        /// <param name="pipelineId">The Pipeline to wait for</param>
        public void WaitAll(string pipelineId)
        {
            var eventBus = _pipelineFactory.GetEventBus(pipelineId) as IAsyncEventBus;
            if (eventBus == null)
            {
                return;
            }

            eventBus.WaitAll();
        }
    }
}
