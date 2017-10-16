using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiler
{
    public class DecompileExpressionVisitor : ExpressionVisitor
    {
        private readonly Type expressionMainType;

        public static Expression Decompile(Expression expression)
        {
            return new DecompileExpressionVisitor(GetExpressionMainType(expression)).Visit(expression);
        }

        /// <summary>
        /// Returns the best expression type from which to handle any abstract calls
        /// </summary>
        private static Type GetExpressionMainType(Expression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Call:
                    var methodCallInstanceType = (expression as MethodCallExpression).Arguments.First().Type;
                    return GetExpressionMainType(methodCallInstanceType);
                default:
                    return GetExpressionMainType(expression.Type);
            }
        }

        /// <summary>
        /// Returns the type itself or the element's type if it represents an Enumeration type
        /// </summary>
        /// <param name="type">The type from which to get best expression main type</param>
        private static Type GetExpressionMainType(Type type)
        {
            if (type.GetInterfaces().Any(i => i.IsGenericType && typeof(IEnumerable).IsAssignableFrom(i)))
            {
                //return the element type
                return type.GetGenericArguments().First();
            }
            return type;
        }

        private DecompileExpressionVisitor(Type expressionMainType)
        {
            this.expressionMainType = expressionMainType ?? throw new ArgumentNullException(nameof(expressionMainType));
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (ShouldDecompile(node.Member))
            {
                var info = node.Member as PropertyInfo;
                if (info != null)
                {
                    return Decompile(info.GetGetMethod(), node.Expression, new List<Expression>());
                }
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.IsGenericMethod && node.Method.GetGenericMethodDefinition() == typeof (ComputedExtension).GetMethod("Computed", BindingFlags.Static | BindingFlags.Public))
            {
                var argument = node.Arguments.SingleOrDefault();

                var member = argument as MemberExpression;
                if (member != null)
                {
                    var info = member.Member as PropertyInfo;
                    if (info != null)
                    {
                        return Decompile(info.GetGetMethod(), member.Expression, new List<Expression>());
                    }
                }
                var methodCall = argument as MethodCallExpression;
                if (methodCall != null)
                {
                    return Decompile(methodCall.Method, methodCall.Object, methodCall.Arguments);
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
            var expression = method.Decompile(expressionMainType);

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

            return Visit(new ReplaceExpressionVisitor(expressions).Visit(expression.Body));
        }
    }
}
