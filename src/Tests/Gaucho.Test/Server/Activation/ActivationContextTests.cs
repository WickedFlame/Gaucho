using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Server;
using NUnit.Framework;
using AwesomeAssertions;

namespace Gaucho.Test.Server.Activation
{
    [TestFixture]
    public class ActivationContextTests
    {
        [Test]
        public void ActivationContext_Basic()
        {
            var instance = new First();

            var ctx = new ActivationContext();
            ctx.Register<First>(() => instance);

            ctx.Registrations[typeof(First)]().Should().BeSameAs(instance);
        }

        public class First { }
    }
}
