using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class StlocProcessor : IProcessor
{
    static readonly Dictionary<OpCode, Func<Instruction, int>> StLocOpcodes = new()
    {
        { OpCodes.Stloc_0, _ => 0 },
        { OpCodes.Stloc_1, _ => 1 },
        { OpCodes.Stloc_2, _ => 2 },
        { OpCodes.Stloc_3, _ => 3 },
        { OpCodes.Stloc_S, FromOperand },
        { OpCodes.Stloc, FromOperand },
    };

    public bool Process(ProcessorState state)
    {
        if (!StLocOpcodes.TryGetValue(state.Instruction.OpCode, out var value))
            return false;

        StLoc(state, value(state.Instruction));
        return true;
    }

    static void StLoc(ProcessorState state, int index)
    {
        var info = state.Locals[index];
        var expression = AdjustType(state.Stack.Pop(), info.Type);
        info.Address = expression.Type == info.Type ? expression : Expression.Convert(expression, info.Type);
    }

    static int FromOperand(Instruction i)
    {
        var operand = (LocalVariableInfo)i.Operand;
        return operand.LocalIndex;
    }

    static Expression AdjustType(Expression expression, Type type)
    {
        if (expression.Type == type)
            return expression;

        if (type.IsAssignableFrom(expression.Type))
            return expression;

        return Expression.Convert(expression, type);
    }
}