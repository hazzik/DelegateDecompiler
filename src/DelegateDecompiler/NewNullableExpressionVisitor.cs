using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DelegateDecompiler
{
    // Convert nullable constructors to a Convert call instead
    public class NewNullableExpressionVisitor : ExpressionVisitor
    {
        protected override Expression VisitNew(NewExpression node)
        {
            // Test if this is a nullable type
            if (node.Type.IsGenericType && node.Type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                return Expression.Convert(node.Arguments[0], node.Type);
            }
            return base.VisitNew(node);
        }
    }
}
