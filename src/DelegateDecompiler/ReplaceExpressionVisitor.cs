using System.Collections.Generic;
using System.Linq.Expressions;

namespace DelegateDecompiler
{
    public class ReplaceExpressionVisitor : ExpressionVisitor
    {
        private readonly IDictionary<Expression, Expression> replacements;

        public ReplaceExpressionVisitor(IDictionary<Expression, Expression> replacements)
        {
            this.replacements = replacements;
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
    }
}