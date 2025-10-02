using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class LdlocProcessor : IProcessor
{
    static readonly Dictionary<OpCode, Func<Instruction, int>> Operations = new()
    {
        { OpCodes.Ldloc_0, _ => 0 },
        { OpCodes.Ldloc_1, _ => 1 },
        { OpCodes.Ldloc_2, _ => 2 },
        { OpCodes.Ldloc_3, _ => 3 },
        { OpCodes.Ldloc, LocalIndex },
        { OpCodes.Ldloc_S, LocalIndex },
        { OpCodes.Ldloca, LocalIndex },
        { OpCodes.Ldloca_S, LocalIndex }
    };

    static int LocalIndex(Instruction i)
    {
        return ((LocalVariableInfo)i.Operand).LocalIndex;
    }

    public bool Process(ProcessorState state)
    {
        if (!Operations.TryGetValue(state.Instruction.OpCode, out var value))
            return false;

        var index = value(state.Instruction);
        var local = state.Locals[index];
        state.Stack.Push(local.Address);
        return true;
    }
}