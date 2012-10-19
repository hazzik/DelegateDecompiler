using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiller
{
    public class ReplaceExpressionVisitor :ExpressionVisitor
    {
        readonly IDictionary<Expression, Expression> replacements;

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