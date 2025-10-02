using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class LdfldProcessor : IProcessor
{
    static readonly HashSet<OpCode> LdfldOpcodes = new()
    {
        OpCodes.Ldfld,
        OpCodes.Ldflda,
        OpCodes.Ldsfld
    };

    public bool Process(ProcessorState state)
    {
        if (!LdfldOpcodes.Contains(state.Instruction.OpCode))
            return false;

        var field = (FieldInfo)state.Instruction.Operand;
            
        if (state.Instruction.OpCode == OpCodes.Ldsfld)
        {
            // Static field
            state.Stack.Push(Expression.Field(null, field));
        }
        else
        {
            // Instance field
            var instance = state.Stack.Pop();
            LdFld(state, instance);
        }
        return true;

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