using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Server.Monitoring;
using Gaucho.Storage;
using Moq;
using NUnit.Framework;

namespace Gaucho.Test.Server.Monitoring
{
	public class BackgroundProcessTests
	{
		[Test]
		public void BackgroundProcess_ctor()
		{
			var storage = new Mock<IStorage>();
			Assert.DoesNotThrow(() => new BackgroundProcess(() => { }, 1000));
		}

		[Test]
		public void BackgroundProcess_ctor_NullAction()
		{
			Assert.Throws<ArgumentNullException>(() => new BackgroundProcess(null, 1000));
		}

		[Test]
		public void BackgroundProcess_ctor_NegativeInterval()
		{
			var storage = new Mock<IStorage>();
			Assert.DoesNotThrow(() => new BackgroundProcess(() => { }, -1));
		}

		[Test]
		public void BackgroundProcess_Dispose()
		{
			var storage = new Mock<IStorage>();
			var process = new BackgroundProcess(() => { }, 1000);
			Assert.DoesNotThrow(() => process.Dispose());
		}
	}
}
