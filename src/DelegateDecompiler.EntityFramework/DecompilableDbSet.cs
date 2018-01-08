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
        internal IDbSet<T> _baseSet;

        public DecompilableDbSet(IDbSet<T> baseSet)
        {
            _baseSet = baseSet;
        }

        public ObservableCollection<T> Local => _baseSet.Local;

        public Expression Expression => _baseSet.Decompile().Expression;

        public Type ElementType => _baseSet.ElementType;

        public IQueryProvider Provider => new EfDecompiledQueryProvider(_baseSet.Provider);

        public virtual T Add(T entity)
        {
            return _baseSet.Add(entity);
        }

        public virtual T Attach(T entity)
        {
            return _baseSet.Attach(entity);
        }

        public virtual T Create()
        {
            return _baseSet.Create();
        }

        public T Find(params object[] keyValues)
        {
            return _baseSet.Create();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _baseSet.GetEnumerator();
        }

        public T Remove(T entity)
        {
            return _baseSet.Remove(entity);
        }

        TDerivedEntity IDbSet<T>.Create<TDerivedEntity>()
        {
            return _baseSet.Create<TDerivedEntity>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _baseSet.GetEnumerator();
        }

        #region Linq overrides

        public IQueryable<T> Include(string path)
        {
            return _baseSet.Include(path).Decompile();
        }

        #endregion
    }
}