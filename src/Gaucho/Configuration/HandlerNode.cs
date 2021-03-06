﻿using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Diagnostics;
using Gaucho.Filters;

namespace Gaucho.Configuration
{
    public class HandlerNode
    {
        public HandlerNode() { }

        public HandlerNode(Type type)
        {
            Type = type;
        }

        public HandlerNode(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public Type Type { get; set; }

        public List<string> Filters { get; set; }

        public Dictionary<string, string> Arguments { get; set; }
    }
}
