using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class LdargParameterProcessor : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Register(new LdargParameterProcessor(),
            OpCodes.Ldarg_S, OpCodes.Ldarg, OpCodes.Ldarga, OpCodes.Ldarga_S);
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        var index = GetParameterIndex(state, instruction);
        state.Stack.Push(state.Args[index]);
    }

    static int GetParameterIndex(ProcessorState state, Instruction instruction)
    {
        var operand = (ParameterInfo)instruction.Operand;
        return state.IsStatic ? operand.Position : operand.Position + 1;
    }
}