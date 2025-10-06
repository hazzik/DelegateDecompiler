using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class StargProcessor : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Register(new StargProcessor(), OpCodes.Starg_S, OpCodes.Starg);
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        var index = GetParameterIndex(state, instruction);
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