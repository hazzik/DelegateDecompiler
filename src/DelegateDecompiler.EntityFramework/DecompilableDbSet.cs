using DelegateDecompiler.EntityFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace ML.WebEnseignes.AppCore.EntityFramework
{
    public class DecompilableDbSet<T>
        : IDbSet<T>
        where T : class
    {
        protected IDbSet<T> inner;

        public DecompilableDbSet(IDbSet<T> baseSet)
        {
            inner = baseSet;
        }

        public ObservableCollection<T> Local => inner.Local;

        public Expression Expression => inner.Decompile().Expression;

        public Type ElementType => inner.ElementType;

        public IQueryProvider Provider => new EfDecompiledQueryProvider(inner.Provider);

        public virtual T Add(T entity)
        {
            return inner.Add(entity);
        }

        public virtual T Attach(T entity)
        {
            return inner.Attach(entity);
        }

        public virtual T Create()
        {
            return inner.Create();
        }

        public T Find(params object[] keyValues)
        {
            return inner.Create();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        public T Remove(T entity)
        {
            return inner.Remove(entity);
        }

        TDerivedEntity IDbSet<T>.Create<TDerivedEntity>()
        {
            return inner.Create<TDerivedEntity>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return inner.GetEnumerator();
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