using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class ConstantProcessor : IProcessor
{
    static readonly Dictionary<OpCode, Func<Instruction, object>> SimpleIntegerValues = new()
    {
        { OpCodes.Ldc_I4_0, _ => 0 },
        { OpCodes.Ldc_I4_1, _ => 1 },
        { OpCodes.Ldc_I4_2, _ => 2 },
        { OpCodes.Ldc_I4_3, _ => 3 },
        { OpCodes.Ldc_I4_4, _ => 4 },
        { OpCodes.Ldc_I4_5, _ => 5 },
        { OpCodes.Ldc_I4_6, _ => 6 },
        { OpCodes.Ldc_I4_7, _ => 7 },
        { OpCodes.Ldc_I4_8, _ => 8 },
        { OpCodes.Ldc_I4_M1, _ => -1 },
        { OpCodes.Ldc_I4_S, i => (int)(sbyte)i.Operand },
        { OpCodes.Ldc_I4, i => i.Operand },
        { OpCodes.Ldc_I8, i => i.Operand },
        { OpCodes.Ldc_R4, i => i.Operand },
        { OpCodes.Ldc_R8, i => i.Operand },
        { OpCodes.Ldstr, i => i.Operand },
        { OpCodes.Ldnull, i => null },
    };

    public bool Process(ProcessorState state)
    {
        if (!SimpleIntegerValues.TryGetValue(state.Instruction.OpCode, out var value))
            return false;

        state.Stack.Push(Expression.Constant(value(state.Instruction)));
        return true;
    }
}