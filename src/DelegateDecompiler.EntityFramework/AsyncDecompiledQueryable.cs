using System;
using System.Data.Entity;
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
            if (inner is IDbAsyncEnumerable<T> asyncEnumerable)
            {
                return asyncEnumerable.GetAsyncEnumerator();
            }

            throw new InvalidOperationException("The source IQueryable doesn't implement IDbAsyncEnumerable<T>.");
        }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            if (inner is IDbAsyncEnumerable<T> asyncEnumerable)
            {
                return asyncEnumerable.GetAsyncEnumerator();
            }

            throw new InvalidOperationException("The source IQueryable doesn't implement IDbAsyncEnumerable<T>.");
        }

        public override IQueryable<T> Include(string path)
        {
            return inner.Include(path).DecompileAsync();
        }

        public override IQueryable<T> AsNoTracking()
        {
            var queryable = (IQueryable<T>) inner.AsNoTracking();
            return queryable.DecompileAsync();
        }
    }
}
