using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DelegateDecompiler
{
    public static class ExpressionExtensions
    {
        public static Expression Decompile(this Expression expression)
        {
            return new DecompileExpressionVisitor().Visit(expression);
        }

        public static Expression Optimize(this Expression expression)
        {
            return OptimizeExpressionVisitor.Optimize(expression);
        }

        public static object Evaluate(this Expression expression)
        {
            var func = Expression.Lambda<Func<object>>(expression).Compile();
            return func.Invoke();
        }
    }
}
