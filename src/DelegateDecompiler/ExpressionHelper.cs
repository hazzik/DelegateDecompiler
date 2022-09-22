using System;
using System.Linq.Expressions;

namespace DelegateDecompiler
{
    internal static class ExpressionHelper
    {
        internal static Expression Default(Type type, object defaultValue = null)
        {
            // LINQ to entities and possibly other providers don't support Expression.Default, so this gets the default
            // value and then uses an Expression.Constant instead
            return Expression.Constant(defaultValue ?? GetDefaultValue(type), type);
        }

        internal static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
