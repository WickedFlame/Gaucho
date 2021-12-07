using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Polaroider;

namespace Gaucho.Test.Filters
{
    public class DictionaryTests
    {
        [Test]
        public void EventDataConverter_Dictionaries_Simple()
        {
            var input = new Dictionary<string, string>
            {
                { "first", "one" },
                { "second", "two" }
            };
            var factory = new EventDataFactory();
            var data = factory.BuildFrom(input);

            data.ToString().MatchSnapshot();
        }

        [Test]
        public void EventDataConverter_Dictionaries_Complex()
        {
            var input = new Dictionary<string, object>
            {
                {
                    "first", new Dictionary<string, object>
                    {
                        {
                            "second", new Dictionary<string, object>
                            {
                                { "third", "value1" }
                            }
                        },
                        {
                            "fourth", "test fourth"
                        }
                    }
                },
                {
                    "fifth", 5
                }
            };
            var factory = new EventDataFactory();
            var data = factory.BuildFrom(input);

            data.ToString().MatchSnapshot();
        }
    }
}
