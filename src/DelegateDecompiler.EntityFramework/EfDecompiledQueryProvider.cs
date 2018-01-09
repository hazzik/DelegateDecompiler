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
    public class EfDecompiledQueryProvider : DecompiledQueryProvider, IDbAsyncQueryProvider
    {
        private static readonly MethodInfo openGenericCreateQueryMethod =
            typeof(EfDecompiledQueryProvider)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(method => method.Name == "CreateQuery" && method.IsGenericMethod);

        private readonly IQueryProvider _inner;

        protected internal EfDecompiledQueryProvider(IQueryProvider inner)
            : base(inner)
        {
            _inner = inner;
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
            var decompiled = new DecompileExpressionVisitor().Visit(expression);
            return new EfDecompiledQueryable<TElement>(this, _inner.CreateQuery<TElement>(decompiled));
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            IDbAsyncQueryProvider asyncProvider = _inner as IDbAsyncQueryProvider;
            if (asyncProvider == null)
            {
                throw new InvalidOperationException("The source IQueryProvider doesn't implement IDbAsyncQueryProvider.");
            }
            var decompiled = new DecompileExpressionVisitor().Visit(expression);
            return asyncProvider.ExecuteAsync(decompiled, cancellationToken);
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            IDbAsyncQueryProvider asyncProvider = _inner as IDbAsyncQueryProvider;
            if (asyncProvider == null)
            {
                throw new InvalidOperationException("The source IQueryProvider doesn't implement IDbAsyncQueryProvider.");
            }
            var decompiled = new DecompileExpressionVisitor().Visit(expression);
            return asyncProvider.ExecuteAsync<TResult>(decompiled, cancellationToken);
        }
    }
}