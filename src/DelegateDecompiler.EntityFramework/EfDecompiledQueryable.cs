using System;
using System.Collections;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace DelegateDecompiler.EntityFramework
{
    internal class EfDecompiledQueryable<T>
        : DecompiledQueryable<T>, IDbAsyncEnumerable<T>, IEnumerable
    {
        protected internal EfDecompiledQueryable(IQueryProvider provider, IQueryable<T> inner)
            : base(provider, inner)
        {
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
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

        #region Linq overrides

        public IQueryable<T> Include(string path)
        {
            return inner.Include(path).Decompile();
        }

        public IQueryable<T> AsNoTracking()
        {
            return ((IQueryable<T>)((IQueryable)inner).AsNoTracking()).Decompile();
        }

        #endregion
    }
}