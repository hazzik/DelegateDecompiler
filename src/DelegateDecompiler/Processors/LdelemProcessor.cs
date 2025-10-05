using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class LdelemProcessor : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        var processor = new LdelemProcessor();
        processors.Add(OpCodes.Ldelem, processor);
        processors.Add(OpCodes.Ldelem_I, processor);
        processors.Add(OpCodes.Ldelem_I1, processor);
        processors.Add(OpCodes.Ldelem_I2, processor);
        processors.Add(OpCodes.Ldelem_I4, processor);
        processors.Add(OpCodes.Ldelem_I8, processor);
        processors.Add(OpCodes.Ldelem_U1, processor);
        processors.Add(OpCodes.Ldelem_U2, processor);
        processors.Add(OpCodes.Ldelem_U4, processor);
        processors.Add(OpCodes.Ldelem_R4, processor);
        processors.Add(OpCodes.Ldelem_R8, processor);
        processors.Add(OpCodes.Ldelem_Ref, processor);
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        var index = state.Stack.Pop();
        var array = state.Stack.Pop();
        state.Stack.Push(Expression.ArrayIndex(array, index));
    }
}