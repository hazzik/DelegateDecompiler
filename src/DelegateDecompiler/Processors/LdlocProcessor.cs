using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

// Processor for constant local variable indices (ldloc.0, ldloc.1, etc.)
internal class LdlocConstantProcessor(int index) : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Add(OpCodes.Ldloc_0, new LdlocConstantProcessor(0));
        processors.Add(OpCodes.Ldloc_1, new LdlocConstantProcessor(1));
        processors.Add(OpCodes.Ldloc_2, new LdlocConstantProcessor(2));
        processors.Add(OpCodes.Ldloc_3, new LdlocConstantProcessor(3));
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        var local = state.Locals[index];
        state.Stack.Push(local.Address);
    }
}

// Processor for dynamic local variable index resolution (ldloc, ldloc.s, ldloca, ldloca.s)
internal class LdlocVariableProcessor : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        var processor = new LdlocVariableProcessor();
        processors.Add(OpCodes.Ldloc, processor);
        processors.Add(OpCodes.Ldloc_S, processor);
        processors.Add(OpCodes.Ldloca, processor);
        processors.Add(OpCodes.Ldloca_S, processor);
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        var index = GetLocalVariableIndex(instruction);
        var local = state.Locals[index];
        state.Stack.Push(local.Address);
    }

    static int GetLocalVariableIndex(Instruction instruction)
    {
        return ((LocalVariableInfo)instruction.Operand).LocalIndex;
    }
}