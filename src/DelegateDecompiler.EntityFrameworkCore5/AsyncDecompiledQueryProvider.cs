using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore.Query;

namespace DelegateDecompiler.EntityFrameworkCore
{
    class AsyncDecompiledQueryProvider(IQueryProvider inner) : DecompiledQueryProvider(inner), IAsyncQueryProvider
    {
        IAsyncQueryProvider AsyncQueryProvider
        {
            get
            {
                if (Inner is IAsyncQueryProvider asyncProvider) return asyncProvider;

                throw new InvalidOperationException($"The source IQueryProvider doesn't implement {typeof(IAsyncQueryProvider)}.");
            }
        }

        public virtual TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var decompiled = expression.Decompile();
            return AsyncQueryProvider.ExecuteAsync<TResult>(decompiled, cancellationToken);
        }
    }
}
