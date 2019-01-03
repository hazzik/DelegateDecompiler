using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;

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

        #region EF-Linq overrides

        //TODO complete all EntityFramework-sepcific Linq methods : AsStreaming...

        public IQueryable<T> Include<TProperty>(Expression<Func<T, TProperty>> path)
        {
            //TODO See whether the path should be decompiled and materialize result with a projection
            return inner.Include(path).Decompile();
        }

        public virtual IQueryable<T> Include(string path)
        {
            //TODO See whether the path should be decompiled and materialize result with a projection
            return inner.Include(path).Decompile();
        }

        public virtual IQueryable<T> AsNoTracking()
        {
            return ((IQueryable<T>)((IQueryable)inner).AsNoTracking()).Decompile();
        }

        #endregion
    }
}