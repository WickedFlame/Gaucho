using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;

namespace WickedFlame.Yaml
{
    public delegate object EmptyConstructor();

    /// <summary>
    /// Factory Class that generates instances of a type
    /// </summary>
    internal static class InstanceFactory
    {
        static Dictionary<Type, EmptyConstructor> _constructorMethods = new Dictionary<Type, EmptyConstructor>();

        /// <summary>
        /// Factory Method that creates an instance of type T
        /// </summary>
        /// <typeparam name="T">The type to create an instance of</typeparam>
        /// <returns>An instance of type T</returns>
        public static T CreateInstance<T>()
        {
            return (T)GetConstructorMethod(typeof(T)).Invoke();
        }

        public static object CreateInstance(this Type type)
        {
            if (type == null)
            {
                return null;
            }

            return GetConstructorMethod(type).Invoke();
        }

        private static EmptyConstructor GetConstructorMethodToCache(Type type)
        {
            if (type.IsInterface)
            {
                if (type.HasGenericType())
                {
                    var genericType = type.GetTypeWithGenericTypeDefinitionOfAny(typeof(IDictionary<,>));

                    if (genericType != null)
                    {
                        var keyType = genericType.GetGenericArguments()[0];
                        var valueType = genericType.GetGenericArguments()[1];
                        return GetConstructorMethodToCache(typeof(Dictionary<,>).MakeGenericType(keyType, valueType));
                    }

                    genericType = type.GetTypeWithGenericTypeDefinitionOfAny(typeof(IEnumerable<>), typeof(ICollection<>), typeof(IList<>));

                    if (genericType != null)
                    {
                        var elementType = genericType.GetGenericArguments()[0];
                        return GetConstructorMethodToCache(typeof(List<>).MakeGenericType(elementType));
                    }
                }
            }
            else if (type.IsArray)
            {
                return () => Array.CreateInstance(type.GetElementType(), 0);
            }
            else if (type.IsGenericTypeDefinition)
            {
                var genericArgs = type.GetGenericArguments();
                var typeArgs = new Type[genericArgs.Length];
                for (var i = 0; i < genericArgs.Length; i++)
                {
                    typeArgs[i] = typeof(object);
                }

                var realizedType = type.MakeGenericType(typeArgs);
                return realizedType.CreateInstance;
            }

            //var emptyCtor = type.GetEmptyConstructor();
            //if (emptyCtor != null)
            //{
            //    var dynamicMethod = new DynamicMethod("MyCtor", type, Type.EmptyTypes, type.Module, true);

            //    var ilGenerator = dynamicMethod.GetILGenerator();
            //    ilGenerator.Emit(System.Reflection.Emit.OpCodes.Nop);
            //    ilGenerator.Emit(System.Reflection.Emit.OpCodes.Newobj, emptyCtor);
            //    ilGenerator.Emit(System.Reflection.Emit.OpCodes.Ret);

            //    return (EmptyConstructorDelegate)dynamicMethod.CreateDelegate(typeof(EmptyConstructorDelegate));
            //}

            if (type == typeof(string))
            {
                return () => string.Empty;
            }

            // Anonymous types don't have empty constructors
            //return () => FormatterServices.GetUninitializedObject(type);
            return () => Activator.CreateInstance(type);
        }

        private static EmptyConstructor GetConstructorMethod(Type type)
        {
            if (_constructorMethods.TryGetValue(type, out var emptyConstructorFunction))
            {
                return emptyConstructorFunction;
            }

            emptyConstructorFunction = GetConstructorMethodToCache(type);

            Dictionary<Type, EmptyConstructor> snapshot;
            Dictionary<Type, EmptyConstructor> newCache;

            do
            {
                snapshot = _constructorMethods;
                newCache = new Dictionary<Type, EmptyConstructor>(_constructorMethods);
                newCache[type] = emptyConstructorFunction;
            }
            while (!ReferenceEquals(Interlocked.CompareExchange(ref _constructorMethods, newCache, snapshot), snapshot));

            return emptyConstructorFunction;
        }
    }
}
