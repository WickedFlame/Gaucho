using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Redis.Serializer;
using NUnit.Framework;
using Polaroider;
using AwesomeAssertions;

namespace Gaucho.Redis.Test
{
	public class JsonSerializerTests
	{
		[Test]
		public void JsonSerializer_Serialize_Object()
		{
			var serializer = new JsonSerializer();
			var value = serializer.Serialize(new JsonModel
			{
				Id =1,
				Name = "first value",
				Doubl =2.2
			});

			value.Should().Be("{Id: '1', Name: 'first value', Doubl: '2.2'}");
		}

		[Test]
		public void JsonSerializer_Serialize_String()
		{
			var serializer = new JsonSerializer();
			var value = serializer.Serialize("test");

			value.Should().Be("test");
		}

		[Test]
		public void JsonSerializer_Serialize_Int()
		{
			var serializer = new JsonSerializer();
			var value = serializer.Serialize(12345);

			value.Should().Be("12345");
		}

		[Test]
		public void JsonSerializer_Serialize_Bool()
		{
			var serializer = new JsonSerializer();
			var value = serializer.Serialize(true);

			value.Should().Be("True");
		}

		[Test]
		public void JsonSerializer_Deserialize_Object()
		{
			var serializer = new JsonSerializer();
			var value = serializer.Deserialize<JsonModel>("{Id: '1', Name: 'first value', Doubl: '2.2'}");

			value.Name.Should().NotBeNull();

			value.MatchSnapshot();
		}

		public class JsonModel
		{
			public int Id { get; set; }

			public string Name { get; set; }

			public double Doubl { get; set; }
		}
	}
}
