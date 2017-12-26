using System;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace DelegateDecompiler.EntityFramework
{
    internal class AsyncDecompiledQueryable<T> : DecompiledQueryable<T>, IDbAsyncEnumerable<T>
    {
        private readonly IQueryable<T> inner;
        internal MergeOption MergeOption { get; private set; }

        protected internal AsyncDecompiledQueryable(IQueryProvider provider, IQueryable<T> inner)
            : base(provider, inner)
        {
            this.inner = inner;
        }

        public IQueryable<T> AsNoTracking()
        {
            MergeOption = MergeOption.NoTracking;
            return this;
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