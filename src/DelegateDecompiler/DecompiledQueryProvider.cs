using System.Linq;
using System.Linq.Expressions;

namespace DelegateDecompiler
{
    class DecompiledQueryProvider : IQueryProvider
    {
        readonly IQueryProvider inner;
        public DecompiledQueryProvider(IQueryProvider inner)
        {
            this.inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return inner.CreateQuery(decompiled);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return inner.CreateQuery<TElement>(decompiled);
        }

        public object Execute(Expression expression)
        {
            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return inner.Execute(decompiled);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return inner.Execute<TResult>(decompiled);
        }
    }
}