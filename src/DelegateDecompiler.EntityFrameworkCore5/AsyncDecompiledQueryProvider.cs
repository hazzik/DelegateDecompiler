using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace DelegateDecompiler.EntityFrameworkCore
{
    class AsyncDecompiledQueryProvider : DecompiledQueryProvider, IAsyncQueryProvider
    {
        public AsyncDecompiledQueryProvider(IQueryProvider inner)
            : base(inner)
        {
        }

        IAsyncQueryProvider AsyncQueryProvider
        {
            get
            {
                if (Inner is IAsyncQueryProvider asyncProvider) return asyncProvider;

                throw new InvalidOperationException("The source IQueryProvider doesn't implement IAsyncQueryProvider.");
            }
        }

        public virtual TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var decompiled = expression.Decompile();
            return AsyncQueryProvider.ExecuteAsync<TResult>(decompiled, cancellationToken);
        }

        [SuppressMessage("EntityFramework", "EF1001")]
        public override IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var decompiled = expression.Decompile();
            return new EntityQueryable<TElement>(this, decompiled);
        }
    }
}
