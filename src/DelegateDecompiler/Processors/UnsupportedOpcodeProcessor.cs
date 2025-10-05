using System;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class UnsupportedOpcodeProcessor : IProcessor
{
    public bool Process(ProcessorState state, Instruction instruction) =>
        throw new NotSupportedException(
            $"The IL opcode '{instruction.OpCode}' is not supported by DelegateDecompiler. " +
            $"This opcode cannot be decompiled into an expression tree. " +
            $"Consider simplifying the method or using a different approach.");
}