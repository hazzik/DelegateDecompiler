using System;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class StfldProcessor : IProcessor
{
    public bool Process(ProcessorState state, Instruction instruction)
    {
        if (instruction.OpCode != OpCodes.Stfld)
            return false;

        var value = state.Stack.Pop();
        var instance = state.Stack.Pop();
        var field = (FieldInfo)instruction.Operand;
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