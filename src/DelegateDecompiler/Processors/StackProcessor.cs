using System;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class StackProcessor : IProcessor
{
    public bool Process(ProcessorState state, Instruction instruction)
    {
        if (instruction.OpCode == OpCodes.Dup)
        {
            state.Stack.Push(state.Stack.Peek());
            return true;
        }

        if (instruction.OpCode == OpCodes.Pop)
        {
            state.Stack.Pop();
            return true;
        }

        return false;
    }
}