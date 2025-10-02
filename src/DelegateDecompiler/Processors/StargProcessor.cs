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
    static readonly Dictionary<OpCode, Func<ProcessorState, int>> Operations = new()
    {
        { OpCodes.Starg_S, GetParameterIndex },
        { OpCodes.Starg, GetParameterIndex },
    };

    public bool Process(ProcessorState state)
    {
        if (!Operations.TryGetValue(state.Instruction.OpCode, out var value))
            return false;

        StArg(state, value(state));
        return true;
    }

    static void StArg(ProcessorState state, int index)
    {
        var arg = state.Args[index];
        var expression = Processor.AdjustType(state.Stack.Pop(), arg.Type);
        
        // When a parameter is modified (Starg), we need to handle it differently than locals
        // since parameters in expression trees are immutable. We update the argument's address
        // to point to the modified value, allowing subsequent loads to get the new value.
        arg.Expression = expression.Type == arg.Type ? expression : Expression.Convert(expression, arg.Type);
    }

    static int GetParameterIndex(ProcessorState state)
    {
        var operand = (ParameterInfo)state.Instruction.Operand;
        
        // For instance methods, parameters are offset by 1 due to the "this" parameter
        return !state.IsStatic ? operand.Position + 1 : operand.Position;
    }
}