using System;
using System.Linq.Expressions;

namespace DelegateDecompiler
{
    public static class ExpressionExtensions
    {
        public static Expression Decompile(this Expression expression)
        {
            return DecompileExpressionVisitor.Decompile(expression);
        }

        public static Expression Dereference(this Expression expression)
        {
            return ReferencedQueryableExpressionVisitor.Decompile(expression);
        }

        public static Expression Optimize(this Expression expression)
        {
            return OptimizeExpressionVisitor.Optimize(expression);
        }

        public static T Evaluate<T>(this Expression expression)
        {
            var func = Expression.Lambda<Func<T>>(expression).Compile();
            return func.Invoke();
        }
    }
}
