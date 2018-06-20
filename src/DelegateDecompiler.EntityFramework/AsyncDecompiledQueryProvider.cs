using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DelegateDecompiler.EntityFramework
{
    internal class AsyncDecompiledQueryProvider
        : DecompiledQueryProvider, IDbAsyncQueryProvider
    {
        protected internal AsyncDecompiledQueryProvider(IQueryProvider inner)
            : base(inner, typeof(EFDecompileExpressionVisitor), typeof(AsyncDecompiledQueryable<>))
        {
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            IDbAsyncQueryProvider asyncProvider = inner as IDbAsyncQueryProvider;
            if (asyncProvider == null)
            {
                throw new InvalidOperationException("The source IQueryProvider doesn't implement IDbAsyncQueryProvider.");
            }
            return asyncProvider.ExecuteAsync(Decompile(expression), cancellationToken);
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            IDbAsyncQueryProvider asyncProvider = inner as IDbAsyncQueryProvider;
            if (asyncProvider == null)
            {
                throw new InvalidOperationException("The source IQueryProvider doesn't implement IDbAsyncQueryProvider.");
            }
            return asyncProvider.ExecuteAsync<TResult>(Decompile(expression), cancellationToken);
        }
    }
}