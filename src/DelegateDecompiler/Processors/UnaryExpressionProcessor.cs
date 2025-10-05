using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class UnaryExpressionProcessor(ExpressionType expressionType) : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Add(OpCodes.Neg, new UnaryExpressionProcessor(ExpressionType.Negate));
        processors.Add(OpCodes.Not, new UnaryExpressionProcessor(ExpressionType.Not));
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        var val = state.Stack.Pop();
        state.Stack.Push(Processor.MakeUnaryExpression(val, expressionType));
    }
}