using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
#if NETSTANDARD2_1_OR_GREATER
using System.Threading;
#endif

namespace DelegateDecompiler
{
    public class DecompiledQueryable<T> : IOrderedQueryable<T>
    #if NETSTANDARD2_1_OR_GREATER
        , IAsyncEnumerable<T>
    #endif
    {
        readonly IQueryable<T> inner;
        readonly IQueryProvider provider;

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
            return inner.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return inner.GetEnumerator();
        }

#if NETSTANDARD2_1_OR_GREATER
        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            if (inner is not IAsyncEnumerable<T> e)
            {
                throw new NotSupportedException($"The source 'IQueryable' doesn't implement {typeof(IAsyncEnumerable<T>)}.");
            }

            return e.GetAsyncEnumerator(cancellationToken);
        }
#endif

        public override string ToString()
        {
            return inner.ToString();
        }
    }
}