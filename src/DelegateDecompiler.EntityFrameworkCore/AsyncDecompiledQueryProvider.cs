using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace DelegateDecompiler.EntityFrameworkCore
{
    class AsyncDecompiledQueryProvider : DecompiledQueryProvider, IAsyncQueryProvider
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
            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return new AsyncDecompiledQueryable<TElement>(this, inner.CreateQuery<TElement>(decompiled));
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            if (!(inner is IAsyncQueryProvider asyncProvider))
            {
                throw new InvalidOperationException("The source IQueryProvider doesn't implement IDbAsyncQueryProvider.");
            }

            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return asyncProvider.ExecuteAsync<TResult>(decompiled);
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            if (!(inner is IAsyncQueryProvider asyncProvider))
            {
                throw new InvalidOperationException("The source IQueryProvider doesn't implement IDbAsyncQueryProvider.");
            }

            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return asyncProvider.ExecuteAsync<TResult>(decompiled, cancellationToken);
        }
    }
}
