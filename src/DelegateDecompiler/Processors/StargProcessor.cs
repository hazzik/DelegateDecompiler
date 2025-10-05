using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class StargProcessor : IProcessor
{
    static readonly Dictionary<OpCode, Func<ProcessorState, Instruction, int>> Operations = new()
    {
        { OpCodes.Starg_S, GetParameterIndex },
        { OpCodes.Starg, GetParameterIndex },
    };

    public bool Process(ProcessorState state, Instruction instruction)
    {
        if (!Operations.TryGetValue(instruction.OpCode, out var value))
            return false;

        StArg(state, value(state, instruction));
        return true;
    }

    static void StArg(ProcessorState state, int index)
    {
        var arg = state.Args[index];
        var expression = Processor.AdjustType(state.Stack.Pop(), arg.Type);
        arg.Expression = expression.Type == arg.Type ? expression : Expression.Convert(expression, arg.Type);
    }

    static int GetParameterIndex(ProcessorState state, Instruction instruction)
    {
        var operand = (ParameterInfo)instruction.Operand;
        return state.IsStatic ? operand.Position : operand.Position + 1;
    }
}