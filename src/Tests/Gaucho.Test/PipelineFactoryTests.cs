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
			Assert.IsNotNull(new PipelineFactory(() => null));
		}

		[Test]
		public void PipelineFactory_()
		{
			var pipeline = new Mock<IEventPipeline>();
			var factory = new PipelineFactory(() => pipeline.Object);

			Assert.IsNotNull(factory.Setup());
		}
	}
}
