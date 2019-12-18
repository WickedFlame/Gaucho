using System;
using System.Collections.Generic;
using System.Text;

namespace Gaucho.Server
{
    public static class TypeExtensions
    {
        public static T CreateInstance<T>(this Type type, Dictionary<Type, object> arguments)
        {
            var activator = new HandlerActivator();
            return activator.CreateInstance<T>(type, arguments);
        }

        public static T CreateInstance<T>(this Type type)
        {
            var activator = new HandlerActivator();
            return activator.CreateInstance<T>(type, new Dictionary<Type, object>());
        }
    }
}
