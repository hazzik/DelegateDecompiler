using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DelegateDecompiler
{
    class DecompiledQueryable<T> : IQueryable<T>
    {
        readonly IQueryable<T> inner;

        public DecompiledQueryable(IQueryable<T> inner)
        {
            this.inner = inner;
            
            Provider = new DecompiledQueryProvider(inner.Provider);
            Expression = inner.Expression;
            ElementType = inner.ElementType;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Expression Expression { get; private set; }
        public Type ElementType { get; private set; }
        public IQueryProvider Provider { get; private set; }
    }
}