using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler
{
    class NopInstructionProcessor : IInstructionProcessor
    {
        public bool Process(Instruction instruction, Stack<Expression> stack)
        {
            return instruction.OpCode == OpCodes.Nop;
        }
    }
}