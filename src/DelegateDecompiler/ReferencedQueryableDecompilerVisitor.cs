using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiler
{
    public class ReferencedQueryableExpressionVisitor
        : ExpressionVisitor
    {
        public static Expression Decompile(Expression expression)
        {
            return new ReferencedQueryableExpressionVisitor().Visit(expression);
        }

        public override Expression Visit(Expression node)
        {
            var result = base.Visit(node);
            return result;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (typeof(IQueryable).IsAssignableFrom(node.Type))
            {
                if (Configuration.Instance.ShouldDecompile(node.Member))
                {
                    return node.Decompile().Dereference();
                }
                else if (node.Member is FieldInfo)
                {
                    return (node.Evaluate<IQueryable>()).Expression.Decompile();
                }
            }
            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (typeof(IQueryable).IsAssignableFrom(node.Type))
            {
                if (Configuration.Instance.ShouldDecompile(node.Method))
                {
                    return node.Decompile().Dereference();
                }
            }
            return base.VisitMethodCall(node);
        }
    }
}