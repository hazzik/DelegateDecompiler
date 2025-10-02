using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class UnaryProcessor : IProcessor
{
    static readonly Dictionary<OpCode, ExpressionType> Operations = new Dictionary<OpCode, ExpressionType>
    {
        {OpCodes.Neg, ExpressionType.Negate},
        {OpCodes.Not, ExpressionType.Not}
    };

    public bool Process(ProcessorState state)
    {
        ExpressionType operation;
        if (Operations.TryGetValue(state.Instruction.OpCode, out operation))
        {
            var val = state.Stack.Pop();
            state.Stack.Push(Processor.MakeUnaryExpression(val, operation));
            return true;
        }

        return false;
    }
}