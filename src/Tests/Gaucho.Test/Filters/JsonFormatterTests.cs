using Gaucho.Filters;
using System.Collections.Generic;

namespace Gaucho.Test.Filters
{
	public class JsonFormatterTests
	{
		[Test]
		public void Gaucho_Filter_JsonFormatter()
		{
			var converter = new EventDataConverter
			{
				new PropertyFilter("Level", "dst_lvl"),
				new PropertyFilter("Message"),
				new JsonFilter(new List<PropertyFilter>
				{
					new PropertyFilter("dst_lvl"),
					new PropertyFilter("Message", "msg")
				})
			};

			var input = new LogMessage
			{
				Level = "Info",
				Message = "The message",
				Title = "title"
			};
			var factory = new EventDataFactory();
			var data = factory.BuildFrom(input);

			data = converter.Convert(data);
			var formatted = converter.Format("json", data);

			formatted.Should().Be("{\"dst_lvl\":\"Info\",\"msg\":\"The message\"}");
		}

		[Test]
		public void Gaucho_Filter_JsonFormatter_FromFilterFactory()
		{
			var converter = new EventDataConverter();
			var filters = new List<string>
			{
				"Level -> dst_lvl",
				"Message",
				"json <- [dst_lvl,Message -> msg]"
			};

			foreach (var filter in filters)
			{
				converter.Add(FilterFactory.BuildFilter(filter));
			}

			var input = new LogMessage
			{
				Level = "Info",
				Message = "The message",
				Title = "title"
			};
			var factory = new EventDataFactory();
			var data = factory.BuildFrom(input);

			data = converter.Convert(data);
			var formatted = converter.Format("json", data);

			formatted.Should().Be("{\"dst_lvl\":\"Info\",\"msg\":\"The message\"}");
		}
	}
}
