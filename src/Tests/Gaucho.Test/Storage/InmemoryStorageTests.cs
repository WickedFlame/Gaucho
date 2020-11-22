using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Storage;
using NUnit.Framework;

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
		public void InmemoryStorage_Add()
		{
			var storage = new InmemoryStorage();
			storage.Add("storage", "key", "value");
		}

		[Test]
		public void InmemoryStorage_Add_Multiple()
		{
			var storage = new InmemoryStorage();
			storage.Add("storage", "key", "one");
			storage.Add("storage", "key", "two");
		}

		[Test]
		public void InmemoryStorage_ReSet()
		{
			var storage = new InmemoryStorage();
			storage.Set("storage", "key", "value");
			storage.Set("storage", "key", "reset");

			Assert.AreEqual("reset", storage.Get<string>("storage", "key"));
		}
	}
}
