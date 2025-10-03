using System;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class UnsupportedOpcodeProcessor : IProcessor
{
    public bool Process(ProcessorState state) =>
        throw new NotSupportedException(
            $"The IL opcode '{state.Instruction.OpCode}' is not supported by DelegateDecompiler. " +
            $"This opcode cannot be decompiled into an expression tree. " +
            $"Consider simplifying the method or using a different approach.");
}