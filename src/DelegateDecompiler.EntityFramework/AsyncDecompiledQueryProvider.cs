using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace DelegateDecompiler.EntityFramework
{
    class AsyncDecompiledQueryProvider : DecompiledQueryProvider, IDbAsyncQueryProvider
    {
        static readonly MethodInfo openGenericCreateQueryMethod =
            typeof(AsyncDecompiledQueryProvider)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(method => method.Name == "CreateQuery" && method.IsGenericMethod);
        readonly IQueryProvider inner;

        protected internal AsyncDecompiledQueryProvider(IQueryProvider inner)
            : base(inner)
        {
            this.inner = inner;
        }

        public override IQueryable CreateQuery(Expression expression)
        {
            Type elementType = expression.Type
                .GetInterfaces()
                .Where(type => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                .Select(type => type.GetGenericArguments().FirstOrDefault())
                .FirstOrDefault();

            if (elementType == null)
            {
                throw new ArgumentException();
            }

            MethodInfo closedGenericCreateQueryMethod = openGenericCreateQueryMethod.MakeGenericMethod(elementType);

            return (IQueryable)closedGenericCreateQueryMethod.Invoke(this, new object[] { expression });
        }

        public override IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var decompiled = expression.Decompile();
            return new AsyncDecompiledQueryable<TElement>(this, inner.CreateQuery<TElement>(decompiled));
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            IDbAsyncQueryProvider asyncProvider = inner as IDbAsyncQueryProvider;
            if (asyncProvider == null)
            {
                throw new InvalidOperationException("The source IQueryProvider doesn't implement IDbAsyncQueryProvider.");
            }
            var decompiled = expression.Decompile();
            return asyncProvider.ExecuteAsync(decompiled, cancellationToken);
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            IDbAsyncQueryProvider asyncProvider = inner as IDbAsyncQueryProvider;
            if (asyncProvider == null)
            {
                throw new InvalidOperationException("The source IQueryProvider doesn't implement IDbAsyncQueryProvider.");
            }
            var decompiled = expression.Decompile();
            return asyncProvider.ExecuteAsync<TResult>(decompiled, cancellationToken);
        }
    }
}