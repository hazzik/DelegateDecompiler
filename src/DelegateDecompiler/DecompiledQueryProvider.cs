using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiler
{
    public class DecompiledQueryProvider : IQueryProvider
    {
        private static readonly MethodInfo openGenericCreateQueryMethod =
            typeof(DecompiledQueryProvider)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(method => method.Name == "CreateQuery" && method.IsGenericMethod);

        protected readonly IQueryProvider inner;
        protected readonly Type visitorType;
        protected readonly Type queryableType;

        protected internal DecompiledQueryProvider(IQueryProvider inner, Type visitorType = null, Type queryableType = null)
        {
            this.inner = inner;
            this.visitorType = visitorType ?? typeof(DecompileExpressionVisitor);
            this.queryableType = queryableType ?? typeof(DecompiledQueryable<>);
        }

        protected Expression Decompile(Expression expression)
        {
            return ((ExpressionVisitor)Activator.CreateInstance(visitorType)).Visit(expression);
        }

        public virtual IQueryable CreateQuery(Expression expression)
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

        public virtual IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var decompiled = Decompile(expression);
            var queryable = queryableType.MakeGenericType(typeof(TElement));
            return (IQueryable<TElement>)Activator.CreateInstance(queryable, BindingFlags.NonPublic | BindingFlags.Instance, null, new object[] { this, inner.CreateQuery<TElement>(decompiled) }, CultureInfo.CurrentCulture);
        }

        public virtual object Execute(Expression expression)
        {
            var decompiled = Decompile(expression);
            return inner.Execute(decompiled);
        }

        public virtual TResult Execute<TResult>(Expression expression)
        {
            return (TResult)Execute(expression);
        }
    }
}