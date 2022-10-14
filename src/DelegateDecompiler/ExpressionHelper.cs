using System;
using System.Linq.Expressions;

namespace DelegateDecompiler
{
    public static class ExpressionHelper
    {
        public static Expression Default(Type type) =>
            // LINQ to entities and possibly other providers don't support Expression.Default, so this gets the default
            // value and then uses an Expression.Constant instead
            Expression.Constant(GetDefaultValue(type), type);

        public static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

    }
}
