using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Configuration;
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
    }
}
