using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Diagnostics;
using NUnit.Framework;

namespace Gaucho.Test.Server
{
    [TestFixture]
    public class CustomEventBusTests
    {
        [Test]
        public void Server_CustomEventBus()
        {
            var pipelineId = Guid.NewGuid().ToString();

            var cnt = 0;

            ProcessingServer.Server.SetupPipeline(pipelineId, s =>
            {
                s.Register(() =>
                {
                    var pipeline = new EventPipeline();
                    pipeline.AddHandler(new ConsoleOutputHandler());
                    cnt = cnt + 1;

                    return pipeline;
                });

                s.Register(new CustomEventBus());
                s.Register(new CustomInputHandler());
            });


            var client = new EventDispatcher();
            client.Process(pipelineId, new LogMessage { Message = "StaticServer_NewPipelinePerEvent1" });
            client.Process(pipelineId, new LogMessage { Message = "StaticServer_NewPipelinePerEvent2" });

            Assert.That(cnt == 1);
        }

        public class CustomEventBus : IEventBus
        {
            private readonly EventQueue _queue;
            private readonly ILogger _logger;
            private IPipelineSetup _pipelineFactory;
            private IEventPipeline _pipeline;

            public CustomEventBus()
            {
                _queue = new EventQueue();

                _logger = LoggerConfiguration.Setup();
            }

            public string PipelineId { get; set; }

            public int ThreadCount { get; } = 0;

            public int QueueSize => _queue.Count;

            public void SetPipeline(IPipelineSetup factory)
            {
                _pipelineFactory = factory;
            }

            public void Publish(Event @event)
            {
                _queue.Enqueue(@event);
                Process();
            }

            public void Process()
            {
                var pipeline = GetPipeline();
                if (pipeline == null)
                {
                    _logger.Write($"Pipeline with the Id {PipelineId} does not exist. Event could not be sent to any Pipeline.", Category.Log, LogLevel.Error, "SimpleEventBus");
                    return;
                }

                while (_queue.TryDequeue(out var @event))
                {
                    pipeline.Run(@event);
                }
            }

            private IEventPipeline GetPipeline()
            {
                if (_pipeline == null)
                {
                    _pipeline = _pipelineFactory.Setup();
                }

                return _pipeline;
            }

            public void Dispose()
            {
            }
        }
    }
}
