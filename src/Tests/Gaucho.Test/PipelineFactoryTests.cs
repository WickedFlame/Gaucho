using System;
using System.Collections.Generic;
using System.Text;
using Moq;
using NUnit.Framework;

namespace Gaucho.Test
{
	public class PipelineFactoryTests
	{
		[Test]
		public void PipelineFactory()
		{
			new PipelineFactory(() => null, new PipelineOptions()).Should().NotBeNull();
		}

		[Test]
		public void PipelineFactory_()
		{
			var pipeline = new Mock<IEventPipeline>();
			var factory = new PipelineFactory(() => pipeline.Object, new PipelineOptions());

			factory.Setup().Should().NotBeNull();
		}
	}
}
