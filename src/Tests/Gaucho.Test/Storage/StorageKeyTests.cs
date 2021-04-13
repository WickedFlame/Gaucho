using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Storage;
using NUnit.Framework;

namespace Gaucho.Test.Storage
{
	public class StorageKeyTests
	{
		[Test]
		public void StorageKey_Valid()
		{
			Assert.DoesNotThrow(() => new StorageKey("pipelineId", "key"));
		}

		[Test]
		public void StorageKey_Servername_Valid()
		{
			Assert.DoesNotThrow(() => new StorageKey("pipelineId", "key", "serverName"));
		}

		[Test]
		public void StorageKey_NUll_Key()
		{
			Assert.Throws<ArgumentNullException>(() => new StorageKey("pipelineId", null));
		}

		[Test]
		public void StorageKey_NUll_PipelineId()
		{
			Assert.Throws<ArgumentNullException>(() => new StorageKey(null, "key"));
		}

		[Test]
		public void StorageKey_NUll_Server()
		{
			Assert.DoesNotThrow(() => new StorageKey("pipelineId", "key", null));
		}
	}
}
