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
            var provider = new DecompiledQueryProvider(self.Provider);
            return provider.CreateQuery<T>(self.Expression);
        }

        public static bool IsAnonymous(this MethodInfo method)
        {
            var invalidChars = new[] { '<', '>' };
            return method.DeclaringType == null || method.Name.Any(invalidChars.Contains);
        }
    }
}