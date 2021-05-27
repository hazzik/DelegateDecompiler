using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiler
{
    class TransparentIdentifierRemovingExpressionVisitor : ExpressionVisitor
    {
        public static Expression RemoveTransparentIdentifiers(Expression expression)
        {
            Expression before;
            var after = expression;
            var visitor = new TransparentIdentifierRemovingExpressionVisitor();
            do
            {
                before = after;
                after = visitor.Visit(after);
            }
            while (after != before);

            return after;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var memberBindings = GetMemberBindingsCreatedByExpression(node.Expression);
            if (memberBindings == null)
                return base.VisitMember(node);

            var matchingAssignment = memberBindings.LastOrDefault(b => Match(b.Member, node.Member));
            if (matchingAssignment == null)
                return base.VisitMember(node);

            return matchingAssignment.Expression;
        }

        private static IEnumerable<MemberAssignment> GetMemberBindingsCreatedByExpression(Expression expression)
        {
            if (expression is MemberInitExpression memberInitExpression)
                return memberInitExpression.Bindings.OfType<MemberAssignment>();

            if (expression is NewExpression newExpression && newExpression.Members != null)
                return newExpression.Members.Select((t, i) => Expression.Bind(t, newExpression.Arguments[i]));
            
            return null;
        }

        private static bool Match(MemberInfo a, MemberInfo b)
        {
            if (a == b) return true;
            
            if (a is PropertyInfo propertyInfo && b is MethodInfo methodInfo && propertyInfo.CanRead && methodInfo == propertyInfo.GetGetMethod(true)) 
                return true;

            return false;
        }
    }
}