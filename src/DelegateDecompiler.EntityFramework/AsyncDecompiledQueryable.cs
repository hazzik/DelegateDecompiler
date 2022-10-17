using System;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace DelegateDecompiler.EntityFramework
{
    class AsyncDecompiledQueryable<T> : DecompiledQueryable<T>, IDbAsyncEnumerable<T>
    {
        private readonly IQueryable<T> inner;

        protected internal AsyncDecompiledQueryable(IQueryProvider provider, IQueryable<T> inner)
            : base(provider, inner)
        {
            this.inner = inner;
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            IDbAsyncEnumerable<T> asyncEnumerable = inner as IDbAsyncEnumerable<T>;
            if (asyncEnumerable == null)
            {
                throw new InvalidOperationException("The source IQueryable doesn't implement IDbAsyncEnumerable<T>.");
            }
            return asyncEnumerable.GetAsyncEnumerator();
        }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            IDbAsyncEnumerable<T> asyncEnumerable = inner as IDbAsyncEnumerable<T>;
            if (asyncEnumerable == null)
            {
                throw new InvalidOperationException("The source IQueryable doesn't implement IDbAsyncEnumerable<T>.");
            }
            return asyncEnumerable.GetAsyncEnumerator();
        }
    }
}
