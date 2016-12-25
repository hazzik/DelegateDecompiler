using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Mono.Reflection;

namespace DelegateDecompiler
{
    public class MethodBodyDecompiler
    {
        readonly IList<Address> args;
        readonly VariableInfo[] locals;
        readonly MethodInfo method;

        public MethodBodyDecompiler(MethodInfo method)
        {
            this.method = method;
            var parameters = method.GetParameters();
            if (method.IsStatic)
                args = parameters
                    .Select(p => (Address) Expression.Parameter(p.ParameterType, p.Name))
                    .ToList();
            else if (method.IsAnonymous())
                args = new[] {(Address) Expression.Constant(null, method.DeclaringType)}
                    .Union(parameters.Select(p => (Address)Expression.Parameter(p.ParameterType, p.Name)))
                    .ToList();
            else
                args = new[] {(Address) Expression.Parameter(method.DeclaringType, "this")}
                    .Union(parameters.Select(p => (Address) Expression.Parameter(p.ParameterType, p.Name)))
                    .ToList();

            var body = method.GetMethodBody();
            var addresses = new VariableInfo[body.LocalVariables.Count];
            for (int i = 0; i < addresses.Length; i++)
            {
                addresses[i] = new VariableInfo(body.LocalVariables[i].LocalType);
            }
            locals = addresses.ToArray();
        }

        public LambdaExpression Decompile()
        {
            var instructions = method.GetInstructions();
            var ex = Processor.Process(locals, args, instructions.First(), method.ReturnType);
            var parameters = args
                .Where(x => x.Expression is ParameterExpression)
                .Select(x => (ParameterExpression) x.Expression);
            return Expression.Lambda(new OptimizeExpressionVisitor().Visit(ex), parameters);
        }
    }
}
