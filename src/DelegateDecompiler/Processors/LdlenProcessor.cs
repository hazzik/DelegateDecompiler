using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class LdlenProcessor : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Register(new LdlenProcessor(), OpCodes.Ldlen);
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        var array = state.Stack.Pop();
        state.Stack.Push(Expression.ArrayLength(array));
    }
}