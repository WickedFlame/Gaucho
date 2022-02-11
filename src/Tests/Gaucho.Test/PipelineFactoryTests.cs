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
			Assert.IsNotNull(new PipelineFactory(() => null, new PipelineOptions()));
		}

		[Test]
		public void PipelineFactory_()
		{
			var pipeline = new Mock<IEventPipeline>();
			var factory = new PipelineFactory(() => pipeline.Object, new PipelineOptions());

			Assert.IsNotNull(factory.Setup());
		}
	}
}
