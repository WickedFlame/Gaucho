
using System.Diagnostics;
using Gaucho.Diagnostics;

namespace Gaucho
{
	/// <summary>
	/// The EventDispatcher is used to send events to the Pipeline
	/// </summary>
    public class EventDispatcher
    {
        private readonly IProcessingServer _server;
        private readonly Stopwatch _stopwatch;
        private readonly ILogger _logger;

        /// <summary>
		/// Creates a new instance of the EventDispatcher
		/// </summary>
        public EventDispatcher() : this(ProcessingServer.Server) { }

		/// <summary>
		/// Creates a new instance of the EventDispatcher
		/// </summary>
		/// <param name="server">The <see cref="IProcessingServer"/> that the events are dispatched to</param>
		public EventDispatcher(IProcessingServer server)
        {
            _server = server;

            _stopwatch = Stopwatch.StartNew();
            _logger = LoggerConfiguration.Setup();
        }

		/// <summary>
		/// Dispatch the event to the pipeline to be processed
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="pipelineId"></param>
		/// <param name="item"></param>
        public void Process<T>(string pipelineId, T item)
        {
            _stopwatch.Restart();

            var plugin = _server.GetHandler<T>(pipelineId);
			//TODO: if pipeline is null use a default pipeline to create a real eventbus that sends the data to all OutpuHandlers without having to create a inputhandler

            if (plugin == null)
            {
	            throw new System.InvalidOperationException($"There is no IInputHandler registered to the id {pipelineId} with type {typeof(T).Name}");
            }

            var @event = plugin.ProcessInput(item);
            if (@event == null)
            {
                return;
            }

            _server.Publish(@event);

            var elapsed = _stopwatch.Elapsed.TotalMilliseconds;
            _logger.Write($"Publishing event {@event.Id} to Queue took {elapsed} ms", LogLevel.Debug, "EventDispatcher", metaData: () => new
            {
                Event = @event.Id,
                Duration = elapsed
            });
        }

        /// <summary>
        /// Checks if the pipelinie is registered in the Server
        /// </summary>
        /// <param name="pipelineId"></param>
        /// <returns></returns>
        public bool ContainsPipeline(string pipelineId)
        {
            return _server.ContainsPipeline(pipelineId);
        }
    }
}
