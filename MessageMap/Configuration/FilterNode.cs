using System;
using System.Collections.Generic;
using System.Text;

namespace MessageMap.Configuration
{
    public class FilterNode
    {
        public FilterNode(string property)
        {
            Source = property;
            Destination = property;
        }

        public FilterNode(string source, string destination)
        {
            Source = source;
            Destination = destination;
        }

        public string Destination { get; set; }

        public string Source { get; set; }
    }
}
