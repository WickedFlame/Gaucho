using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Redis.Serializer;
using NUnit.Framework;
using Polaroider;

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
				Id = 1,
				Name = "first value",
				Doubl = 2.2
			});

			Assert.AreEqual("{Id: '1', Name: 'first value', Doubl: '2.2'}", value);
		}

		[Test]
		public void JsonSerializer_Serialize_String()
		{
			var serializer = new JsonSerializer();
			var value = serializer.Serialize("test");

			Assert.AreEqual("test", value);
		}

		[Test]
		public void JsonSerializer_Serialize_Int()
		{
			var serializer = new JsonSerializer();
			var value = serializer.Serialize(12345);

			Assert.AreEqual("12345", value);
		}

		[Test]
		public void JsonSerializer_Serialize_Bool()
		{
			var serializer = new JsonSerializer();
			var value = serializer.Serialize(true);

			Assert.AreEqual("True", value);
		}





		[Test]
		public void JsonSerializer_Deserialize_Object()
		{
			var serializer = new JsonSerializer();
			var value = serializer.Deserialize<JsonModel>("{Id: '1', Name: 'first value', Doubl: '2.2'}");

			Assert.IsNotNull(value.Name);

			value.MatchSnapshot();
		}

		//[Test]
		//public void JsonSerializer_Deserialize_String()
		//{
		//	var serializer = new JsonSerializer();
		//	var value = serializer.Deserialize<string>("test");

		//	Assert.IsNotNull(value.Name);

		//	value.MatchSnapshot();
		//}

		public class JsonModel
		{
			public int Id { get; set; }

			public string Name { get; set; }

			public double Doubl { get; set; }
		}
	}
}
