using System;
using System.Collections.Generic;
using System.Linq;

namespace Gaucho.Server
{
    public class HandlerActivator
    {
        public T CreateInstance<T>(Type type, Dictionary<Type, object> dependencies)
        {
            var parameters = SubstituteArguments(type, dependencies);
            if(parameters.Any())
            {
                return (T) Activator.CreateInstance(type, parameters);
            }

            return (T)Activator.CreateInstance(type);
        }

        private static object[] SubstituteArguments(Type type, Dictionary<Type, object> dependencies)
        {
            var ctor = type.GetConstructors().OrderByDescending(c => c.GetParameters().Count()).FirstOrDefault(c => c.GetParameters().All(p => dependencies.ContainsKey(p.ParameterType)));
            if (ctor == null)
            {
                ctor = type.GetConstructors().FirstOrDefault(c => !c.GetParameters().Any());
            }

            var parameters = ctor.GetParameters(); 
            var result = new List<object>(parameters.Count());

            for (var i = 0; i < parameters.Count(); i++)
            {
                var parameter = parameters[i];

                var value = dependencies.ContainsKey(parameter.ParameterType)
                    ? dependencies[parameter.ParameterType]
                    : null;

                result.Add(value);
            }

            return result.ToArray();
        }
    }
}
