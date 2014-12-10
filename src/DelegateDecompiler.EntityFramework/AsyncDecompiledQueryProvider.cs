using System;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Threading;

namespace DelegateDecompiler.EntityFramework
{
    class AsyncDecompiledQueryProvider : DecompiledQueryProvider, IDbAsyncQueryProvider
    {
        readonly IQueryProvider inner;

        protected internal AsyncDecompiledQueryProvider(IQueryProvider inner)
            : base(inner)
        {
            this.inner = inner;
        }

        public override IQueryable CreateQuery(Expression expression)
        {
            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return new AsyncDecompiledQueryable(this, inner.CreateQuery(decompiled));
        }

        public override IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return new AsyncDecompiledQueryable<TElement>(this, inner.CreateQuery<TElement>(decompiled));
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            IDbAsyncQueryProvider asyncProvider = inner as IDbAsyncQueryProvider;
            if (asyncProvider == null)
            {
                throw new InvalidOperationException("The source IQueryProvider doesn't implement IDbAsyncQueryProvider.");
            }
            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return asyncProvider.ExecuteAsync(decompiled, cancellationToken);
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            IDbAsyncQueryProvider asyncProvider = inner as IDbAsyncQueryProvider;
            if (asyncProvider == null)
            {
                throw new InvalidOperationException("The source IQueryProvider doesn't implement IDbAsyncQueryProvider.");
            }
            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return asyncProvider.ExecuteAsync<TResult>(decompiled, cancellationToken);
        }
    }
}