using System.Linq;
using System.Linq.Expressions;

namespace DelegateDecompiler
{
    public class DecompiledQueryProvider : IQueryProvider
    {
        readonly IQueryProvider inner;

        protected internal DecompiledQueryProvider(IQueryProvider inner)
        {
            this.inner = inner;
        }

        public virtual IQueryable CreateQuery(Expression expression)
        {
            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return new DecompiledQueryable(this, inner.CreateQuery(decompiled));
        }

        public virtual IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            var decompiled = DecompileExpressionVisitor.Decompile(expression);
            return new DecompiledQueryable<TElement>(this, inner.CreateQuery<TElement>(decompiled));
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