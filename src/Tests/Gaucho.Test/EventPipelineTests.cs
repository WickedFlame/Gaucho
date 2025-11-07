using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using NUnit.Framework;

namespace Gaucho.Test
{
	public class EventPipelineTests
	{
		[Test]
		public void EventPipeline_Ctor()
		{
			var pipeline = new EventPipeline();
			pipeline.Handlers.Should().NotBeNull();
		}

		[Test]
		public void EventPipeline_AddHandler()
		{
			var handler = new Mock<IOutputHandler>();
			var pipeline = new EventPipeline();
			pipeline.AddHandler(handler.Object);

			pipeline.Handlers.Any().Should().BeTrue();
		}

		[Test]
		public void EventPipeline_Run()
		{
			var handler = new Mock<IOutputHandler>();
			var pipeline = new EventPipeline();
			pipeline.AddHandler(handler.Object);

			pipeline.Run(new Event("id", "value"));

			handler.Verify(exp => exp.Handle(It.Is<Event>(e => e.PipelineId == "id")), Times.Once);
		}
	}
}
