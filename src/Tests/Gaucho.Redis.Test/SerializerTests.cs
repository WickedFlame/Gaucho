using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Redis.Serializer;
using Gaucho.Storage;
using Moq;
using NUnit.Framework;
using StackExchange.Redis;

namespace Gaucho.Redis.Test
{
	public class SerializerTests
	{
		private Mock<IConnectionMultiplexer> _multiplexer;
		private Mock<IDatabase> _db;

		[SetUp]
		public void Setup()
		{
			_multiplexer = new Mock<IConnectionMultiplexer>();
			_db = new Mock<IDatabase>();
			_multiplexer.Setup(exp => exp.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_db.Object);
			var server = new Mock<IServer>();
			_multiplexer.Setup(exp => exp.GetServer(It.IsAny<System.Net.EndPoint>(), null)).Returns(server.Object);
		}

		[Test]
		public void Options_Serializer_Override_Serialize()
		{
			var serializer = new Mock<ISerializer>();
			serializer.Setup(exp => exp.Serialize(It.IsAny<object>())).Returns(() => "Testvalue");

			var options = new RedisStorageOptions
			{
				Serializer = serializer.Object
			};

			var storage = new RedisStorage(_multiplexer.Object, options);
			storage.AddToList(new StorageKey("storage", "key"), new StorageModel { Value = "value" });

			_db.Verify(exp => exp.ListRightPushAsync(It.IsAny<RedisKey>(), new RedisValue("Testvalue"), When.Always, CommandFlags.None), Times.Once);
		}

		[Test]
		public void Options_Serializer_Override_Deserialize()
		{
			var serializer = new Mock<ISerializer>();
			serializer.Setup(exp => exp.Deserialize<StorageModel>(It.IsAny<string>())).Returns(() => new StorageModel {Value = "testmodel"});

			_db.Setup(exp => exp.ListRange(It.IsAny<RedisKey>(), It.IsAny<long>(), It.IsAny<long>(), CommandFlags.None)).Returns(() => new[]
			{
				new RedisValue("test")
			});

			var options = new RedisStorageOptions
			{
				Serializer = serializer.Object
			};

			var storage = new RedisStorage(_multiplexer.Object, options);
			var items = storage.GetList<StorageModel>(new StorageKey("storage", "key"));

			Assert.AreEqual("testmodel", items.First().Value);
		}

		public class StorageModel
		{
			public string Value { get; set; }
		}

		public class TestSerializer : ISerializer
		{
			public string Serialize(object item)
			{
				var dict = item.GetType().GetProperties()
					.Where(x => x.GetValue(item) != null)
					.ToDictionary(p => p.Name, p=>  p.GetValue(item)?.ToString());

				return string.Join(";", dict.Select(d => $"{d.Key}:{d.Value}"));
			}

			public T Deserialize<T>(string json) where T : class, new()
			{
				var dict = json.Split(';').Select(s => s.Split(':')).ToDictionary(a => a[0], a => a[1]);
				var item = new T();
				foreach (var property in item.GetType().GetProperties())
				{
					if (dict.ContainsKey(property.Name))
					{
						continue;
					}

					property.SetValue(item, TypeConverter.Convert(property.PropertyType, dict[property.Name]));
				}

				return item;
			}
		}
	}
}
