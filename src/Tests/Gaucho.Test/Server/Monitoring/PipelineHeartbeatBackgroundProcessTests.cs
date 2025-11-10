using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Server.Monitoring;
using Gaucho.Storage;
using Moq;
using NUnit.Framework;
using AwesomeAssertions;

namespace Gaucho.Test.Server.Monitoring
{
	public class PipelineHeartbeatBackgroundProcessTests
	{
		[Test]
		public void PipelineHeartbeatBackgroundProcess_ctor()
		{
			var storage = new Mock<IStorage>();
			new PipelineHeartbeatBackgroundProcess(storage.Object, "pipelineId").Should().NotBeNull();
		}

		[Test]
		public void PipelineHeartbeatBackgroundProcess_ctor_NullStorage()
		{
			((Action)(() => new PipelineHeartbeatBackgroundProcess(null, "pipelineId"))).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void PipelineHeartbeatBackgroundProcess_ctor_NullPipelineId()
		{
			var storage = new Mock<IStorage>();
			((Action)(() => new PipelineHeartbeatBackgroundProcess(storage.Object, null))).Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void PipelineHeartbeatBackgroundProcess_Dispose()
		{
			var storage = new Mock<IStorage>();
			var process = new PipelineHeartbeatBackgroundProcess(storage.Object, "pipelineId");
			((Action)(() => process.Dispose())).Should().NotThrow();
		}
	}
}
