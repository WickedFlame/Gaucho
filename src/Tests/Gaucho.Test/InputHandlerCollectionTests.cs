using Moq;
using System.Linq;
using System;

namespace Gaucho.Test
{
    public class InputHandlerCollectionTests
    {
        [Test]
        public void InputHandlerCollection_ctor()
        {
            Action act = () => new InputHandlerCollection();
            act.Should().NotThrow();
        }

        [Test]
        public void InputHandlerCollection_Register()
        {
            var handler = new Mock<IInputHandler>();

            var collection = new InputHandlerCollection();
            collection.Register("pipeline", handler.Object);

            collection.Single().Should().BeSameAs(handler.Object);
        }

        [Test]
        public void InputHandlerCollection_ReRegister()
        {
            var handler = new Mock<IInputHandler>();

            var collection = new InputHandlerCollection();
            collection.Register("pipeline", new Mock<IInputHandler>().Object);
            collection.Register("pipeline", handler.Object);

            collection.Single().Should().BeSameAs(handler.Object);
        }

        [Test]
        public void InputHandlerCollection_Gethandler()
        {
            var handler = new Mock<IInputHandler<TestObject>>();

            var collection = new InputHandlerCollection();
            collection.Register("pipeline", handler.Object);

            collection.GetHandler<TestObject>("pipeline").Should().BeSameAs(handler.Object);
        }

        [Test]
        public void InputHandlerCollection_Gethandler_InvalidType()
        {
            var handler = new Mock<IInputHandler>();

            var collection = new InputHandlerCollection();
            collection.Register("pipeline", handler.Object);

            collection.GetHandler<InputHandlerCollectionTests>("pipeline").Should().BeNull();
        }

        [Test]
        public void InputHandlerCollection_Gethandler_InvalidPipeline()
        {
            var collection = new InputHandlerCollection();

            collection.GetHandler<IInputHandler>("pipeline").Should().BeNull();
        }

        [Test]
        public void InputHandlerCollection_Gethandler_IgnoreCase()
        {
            var collection = new InputHandlerCollection();
            collection.Register("PIPELINE", new Mock<IInputHandler<TestObject>>().Object);

            collection.GetHandler<TestObject>("pipeline").Should().NotBeNull();
        }

        public class TestObject
        {
        }
    }
}
