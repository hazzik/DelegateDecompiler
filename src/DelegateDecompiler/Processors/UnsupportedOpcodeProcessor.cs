using System;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class UnsupportedOpcodeProcessor : IProcessor
{
    public bool Process(ProcessorState state)
    {
        // This processor handles all opcodes that are not supported by other processors
        // by throwing a descriptive NotSupportedException
        var opCode = state?.Instruction?.OpCode ?? OpCodes.Nop; // Default for testing
        
        throw new NotSupportedException($"The IL opcode '{opCode}' is not supported by DelegateDecompiler. " +
                                      $"This opcode cannot be decompiled into an expression tree. " +
                                      $"Consider simplifying the method or using a different approach.");
    }
}