using System;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class StackProcessor : IProcessor
{
    public bool Process(ProcessorState state)
    {
        if (state.Instruction.OpCode == OpCodes.Dup)
        {
            state.Stack.Push(state.Stack.Peek());
            return true;
        }

        if (state.Instruction.OpCode == OpCodes.Pop)
        {
            state.Stack.Pop();
            return true;
        }

        return false;
    }
}