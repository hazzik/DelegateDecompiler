using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
#if NET5_0
using Microsoft.EntityFrameworkCore.Query;
#endif
using Microsoft.EntityFrameworkCore.Query.Internal;

namespace DelegateDecompiler.EntityFrameworkCore
{
    class AsyncDecompiledQueryProviderBase : DecompiledQueryProvider
    {
        static readonly MethodInfo OpenGenericExecuteAsync2 = typeof(IAsyncQueryProvider)
            .GetMethod("ExecuteAsync", new[] {typeof(Expression), typeof(CancellationToken)});

        static readonly MethodInfo OpenGenericExecuteAsync1 = typeof(IAsyncQueryProvider)
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
            var decompiled = DecompileExpressionVisitor.Decompile(expression, typeof(TResult));
            return (TResult) MethodCache<TResult>.ExecuteAsync(AsyncQueryProvider, decompiled, cancellationToken);
        }

        protected static class MethodCache<T>
        {
            static readonly Func<IAsyncQueryProvider, Expression, object> ExecuteAsync1 =
                (Func<IAsyncQueryProvider, Expression, object>) CompileDelegate(OpenGenericExecuteAsync1?.MakeGenericMethod(typeof(T)));

            static readonly Func<IAsyncQueryProvider, Expression, CancellationToken, object> ExecuteAsync2 =
                (Func<IAsyncQueryProvider, Expression, CancellationToken, object>) CompileDelegate(OpenGenericExecuteAsync2?.MakeGenericMethod(typeof(T)));

            public static object ExecuteAsync(IAsyncQueryProvider asyncQueryProvider, Expression expression)
            {
                return ExecuteAsync1(asyncQueryProvider, expression);
            }

            public static object ExecuteAsync(IAsyncQueryProvider asyncQueryProvider, Expression expression, CancellationToken cancellationToken)
            {
                return ExecuteAsync2(asyncQueryProvider, expression, cancellationToken);
            }

            static Delegate CompileDelegate(MethodInfo method)
            {
                if (method == null)
                {
                    return null;
                }

                var instance = method.DeclaringType != null && !method.IsStatic
                    ? Expression.Parameter(method.DeclaringType)
                    : null;

                var parameters = Array.ConvertAll(method.GetParameters(),
                    pi => Expression.Parameter(pi.ParameterType));

                return Expression.Lambda(
                        Expression.Call(
                            instance,
                            method,
                            parameters),
                        new[] {instance}.Concat(parameters))
                    .Compile();
            }
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
            var decompiled = DecompileExpressionVisitor.Decompile(expression, typeof(TElement));
            return new EntityQueryable<TElement>(this, decompiled);
        }

        public virtual IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression)
        {
            var decompiled = DecompileExpressionVisitor.Decompile(expression, typeof(TResult));
            return (IAsyncEnumerable<TResult>) MethodCache<TResult>.ExecuteAsync(AsyncQueryProvider, decompiled);
        }

        public new virtual Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            var decompiled = DecompileExpressionVisitor.Decompile(expression, typeof(TResult));
            return (Task<TResult>) MethodCache<TResult>.ExecuteAsync(AsyncQueryProvider, decompiled, cancellationToken);
        }
    }
}
