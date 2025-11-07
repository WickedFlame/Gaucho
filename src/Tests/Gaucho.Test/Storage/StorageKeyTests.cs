using System;
using Gaucho.Storage;
using NUnit.Framework;
using AwesomeAssertions;

namespace Gaucho.Test.Storage
{
	public class StorageKeyTests
	{
		[Test]
		public void StorageKey_Valid()
		{
			Action act = () => new StorageKey("pipelineId", "key");
			act.Should().NotThrow();
		}

		[Test]
		public void StorageKey_Servername_Valid()
		{
			Action act = () => new StorageKey("pipelineId", "key", "serverName");
			act.Should().NotThrow();
		}

		[Test]
		public void StorageKey_NUll_Key()
		{
			Action act = () => new StorageKey("pipelineId", null);
			act.Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void StorageKey_NUll_PipelineId()
		{
			Action act = () => new StorageKey(null, "key");
			act.Should().Throw<ArgumentNullException>();
		}

		[Test]
		public void StorageKey_NUll_Server()
		{
			Action act = () => new StorageKey("pipelineId", "key", null);
			act.Should().NotThrow();
		}

		[Test]
		public void StorageKey_ToString()
		{
			new StorageKey("pipelineId", "key", "server").ToString().Should().Be("server:pipelineid:key");
		}

		[Test]
		public void StorageKey_ToString_Case()
		{
			new StorageKey("pipelineId", "KEY", "Server").ToString().Should().Be("server:pipelineid:key");
		}
	}
}
