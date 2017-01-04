using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiler
{
    public static class DecompileExtensions
    {
        private static readonly Cache<MethodInfo, LambdaExpression> cache = new Cache<MethodInfo, LambdaExpression>();

        public static LambdaExpression Decompile(this Delegate @delegate)
        {
            var expression = Decompile(@delegate.Method);
            if (@delegate.Method.IsStatic) return expression;

            var visitor = new ReplaceExpressionVisitor(new Dictionary<Expression, Expression>
            {
                {expression.Parameters[0], Expression.Constant(@delegate.Target)}
            });
            var transformed = visitor.Visit(expression.Body);
            return Expression.Lambda(transformed, expression.Parameters.Skip(1));
        }

        public static LambdaExpression Decompile(this MethodInfo method)
        {
            return cache.GetOrAdd(method, m => MethodBodyDecompiler.Decompile(method));
        }

        public static IQueryable<T> Decompile<T>(this IQueryable<T> self)
        {
            var provider = new DecompiledQueryProvider(self.Provider);
            return provider.CreateQuery<T>(self.Expression);
        }
    }
}
