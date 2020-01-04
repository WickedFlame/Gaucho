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
            var bus = new EventBus("one");
            var factory = new EventBusFactory();

            Assert.Throws<Exception>(() => factory.Register("two", bus));
        }
    }
}
