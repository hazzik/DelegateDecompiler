using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class LdargProcessor : IProcessor
{
    static readonly Dictionary<OpCode, Func<ProcessorState, Instruction, int>> Operations = new()
    {
        { OpCodes.Ldarg_0, (_, _) => 0 },
        { OpCodes.Ldarg_1, (_, _) => 1 },
        { OpCodes.Ldarg_2, (_, _) => 2 },
        { OpCodes.Ldarg_3, (_, _) => 3 },
        { OpCodes.Ldarg_S, GetParameterIndex },
        { OpCodes.Ldarg, GetParameterIndex },
        { OpCodes.Ldarga, GetParameterIndex },
        { OpCodes.Ldarga_S, GetParameterIndex }
    };

    public bool Process(ProcessorState state, Instruction instruction)
    {
        if (!Operations.TryGetValue(instruction.OpCode, out var value))
            return false;

        var index = value(state, instruction);
        state.Stack.Push(state.Args[index]);
        return true;
    }

    static int GetParameterIndex(ProcessorState state, Instruction instruction)
    {
        var operand = (ParameterInfo)instruction.Operand;
        return state.IsStatic ? operand.Position : operand.Position + 1;
    }
}