using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Gaucho.Test
{
    [TestFixture]
    public class ProcessingServerTests
    {
        [Test]
        public void ProcessingServer_RegisterEventBus_UnequalPipelineId()
        {
            var bus = new EventBus("one");
            var server = new ProcessingServer();

            Assert.Throws<Exception>(() => server.Register("two", bus));
        }
    }
}
