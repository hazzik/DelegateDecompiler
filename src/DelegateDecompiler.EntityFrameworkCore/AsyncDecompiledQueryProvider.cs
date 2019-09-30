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
    class AsyncDecompiledQueryProviderBase : DecompiledQueryProvider
    {
        static readonly MethodInfo ExecuteAsync2 = typeof(IAsyncQueryProvider)
            .GetMethod("ExecuteAsync", new[] {typeof(Expression), typeof(CancellationToken)});

        static readonly MethodInfo ExecuteAsync1 = typeof(IAsyncQueryProvider)
            .GetMethod("ExecuteAsync", new[] {typeof(Expression)});

        protected AsyncDecompiledQueryProviderBase(IQueryProvider inner)
            : base(inner)
        {
        }

        protected IAsyncQueryProvider AsyncQueryProvider
        {
            get
            {
                if (Inner is IAsyncQueryProvider asyncProvider) return asyncProvider;

                throw new InvalidOperationException("The source IQueryProvider doesn't implement IAsyncQueryProvider.");
            }
        }

        // ReSharper disable once UnusedMember.Global
        // ReSharper disable once VirtualMemberNeverOverridden.Global
        public virtual TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return (TResult) ExecuteAsync<TResult>(AsyncQueryProvider, decompiled, cancellationToken);
        }

        protected static object ExecuteAsync<TResult>(IAsyncQueryProvider asyncQueryProvider, Expression expression, CancellationToken cancellationToken)
        {
            return ExecuteAsync2.MakeGenericMethod(typeof(TResult)).Invoke(
                asyncQueryProvider,
                new object[]
                {
                    expression, cancellationToken
                });
        }

        protected static object ExecuteAsync<TResult>(IAsyncQueryProvider asyncQueryProvider, Expression expression)
        {
            return ExecuteAsync1.MakeGenericMethod(typeof(TResult)).Invoke(
                asyncQueryProvider,
                new object[]
                {
                    expression
                });
        }
    }

    class AsyncDecompiledQueryProvider : AsyncDecompiledQueryProviderBase, IAsyncQueryProvider
    {
        public AsyncDecompiledQueryProvider(IQueryProvider inner)
            : base(inner)
        {
        }

        public override IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return new EntityQueryable<TElement>(this, decompiled);
        }

        public virtual IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return (IAsyncEnumerable<TResult>) ExecuteAsync<TResult>(AsyncQueryProvider, decompiled);
        }

        public new virtual Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return (Task<TResult>) ExecuteAsync<TResult>(AsyncQueryProvider, decompiled, cancellationToken);
        }
    }
}
