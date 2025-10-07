using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class StlocVariableProcessor : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Register(new StlocVariableProcessor(), OpCodes.Stloc_S, OpCodes.Stloc);
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        var index = GetLocalVariableIndex(instruction);
        var info = state.Locals[index];
        var expression = Processor.AdjustType(state.Stack.Pop(), info.Type);
        info.Address = expression.Type == info.Type ? expression : Expression.Convert(expression, info.Type);
    }

    static int GetLocalVariableIndex(Instruction instruction)
    {
        return ((LocalVariableInfo)instruction.Operand).LocalIndex;
    }
}