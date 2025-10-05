using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class StlocProcessor : IProcessor
{
    static readonly Dictionary<OpCode, Func<Instruction, int>> Operations = new()
    {
        { OpCodes.Stloc_0, _ => 0 },
        { OpCodes.Stloc_1, _ => 1 },
        { OpCodes.Stloc_2, _ => 2 },
        { OpCodes.Stloc_3, _ => 3 },
        { OpCodes.Stloc_S, FromOperand },
        { OpCodes.Stloc, FromOperand },
    };

    public bool Process(ProcessorState state, Instruction instruction)
    {
        if (!Operations.TryGetValue(instruction.OpCode, out var value))
            return false;

        StLoc(state, value(instruction));
        return true;
    }

    static void StLoc(ProcessorState state, int index)
    {
        var info = state.Locals[index];
        var expression = Processor.AdjustType(state.Stack.Pop(), info.Type);
        info.Address = expression.Type == info.Type ? expression : Expression.Convert(expression, info.Type);
    }

    static int FromOperand(Instruction i)
    {
        return ((LocalVariableInfo)i.Operand).LocalIndex;
    }
}