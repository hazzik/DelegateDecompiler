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
        static readonly ConcurrentDictionary<KeyValuePair<MethodInfo, Type>, Lazy<LambdaExpression>> Cache =
            new ConcurrentDictionary<KeyValuePair<MethodInfo, Type>, Lazy<LambdaExpression>>();

        static readonly Func<KeyValuePair<MethodInfo, Type>, Lazy<LambdaExpression>> DecompileDelegate =
            infoAndType => new Lazy<LambdaExpression>(() => MethodBodyDecompiler.Decompile(infoAndType));

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
            return Decompile(method, null);
        }

        public static LambdaExpression Decompile(this MethodInfo method, Type mainType)
        {
            return Cache.GetOrAdd(new KeyValuePair<MethodInfo, Type>(method, mainType), DecompileDelegate).Value;
        }

        public static IQueryable<T> Decompile<T>(this IQueryable<T> self)
        {
            var provider = new DecompiledQueryProvider(self.Provider);
            return provider.CreateQuery<T>(self.Expression);
        }
    }
}
