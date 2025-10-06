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
        processors.Register(new LdlocConstantProcessor(0), OpCodes.Ldloc_0);
        processors.Register(new LdlocConstantProcessor(1), OpCodes.Ldloc_1);
        processors.Register(new LdlocConstantProcessor(2), OpCodes.Ldloc_2);
        processors.Register(new LdlocConstantProcessor(3), OpCodes.Ldloc_3);
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
        processors.Register(processor, OpCodes.Ldloc, OpCodes.Ldloc_S, OpCodes.Ldloca, OpCodes.Ldloca_S);
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