using System.Collections.Generic;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class PopProcessor : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Add(OpCodes.Pop, new PopProcessor());
    }
    
    public void Process(ProcessorState state, Instruction instruction)
    {
        state.Stack.Pop();
    }
}