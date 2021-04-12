using System;

namespace Gaucho.Server
{
	/// <summary>
	/// Interface for the ActivationContext
	/// </summary>
	public interface IActivationContext
	{
		/// <summary>
		/// Register a service that can be resolved
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <typeparam name="TImpl"></typeparam>
		void Register<TService, TImpl>() where TImpl : TService;

		/// <summary>
		/// Register a service with a factory
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <param name="instanceCreator"></param>
		void Register<TService>(Func<TService> instanceCreator);

		/// <summary>
		/// Resolve a service
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		T Resolve<T>();

		/// <summary>
		/// Resolve a service
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		T Resolve<T>(Type serviceType);

		/// <summary>
		/// Resolve a service
		/// </summary>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		object Resolve(Type serviceType);

		/// <summary>
		/// Create a childcontext
		/// </summary>
		/// <returns></returns>
		IActivationContext ChildContext();
	}
}
