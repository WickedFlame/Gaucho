using System;
using Gaucho.Diagnostics;
using Gaucho.Server.Monitoring;

namespace Gaucho
{
    public interface IProcessingServer
    {
        IEventBusFactory EventBusFactory { get; }

        void Register(string pipelineId, Func<EventPipeline> factory);

        void Register(string pipelineId, IEventBus eventBus);

        void Register(string pipelineId, IInputHandler plugin);

        IInputHandler<T> GetHandler<T>(string pipelineId);

        void Publish(Event @event);

        /// <summary>
        /// Wait for all events to be handled in the pipeline
        /// </summary>
        /// <param name="pipelineId">The Pipeline to wait for</param>
        void WaitAll(string pipelineId);
    }

    public class ProcessingServer : IProcessingServer, IDisposable
    {
        static IProcessingServer _server;
        public static IProcessingServer Server
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

        private readonly IEventBusFactory _eventBusFactory;
        private readonly InputHandlerCollection _inputHandlers;

        private readonly ILogger _logger;

        public ProcessingServer() 
            : this(new EventBusFactory())
        {
        }

        public ProcessingServer(IEventBusFactory factory)
        {
            _eventBusFactory = factory;

            _inputHandlers = new InputHandlerCollection();
            _logger = LoggerConfiguration.Setup();
        }

        public IEventBusFactory EventBusFactory => _eventBusFactory;

        //public InputHandlerCollection InputHandlers => _inputHandlers;

        public void Register(string pipelineId, Func<EventPipeline> factory)
        {
			lock(_server)
			{
				_eventBusFactory.Register(pipelineId, factory);
			}
        }

        public void Register(string pipelineId, IEventBus eventBus)
        {
			lock(_server)
			{
				if (pipelineId != eventBus.PipelineId)
				{
					throw new Exception($"The EventBus with PipelineId {eventBus.PipelineId} cannot be registered to the pipeline {pipelineId}");
				}

				_eventBusFactory.Register(pipelineId, eventBus);
			}
        }

        public void Register(string pipelineId, IInputHandler plugin)
        {
			lock(_server)
			{
				if (plugin is IServerInitialize init)
				{
					init.Initialize(this);
				}

				_inputHandlers.Register(pipelineId, plugin);
			}
        }

        public IInputHandler<T> GetHandler<T>(string pipelineId)
        {
            return _inputHandlers.GetHandler<T>(pipelineId);
        }

        public void Publish(Event @event)
        {
            var pipeline = _eventBusFactory.GetEventBus(@event.PipelineId);
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
            var eventBus = _eventBusFactory.GetEventBus(pipelineId) as IAsyncEventBus;
            if (eventBus == null)
            {
                return;
            }

            eventBus.WaitAll();
        }

        public void Dispose()
        {
            foreach (var handler in _inputHandlers)
            {
                var dispose = handler as IDisposable;
                dispose?.Dispose();
            }

            _logger.Write("ProcessingServer stopped", Category.Log);
        }
    }
}
