using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class LdfldProcessor : IProcessor
{
    public bool Process(ProcessorState state, Instruction instruction)
    {
        var field = (FieldInfo)instruction.Operand;
        
        if (instruction.OpCode == OpCodes.Ldsfld)
        {
            // Static field
            state.Stack.Push(Expression.Field(null, field));
            return true;
        }

        if (instruction.OpCode == OpCodes.Ldfld || instruction.OpCode == OpCodes.Ldflda)
        {
            // Instance field
            var instance = state.Stack.Pop();
            LdFld(state, instruction, instance);
            return true;
        }

        return false;
    }

    static void LdFld(ProcessorState state, Instruction instruction, Address instance)
    {
        var field = (FieldInfo)instruction.Operand;
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