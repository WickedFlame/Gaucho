using System;
using Broadcast;

namespace Gaucho.Server.Test.Handlers
{
    public class RecurringInputHandler : IInputHandler, IServerInitialize, IDisposable
    {
        private IProcessingServer _server;
        private Broadcaster _broadcaster;
        private ConfiguredArgumentsCollection _arguments;

        public RecurringInputHandler(ConfiguredArgumentsCollection arguments)
        {
            _arguments = arguments;
        }

        public string PipelineId { get; set; }

        public void Initialize(IProcessingServer server)
        {
            _server = server;
            _broadcaster = new Broadcaster(ProcessorMode.Background);

            //TODO: Recurring Interval should be passed in as Parameter
            _broadcaster.Recurring(Process, TimeSpan.FromSeconds(2));
        }

        private int _count = 0;

        public void Process()
        {
            _count += 1;
            var input = new
            {
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
