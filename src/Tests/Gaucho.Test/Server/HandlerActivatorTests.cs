using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Server;
using NUnit.Framework;

namespace Gaucho.Test.Server
{
    [TestFixture]
    public class HandlerActivatorTests
    {
        [Test]
        public void Server_HandlerActivator_CreateInstance()
        {
            var builder = new HandlerActivator();
            var instance = builder.CreateInstance<Simple>(typeof(Simple), new Dictionary<Type, object>());

            Assert.IsNotNull(instance);
        }

        [Test]
        public void Server_HandlerActivator_CreateInstance_WithArguments()
        {
            var builder = new HandlerActivator();
            var dependencies = new Dictionary<Type, object>
            {
                {typeof(Simple), new Simple()}
            };
            var instance = builder.CreateInstance<Complex>(typeof(Complex), dependencies);

            Assert.IsNotNull(instance);
        }

        [Test]
        public void Server_HandlerActivator_CreateInstance_MultipleCtor()
        {
            var builder = new HandlerActivator();
            var dependencies = new Dictionary<Type, object>
            {
                {typeof(Simple), new Simple()},
                {typeof(Complex), new Complex(new Simple())}
            };
            var instance = builder.CreateInstance<Extended>(typeof(Extended), dependencies);

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Simple);
            Assert.IsNotNull(instance.Complex);
        }

        [Test]
        public void Server_HandlerActivator_CreateInstance_MultipleCtor_SelectCtor()
        {
            var builder = new HandlerActivator();
            var dependencies = new Dictionary<Type, object>
            {
                {typeof(Simple), new Simple()}
            };
            var instance = builder.CreateInstance<Extended>(typeof(Extended), dependencies);

            Assert.IsNotNull(instance);
            Assert.IsNotNull(instance.Simple);
            Assert.IsNull(instance.Complex);
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
