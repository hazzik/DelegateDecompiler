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

        protected override Expression VisitBlock(BlockExpression node)
        {
            if (node.Expressions.Count <= 1)
            {
                return node.Expressions.FirstOrDefault();
            }
            return base.VisitBlock(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (ShouldDecompile(node.Member) && node.Member is PropertyInfo property)
            {
                return Decompile(property.GetGetMethod(true), node.Expression, new List<Expression>());
            }

            if (node.Expression is ConstantExpression constExpression)
            {
                if (constExpression.Value == null)
                {
                    return ExpressionHelper.Default(node.Type);
                }
                var value = (node.Member as FieldInfo)?.GetValue(constExpression.Value)
                    ?? (node.Member as PropertyInfo)?.GetValue(constExpression.Value, new object[] { });
                if (value == null)
                {
                    return ExpressionHelper.Default(node.Type);
                }
                // if the value is an IQueryable we visit its expression
                if (value is IQueryable)
                {
                    return Visit((value as IQueryable).Expression);
                }
            }
            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.IsGenericMethod && node.Method.GetGenericMethodDefinition() == typeof (ComputedExtension).GetMethod("Computed", BindingFlags.Static | BindingFlags.Public))
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
            var expression = method.Decompile(instance?.Type);

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

            var body = new ReplaceExpressionVisitor(expressions).Visit(expression.Body);
            body = TransparentIdentifierRemovingExpressionVisitor.RemoveTransparentIdentifiers(body);
            return Visit(body);
        }
    }
}
