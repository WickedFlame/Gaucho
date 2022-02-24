using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Storage;
using Moq;
using NUnit.Framework;
using Polaroider;
using StackExchange.Redis;

namespace Gaucho.Redis.Test
{
	public class RedisStorageTests
	{
		private Mock<IConnectionMultiplexer> _multiplexer;
		private Mock<IDatabase> _db;

		[SetUp]
		public void Setup()
		{
			_multiplexer = new Mock<IConnectionMultiplexer>();
			_db = new Mock<IDatabase>();
			var server = new Mock<IServer>();
			_multiplexer.Setup(exp => exp.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_db.Object);
			_multiplexer.Setup(exp => exp.GetServer(It.IsAny<System.Net.EndPoint>(), null)).Returns(server.Object);
		}

		[Test]
		public void RedisStorage_Ctor()
		{
			var multiplexer = new Mock<IConnectionMultiplexer>();
			var storage = new RedisStorage(multiplexer.Object);
		}

		[Test]
		public void RedisStorage_Set()
		{
			var storage = new RedisStorage(_multiplexer.Object);
			storage.Set(new StorageKey("storage", "key"), "value");

			_db.Verify(exp => exp.HashSetAsync(It.IsAny<RedisKey>(), new[] { new HashEntry("value".GetType().Name, "value") }, CommandFlags.None), Times.Once);
		}

		[Test]
		public void RedisStorage_AddToList()
		{
			var storage = new RedisStorage(_multiplexer.Object);
			storage.AddToList(new StorageKey("storage", "key"), new { Value = "value" });

			_db.Verify(exp => exp.ListRightPushAsync(It.IsAny<RedisKey>(), new RedisValue("{Value: 'value'}"), When.Always, CommandFlags.None), Times.Once);
		}

		[Test]
		public void RedisStorage_AddToList_Multiple()
		{
			var storage = new RedisStorage(_multiplexer.Object);
			storage.AddToList(new StorageKey("storage", "key"), new { Value = "one" });
			storage.AddToList(new StorageKey("storage", "key"), new { Value = "two" });

			_db.Verify(exp => exp.ListRightPushAsync(It.IsAny<RedisKey>(), new RedisValue("{Value: 'one'}"), When.Always, CommandFlags.None), Times.Once);
			_db.Verify(exp => exp.ListRightPushAsync(It.IsAny<RedisKey>(), new RedisValue("{Value: 'two'}"), When.Always, CommandFlags.None), Times.Once);
		}

		[Test]
		public void RedisStorage_Get()
		{
			_db.Setup(exp => exp.HashGetAll(It.IsAny<RedisKey>(), CommandFlags.None)).Returns(() => new[]
			{
				new HashEntry("Value", "value"),
				new HashEntry("Id", "1")
			});
			var storage = new RedisStorage(_multiplexer.Object);

			Assert.AreEqual("value", storage.Get<StorageModel>(new StorageKey("storage", "key")).Value);
			Assert.AreEqual(1, storage.Get<StorageModel>(new StorageKey("storage", "key")).Id);
		}

		[Test]
		public void RedisStorage_GetList_Objects()
		{
			_db.Setup(exp => exp.ListRange(It.IsAny<RedisKey>(), It.IsAny<long>(), It.IsAny<long>(), CommandFlags.None)).Returns(() => new[]
			{
				new RedisValue("{Id: '1', Value: 'one'}"),
				new RedisValue("{Id: '2', Value: 'two'}")
			});
			var storage = new RedisStorage(_multiplexer.Object);

			var items = storage.GetList<StorageModel>(new StorageKey("storage", "key"));
			items.MatchSnapshot();
		}

		[Test]
		public void RedisStorage_Get_Object()
		{
			_db.Setup(exp => exp.HashGetAll(It.IsAny<RedisKey>(), CommandFlags.None)).Returns(() => new[]
			{
				new HashEntry("Value", "one"),
				new HashEntry("Id", "1")
			});

			var storage = new RedisStorage(_multiplexer.Object);
			//storage.Set(new StorageKey("storage", "key"), new StorageModel { Id = 1, Value = "one" });

			var item = storage.Get<StorageModel>(new StorageKey("storage", "key"));
			item.MatchSnapshot();
		}

		[Test]
		public void RedisStorage_Get_ServerInKey_Object()
		{
			_db.Setup(exp => exp.HashGetAll(It.Is<RedisKey>(k => k.ToString() == $"gaucho:server:storage:key"), CommandFlags.None)).Returns(() => new[]
			{
				new HashEntry("Value", "one"),
				new HashEntry("Id", "1")
			});

			var storage = new RedisStorage(_multiplexer.Object);

			storage.Get<StorageModel>(new StorageKey("storage", "key", "server"));

			_db.Verify();
		}

        [Test]
        public void RedisStorage_Delete()
        {
            var storage = new RedisStorage(_multiplexer.Object);

            storage.Delete(new StorageKey("storage", "key", "server"));

            _db.Verify(x => x.KeyDelete(It.Is<RedisKey>(k => k == "{gaucho}:server:storage:key"), It.IsAny<CommandFlags>()));
        }

		public class StorageModel
		{
			public int Id { get; set; }

			public string Value { get; set; }
		}
	}
}
