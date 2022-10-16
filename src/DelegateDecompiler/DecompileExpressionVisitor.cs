using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiler
{
    public class DecompileExpressionVisitor : ExpressionVisitor
    {
        public static Expression Decompile(Expression expression)
        {
            return new DecompileExpressionVisitor().Visit(expression);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (ShouldDecompile(node.Member) && node.Member is PropertyInfo property)
            {
                return Decompile(property.GetGetMethod(true), node.Expression, new List<Expression>());
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.IsGenericMethod && node.Method.GetGenericMethodDefinition() == typeof(ComputedExtension).GetMethod("Computed", BindingFlags.Static | BindingFlags.Public))
            {
                var argument = node.Arguments.SingleOrDefault();

                switch (argument)
                {
                    case MemberExpression member when member.Member is PropertyInfo property:
                    {
                        return Decompile(property.GetGetMethod(true), member.Expression, new List<Expression>());
                    }
                    case MethodCallExpression methodCall:
                    {
                        return Decompile(methodCall.Method, methodCall.Object, methodCall.Arguments);
                    }
                }
            }

            if (ShouldDecompile(node.Method))
            {
                return Decompile(node.Method, node.Object, node.Arguments);
            }

            return base.VisitMethodCall(node);
        }

        protected virtual bool ShouldDecompile(MemberInfo methodInfo)
        {
            return Configuration.Instance.ShouldDecompile(methodInfo);
        }

        Expression Decompile(MethodInfo method, Expression instance, IList<Expression> arguments)
        {
            var unwrappedInstance = UnwrapUpcast(instance);
            var expression = method.Decompile(unwrappedInstance?.Type);

            var expressions = new Dictionary<Expression, Expression>();
            var argIndex = 0;
            for (var index = 0; index < expression.Parameters.Count; index++)
            {
                var parameter = expression.Parameters[index];
                if (index == 0 && method.IsStatic == false)
                {
                    expressions.Add(parameter, unwrappedInstance);
                }
                else
                {
                    expressions.Add(parameter, arguments[argIndex++]);
                }
            }

            var body = new ReplaceExpressionVisitor(expressions).Visit(expression.Body);
            body = TransparentIdentifierRemovingExpressionVisitor.RemoveTransparentIdentifiers(body);
            return Visit(body);
        }

        static Expression UnwrapUpcast(Expression instance)
        {
            return instance is UnaryExpression cast &&
                   cast.NodeType == ExpressionType.Convert &&
                   cast.Type.IsAssignableFrom(cast.Operand.Type)
                ? cast.Operand
                : instance;
        }
    }
}
