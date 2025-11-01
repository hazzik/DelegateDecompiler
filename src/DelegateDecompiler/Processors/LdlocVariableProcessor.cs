using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class LdlocVariableProcessor : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        var processor = new LdlocVariableProcessor();
        processors.Register(processor, OpCodes.Ldloc, OpCodes.Ldloc_S, OpCodes.Ldloca, OpCodes.Ldloca_S);
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        var index = GetLocalVariableIndex(instruction);
        var local = state.Locals[index];
        state.Stack.Push(local.Address);
    }

    static int GetLocalVariableIndex(Instruction instruction)
    {
        return ((LocalVariableInfo)instruction.Operand).LocalIndex;
    }
}