using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Server;
using NUnit.Framework;
using AwesomeAssertions;

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
            instance.Should().NotBeNull();
        }

        [Test]
        public void Server_ActivationContext_CreateInstance_WithArguments()
        {
            var builder = new ActivationContext();
            var instance = builder.Resolve<Complex>();
            instance.Should().NotBeNull();
        }

        [Test]
        public void Server_ActivationContext_CreateInstance_WithMoreArguments()
        {
            var builder = new ActivationContext();
            builder.Register<Simple>(() => new Simple());
            var instance = builder.Resolve<Complex>(typeof(Complex));
            instance.Should().NotBeNull();
        }

        [Test]
        public void Server_ActivationContext_CreateInstance_MultipleCtor()
        {
            var simple = new Simple();
            var builder = new ActivationContext();
            builder.Register<Simple>(() => simple);
            var instance = builder.Resolve<Extended>();
            instance.Should().NotBeNull();
            instance.Simple.Should().NotBeNull();
            instance.Complex.Should().NotBeNull();
        }

        [Test]
        public void Server_ActivationContext_CreateInstance_MultipleCtor_SelectCtor()
        {
            var builder = new ActivationContext();
            var instance = builder.Resolve<Extended>();
            instance.Should().NotBeNull();
            instance.Simple.Should().NotBeNull();
            instance.Complex.Should().NotBeNull();
        }

        public class Simple { }
        public class Complex { public Complex(Simple simple) { } }
        public class Extended
        {
            public Extended() { }
            public Extended(Simple simple) { Simple = simple; }
            public Extended(Complex complex, Simple simple) { Complex = complex; Simple = simple; }
            public Complex Complex { get; }
            public Simple Simple { get; }
        }
    }
}
