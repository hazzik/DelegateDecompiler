using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class LdlocProcessor : IProcessor
{
    static readonly Dictionary<OpCode, Func<ProcessorState, int>> Operations = new()
    {
        { OpCodes.Ldloc_0, _ => 0 },
        { OpCodes.Ldloc_1, _ => 1 },
        { OpCodes.Ldloc_2, _ => 2 },
        { OpCodes.Ldloc_3, _ => 3 },
        { OpCodes.Ldloc, state => ((LocalVariableInfo)state.Instruction.Operand).LocalIndex },
        { OpCodes.Ldloc_S, state => ((LocalVariableInfo)state.Instruction.Operand).LocalIndex },
        { OpCodes.Ldloca, state => ((LocalVariableInfo)state.Instruction.Operand).LocalIndex },
        { OpCodes.Ldloca_S, state => ((LocalVariableInfo)state.Instruction.Operand).LocalIndex }
    };

    public bool Process(ProcessorState state)
    {
        if (Operations.TryGetValue(state.Instruction.OpCode, out var getIndex))
        {
            LdLoc(state, getIndex(state));
            return true;
        }

        return false;
    }

    static void LdLoc(ProcessorState state, int index)
    {
        var local = state.Locals[index];
        state.Stack.Push(local.Address);
    }
}