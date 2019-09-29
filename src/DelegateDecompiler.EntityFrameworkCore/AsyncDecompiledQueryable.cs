using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace DelegateDecompiler.EntityFrameworkCore
{
    class AsyncDecompiledQueryable<T> : DecompiledQueryable<T>, IAsyncEnumerable<T>
    {
        private readonly IQueryable<T> inner;

        protected internal AsyncDecompiledQueryable(IQueryProvider provider, IQueryable<T> inner)
            : base(provider, inner)
        {
            this.inner = inner;
        }

#if NETSTANDARD2_0
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetEnumerator()
        {
            if (inner is IAsyncEnumerable<T> asyncEnumerable)
                return asyncEnumerable.GetEnumerator();
            
            throw new InvalidOperationException("The source IQueryable doesn't implement IDbAsyncEnumerable<T>.");
        }
#else
        IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken = new CancellationToken())
        {
            if (inner is IAsyncEnumerable<T> asyncEnumerable)
                return asyncEnumerable.GetAsyncEnumerator(cancellationToken);
            
            throw new InvalidOperationException("The source IQueryable doesn't implement IDbAsyncEnumerable<T>.");
        }
#endif
    }
}
