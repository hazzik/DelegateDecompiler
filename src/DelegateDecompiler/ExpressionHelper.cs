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

        public static bool IsDefault(Expression expression, Type type)
        {
            //TODO maybe handle the possible convertions
            return expression.Type == type && (expression as ConstantExpression)?.Value == ((ConstantExpression)Default(type)).Value;
        }

        public static object GetDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }
    }
}
