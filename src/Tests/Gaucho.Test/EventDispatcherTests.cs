using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;

namespace Gaucho.Test
{
	public class EventDispatcherTests
	{
		[Test]
		public void EventDispatcher()
		{
			Assert.IsNotNull(new EventDispatcher(new ProcessingServer()));
		}

		[Test]
		public void EventDispatcher_Publish()
		{
			var handler = new Mock<IInputHandler<Message>>();
			handler.Setup(exp => exp.ProcessInput(It.IsAny<Message>())).Returns(() => new Event("pipeline", SimpleData.From("test")));

			var server = new Mock<IProcessingServer>();
			server.Setup(exp => exp.GetHandler<Message>("pipeline")).Returns(handler.Object);

			var dispatcher = new EventDispatcher(server.Object);
			dispatcher.Process("pipeline", new Message());

			server.Verify(exp => exp.Publish(It.Is<Event>(e => e.PipelineId == "pipeline")), Times.Once);
		}

		[Test]
		public void EventDispatcher_Null_InputHandler()
		{
			var server = new Mock<IProcessingServer>();
			server.Setup(exp => exp.GetHandler<Message>("pipeline")).Returns((IInputHandler<Message>)null);

			var dispatcher = new EventDispatcher(server.Object);
			Assert.Throws<InvalidOperationException>(() => dispatcher.Process("pipeline", new Message()));
		}

        [Test]
        public void EventDispatcher_Handler_Null_Return()
        {
            var handler = new Mock<IInputHandler<Message>>();
            handler.Setup(x => x.ProcessInput(It.IsAny<Message>())).Returns(() => null);

            var server = new Mock<IProcessingServer>();
            server.Setup(exp => exp.GetHandler<Message>("pipeline")).Returns(handler.Object);

            var dispatcher = new EventDispatcher(server.Object);
            dispatcher.Process("pipeline", new Message());

            server.Verify(x => x.Publish(It.IsAny<Event>()), Times.Never);
        }

		public class Message
		{

		}
	}
}
