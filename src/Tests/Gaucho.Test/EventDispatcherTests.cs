using Moq;
using System;

namespace Gaucho.Test
{
	public class EventDispatcherTests
	{
		[Test]
		public void EventDispatcher()
		{
			var act = () => new EventDispatcher(new ProcessingServer());
			act.Should().NotThrow();
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
			var act = () => dispatcher.Process("pipeline", new Message());

            act.Should().Throw<InvalidOperationException>();
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

		[TestCase("pipeline_1", true)]
        [TestCase("pipeline_2", false)]
        public void EventDispatcher_ContainsPipeline(string pipelineId, bool expected)
        {
            var server = new Mock<IProcessingServer>();
            server.Setup(exp => exp.ContainsPipeline("pipeline_1")).Returns(() => true);

            var dispatcher = new EventDispatcher(server.Object);
            dispatcher.ContainsPipeline(pipelineId).Should().Be(expected);
        }

        public class Message
		{

		}
	}
}
