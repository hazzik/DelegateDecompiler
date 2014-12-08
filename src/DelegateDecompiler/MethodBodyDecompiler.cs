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
        readonly IList<ParameterExpression> args;
        readonly Expression[] locals;
        readonly MethodInfo method;

        public MethodBodyDecompiler(MethodInfo method)
        {
            locals = new Expression[0];
            this.method = method;
            var parameters = method.GetParameters();
            if (method.IsStatic)
                args = parameters
                    .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                    .ToList();
            else
                args = new[] { Expression.Parameter(method.DeclaringType, "this") }
                    .Union(parameters.Select(p => Expression.Parameter(p.ParameterType, p.Name)))
                    .ToList();

            var body = method.GetMethodBody();
            locals = new Expression[body.LocalVariables.Count];
        }

        public LambdaExpression Decompile()
        {
            var instructions = method.GetInstructions();
            var ex = Processor.Create(locals, args).Process(instructions.First(), method.ReturnType);
            LambdaExpression lambda = Expression.Lambda(ex, args);
            lambda = (LambdaExpression)new NewNullableExpressionVisitor().Visit(lambda);
            return lambda;
        }
    }
}
