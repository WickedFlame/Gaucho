using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using StackExchange.Redis;
using Gaucho.Redis.Serializer;
using AwesomeAssertions;

namespace Gaucho.Redis.Test
{
	public class ObjectExtensionTests
	{
		[Test]
		public void ObjectExtensions_DeserializeRedis()
		{
			var hash = new HashEntry[]
			{
				new HashEntry("Id", "1"),
				new HashEntry("Value", "test")
			};
			var result = hash.DeserializeRedis<HashTest>();

			result.Id.Should().Be(1);
			result.Value.Should().Be("test");
		}

		[Test]
		public void ObjectExtensions_DeserializeRedis_NoHashValues()
		{
			var hash = new HashEntry[] { };
			var result = hash.DeserializeRedis<HashTest>();

			result.Should().BeNull();
		}

		[Test]
		public void ObjectExtensions_DeserializeRedis_NotAllSet()
		{
			var hash = new HashEntry[]
			{
				new HashEntry("Id", "1")
			};
			var result = hash.DeserializeRedis<HashTest>();

			result.Id.Should().Be(1);
			result.Value.Should().BeNull();
		}

		[Test]
		public void ObjectExtensions_DeserializeRedis_Default_Int()
		{
			var hash = new HashEntry[]
			{
				new HashEntry("Value", "test")
			};
			var result = hash.DeserializeRedis<HashTest>();

			result.Id.Should().Be(0);
			result.Value.Should().Be("test");
		}

		[Test]
		public void ObjectExtensions_DeserializeRedis_Invalid()
		{
			var hash = new HashEntry[]
			{
				new HashEntry("Id", "test"),
				new HashEntry("Value", "test")
			};
			var result = hash.DeserializeRedis<HashTest>();

			result.Id.Should().Be(0);
			result.Value.Should().Be("test");
		}

		[Test]
		public void ObjectExtensions_DeserializeRedis_CaseSensitive()
		{
			var hash = new HashEntry[]
			{
				new HashEntry("id", "1"),
				new HashEntry("value", "test")
			};
			var result = hash.DeserializeRedis<HashTest>();

			result.Id.Should().Be(0);
			result.Value.Should().BeNull();
		}

		[Test]
		public void ObjectExtensions_DeserializeRedis_Primitive_Int()
		{
			var hash = new HashEntry[]
			{
				new HashEntry("id", "1")
			};
			var result = hash.DeserializeRedis<int>();

			result.Should().Be(1);
		}

		[Test]
		public void ObjectExtensions_DeserializeRedis_Primitive_Invalid()
		{
			var hash = new HashEntry[]
			{
				new HashEntry("id", "test")
			};
			Action act = () => hash.DeserializeRedis<int>();
			act.Should().Throw<NullReferenceException>();
		}

		public class HashTest
		{
			public int Id { get; set; }

			public string Value { get; set; }
		}
	}
}
