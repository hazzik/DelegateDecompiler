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
            var args = method.GetParameters()
                .Select(p => (Address) Expression.Parameter(p.ParameterType, p.Name))
                .ToList();

            if (!method.IsStatic)
                args.Insert(0, Expression.Parameter(method.DeclaringType, "this"));

            var expression = method.IsVirtual
                ? DecompileAbstract(method.GetBaseDefinition(), args)
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

        static Expression DecompileAbstract(MethodInfo method, IList<Address> args)
        {
            var declaringType = method.DeclaringType ??
                                throw new InvalidOperationException($"Method {method.Name} does not have a declaring type");
           
            var defaultValueExpression = ExpressionHelper.Default(method.ReturnType);

            var @this = args[0].Expression;

            var result = defaultValueExpression;

            var childrenTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => t.IsSubclassOf(declaringType))
                .OrderBy(t => t, new TypeHierarchyComparer());

            //Create a global is-as expression for all children types;
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

        static MethodInfo GetDeclaredMethod(Type type, MethodInfo method)
        {
            return type.GetMethod(method.Name,
                BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        }
    }
}