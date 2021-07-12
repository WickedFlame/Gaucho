using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Server.Monitoring;
using Gaucho.Storage;
using Moq;
using NUnit.Framework;

namespace Gaucho.Test.Server.Monitoring
{
	public class ServerHeartbeatBackgroundProcessTests
	{
		[Test]
		public void ServerHeartbeatBackgroundProcess_ctor()
		{
			var storage = new Mock<IStorage>();
			Assert.DoesNotThrow(() => new ServerHeartbeatBackgroundProcess(storage.Object));
		}

		[Test]
		public void ServerHeartbeatBackgroundProcess_ctor_NullStorage()
		{
			Assert.Throws<ArgumentNullException>(() => new ServerHeartbeatBackgroundProcess(null));
		}

		[Test]
		public void ServerHeartbeatBackgroundProcess_Dispose()
		{
			var storage = new Mock<IStorage>();
			var process = new ServerHeartbeatBackgroundProcess(storage.Object);
			Assert.DoesNotThrow(() => process.Dispose());
		}
	}
}
