using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class BrfalseProcessor : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Register(new BrfalseProcessor(), OpCodes.Brfalse, OpCodes.Brfalse_S);
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        var val = state.Stack.Pop();
        var condition = Expression.Equal(val, ExpressionHelper.Default(val.Type));
        state.Stack.Push(condition);
    }
}