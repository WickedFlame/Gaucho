using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Server;
using NUnit.Framework;

namespace Gaucho.Test.Server.Activation
{
    [TestFixture]
    public class HandlerActivatorTests
    {
        [Test]
        public void Server_ActivationContext_CreateInstance()
        {
            var builder = new ActivationContext();
            var instance = builder.Resolve<Simple>(typeof(Simple));

            Assert.IsNotNull(instance);
        }

        [Test]
        public void Server_ActivationContext_CreateInstance_WithArguments()
        {
            var builder = new ActivationContext();
            
            var instance = builder.Resolve<Complex>();

            Assert.IsNotNull(instance);
        }

        [Test]
        public void Server_ActivationContext_CreateInstance_WithMoreArguments()
        {
            var builder = new ActivationContext();
            builder.Register<Simple>(() => new Simple());
            var instance = builder.Resolve<Complex>(typeof(Complex));

            Assert.IsNotNull(instance);
        }

        [Test]
        public void Server_ActivationContext_CreateInstance_MultipleCtor()
        {
            var simple = new Simple();

            var builder = new ActivationContext();
            builder.Register<Simple>(() => simple);

            var instance = builder.Resolve<Extended>();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Simple);
            Assert.IsNotNull(instance.Complex);
        }

        [Test]
        public void Server_ActivationContext_CreateInstance_MultipleCtor_SelectCtor()
        {
            var builder = new ActivationContext();

            var instance = builder.Resolve<Extended>();

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Simple);
            Assert.IsNotNull(instance.Complex);
        }

        public class Simple
        {
            //public Simple() { }
        }

        public class Complex
        {
            public Complex(Simple simple) { }
        }

        public class Extended
        {
            public Extended() { }

            public Extended(Simple simple)
            {
                Simple = simple;
            }

            public Extended(Complex complex, Simple simple)
            {
                Complex = complex;
                Simple = simple;
            }

            public Complex Complex { get; }

            public Simple Simple { get; }
        }
    }
}
