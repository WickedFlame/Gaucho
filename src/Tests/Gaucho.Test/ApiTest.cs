using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Gaucho.Diagnostics;
using NUnit.Framework;

namespace Gaucho.Test
{
    public class ApiTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void EventBus()
        {
            var pipelineId = Guid.NewGuid().ToString();

            var eventBus = new EventBus(() =>
            {
                var pipeline = new EventPipeline();
                pipeline.AddHandler(new ConsoleOutputHandler());

                return pipeline;
            });

            var @event = new Event(pipelineId, new SingleNode("Testdata"));

            eventBus.Publish(@event);
        }

        [Test]
        public void InstanceServer()
        {
            var pipelineId = Guid.NewGuid().ToString();

            var factory = new EventBusFactory();
            factory.Register(pipelineId, () =>
            {
                var pipeline = new EventPipeline();
                pipeline.AddHandler(new ConsoleOutputHandler());

                return pipeline;
            });

            var server = new ProcessingServer(factory);
            server.Register(pipelineId, new LogInputHandler());

            var client = new EventDispatcher(server);
            client.Process(pipelineId, new LogMessage { Message = "InstanceServer" });
        }

        [Test]
        public void InstanceServer_FactorySetEventBus()
        {
            var pipelineId = Guid.NewGuid().ToString();

            var factory = new EventBusFactory();
            factory.Register(pipelineId, () =>
            {
                var pipeline = new EventPipeline();
                pipeline.AddHandler(new ConsoleOutputHandler());

                return pipeline;
            });
            factory.Register(pipelineId, new EventBus());

            var server = new ProcessingServer(factory);
            server.Register(pipelineId, new LogInputHandler());

            var client = new EventDispatcher(server);
            client.Process(pipelineId, new LogMessage { Message = "InstanceServer" });
        }

        [Test]
        public void InstanceServer_Simple()
        {
            var pipelineId = Guid.NewGuid().ToString();

            var server = new ProcessingServer();
            server.Register(pipelineId, () =>
            {
                var pipeline = new EventPipeline();
                pipeline.AddHandler(new ConsoleOutputHandler());

                return pipeline;
            });
            server.Register(pipelineId, new LogInputHandler());


            var client = new EventDispatcher(server);
            client.Process(pipelineId, new LogMessage { Message = "InstanceServer" });
        }

        [Test]
        public void InstanceServer_SetEventBus()
        {
            var pipelineId = Guid.NewGuid().ToString();

            var server = new ProcessingServer();
            server.Register(pipelineId, () =>
            {
                var pipeline = new EventPipeline();
                pipeline.AddHandler(new ConsoleOutputHandler());

                return pipeline;
            });
            server.Register(pipelineId, new EventBus());
            server.Register(pipelineId, new LogInputHandler());


            var client = new EventDispatcher(server);
            client.Process(pipelineId, new LogMessage { Message = "InstanceServer" });
        }

        [Test]
        public void StaticServer()
        {
            var pipelineId = Guid.NewGuid().ToString();

            ProcessingServer.Server.Register(pipelineId, () =>
            {
                var pipeline = new EventPipeline();
                pipeline.AddHandler(new ConsoleOutputHandler());

                return pipeline;
            });
            ProcessingServer.Server.Register(pipelineId, new LogInputHandler());


            var client = new EventDispatcher();
            client.Process(pipelineId, new LogMessage {Message = "StaticServer" });
        }

        [Test]
        public void StaticServer_SetupPipeline()
        {
            var pipelineId = Guid.NewGuid().ToString();

            ProcessingServer.Server.SetupPipeline(pipelineId, s =>
            {
                s.Register(() =>
                {
                    var pipeline = new EventPipeline();
                    pipeline.AddHandler(new ConsoleOutputHandler());

                    return pipeline;
                });

                s.Register(new LogInputHandler());
            });


            var client = new EventDispatcher();
            client.Process(pipelineId, new LogMessage { Message = "StaticServer" });
        }

        [Test]
        public void StaticServer_SetupPipeline_SimpleServer()
        {
            var pipelineId = new Guid("42C75D31-8679-49B4-B0EE-2B90D4C6B893").ToString();

            var server = new ProcessingServer();
            server.SetupPipeline(pipelineId, s =>
            {
                s.Register(() =>
                {
                    var pipeline = new EventPipeline();
                    pipeline.AddHandler(new ConsoleOutputHandler());

                    return pipeline;
                });

                s.Register(new LogInputHandler());
            });


            var client = new EventDispatcher(server);
            client.Process(pipelineId, new LogMessage { Message = "SetupPipeline_SimpleServer" });
        }

        [Test]
        public void StaticServer_SamePipelineAllEvents()
        {
            var pipelineId = new Guid("FB4336C9-23E7-42B0-B25F-0A92D06508A7").ToString();

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

                s.Register(new CustomInputHandler());
            });


            var client = new EventDispatcher();
            client.Process(pipelineId, new LogMessage { Message = "StaticServer_NewPipelinePerEvent1" });
            client.Process(pipelineId, new LogMessage { Message = "StaticServer_NewPipelinePerEvent2" });

            Assert.That(cnt >= 0);

            ProcessingServer.Server.WaitAll(pipelineId);

            Assert.That(cnt >= 1);
        }

        [Test]
        public void StaticServer_SamePipelineAllEvents_Sync()
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

                s.Register(new SimpleEventBus());
                s.Register(new CustomInputHandler());
            });


            var client = new EventDispatcher();
            client.Process(pipelineId, new LogMessage { Message = "StaticServer_NewPipelinePerEvent1" });
            client.Process(pipelineId, new LogMessage { Message = "StaticServer_NewPipelinePerEvent2" });

            Assert.That(cnt == 1);
        }

        [Test]
        public void StaticServer_WaitAll()
        {
            var pipelineId = Guid.NewGuid().ToString();

            var logHandler = new LogQueueHandler();

            ProcessingServer.Server.SetupPipeline(pipelineId, s =>
            {
                s.Register(() =>
                {
                    var pipeline = new EventPipeline();
                    pipeline.AddHandler(new ConsoleOutputHandler());
                    pipeline.AddHandler(logHandler);
                    //pipeline.AddHandler(new ThreadWaitHandler());

                    return pipeline;
                });

                s.Register(new CustomInputHandler());
            });


            var client = new EventDispatcher();
            client.Process(pipelineId, new LogMessage { Message = "StaticServer_NewPipelinePerEvent1" });
            client.Process(pipelineId, new LogMessage { Message = "StaticServer_NewPipelinePerEvent2" });

            ProcessingServer.Server.WaitAll(pipelineId);

            Assert.That(logHandler.Log.Count() == 2);

            client.Process(pipelineId, new LogMessage { Message = "StaticServer_NewPipelinePerEvent3" });
            client.Process(pipelineId, new LogMessage { Message = "StaticServer_NewPipelinePerEvent4" });

            ProcessingServer.Server.WaitAll(pipelineId);

            Assert.That(logHandler.Log.Count() == 4);
        }

        public class ThreadWaitHandler : IOutputHandler
        {
            public void Handle(Event @event)
            {
                Thread.Sleep(5000);
            }
        }
    }
}