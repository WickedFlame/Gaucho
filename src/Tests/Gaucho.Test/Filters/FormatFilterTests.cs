using System;
using System.Collections.Generic;
using System.Text;
using FluentAssertions;
using Gaucho.Filters;
using NUnit.Framework;

namespace Gaucho.Test.Filters
{
	public class FormatFilterTests
	{
		[Test]
		public void FormatFilter()
		{
			var filter = new FormatFilter("tmp", "${lvl}_error_${Message}");

			var data = new EventData()
				.Add("lvl", "Debug")
				.Add("Message", "theMessage");

			var result = filter.Filter(data);

			result.Value.Should().Be("Debug_error_theMessage");
			result.Key.Should().Be("tmp");
		}

        [Test]
        public void FormatFilter_Simple()
        {
            var filter = new FormatFilter("tmp", "${lvl}");

            var data = new EventData()
                .Add("lvl", "Debug")
                .Add("Message", "theMessage");

            var result = filter.Filter(data);

            result.Value.Should().Be("Debug");
            result.Key.Should().Be("tmp");
        }

		[Test]
		public void FormatFilter_InvalidProperty()
		{
			var filter = new FormatFilter("tmp", "${lvl}_error_${message}");

			var data = new EventData()
				.Add("lvl", "Debug")
				.Add("Message", "theMessage");

			var result = filter.Filter(data);

			result.Value.Should().Be("Debug_error_${message}");
			result.Key.Should().Be("tmp");
		}

        [Test]
        public void FormatFilter_Format_DateTime()
        {
            var filter = new FormatFilter("tmp", "${date:yyyy-MM-ddTHH:mm:ss.sssK}");

            var data = new EventData()
                .Add("lvl", "Debug")
                .Add("date", DateTime.Parse("2012-12-21T11:11:11.0000000+02:00"));

            var result = filter.Filter(data);

            result.Value.Should().Be("2012-12-21T10:11:11.11+01:00");
        }

        [Test]
        public void FormatFilter_Format_DateTime_ISO()
        {
            var filter = new FormatFilter("tmp", "${date:o}");

            var date = DateTime.Parse("2012-12-21T11:11:11.0000000+02:00");

            var data = new EventData()
                .Add("lvl", "Debug")
                .Add("date", date);

            var result = filter.Filter(data);

            DateTime.Parse(result.Value as string).Should().Be(date);
        }
	}
}
