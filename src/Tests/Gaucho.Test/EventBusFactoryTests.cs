using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Gaucho.Test
{
    [TestFixture]
    public class EventBusFactoryTests
    {
        [Test]
        public void EventBusFactory_RegisterEventBus_UnequalPipelineId()
        {
	        var bus = new EventBus(() => null, "one");
            var factory = new EventBusFactory();

            Assert.Throws<Exception>(() => factory.Register("two", bus));
        }

        [Test]
        public void EventBusFactory_UpdatePipeline()
        {
	        var factory = new EventBusFactory();
	        factory.Register("test", () =>
	        {
		        var pipeline = new EventPipeline();
		        pipeline.AddHandler(new ConsoleOutputHandler());

		        return pipeline;
	        });

	        var first = factory.GetEventBus("test");
	        var second = factory.GetEventBus("test");

	        Assert.AreSame(first, second);

			factory.Register("test", () =>
	        {
		        var pipeline = new EventPipeline();
		        pipeline.AddHandler(new ConsoleOutputHandler());

		        return pipeline;
	        });

	        var third = factory.GetEventBus("test");

	        Assert.AreNotSame(first, third);
        }
    }
}
