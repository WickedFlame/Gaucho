using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace Gaucho.Test
{
    [TestFixture]
    public class MetricProviderTests
    {
        [Test]
        public void EventBus_Metrics()
        {
	        var bus = new EventBus(() => null, new Guid("6815F991-82F5-49CB-8B02-C6A047765FA0").ToString());
        }
    }
}
