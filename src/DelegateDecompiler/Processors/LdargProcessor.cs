using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class LdargProcessor : IProcessor
{
    static readonly Dictionary<OpCode, Func<ProcessorState, int>> Operations = new()
    {
        { OpCodes.Ldarg_0, _ => 0 },
        { OpCodes.Ldarg_1, _ => 1 },
        { OpCodes.Ldarg_2, _ => 2 },
        { OpCodes.Ldarg_3, _ => 3 },
        { OpCodes.Ldarg_S, GetParameterIndex },
        { OpCodes.Ldarg, GetParameterIndex },
        { OpCodes.Ldarga, GetParameterIndex },
        { OpCodes.Ldarga_S, GetParameterIndex }
    };

    public bool Process(ProcessorState state)
    {
        if (!Operations.TryGetValue(state.Instruction.OpCode, out var value))
            return false;

        var index = value(state);
        state.Stack.Push(state.Args[index]);
        return true;
    }

    static int GetParameterIndex(ProcessorState state)
    {
        var operand = (ParameterInfo)state.Instruction.Operand;
        return state.IsStatic ? operand.Position : operand.Position + 1;
    }
}