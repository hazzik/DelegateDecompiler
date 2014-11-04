using System;
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
            return Decompile(@delegate.Method);
        }

        public static LambdaExpression Decompile(this MethodInfo method)
        {
            return cache.GetOrAdd(method, m => new MethodBodyDecompiler(method).Decompile());
        }

        public static IQueryable<T> Decompile<T>(this IQueryable<T> self)
        {
            var expression = DecompileExpressionVisitor.Decompile(self.Expression);
            if (expression != self.Expression)
            {
                return self.Provider.CreateQuery<T>(expression);
            }
            return self;
        }

        public static IQueryable<T> Decompiled<T>(this IQueryable<T> source)
        {
            return new DecompiledQueryable<T>(source);
        }
    }
}