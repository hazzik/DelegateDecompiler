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

        static Expression DecompileConcrete(
            MethodInfo method,
            IList<Address> args,
            IDictionary<MethodInfo, Expression> cache)
        {
            var result = DecompileConcrete(method, args);
            cache[method] = result;
            return result;
        }

        static Expression DecompileVirtual(Type declaringType, MethodInfo method, IList<Address> args)
        {
            if (declaringType == null)
                throw new InvalidOperationException($"Method {method.Name} does not have a declaring type");

            var baseCalls = new Dictionary<MethodInfo, Expression>();
            
            var @this = args[0].Expression;

            var result = GetDefaultImplementation(declaringType, method, args, baseCalls);
 
            var descendants = AppDomain.CurrentDomain.GetAssemblies()
                .Where(a => !a.IsDynamic)
                .SelectMany(a => SafeGetTypes(a))
                .Where(t => declaringType.IsAssignableFrom(t) && t != declaringType);

            var sorted = TypeHierarchy.Traverse(declaringType, descendants);

            foreach (var type in sorted)
            {
                var declaredMethod = GetDeclaredMethod(type, method);
                if (declaredMethod != null && !declaredMethod.IsAbstract)
                {
                    var localArgs = args.ToList();
                    localArgs[0] = Expression.Convert(@this, type);

                    var childExpression = DecompileConcrete(declaredMethod, localArgs, baseCalls);

                    result = Expression.Condition(Expression.TypeIs(@this, type), childExpression, result);
                }
            }

            return new ReplaceMethodCallsExpressionVisitor(baseCalls).Visit(result);
        }
        
        static IEnumerable<Type> SafeGetTypes(Assembly a)
        {
            try
            {
                return a.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types;
            }
        }

        static Expression GetDefaultImplementation(
            Type declaringType, 
            MethodInfo method, 
            IList<Address> args,
            IDictionary<MethodInfo, Expression> calls)
        {
            for (var type = declaringType; type != null && type != typeof(object); type = type.BaseType)
            {
                var declaredMethod = GetDeclaredMethod(type, method);
                if (declaredMethod != null && !declaredMethod.IsAbstract)
                {
                    return DecompileConcrete(declaredMethod, args, calls);
                }
            }

            return ExpressionHelper.Default(method.ReturnType);
        }

        static MethodInfo GetDeclaredMethod(Type type, MethodInfo method)
        {
            return type.GetMethod(method.Name,
                BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                Array.ConvertAll(method.GetParameters(), p => p.ParameterType),
                null);
        }

        class ReplaceMethodCallsExpressionVisitor : ExpressionVisitor
        {
            readonly IDictionary<MethodInfo, Expression> replacements;

            public ReplaceMethodCallsExpressionVisitor(IDictionary<MethodInfo, Expression> replacements)
            {
                this.replacements = replacements;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (replacements.TryGetValue(node.Method, out var replacement))
                {
                    return Visit(replacement);
                }

                return base.VisitMethodCall(node);
            }
            
            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Member is PropertyInfo property &&
                    replacements.TryGetValue(property.GetGetMethod(), out var replacement))
                {
                    return Visit(replacement);
                }

                return base.VisitMember(node);
            }
        }
    }
}
