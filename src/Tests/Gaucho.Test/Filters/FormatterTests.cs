using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Filters;
using NUnit.Framework;
using AwesomeAssertions; // added

namespace Gaucho.Test.Filters
{
	public class FormatterTests
	{
		[Test]
		public void Gaucho_Filter_PropertyFormatter()
		{
			var converter = new EventDataConverter
			{
				new PropertyFilter("Level", "lvl"),
				new PropertyFilter("Message"),
				new FormatFilter("prop", "${lvl}_error_${Message}")
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
			var formatted = converter.Format("prop", data);

			formatted.Should().Be("Info_error_The message");
		}

		[Test]
		public void Gaucho_Filter_PropertyFormatter_FromFilterFactory()
		{
			var converter = new EventDataConverter();
			var filters = new List<string>
			{
				"Level -> lvl",
				"Message",
				"prop <- ${lvl}_error_${Message}"
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
			var formatted = converter.Format("prop", data);

			formatted.Should().Be("Info_error_The message");
		}

		[Test]
		public void Gaucho_Filter_PropertyFormatter_UnknownFilter()
		{
			var converter = new EventDataConverter();
			var filters = new List<string>
			{
				"Level -> lvl",
				"Message",
				"prop <- ${lvl}_error_${Message}"
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
			var formatted = converter.Format("PROP", data);

			formatted.Contains("lvl -> Info").Should().BeTrue();
			formatted.Contains("Message -> The message").Should().BeTrue();
		}
	}
}
