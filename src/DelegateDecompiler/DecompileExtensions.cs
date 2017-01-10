using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiler
{
    public static class DecompileExtensions
    {
        static readonly ConcurrentDictionary<MethodInfo, Lazy<LambdaExpression>> Cache =
            new ConcurrentDictionary<MethodInfo, Lazy<LambdaExpression>>();

        static readonly Func<MethodInfo, Lazy<LambdaExpression>> DecompileDelegate =
            info => new Lazy<LambdaExpression>(() => MethodBodyDecompiler.Decompile(info));

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
            return Cache.GetOrAdd(method, DecompileDelegate).Value;
        }

        public static IQueryable<T> Decompile<T>(this IQueryable<T> self)
        {
            var provider = new DecompiledQueryProvider(self.Provider);
            return provider.CreateQuery<T>(self.Expression);
        }
    }
}
