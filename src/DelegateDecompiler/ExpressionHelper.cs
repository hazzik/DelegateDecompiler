using System;
using System.Linq.Expressions;

namespace DelegateDecompiler
{
    internal static class ExpressionHelper
    {
        internal static Expression Default(Type type) =>
            // LINQ to entities and possibly other providers don't support Expression.Default, so
            // this gets the default value and then uses an Expression.Constant instead
            Expression.Constant(type.IsValueType ? Activator.CreateInstance(type) : null, type);

        internal static Expression DiscardConversion(this Expression expr)
        {
            if (!(expr is UnaryExpression)) return expr;
            while (expr is UnaryExpression && (expr.NodeType == ExpressionType.Convert || expr.NodeType == ExpressionType.ConvertChecked))
            {
                expr = (expr as UnaryExpression).Operand;
            }
            return expr;
        }

        internal static Address DiscardConversion(this Address expr)
        {
            return DiscardConversion((Expression)expr);
        }
    }
}