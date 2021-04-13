using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gaucho.Server
{
	/// <summary>
	/// The ActivationContext
	/// </summary>
    public class ActivationContext : IActivationContext
    {
        private readonly Dictionary<Type, Func<object>> _registrations = new Dictionary<Type, Func<object>>();

		/// <summary>
		/// Gets a readonly dictionary for all registrations
		/// </summary>
        public IReadOnlyDictionary<Type, Func<object>> Registrations => _registrations;

		/// <inheritdoc/>
        public void Register<TService, TImpl>() where TImpl : TService
        {
            _registrations.Add(typeof(TService), () => Resolve(typeof(TImpl)));
        }

		/// <inheritdoc/>
		public void Register<TService>(Func<TService> instanceCreator)
        {
            _registrations.Add(typeof(TService), () => instanceCreator());
        }

		/// <inheritdoc/>
		public T Resolve<T>()
{
            return (T)Resolve(typeof(T));
        }

		/// <inheritdoc/>
		public T Resolve<T>(Type serviceType)
        {
            return (T) Resolve(serviceType);
        }

		/// <inheritdoc/>
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

		/// <inheritdoc/>
		public IActivationContext ChildContext()
        {
            var ctx = new ActivationContext();
            foreach (var registration in _registrations)
            {
                ctx._registrations.Add(registration.Key, registration.Value);
            }

            return ctx;
        }

        private object CreateInstance(Type implementationType)
        {
            var ctor = implementationType.GetConstructors().OrderByDescending(c => c.GetParameters().Length).First();
            var parameterTypes = ctor.GetParameters().Select(p => p.ParameterType);
            var dependencies = parameterTypes.Select(t => Resolve(t)).ToArray();

            return Activator.CreateInstance(implementationType, dependencies);
        }
    }
}
