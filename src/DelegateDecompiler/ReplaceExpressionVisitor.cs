using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DelegateDecompiler
{
    public class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly IDictionary<Expression, Expression> replacements;

        public ReplaceExpressionVisitor(IDictionary<Expression, Expression> replacements)
        {
            this.replacements = replacements ?? new Dictionary<Expression, Expression>();
        }

        public override Expression Visit(Expression node)
        {
            if (node == null)
                return null;

            Expression replacement;
            if (replacements.TryGetValue(node, out replacement))
            {
                return base.Visit(replacement);
            }
            return base.Visit(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var objectType = node.Object?.Type;
            if (objectType!=null && objectType.IsGenericType && typeof(Nullable<>) == objectType.GetGenericTypeDefinition() && node.Method.Name == "GetValueOrDefault")
            {
                var elementType = objectType.GetGenericArguments().First();
                if (elementType.IsPrimitive) { 
                    return Expression.Coalesce(Visit(node.Object), ExpressionHelper.Default(elementType, node.Arguments.FirstOrDefault() ?? null));
                }
            }
            return base.VisitMethodCall(node);
        }
    }
}