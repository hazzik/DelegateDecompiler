using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class StfldProcessor : IProcessor
{
    static readonly HashSet<OpCode> Operations = new()
    {
        OpCodes.Stfld,
        OpCodes.Stsfld
    };

    public bool Process(ProcessorState state)
    {
        if (!Operations.Contains(state.Instruction.OpCode))
            return false;

        var value = state.Stack.Pop();
        var instance = state.Stack.Pop();
        var field = (FieldInfo)state.Instruction.Operand;
        if (Processor.IsCachedAnonymousMethodDelegate(field))
        {
            state.Delegates[Tuple.Create(instance, field)] = value;
        }
        else
        {
            var expression = Processor.BuildAssignment(instance.Expression, field, value, out var push);
            if (push)
                state.Stack.Push(expression);
            else
                instance.Expression = expression;
        }

        return true;
    }
}