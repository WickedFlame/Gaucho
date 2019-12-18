using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gaucho.Server
{
    public class HandlerActivator
    {
        public T CreateInstance<T>(Type type, Dictionary<Type, object> arguments)
        {
            var parameters = SubstituteArguments(type, arguments);
            if(parameters.Any())
            {
                return (T) Activator.CreateInstance(type, parameters);
            }

            return (T)Activator.CreateInstance(type);
        }

        private static object[] SubstituteArguments(Type type, Dictionary<Type, object> arguments)
        {
            var ctor = type.GetConstructors().FirstOrDefault(c => c.GetParameters().Count() == arguments.Count && c.GetParameters().All(p => arguments.ContainsKey(p.ParameterType)));
            if (ctor == null)
            {
                ctor = type.GetConstructors().FirstOrDefault(c => !c.GetParameters().Any());
            }

            var parameters = ctor.GetParameters(); 
            var result = new List<object>(parameters.Count());

            for (var i = 0; i < parameters.Count(); i++)
            {
                var parameter = parameters[i];

                var value = arguments.ContainsKey(parameter.ParameterType)
                    ? arguments[parameter.ParameterType]
                    : null;

                result.Add(value);
            }

            return result.ToArray();
        }
    }
}
