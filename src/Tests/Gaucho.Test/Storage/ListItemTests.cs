using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gaucho.Storage;
using Gaucho.Storage.Inmemory;
using NUnit.Framework;
using AwesomeAssertions;

namespace Gaucho.Test.Storage
{
	public class ListItemTests
	{
		[Test]
		public void ListItem_Ctor()
		{
			new ListItem().Should().NotBeNull();
		}

		[Test]
		public void ListItem_SetValue()
		{
			var item = new ListItem();
			item.SetValue("value");
		}

		[Test]
		public void ListItem_Multiple_Values()
		{
			var item = new ListItem();
			item.SetValue("one");
			item.SetValue("two");

			((List<object>)item.GetValue()).Count().Should().Be(2);
		}

		[Test]
		public void ListItem_GetValue()
		{
			var item = new ListItem();
			item.SetValue("value");
		}

		[Test]
		public void ListItem_GetValue_IEnumerable()
		{
			var item = new ListItem();
			item.SetValue("value");

			item.GetValue().Should().BeOfType<List<object>>();

		}

		[Test]
		public void ListItem_GetValue_EnsureSame()
		{
			var items = new[]
			{
				"one",
				"two"
			};
			var item = new ListItem();
			item.SetValue(items[0]);
			item.SetValue(items[1]);

			var resolved = item.GetValue() as List<object>;
			for (var i =0; i < resolved.Count; i++)
			{
				resolved[i].Should().BeSameAs(items[i]);
			}
		}
	}
}
