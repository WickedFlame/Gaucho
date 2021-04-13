using System;
using Broadcast;

namespace Gaucho.Server.Test.Handlers
{
    public class RecurringInputHandler : IInputHandler, IServerInitialize, IDisposable
    {
        private IProcessingServer _server;
        private Broadcaster _broadcaster;
        private ConfiguredArguments _arguments;

        public RecurringInputHandler(ConfiguredArguments arguments)
        {
            _arguments = arguments;
        }

        public string PipelineId { get; set; }

        public void Initialize(IProcessingServer server)
        {
            _server = server;
            _broadcaster = new Broadcaster(ProcessorMode.Background);

            var interval = _arguments.GetValue<int>("Interval");
            if (interval == 0)
            {
                interval = 10;
            }

			// delay start to end initializing
            _broadcaster.Schedule(() => _broadcaster.Recurring(Process, TimeSpan.FromSeconds(interval)), TimeSpan.FromSeconds(10));
        }

        private int _count = 0;

        public void Process()
        {
            _count += 1;
            var input = new
            {
				Date = DateTime.Now,
                Message = $"RecurrinHandler count: {_count}"
            };

            var factory = new EventDataFactory();
            var data = factory.BuildFrom(input);

            _server.Publish(new Event(PipelineId, data));
        }

        public void Dispose()
        {
            _broadcaster.Dispose();
        }
    }
}
