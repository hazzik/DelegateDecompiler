using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class BoxProcessor : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Register(new BoxProcessor(), OpCodes.Box);
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        state.Stack.Push(Box(state.Stack.Pop(), (Type)instruction.Operand));
    }

    static Expression Box(Expression expression, Type type)
    {
        if (expression.Type == type)
            return expression;

        // Required for correctness: convert constant to enum
        if (expression is ConstantExpression constantExpression)
        {
            if (type.IsEnum)
                return Expression.Constant(Enum.ToObject(type, constantExpression.Value));
        }

        if (expression.Type.IsEnum)
            return Expression.Convert(expression, type);

        return expression;
    }
}