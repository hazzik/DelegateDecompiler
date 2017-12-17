using System;
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

        private Dictionary<Tuple<Expression, string>, Type> _methodInheritanceContext = new Dictionary<Tuple<Expression, string>, Type>();

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // PROBLEM - the MethodCallExpression does not always (?never?) point to the most
            // specific method for the object when a Computed method is overriden

            Expression instanceParameter = node.Object;
            //Allows to decompile overriding implementations of a Computed method
            bool shouldDecompileOverriddenCall = false;
            try
            {
                // WORKAROUND - find the most specific implementation not used for the parameter and
                // redirects the call the base implementation when the method is called again in the
                // same decompilation unit

                // Side Effect => this makes it impossible to use recursive calls but those would
                // probably be refused by any IQueryProvider ?? Solution => add a configuration
                // setting to allow this behaviour ?

                // TODO Discover all specific implementations of the method sorted by specificity and
                // expand the call into a : iif(o is XXX, o.SubType.Method, iif(..)) expression
                if (node.Method.IsVirtual && instanceParameter != null && node.Object.Type != node.Method.DeclaringType)
                {
                    Tuple<Expression, string> implementation = new Tuple<Expression, string>(instanceParameter, node.Method.Name);
                    Type currentType;
                    if (!_methodInheritanceContext.TryGetValue(implementation, out currentType))
                    {
                        currentType = node.Object.Type;
                        _methodInheritanceContext[implementation] = currentType;
                    }
                    else
                    {
                        currentType = currentType.BaseType;
                    }
                    MethodInfo method = currentType != null ? currentType.GetMethod(node.Method.Name, node.Arguments.Select(a => a.Type).ToArray()) : null;
                    if (method != null && method != node.Method)
                    {
                        shouldDecompileOverriddenCall = true;
                        currentType = method.DeclaringType;
                        if (method.IsGenericMethod)
                        {
                            method = method.MakeGenericMethod(node.Method.GetGenericArguments());
                        }
                        node = Expression.Call(node.Object, method, node.Arguments);
                    }
                }
                // END OF WORKAROUND
                if (node.Method.IsGenericMethod && node.Method.GetGenericMethodDefinition() == typeof(ComputedExtension).GetMethod("Computed", BindingFlags.Static | BindingFlags.Public))
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
                        return Decompile(methodCall.Method, instanceParameter, methodCall.Arguments);
                    }
                }

                if (shouldDecompileOverriddenCall || ShouldDecompile(node.Method))
                {
                    return Decompile(node.Method, node.Object, node.Arguments);
                }

                return base.VisitMethodCall(node);
            }
            finally
            {
                foreach (var parameterKey in _methodInheritanceContext.Keys.Where(key => key.Item1 == instanceParameter).ToList())
                    _methodInheritanceContext.Remove(parameterKey);
            }
        }

        protected virtual bool ShouldDecompile(MemberInfo methodInfo)
        {
            return Configuration.Instance.ShouldDecompile(methodInfo);
        }

        private Expression Decompile(MethodInfo method, Expression instance, IList<Expression> arguments)
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
            Expression result = new ReplaceExpressionVisitor(expressions).Visit(expression.Body);
            return Visit(result);
        }
    }
}