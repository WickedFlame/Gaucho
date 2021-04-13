using System;
using System.Collections.Generic;
using Gaucho.Diagnostics;

namespace Gaucho
{
	/// <summary>
	/// The ProcessingServer
	/// </summary>
    public class ProcessingServer : IProcessingServer, IDisposable
    {
	    static ProcessingServer()
        {
			// setup the default server
			// this is created even if it is not used
			Server  = new ProcessingServer(new EventBusFactory());
		}

		/// <summary>
		/// Gets the singelton of the ProcessingServer
		/// </summary>
        public static IProcessingServer Server { get; }

		private readonly IEventBusFactory _eventBusFactory;
        private readonly InputHandlerCollection _inputHandlers;

        private readonly ILogger _logger;

		/// <summary>
		/// Create a new instance of the ProcessingServer
		/// </summary>
		public ProcessingServer() 
            : this(new EventBusFactory())
        {
        }

		/// <summary>
		/// Create a new instance of the ProcessingServer
		/// </summary>
		/// <param name="factory"></param>
        public ProcessingServer(IEventBusFactory factory)
        {
            _eventBusFactory = factory;

            _inputHandlers = new InputHandlerCollection();
            _logger = LoggerConfiguration.Setup();
        }

		/// <summary>
		/// Gets the associated EventBusFactory
		/// </summary>
        public IEventBusFactory EventBusFactory => _eventBusFactory;

		/// <summary>
		/// gets all registered inputhandlers
		/// </summary>
        public IEnumerable<IInputHandler> InputHandlers => _inputHandlers;

		/// <summary>
		/// Register an EventPipeline to the pipeline
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="factory"></param>
		public void Register(string pipelineId, Func<EventPipeline> factory)
        {
	        _eventBusFactory.Register(pipelineId, factory);
        }

		/// <summary>
		/// Register an EventBus to the pipeline
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="eventBus"></param>
        public void Register(string pipelineId, IEventBus eventBus)
        {
	        if (pipelineId != eventBus.PipelineId)
	        {
		        throw new Exception($"The EventBus with PipelineId {eventBus.PipelineId} cannot be registered to the pipeline {pipelineId}");
	        }

	        _eventBusFactory.Register(pipelineId, eventBus);
        }

		/// <summary>
		/// Register an InputHandler to the pipeline
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="plugin"></param>
		public void Register(string pipelineId, IInputHandler plugin)
        {
	        if (plugin is IServerInitialize init)
	        {
		        init.Initialize(this);
	        }

	        _inputHandlers.Register(pipelineId, plugin);
        }

		/// <summary>
		/// Get the InputHandler for the pipeline
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pipelineId"></param>
		/// <returns></returns>
		public IInputHandler<T> GetHandler<T>(string pipelineId)
        {
            return _inputHandlers.GetHandler<T>(pipelineId);
        }

		/// <summary>
		/// Publish an envent to the Pipeline
		/// </summary>
		/// <param name="event"></param>
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
