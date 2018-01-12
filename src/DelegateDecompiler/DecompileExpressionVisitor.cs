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

        // A list of all possible overridden method calls from a base MethodCallExpression
        private List<Tuple<Expression, MethodInfo>> _virtualCallContexts = new List<Tuple<Expression, MethodInfo>>();

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // PROBLEM - the MethodCallExpression passed by the first ExpressionVisitor.Visit()
            // points to the least specific MethodInfo ie the method declared as virtual

            Expression instance = node.Object;
            // Check whether the call is issued by ExpressionVisitor.Visit() or specific
            // implementations have already been resolved
            bool isExplicitCall = !node.Method.IsVirtual || _virtualCallContexts.Any(vcc => vcc.Item1 == instance);
            if (!isExplicitCall)
            {
                var castObjects = new HashSet<Expression>();
                try
                {
                    var implementations = GetImplementationsFor(node.Method);
                    _virtualCallContexts.Add(new Tuple<Expression, MethodInfo>(instance, node.Method));
                    MethodInfo mostSpecificImplementation;
                    if (implementations is IEnumerable)
                    {
                        Expression expandedCall = null;
                        mostSpecificImplementation = (implementations as List<KeyValuePair<Type, MethodInfo>>).Where(vCall => vCall.Key.IsAssignableFrom(node.Object.Type)).Select(kv => kv.Value).LastOrDefault();
                        if (mostSpecificImplementation == null) return node;

                        var overridingImplementations = (implementations as List<KeyValuePair<Type, MethodInfo>>).Where(vCall => vCall.Key.IsSubclassOf(node.Object.Type)).GroupBy(kv => kv.Key);
                        if (overridingImplementations.Any())
                        {
                            bool requiresConditional = false;
                            if (mostSpecificImplementation != null)
                            {
                                var targetInstance = mostSpecificImplementation.DeclaringType.IsAssignableFrom(node.Object.Type) ? node.Object : Expression.TypeAs(instance, mostSpecificImplementation.DeclaringType);
                                castObjects.Add(targetInstance);
                                _virtualCallContexts.Add(new Tuple<Expression, MethodInfo>(targetInstance, mostSpecificImplementation));
                                expandedCall = base.Visit(Expression.Call(targetInstance, mostSpecificImplementation, node.Arguments));
                                requiresConditional = true;
                            }
                            foreach (var explicitCall in overridingImplementations)
                            {
                                var targetInstance = node.Object.Type == explicitCall.Key ? node.Object : Expression.TypeAs(instance, explicitCall.Key);
                                castObjects.Add(targetInstance);
                                _virtualCallContexts.AddRange(explicitCall.Select(vc => new Tuple<Expression, MethodInfo>(targetInstance, vc.Value)));
                                if (!requiresConditional)
                                {
                                    expandedCall = base.Visit(Expression.Call(targetInstance, explicitCall.Select(mostSpecific => mostSpecific.Value).First(), node.Arguments));
                                    requiresConditional = true;
                                }
                                else
                                {
                                    expandedCall = Expression.Condition(Expression.TypeIs(node.Object, explicitCall.Key), base.Visit(Expression.Call(targetInstance, explicitCall.Select(mostSpecific => mostSpecific.Value).First() as MethodInfo, node.Arguments)), expandedCall);
                                }
                            }
                            return expandedCall;
                        }
                    }
                    else
                    {
                        mostSpecificImplementation = implementations as MethodInfo;
                    }
                    node = Expression.Call(node.Object, mostSpecificImplementation, node.Arguments);
                }
                finally
                {
                    // clear all references to node.Object casts
                    _virtualCallContexts.RemoveAll(items => castObjects.Contains(items.Item1));
                }
            }
            try
            {
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
                        return Decompile(methodCall.Method, instance, methodCall.Arguments);
                    }
                }

                if (ShouldDecompile(node.Method))
                {
                    return Decompile(node.Method, node.Object, node.Arguments);
                }

                return base.VisitMethodCall(node);
            }
            finally
            {
                _virtualCallContexts.Remove(new Tuple<Expression, MethodInfo>(node.Object, node.Method));
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

        // values are either MethodInfo or List<MethodInfo>
        private static Dictionary<MethodInfo, object> _specificImplementations = new Dictionary<MethodInfo, object>();

        private object GetImplementationsFor(MethodInfo method)
        {
            object implementations;
            // first perform a lookup
            if (_specificImplementations.TryGetValue(method, out implementations))
            {
                return implementations;
            }
            var implementationsList = new List<KeyValuePair<Type, MethodInfo>>();
            bool shouldDecompile = ShouldDecompile(method);
            if (!method.IsAbstract) implementationsList.Add(new KeyValuePair<Type, MethodInfo>(method.DeclaringType, method));
            var subclasses = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic).SelectMany(a => a.GetExportedTypes().Where(t => t.IsSubclassOf(method.DeclaringType))).Where(t => t != null).ToList();
            subclasses.Sort((t1, t2) => t1 == null || t2 == null ? 0 : t1.IsSubclassOf(t2) ? 1 : t1.FullName.CompareTo(t2.FullName));
            foreach (var c in subclasses)
            {
                MethodInfo impl = c.GetMethod(method.Name, method.GetParameters().Select(a => a.ParameterType).ToArray());
                if (impl.DeclaringType == c)
                {
                    if (impl.IsGenericMethod)
                    {
                        impl = method.MakeGenericMethod(impl.GetGenericArguments());
                    }
                    if (shouldDecompile) Configuration.Instance.RegisterDecompileableMember(impl);
                    implementationsList.Add(new KeyValuePair<Type, MethodInfo>(c, impl));
                }
            };
            _specificImplementations[method] = implementationsList;
            return implementationsList;
        }
    }
}