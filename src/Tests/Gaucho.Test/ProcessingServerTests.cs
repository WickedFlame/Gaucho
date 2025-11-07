using Gaucho.Configuration;
using Gaucho.Server.Monitoring;
using Gaucho.Storage;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using AwesomeAssertions;

namespace Gaucho.Test
{
    [TestFixture]
    public class ProcessingServerTests
    {
	    [Test]
	    public void ProcessingServer_ctor()
	    {
		    var bus = new EventBus(() => null, "one");
		    Action act = () => new ProcessingServer();
			act.Should().NotThrow();
	    }

	    [Test]
	    public void ProcessingServer_ctor_EventBusFactory()
	    {
		    var factory = new Mock<IEventBusFactory>();
		    Action act = () => new ProcessingServer(factory.Object);
			act.Should().NotThrow();
	    }

	    [Test]
	    public void ProcessingServer_ctor_NullEventBusFactory()
	    {
		    Action act = () => new ProcessingServer(null);
			act.Should().Throw<ArgumentNullException>();
	    }

		[Test]
        public void ProcessingServer_RegisterEventBus_UnequalPipelineId()
        {
	        var bus = new EventBus(() => null, "one");
            var server = new ProcessingServer();

            Action act = () => server.Register("two", bus);
			act.Should().Throw<Exception>();
        }

        [Test]
        public void ProcessingServer_UpdatePipeline()
        {
	        var factory = new EventBusFactory();
			var server = new ProcessingServer(factory);


			var config = new PipelineConfiguration
			{
				Id = "test",
				InputHandler = new HandlerNode
				{
					Type = typeof(LogInputHandler)
				},
				OutputHandlers = new List<HandlerNode>
				{
					new HandlerNode
					{
						Type = typeof(ConsoleOutputHandler)
					}
				}
			};


			server.SetupPipeline("test", config);
	        
	        var first = factory.GetEventBus("test");
	        var second = factory.GetEventBus("test");

	        second.Should().BeSameAs(first);

			// update
	        server.SetupPipeline("test", config);

			var third = factory.GetEventBus("test");

			third.Should().NotBeSameAs(first);
		}

        [Test]
        public void ProcessingServer_Heartbeat_RegisterFactory()
        {
	        var storage = new Mock<IStorage>();
	        GlobalConfiguration.Setup(s => s.Register<IStorage>(storage.Object));

	        var factory = new EventBusFactory();
	        var server = new ProcessingServer(factory);
	        server.Register("pipeline", () => new EventPipeline());

	        // give the hearbeat some time to execute
			Task.Delay(500).Wait();

			storage.Verify(exp => exp.Set<ServerModel>(It.IsAny<StorageKey>(), It.IsAny<ServerModel>()), Times.Once);

			// cleanup
	        GlobalConfiguration.Setup(s => { });
		}

        [Test]
        public void ProcessingServer_Heartbeat_RegisterEventBus()
        {
	        var storage = new Mock<IStorage>();
	        GlobalConfiguration.Setup(s => s.Register<IStorage>(storage.Object));

	        var factory = new EventBusFactory();
	        factory.Register("pipeline", () => new EventPipeline());
	        var server = new ProcessingServer(factory);
	        server.Register("pipeline", new EventBus(() => new Mock<IEventPipeline>().Object, "pipeline"));

			// give the hearbeat some time to execute
	        Task.Delay(500).Wait();

	        storage.Verify(exp => exp.Set<ServerModel>(It.IsAny<StorageKey>(), It.IsAny<ServerModel>()), Times.Once);

	        // cleanup
			GlobalConfiguration.Setup(s => { });
		}

        [Test]
        public void ProcessingServer_Heartbeat_NoRegistration()
        {
	        var storage = new Mock<IStorage>();
	        GlobalConfiguration.Setup(s => s.Register<IStorage>(storage.Object));

	        var factory = new EventBusFactory();
	        var server = new ProcessingServer(factory);

	        // give the hearbeat some time to execute
	        Task.Delay(500).Wait();

			storage.Verify(exp => exp.Set<ServerModel>(It.IsAny<StorageKey>(), It.IsAny<ServerModel>()), Times.Never);

	        // cleanup
			GlobalConfiguration.Setup(s => { });
		}

        [Test]
        public void ProcessingServer_Dispose_RegisterFactory()
        {
	        var factory = new Mock<IEventBusFactory>();
	        var server = new ProcessingServer(factory.Object);

	        server.Register("pipeline", () => (EventPipeline) null);

			Action act = () => server.Dispose();
			act.Should().NotThrow();
        }

        [Test]
        public void ProcessingServer_Dispose_RegisterEventBus()
        {
	        var factory = new Mock<IEventBusFactory>();
	        var server = new ProcessingServer(factory.Object);

	        var bus = new Mock<IEventBus>();
	        bus.Setup(exp => exp.PipelineId).Returns("pipeline");

			server.Register("pipeline", bus.Object);

	        Action act = () => server.Dispose();
			act.Should().NotThrow();
        }

		[Test]
        public void ProcessingServer_Dispose_Uninitialized()
        {
	        var factory = new Mock<IEventBusFactory>();
	        var server = new ProcessingServer(factory.Object);

			Action act = () => server.Dispose();
			act.Should().NotThrow();
        }


        [Test]
        public void ProcessingServer_Publish()
        {
	        var bus = new Mock<IEventBus>();
	        bus.Setup(exp => exp.PipelineId).Returns("pipeline");

			var factory = new Mock<IEventBusFactory>();
			factory.Setup(exp => exp.GetEventBus(It.IsAny<string>())).Returns(() => bus.Object);
	        var server = new ProcessingServer(factory.Object);
			
			server.Publish(new Event("pipeline", new EventData()));

			bus.Verify(exp => exp.Publish(It.IsAny<Event>()), Times.Once);
        }

		[Test]
        public void ProcessingServer_Publish_NoPipeline()
        {
	        var factory = new Mock<IEventBusFactory>();
	        var server = new ProcessingServer(factory.Object);

	        Action act = () => server.Publish(new Event("pipeline", new EventData()));
			act.Should().NotThrow();
        }


        [Test]
        public void ProcessingServer_PipelineOptions()
        {
            var factory = new EventBusFactory();
            var server = new ProcessingServer(factory);


            var config = new PipelineConfiguration
            {
                Id = "test",
                InputHandler = new HandlerNode
                {
                    Type = typeof(LogInputHandler)
                },
                OutputHandlers = new List<HandlerNode>
                {
                    new HandlerNode
                    {
                        Type = typeof(ConsoleOutputHandler)
                    }
                },
				Options = new PipelineOptions
                {
                    MinProcessors = 1,
					MaxItemsInQueue = 100,
					MaxProcessors = 1
                }
            };


            server.SetupPipeline("test", config);

            var bus = factory.GetEventBus("test");
            bus.PipelineFactory.Options.Should().BeSameAs(config.Options);
        }

        [TestCase("pipeline_1", true)]
        [TestCase("PIPELINE_1", true)]
        [TestCase("pipeline_2", false)]
        public void ProcessingServer_ContainsPipeline(string pipelineId, bool expected)
        {
            var factory = new Mock<IEventBusFactory>();
            factory.Setup(x => x.Pipelines).Returns(() => new[] { "pipeline_1" });
            var server = new ProcessingServer(factory.Object);

            server.ContainsPipeline(pipelineId).Should().Be(expected);
        }
    }
}
