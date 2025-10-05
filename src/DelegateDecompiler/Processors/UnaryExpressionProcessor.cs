using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class UnaryExpressionProcessor : IProcessor
{
    static readonly Dictionary<OpCode, ExpressionType> Operations = new()
    {
        { OpCodes.Neg, ExpressionType.Negate },
        { OpCodes.Not, ExpressionType.Not }
    };

    public bool Process(ProcessorState state, Instruction instruction)
    {
        if (!Operations.TryGetValue(instruction.OpCode, out var operation))
            return false;

        var val = state.Stack.Pop();
        state.Stack.Push(Processor.MakeUnaryExpression(val, operation));
        return true;
    }
}