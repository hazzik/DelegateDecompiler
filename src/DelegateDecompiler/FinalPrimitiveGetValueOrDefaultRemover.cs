using System;
using System.Linq.Expressions;

namespace DelegateDecompiler
{
    public class FinalPrimitiveGetValueOrDefaultRemover : ExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (IsGetValueOrDefault(node) && node.Arguments.Count == 0 && node.Type.IsPrimitive)
                return Expression.Coalesce(node.Object, Expression.Constant(Activator.CreateInstance(node.Type)));

            return base.VisitMethodCall(node);
        }

        bool IsGetValueOrDefault(MethodCallExpression method)
        {
            return method.Method.Name == "GetValueOrDefault" && method.Object != null && IsNullable(method.Object.Type);
        }

        static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}