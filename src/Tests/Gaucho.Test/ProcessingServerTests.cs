using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Gaucho.Configuration;
using Gaucho.Server.Monitoring;
using Gaucho.Storage;
using Moq;
using NUnit.Framework;

namespace Gaucho.Test
{
    [TestFixture]
    public class ProcessingServerTests
    {
        [Test]
        public void ProcessingServer_RegisterEventBus_UnequalPipelineId()
        {
	        var bus = new EventBus(() => null, "one");
            var server = new ProcessingServer();

            Assert.Throws<Exception>(() => server.Register("two", bus));
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

	        Assert.AreSame(first, second);

			// update
	        server.SetupPipeline("test", config);

			var third = factory.GetEventBus("test");

	        Assert.AreNotSame(first, third);
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

			storage.Verify(exp => exp.Set<HeartbeatModel>(It.IsAny<StorageKey>(), It.IsAny<HeartbeatModel>()), Times.Once);

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
	        server.Register("pipeline", new EventBus(() => null, "pipeline"));

			// give the hearbeat some time to execute
	        Task.Delay(500).Wait();

	        storage.Verify(exp => exp.Set<HeartbeatModel>(It.IsAny<StorageKey>(), It.IsAny<HeartbeatModel>()), Times.Once);

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

			storage.Verify(exp => exp.Set<HeartbeatModel>(It.IsAny<StorageKey>(), It.IsAny<HeartbeatModel>()), Times.Never);

	        // cleanup
			GlobalConfiguration.Setup(s => { });
		}
	}
}
