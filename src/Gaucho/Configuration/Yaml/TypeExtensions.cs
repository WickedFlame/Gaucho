using System;

namespace WickedFlame.Yaml
{
    internal static class TypeExtensions
    {
        public static bool HasGenericType(this Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType)
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }

        public static Type GetTypeWithGenericTypeDefinitionOfAny(this Type type, params Type[] genericTypeDefinitions)
        {
            foreach (var genericTypeDefinition in genericTypeDefinitions)
            {
                var genericType = type.GetTypeWithGenericTypeDefinitionOf(genericTypeDefinition);
                if (genericType == null && type == genericTypeDefinition)
                {
                    genericType = type;
                }

                if (genericType != null)
                {
                    return genericType;
                }
            }

            return null;
        }

        public static Type GetTypeWithGenericTypeDefinitionOf(this Type type, Type genericTypeDefinition)
        {
            foreach (var t in type.GetInterfaces())
            {
                if (t.IsGenericType && t.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    return t;
                }
            }

            var genericType = type.GetGenericType();
            if (genericType != null && genericType.GetGenericTypeDefinition() == genericTypeDefinition)
            {
                return genericType;
            }

            return null;
        }

        public static Type GetGenericType(this Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType)
                {
                    return type;
                }

                type = type.BaseType;
            }

            return null;
        }
    }
}
