using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiller
{
    public class DecompileExpressionVisitor : ExpressionVisitor
    {
        public static Expression Decompile(Expression expression)
        {
            return new DecompileExpressionVisitor().Visit(expression);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (ShouldDecompile(node.Member))
            {
                var info = node.Member as PropertyInfo;
                if (info != null)
                {
                    var method = info.GetGetMethod();
                    return Decompile(method, node.Expression, new List<Expression>());
                }
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (ShouldDecompile(node.Method))
            {
                return Decompile(node.Method, node.Object, node.Arguments);
            }

            return base.VisitMethodCall(node);
        }

        static bool ShouldDecompile(ICustomAttributeProvider methodInfo)
        {
            return methodInfo.GetCustomAttributes(typeof (DecompileAttribute), true).Length > 0 ||
                   methodInfo.GetCustomAttributes(typeof (ComputedAttribute), true).Length > 0;
        }

        static Expression Decompile(MethodBase method, Expression instance, IList<Expression> arguments)
        {
            var expression = method.Decompile();

            var expressions = new Dictionary<Expression, Expression>();
            var argIndex = 0;
            for (var index = 0; index < expression.Parameters.Count; index++)
            {
                var parameter = expression.Parameters[index];
                if (index == 0 && method.IsStatic == false)
                {
                    expressions.Add(parameter, instance);
                }
                else
                {
                    expressions.Add(parameter, arguments[argIndex++]);
                }
            }

            return new ReplaceExpressionVisitor(expressions).Visit(expression.Body);
        }
    }
}
