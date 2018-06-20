using Microsoft.EntityFrameworkCore.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DelegateDecompiler.EntityFrameworkCore
{
    internal class AsyncDecompiledQueryProvider : DecompiledQueryProvider, IAsyncQueryProvider
    {
        private static readonly MethodInfo openGenericCreateQueryMethod =
            typeof(AsyncDecompiledQueryProvider)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(method => method.Name == "CreateQuery" && method.IsGenericMethod);

        protected internal AsyncDecompiledQueryProvider(IQueryProvider inner)
            : base(inner, typeof(DecompileExpressionVisitor), typeof(AsyncDecompiledQueryable<>))
        {
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            if (!(inner is IAsyncQueryProvider asyncProvider))
            {
                throw new InvalidOperationException("The source IQueryProvider doesn't implement IDbAsyncQueryProvider.");
            }

            return asyncProvider.ExecuteAsync<TResult>(CreateQuery(expression).Expression);
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            if (!(inner is IAsyncQueryProvider asyncProvider))
            {
                throw new InvalidOperationException("The source IQueryProvider doesn't implement IDbAsyncQueryProvider.");
            }

            return asyncProvider.ExecuteAsync<TResult>(CreateQuery(expression).Expression, cancellationToken);
        }
    }
}