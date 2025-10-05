using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class LdargParameterProcessor : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        var processor = new LdargParameterProcessor();
        processors.Add(OpCodes.Ldarg_S, processor);
        processors.Add(OpCodes.Ldarg, processor);
        processors.Add(OpCodes.Ldarga, processor);
        processors.Add(OpCodes.Ldarga_S, processor);
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