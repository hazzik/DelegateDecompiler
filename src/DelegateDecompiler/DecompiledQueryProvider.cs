using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiler
{
    public class DecompiledQueryProvider : IQueryProvider
    {
        static readonly MethodInfo OpenGenericCreateQueryMethod =
            typeof(DecompiledQueryProvider)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .Single(method => method.Name == "CreateQuery" && method.IsGenericMethod);

        protected IQueryProvider Inner { get; }

        protected internal DecompiledQueryProvider(IQueryProvider inner)
        {
            Inner = inner;
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

            var closedGenericCreateQueryMethod = OpenGenericCreateQueryMethod.MakeGenericMethod(elementType);

            return (IQueryable)closedGenericCreateQueryMethod.Invoke(this, new object[] { expression });
        }

        public virtual IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return new DecompiledQueryable<TElement>(this, Inner.CreateQuery<TElement>(decompiled));
        }

        public object Execute(Expression expression)
        {
            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return Inner.Execute(decompiled);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return Inner.Execute<TResult>(decompiled);
        }
    }
}