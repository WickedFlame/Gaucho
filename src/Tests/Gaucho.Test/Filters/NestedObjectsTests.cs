using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Filters;
using NUnit.Framework;
using Polaroider;

namespace Gaucho.Test.Filters
{
	public class NestedObjectsTests
	{
		[Test]
		public void Formatter_NestedObject()
		{
			var converter = new EventDataConverter();
			var filters = new List<string>
			{
				"Sub.Value -> sub.value",
				"Message"
			};

			foreach (var filter in filters)
			{
				converter.Add(FilterFactory.BuildFilter(filter));
			}

			var input = new NestedLog
			{
				Sub= new SubClass
				{
					Value = "nested value"
				},
				Message = "The message"
			};
			var factory = new EventDataFactory();
			var data = factory.BuildFrom(input);

			data = converter.Convert(data);

            data.ToString().MatchSnapshot();
		}

        [Test]
        public void Formatter_NestedObject_Complex()
        {
            var input = new
            {
                Sub = new
                {
                    Value = "nested value",
					Last = new
                    {
						Id = Guid.Empty
                    }
                },
                Message = "The message"
            };
            var factory = new EventDataFactory();
            var data = factory.BuildFrom(input);

            data.ToString().MatchSnapshot();
        }

		public class NestedLog
		{
			public SubClass Sub { get; set; }

			public string Message{ get; set; }
		}

		public class SubClass
		{
			public string Value { get; set; }
		}
	}
}
