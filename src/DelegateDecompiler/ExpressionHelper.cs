using System;
using System.Linq.Expressions;

namespace DelegateDecompiler
{
    internal static class ExpressionHelper
    {
        internal static Expression Default(Type type) =>
            // LINQ to entities and possibly other providers don't support Expression.Default, so this gets the default
            // value and then uses an Expression.Constant instead
            Expression.Constant(type.IsValueType ? Activator.CreateInstance(type) : null, type);
    }
}
