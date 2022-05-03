using System;
using System.Collections.Generic;
using System.Text;
using Gaucho.Diagnostics;
using Gaucho.Filters;
using Gaucho.Server;

namespace Gaucho.Configuration
{
    /// <summary>
    /// Configuration item representing a <see cref="IInputHandler"/> or <see cref="IOutputHandler"/>
    /// </summary>
    public class HandlerNode
    {
        /// <summary>
        /// 
        /// </summary>
        public HandlerNode() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        public HandlerNode(Type type)
        {
            Type = type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public HandlerNode(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets the name of the InputHandler that is registered in a <see cref="HandlerRegistration"/>
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets tht type of the Handler
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Getst the filters registered for the Handler
        /// </summary>
        public List<string> Filters { get; set; }

        /// <summary>
        /// Gets a set of arguments that are passed to the Handler
        /// </summary>
        public Dictionary<string, string> Arguments { get; set; }
    }
}
