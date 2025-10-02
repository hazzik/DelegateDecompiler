using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class LdfldProcessor : IProcessor
{
    public bool Process(ProcessorState state)
    {
        var field = (FieldInfo)state.Instruction.Operand;
        
        if (state.Instruction.OpCode == OpCodes.Ldsfld)
        {
            // Static field
            state.Stack.Push(Expression.Field(null, field));
            return true;
        }

        if (state.Instruction.OpCode == OpCodes.Ldfld || state.Instruction.OpCode == OpCodes.Ldflda)
        {
            // Instance field
            var instance = state.Stack.Pop();
            LdFld(state, instance);
            return true;
        }

        return false;
    }

    static void LdFld(ProcessorState state, Address instance)
    {
        var field = (FieldInfo)state.Instruction.Operand;
        if (Processor.IsCachedAnonymousMethodDelegate(field) &&
            state.Delegates.TryGetValue(Tuple.Create(instance, field), out var address))
        {
            state.Stack.Push(address);
        }
        else
        {
            state.Stack.Push(Expression.Field(instance?.Expression, field));
        }
    }
}