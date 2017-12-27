using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace DelegateDecompiler.EntityFramework
{
    internal class AsyncDecompiledQueryable<T> : DecompiledQueryable<T>, IDbAsyncEnumerable<T>, IEnumerable
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override IEnumerator<T> GetEnumerator()
        {
            if (MergeOption == MergeOption.NoTracking) return (IEnumerator<T>)((this.inner as IQueryable).AsNoTracking()).GetEnumerator();
            return base.GetEnumerator();
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            IDbAsyncEnumerable<T> asyncEnumerable = (IEnumerator<T>)((this.inner as IQueryable).AsNoTracking()) as IDbAsyncEnumerable<T>;
            if (asyncEnumerable == null)
            {
                throw new InvalidOperationException("The source IQueryable doesn't implement IDbAsyncEnumerable<T>.");
            }
            return asyncEnumerable.GetAsyncEnumerator();
        }
    }
}