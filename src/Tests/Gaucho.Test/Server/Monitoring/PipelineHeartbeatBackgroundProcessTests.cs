using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Server.Monitoring;
using Gaucho.Storage;
using Moq;
using NUnit.Framework;

namespace Gaucho.Test.Server.Monitoring
{
	public class PipelineHeartbeatBackgroundProcessTests
	{
		[Test]
		public void PipelineHeartbeatBackgroundProcess_ctor()
		{
			var storage = new Mock<IStorage>();
			Assert.DoesNotThrow(() => new PipelineHeartbeatBackgroundProcess(storage.Object, "pipelineId"));
		}

		[Test]
		public void PipelineHeartbeatBackgroundProcess_ctor_NullStorage()
		{
			Assert.Throws<ArgumentNullException>(() => new PipelineHeartbeatBackgroundProcess(null, "pipelineId"));
		}

		[Test]
		public void PipelineHeartbeatBackgroundProcess_ctor_NullPipelineId()
		{
			var storage = new Mock<IStorage>();
			Assert.Throws<ArgumentNullException>(() => new PipelineHeartbeatBackgroundProcess(storage.Object, null));
		}

		[Test]
		public void PipelineHeartbeatBackgroundProcess_Dispose()
		{
			var storage = new Mock<IStorage>();
			var process = new PipelineHeartbeatBackgroundProcess(storage.Object, "pipelineId");
			Assert.DoesNotThrow(() => process.Dispose());
		}
	}
}
