using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Reflection;

namespace DelegateDecompiler
{
    public static class MethodBodyDecompiler
    {
        public static LambdaExpression Decompile(KeyValuePair<MethodInfo, Type> methodInfoAndType)
        {
            var method = methodInfoAndType.Key;
            var mainType = methodInfoAndType.Value;

            var args = method.GetParameters()
                .Select(p => (Address) Expression.Parameter(p.ParameterType, p.Name))
                .ToList();

            if (!method.IsStatic)
                args.Insert(0, Expression.Parameter(method.DeclaringType, "this"));

            return method.IsAbstract ? DecompileAbstract(mainType, method, args) : DecompileConcrete(method, args);
        }

        private static LambdaExpression Decompile(Type mainType, MethodInfo method, List<Address> args)
        {
            return method.IsAbstract ? DecompileAbstract(mainType, method, args) : DecompileConcrete(method, args);
        }

        private static LambdaExpression DecompileConcrete(MethodInfo method, List<Address> args)
        {
            var body = method.GetMethodBody();
            var addresses = new VariableInfo[body.LocalVariables.Count];
            for (var i = 0; i < addresses.Length; i++)
                addresses[i] = new VariableInfo(body.LocalVariables[i].LocalType);
            var locals = addresses.ToArray();

            var instructions = method.GetInstructions();
            var ex = Processor.Process(locals, args, instructions.First(), method.ReturnType);
            var optimizedExpression = new OptimizeExpressionVisitor().Visit(ex);
            var parameterExpressions = args.Select(x => (ParameterExpression) x.Expression);
            var result = Expression.Lambda(optimizedExpression, parameterExpressions);
            return result;
        }

        private static LambdaExpression DecompileAbstract(Type mainType, MethodInfo method, List<Address> args)
        {
            var declaringType = mainType ?? method.DeclaringType ?? throw new InvalidOperationException($"Method {method.Name} does not have a declaring type");
            var childrenTypes = declaringType.Assembly.GetTypes().Where(t => t.IsSubclassOf(declaringType) && HasConcrete(t, method));

            var childExpressions = new List<KeyValuePair<Type, Expression>>();
            foreach (var childrenType in childrenTypes)
            {
                var childMethod = GetMethodInfo(childrenType, method);
                var childExpression = Decompile(mainType, childMethod, args);
                childExpressions.Add(new KeyValuePair<Type, Expression>(childrenType, childExpression.Body));
            }

            //Create a global is-as expression for all children types/expressions :

            var defaultValueExpression = Expression.Constant(method.ReturnType.IsValueType ? Activator.CreateInstance(method.ReturnType) : null, method.ReturnType);
            var parameterExpression = (ParameterExpression)args.First().Expression;
            var memberExpression = Expression.Property(parameterExpression, method);
            var returnExpression = BuildConditionalIsTypeAsForExpressionsList(0, memberExpression, childExpressions, defaultValueExpression);

            var lambdaResult = Expression.Lambda(returnExpression, parameterExpression);
            
            return lambdaResult;
        }

        private static bool HasConcrete(Type type, MethodInfo method)
        {
            return GetMethodInfo(type, method) != null;
        }

        private static MethodInfo GetMethodInfo(Type type, MethodInfo method)
        {
            //TODO: probably adapt binding flags depending on the nature of the methodInfo (static, virtual, ...)
            return type.GetMethod(method.Name, BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public);
        }

        private static Expression BuildConditionalIsTypeAsForExpressionsList(int index, MemberExpression currentMemberExpression, List<KeyValuePair<Type, Expression>> expressionsPerTypes, ConstantExpression defaultValueExpression)
        {
            var concreteType = expressionsPerTypes[index].Key;
            return Expression.Condition(
                Expression.TypeIs(currentMemberExpression.Expression, concreteType),
                expressionsPerTypes[index].Value,
                index < expressionsPerTypes.Count - 1
                    ? BuildConditionalIsTypeAsForExpressionsList(index + 1, currentMemberExpression, expressionsPerTypes, defaultValueExpression)
                    : defaultValueExpression);
        }
    }
}