using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using AwesomeAssertions;

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

            Action act = () => factory.Register("two", bus);
            act.Should().Throw<Exception>();
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

	        second.Should().BeSameAs(first);

			factory.Register("test", () =>
	        {
		        var pipeline = new EventPipeline();
		        pipeline.AddHandler(new ConsoleOutputHandler());

		        return pipeline;
	        });

	        var third = factory.GetEventBus("test");

	        third.Should().NotBeSameAs(first);
        }
    }
}
