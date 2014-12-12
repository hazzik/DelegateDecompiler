using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DelegateDecompiler.EntityFramework
{
    public static class DecompileExtensions
    {
        private static readonly ConcurrentDictionary<MethodInfo, LambdaExpression> cache = new ConcurrentDictionary<MethodInfo, LambdaExpression>();

        public static IQueryable<T> DecompileAsync<T>(this IQueryable<T> self)
        {
            var provider = new AsyncDecompiledQueryProvider(self.Provider);
            return provider.CreateQuery<T>(self.Expression);
        }
    }
}
