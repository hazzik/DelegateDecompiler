using Mono.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

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
                .Select(p => (Address)Expression.Parameter(p.ParameterType, p.Name))
                .ToList();

            var methodType = declaringType ?? method.DeclaringType;
            if (!method.IsStatic)
                args.Insert(0, Expression.Parameter(methodType, "this"));

            var expression = method.IsVirtual && !DecompileExtensions.ConcreteCalls.ContainsKey(new Tuple<object, MethodInfo>(args.FirstOrDefault(), method))
                ? DecompileVirtual(methodType, method, args)
                : DecompileConcrete(method, args);

            var optimizedExpression = new OptimizeExpressionVisitor().Visit(expression);

            return Expression.Lambda(optimizedExpression, args.Select(x => (ParameterExpression)x.Expression));
        }

        internal static Expression DecompileConcrete(MethodInfo method, IList<Address> args)
        {
            HashSet<Tuple<object, MethodInfo>> baseCalls = new HashSet<Tuple<object, MethodInfo>>();

            var body = method.GetMethodBody();
            var addresses = new VariableInfo[body.LocalVariables.Count];
            for (var i = 0; i < addresses.Length; i++)
                addresses[i] = new VariableInfo(body.LocalVariables[i].LocalType);
            var locals = addresses.ToArray();

            if (!method.IsStatic)
            {
                var concreteInstance = Processor.DiscardConversion(args[0]);
                var declaringType = method.DeclaringType;
                baseCalls.UnionWith(AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .Where(t => t.IsAssignableFrom(declaringType) && t != declaringType)
                    .OrderBy(t => t, new TypeHierarchyComparer())
                    .Select(t => GetDeclaredMethod(t, method))
                    .Where(m => m != null && m.ReflectedType == m.DeclaringType && !m.IsAbstract)
                    .Distinct()
                    .Select(m => new Tuple<object, MethodInfo>(concreteInstance, m))
                );
                foreach (var call in baseCalls)
                {
                    DecompileExtensions.ConcreteCalls[call] = true;
                }
            }

            var instructions = method.GetInstructions();
            try
            {
                return Processor.Process(locals, args, instructions.First(), method.ReturnType);
            }
            finally
            {
                foreach (var call in baseCalls)
                {
                    bool value;
                    DecompileExtensions.ConcreteCalls.TryRemove(call, out value);
                }
            }
        }

        private static Expression DecompileVirtual(Type declaringType, MethodInfo method, IList<Address> args)
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

        private static MethodInfo GetDeclaredMethod(Type type, MethodInfo method)
        {
            return type.GetMethod(method.Name,
                BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                method.GetParameters().Select(p => p.ParameterType).ToArray(),
                null);
        }
    }
}