using AwesomeAssertions;
using NUnit.Framework;

namespace Gaucho.Test
{
	public class EventDataTests
	{
		[Test]
		public void EventData_ctor()
		{
			var act = () => new EventData();
			act.Should().NotThrow();
		}

		[Test]
		public void EventData_Contains_true()
		{
			var data = new EventData {{"test", "value"}};
			data.Contains("test").Should().BeTrue();
		}

		[Test]
		public void EventData_Contains_false()
		{
			var data = new EventData {{"test", "value"}};
			data.Contains("test2").Should().BeFalse();
		}

		[Test]
		public void EventData_Contains_Key_CaseSensitive()
		{
			var data = new EventData {{"test", "value"}};
			data.Contains("Test").Should().BeFalse();
		}

		[Test]
		public void EventData_Add_Variable()
		{
			var variable = "value";
			var data = new EventData();
			data.Add(() => variable);
			data.Contains("variable").Should().BeTrue();
		}

		[Test]
		public void EventData_Add_Property()
		{
			var model = new EventDataTestModel { Id = "testmodel" };
			var data = new EventData();
			data.Add(() => model.Id);
			data.Contains("Id").Should().BeTrue();
		}

		public class EventDataTestModel
		{
			public string Id { get; set; }
		}
	}
}
