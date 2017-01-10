using System;
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
                .Select(p => (Address)Expression.Parameter(p.ParameterType, p.Name))
                .ToList();

            if (!method.IsStatic)
            {
                args.Insert(0, Expression.Parameter(method.DeclaringType, "this"));
            }

            var body = method.GetMethodBody();
            var addresses = new VariableInfo[body.LocalVariables.Count];
            for (int i = 0; i < addresses.Length; i++)
            {
                addresses[i] = new VariableInfo(body.LocalVariables[i].LocalType);
            }
            var locals = addresses.ToArray();

            var instructions = method.GetInstructions();
            var ex = Processor.Process(locals, args, instructions.First(), method.ReturnType);
            return Expression.Lambda(new OptimizeExpressionVisitor().Visit(ex), args.Select(x => (ParameterExpression)x.Expression));
        }
    }
}
