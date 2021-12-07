using System;
using System.Collections.Generic;

namespace DelegateDecompiler
{
    static class TypeExtensions
    {
        public static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static IEnumerable<Type> SelfAndBaseTypes(this Type type)
        {
            if (type == typeof(object) || type == null)
            {
                yield break;
            }

            for (var t = type; t != typeof(object) && t != null; t = t.BaseType)
            {
                yield return t;
            }
        }

        public static IEnumerable<Type> BaseTypes(this Type type) =>
            type.BaseType.SelfAndBaseTypes();
    }
}
