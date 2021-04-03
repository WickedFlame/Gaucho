using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Storage;
using NUnit.Framework;
using Polaroider;

namespace Gaucho.Test.Storage
{
	public class InmemoryStorageTests
	{
		[Test]
		public void InmemoryStorage_Ctor()
		{
			var storage = new InmemoryStorage();
		}

		[Test]
		public void InmemoryStorage_Set()
		{
			var storage = new InmemoryStorage();
			storage.Set("storage", "key", "value");
		}

		[Test]
		public void InmemoryStorage_AddToList()
		{
			var storage = new InmemoryStorage();
			storage.AddToList("storage", "key", "value");
		}

		[Test]
		public void InmemoryStorage_AddToList_Multiple()
		{
			var storage = new InmemoryStorage();
			storage.AddToList("storage", "key", "one");
			storage.AddToList("storage", "key", "two");
		}

		[Test]
		public void InmemoryStorage_Get()
		{
			var storage = new InmemoryStorage();
			storage.Set("storage", "key", "value");

			Assert.AreEqual("value", storage.Get<string>("storage", "key"));
		}

		[Test]
		public void InmemoryStorage_Update()
		{
			var storage = new InmemoryStorage();
			storage.Set("storage", "key", "value");
			storage.Set("storage", "key", "reset");

			Assert.AreEqual("reset", storage.Get<string>("storage", "key"));
		}

		[Test]
		public void InmemoryStorage_List_Objects()
		{
			var storage = new InmemoryStorage();
			storage.AddToList("storage", "key", new StorageModel {Id = 1, Value = "one"});
			storage.AddToList("storage", "key", new StorageModel { Id = 2, Value = "two" });

			var items = storage.GetList<StorageModel>("storage", "key");
			items.MatchSnapshot();
		}

		[Test]
		public void InmemoryStorage_Item_Object()
		{
			var storage = new InmemoryStorage();
			storage.Set("storage", "key", new StorageModel { Id = 1, Value = "one" });

			var item = storage.Get<StorageModel>("storage", "key");
			item.MatchSnapshot();
		}

		public class StorageModel
		{
			public int Id { get; set; }

			public string Value { get; set; }
		}
	}
}
