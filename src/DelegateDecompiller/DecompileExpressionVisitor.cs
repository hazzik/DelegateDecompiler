using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiller
{
    public class DecompileExpressionVisitor :ExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var decompileAttributes = node.Method.GetCustomAttributes(typeof(DecompileAttribute), true);
            if (decompileAttributes.Length > 0)
            {
                var expression = node.Method.Decompile();
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var decompileAttributes = node.Member.GetCustomAttributes(typeof(DecompileAttribute), true);
            if (decompileAttributes.Length > 0)
            {
                var info = node.Member as PropertyInfo;
                if (info != null)
                {
                    var method = info.GetGetMethod();
                    var expression = method.Decompile();

                    var expressions = new Dictionary<Expression, Expression>();
                    for (int index = 0; index < expression.Parameters.Count; index++)
                    {
                        var parameter = expression.Parameters[index];
                        if (index == 0 && method.IsStatic == false)
                        {
                            expressions.Add(parameter, node.Expression);
                        }
                    }

                    var visitor = new ReplaceExpressionVisitor(expressions).Visit(expression.Body);
                    return visitor;
                }
            }
            return base.VisitMember(node);
        }
    }
}