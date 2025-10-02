using System;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class StackProcessor : IProcessor
{
    public bool Process(ProcessorState state)
    {
        switch (state.Instruction.OpCode)
        {
            case OpCodes.Dup:
                state.Stack.Push(state.Stack.Peek());
                return true;
            case OpCodes.Pop:
                state.Stack.Pop();
                return true;
            case OpCodes.Castclass:
                var val1 = state.Stack.Pop();
                state.Stack.Push(Expression.Convert(val1, (Type)state.Instruction.Operand));
                return true;
            default:
                return false;
        }
    }
}