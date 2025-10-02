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
            default:
                return false;
        }
    }
}