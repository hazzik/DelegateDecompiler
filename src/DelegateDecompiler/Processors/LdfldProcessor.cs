using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class LdfldProcessor(bool isStatic) : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Add(OpCodes.Ldsfld, new LdfldProcessor(true));
        processors.Add(OpCodes.Ldfld, new LdfldProcessor(false));
        processors.Add(OpCodes.Ldflda, new LdfldProcessor(false));
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        var field = (FieldInfo)instruction.Operand;
        
        if (isStatic)
        {
            // Static field
            state.Stack.Push(Expression.Field(null, field));
        }
        else
        {
            // Instance field
            var instance = state.Stack.Pop();
            LdFld(state, instruction, instance);
        }
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