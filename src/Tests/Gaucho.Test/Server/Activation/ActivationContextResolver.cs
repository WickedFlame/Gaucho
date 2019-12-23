using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Server;
using NUnit.Framework;

namespace Gaucho.Test.Server.Activation
{
    [TestFixture]
    public class ActivationContextResolver
    {
        [Test]
        public void ActivationContext_Resolver_Basic()
        {
            var ctx = new ActivationContext();
            var item = ctx.Resolve<First>();

            Assert.IsNotNull(item);
        }

        [Test]
        public void ActivationContext_Resolver_BasicInjection()
        {
            var ctx = new ActivationContext();
            var item = ctx.Resolve<Second>();

            Assert.IsNotNull(item);
        }

        [Test]
        public void ActivationContext_Resolver_Interface()
        {
            var ctx = new ActivationContext();
            ctx.Register<IOne, One>();

            var item = ctx.Resolve<IOne>();

            Assert.IsNotNull(item);
        }

        [Test]
        public void ActivationContext_Resolver_InterfaceInjection()
        {
            var ctx = new ActivationContext();
            ctx.Register<IOne, One>();
            ctx.Register<ITwo, Two>();

            var item = ctx.Resolve<ITwo>();

            Assert.IsNotNull(item);
        }

        public class First { }

        public class Second
        {
            public Second(First first)
            {
                First = first;
            }

            public First First { get; }
        }

        public interface IOne { }

        public class One : IOne { }

        public interface ITwo { }

        public class Two : ITwo
        {
            public Two(IOne one)
            {
                One = one;
            }

            public IOne One { get; }
        }
    }
}
