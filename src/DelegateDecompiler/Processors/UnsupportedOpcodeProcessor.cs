using System;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class UnsupportedOpcodeProcessor : IProcessor
{
    public bool Process(ProcessorState state)
    {
        // This processor handles all opcodes that are not supported by other processors
        // by throwing a descriptive NotSupportedException
        if (state?.Instruction == null)
        {
            throw new NotSupportedException("Cannot process null instruction. " +
                                          "This indicates an internal error in the decompilation process.");
        }
        
        var opCode = state.Instruction.OpCode;
        
        throw new NotSupportedException($"The IL opcode '{opCode}' is not supported by DelegateDecompiler. " +
                                      $"This opcode cannot be decompiled into an expression tree. " +
                                      $"Consider simplifying the method or using a different approach.");
    }
}