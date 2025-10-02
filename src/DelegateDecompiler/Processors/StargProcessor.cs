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
    static readonly Dictionary<OpCode, Func<Instruction, int>> Operations = new()
    {
        { OpCodes.Starg_S, FromOperand },
        { OpCodes.Starg, FromOperand },
    };

    public bool Process(ProcessorState state)
    {
        if (!Operations.TryGetValue(state.Instruction.OpCode, out var value))
            return false;

        StArg(state, value(state.Instruction));
        return true;
    }

    static void StArg(ProcessorState state, int index)
    {
        var arg = state.Args[index];
        var expression = Processor.AdjustType(state.Stack.Pop(), arg.Type);
        arg.Expression = expression.Type == arg.Type ? expression : Expression.Convert(expression, arg.Type);
    }

    static int FromOperand(Instruction instruction)
    {
        var operand = (ParameterInfo)instruction.Operand;
        // For Starg operations, we need to find the parameter by its position or name
        return operand.Position;
    }
}