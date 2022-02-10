using System;

namespace Gaucho.Server
{
	/// <summary>
	/// Used to register elements to a pipeline
	/// </summary>
    public class PipelineRegistration
    {
        private readonly IProcessingServer _server;
        private readonly string _pipelineId;

		/// <summary>
		/// Creates a new instance of the PipelineRegistration
		/// </summary>
		/// <param name="pipelineId"></param>
		/// <param name="server"></param>
        internal PipelineRegistration(string pipelineId, IProcessingServer server)
        {
            _pipelineId = pipelineId;
            _server = server;
        }

        /// <summary>
        /// Register a <see cref="EventPipeline"/> Factory to the Server
        /// </summary>
        /// <param name="factory"></param>
        public void Register(Func<EventPipeline> factory) 
            => Register(factory, new PipelineOptions());

		/// <summary>
		/// Register a <see cref="EventPipeline"/> Factory to the Server
		/// </summary>
		/// <param name="factory"></param>
		/// <param name="options"></param>
		public void Register(Func<EventPipeline> factory, PipelineOptions options)
        {
            _server.Register(_pipelineId, factory, options);
        }

		/// <summary>
		/// Register a <see cref="IEventBus"/> to the Server
		/// </summary>
		/// <param name="eventBus"></param>
		public void Register(IEventBus eventBus)
        {
            if (_pipelineId != eventBus.PipelineId)
            {
                throw new Exception($"The EventBus with PipelineId {eventBus.PipelineId} cannot be registered to the pipeline {_pipelineId}");
            }

            _server.Register(_pipelineId, eventBus);
        }

		/// <summary>
		/// Register a <see cref="IInputHandler"/> to the Server
		/// </summary>
		/// <param name="plugin"></param>
		public void Register(IInputHandler plugin)
        {
            if (plugin == null)
            {
                return;
            }

            _server.Register(_pipelineId, plugin);
        }
    }
}
