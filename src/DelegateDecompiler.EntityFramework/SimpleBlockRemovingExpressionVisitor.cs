using System.Linq;
using System.Linq.Expressions;

namespace DelegateDecompiler.EntityFramework
{
    internal class SimpleBlockRemovingExpressionVisitor
        : ExpressionVisitor
    {
        protected override Expression VisitBlock(BlockExpression node)
        {
            if (node.Expressions.Count <= 1)
            {
                return node.Expressions.FirstOrDefault();
            }
            return base.VisitBlock(node);
        }

        public static Expression Process(Expression expression)
        {
            return new SimpleBlockRemovingExpressionVisitor().Visit(expression);
        }
    }
}
