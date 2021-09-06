using NUnit.Framework;

namespace Gaucho.Test
{
	public class EventDataTests
	{
		[Test]
		public void EventData_ctor()
		{
			Assert.DoesNotThrow(() => new EventData());
		}

		[Test]
		public void EventData_Contains_true()
		{
			var data = new EventData {{"test", "value"}};

			Assert.IsTrue(data.Contains("test"));
		}

		[Test]
		public void EventData_Contains_false()
		{
			var data = new EventData {{"test", "value"}};

			Assert.IsFalse(data.Contains("test2"));
		}

		[Test]
		public void EventData_Contains_Key_CaseSensitive()
		{
			var data = new EventData {{"test", "value"}};

			Assert.IsFalse(data.Contains("Test"));
		}

		[Test]
		public void EventData_Add_Variable()
		{
			var variable = "value";
			var data = new EventData();
			data.Add(() => variable);

			Assert.IsTrue(data.Contains("variable"));
		}

		[Test]
		public void EventData_Add_Property()
		{
			var model = new EventDataTestModel
			{
				Id = "testmodel"
			};
			var data = new EventData();
			data.Add(() => model.Id);

			Assert.IsTrue(data.Contains("Id"));
		}

		public class EventDataTestModel
		{
			public string Id { get; set; }
		}
	}
}
