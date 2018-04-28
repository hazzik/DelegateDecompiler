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
        public static LambdaExpression Decompile(MethodInfo method)
        {
            return Decompile(method, null);
        }

        public static LambdaExpression Decompile(MethodInfo method, Type declaringType)
        {
            var args = method.GetParameters()
                .Select(p => (Address) Expression.Parameter(p.ParameterType, p.Name))
                .ToList();

            var methodType = declaringType ?? method.DeclaringType;
            if (!method.IsStatic)
                args.Insert(0, Expression.Parameter(methodType, "this"));

            var expression = method.IsVirtual
                ? DecompileVirtual(methodType, method, args)
                : DecompileConcrete(method, args);

            var optimizedExpression = new OptimizeExpressionVisitor().Visit(expression);

            return Expression.Lambda(optimizedExpression, args.Select(x => (ParameterExpression) x.Expression));
        }

        static Expression DecompileConcrete(MethodInfo method, IList<Address> args)
        {
            var body = method.GetMethodBody();
            var addresses = new VariableInfo[body.LocalVariables.Count];
            for (var i = 0; i < addresses.Length; i++)
                addresses[i] = new VariableInfo(body.LocalVariables[i].LocalType);
            var locals = addresses.ToArray();

            var instructions = method.GetInstructions();
            return Processor.Process(locals, args, instructions.First(), method.ReturnType);
        }

        static Expression DecompileVirtual(Type declaringType, MethodInfo method, IList<Address> args)
        {
            if (declaringType == null)
                throw new InvalidOperationException($"Method {method.Name} does not have a declaring type");

            var @this = args[0].Expression;

            var result = GetDefaultImplementation(declaringType, method, args);

            var childrenTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => declaringType.IsAssignableFrom(t) && t != declaringType)
                .OrderBy(t => t, new TypeHierarchyComparer());

            foreach (var type in childrenTypes)
            {
                var declaredMethod = GetDeclaredMethod(type, method);
                if (declaredMethod != null && !declaredMethod.IsAbstract)
                {
                    var localArgs = args.ToList();
                    localArgs[0] = Expression.Convert(@this, type);

                    var childExpression = DecompileConcrete(declaredMethod, localArgs);

                    result = Expression.Condition(Expression.TypeIs(@this, type), childExpression, result);
                }
            }

            return result;
        }

        private static Expression GetDefaultImplementation(Type declaringType, MethodInfo method, IList<Address> args)
        {
            for (var type = declaringType; type != null && type != typeof(object); type = type.BaseType)
            {
                var declaredMethod = GetDeclaredMethod(type, method);
                if (declaredMethod != null && !declaredMethod.IsAbstract)
                {
                    return DecompileConcrete(declaredMethod, args);
                }
            }

            return ExpressionHelper.Default(method.ReturnType);
        }

        static MethodInfo GetDeclaredMethod(Type type, MethodInfo method)
        {
            return type.GetMethod(method.Name,
                BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
    }
}
