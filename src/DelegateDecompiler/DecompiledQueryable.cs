using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DelegateDecompiler
{
    public class DecompiledQueryable<T> : IOrderedQueryable<T>
    {
        private readonly IQueryable<T> inner;
        private readonly IQueryProvider provider;

        protected internal DecompiledQueryable(IQueryProvider provider, IQueryable<T> inner)
        {
            this.inner = inner;
            this.provider = provider;
        }

        public Expression Expression
        {
            get { return inner.Expression; }
        }

        public Type ElementType
        {
            get { return inner.ElementType; }
        }

        public IQueryProvider Provider
        {
            get { return provider; }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

        public override string ToString()
        {
            return inner.ToString();
        }
    }
}