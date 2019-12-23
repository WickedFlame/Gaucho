using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gaucho.Server
{
    public interface IActivationContext
    {
        void Register<TService, TImpl>() where TImpl : TService;

        void Register<TService>(Func<TService> instanceCreator);
        
        T Resolve<T>();
        
        object Resolve(Type serviceType);
    }

    public class ActivationContext : IActivationContext
    {
        private readonly Dictionary<Type, Func<object>> _registrations = new Dictionary<Type, Func<object>>();

        public IReadOnlyDictionary<Type, Func<object>> Registrations => _registrations;

        public void Register<TService, TImpl>() where TImpl : TService
        {
            _registrations.Add(typeof(TService), () => Resolve(typeof(TImpl)));
        }

        public void Register<TService>(Func<TService> instanceCreator)
        {
            _registrations.Add(typeof(TService), () => instanceCreator());
        }

        public void RegisterSingleton<TService>(TService instance)
        {
            _registrations.Add(typeof(TService), () => instance);
        }

        public void RegisterSingleton<TService>(Func<TService> instanceCreator)
        {
            var lazy = new Lazy<TService>(instanceCreator);
            Register<TService>(() => lazy.Value);
        }

        public T Resolve<T>()
        {
            return (T)Resolve(typeof(T));
        }

        public object Resolve(Type serviceType)
        {
            if (_registrations.TryGetValue(serviceType, out var creator))
            {
                return creator();
            }

            if (!serviceType.IsAbstract)
            {
                return CreateInstance(serviceType);
            }

            throw new InvalidOperationException("No registration for " + serviceType);
        }

        private object CreateInstance(Type implementationType)
        {
            var ctor = implementationType.GetConstructors().Single();
            var parameterTypes = ctor.GetParameters().Select(p => p.ParameterType);
            var dependencies = parameterTypes.Select(t => Resolve(t)).ToArray();

            return Activator.CreateInstance(implementationType, dependencies);
        }
    }
}
