using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class BoxProcessor : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Add(OpCodes.Box, new BoxProcessor());
    }
    
    public void Process(ProcessorState state, Instruction instruction)
    {
        state.Stack.Push(Processor.Box(state.Stack.Pop(), (Type)instruction.Operand));
    }
}