using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;

namespace Gaucho.Test
{
    public class InputHandlerCollectionTests
    {
        [Test]
        public void InputHandlerCollection_ctor()
        {
            Assert.DoesNotThrow(() => new InputHandlerCollection());
        }

        [Test]
        public void InputHandlerCollection_Register()
        {
            var handler = new Mock<IInputHandler>();

            var collection = new InputHandlerCollection();
            collection.Register("pipeline", handler.Object);

            Assert.AreSame(handler.Object, collection.Single());
        }

        [Test]
        public void InputHandlerCollection_ReRegister()
        {
            var handler = new Mock<IInputHandler>();

            var collection = new InputHandlerCollection();
            collection.Register("pipeline", new Mock<IInputHandler>().Object);
            collection.Register("pipeline", handler.Object);

            Assert.AreSame(handler.Object, collection.Single());
        }

        [Test]
        public void InputHandlerCollection_Gethandler()
        {
            var handler = new Mock<IInputHandler<TestObject>>();

            var collection = new InputHandlerCollection();
            collection.Register("pipeline", handler.Object);

            Assert.AreSame(handler.Object, collection.GetHandler<TestObject>("pipeline"));
        }

        [Test]
        public void InputHandlerCollection_Gethandler_InvalidType()
        {
            var handler = new Mock<IInputHandler>();

            var collection = new InputHandlerCollection();
            collection.Register("pipeline", handler.Object);

            Assert.IsNull(collection.GetHandler<InputHandlerCollectionTests>("pipeline"));
        }

        [Test]
        public void InputHandlerCollection_Gethandler_InvalidPipeline()
        {
            var collection = new InputHandlerCollection();

            Assert.IsNull(collection.GetHandler<IInputHandler>("pipeline"));
        }

        public class TestObject
        {
        }
    }
}
