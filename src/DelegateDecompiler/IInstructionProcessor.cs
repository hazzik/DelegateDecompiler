using System.Collections.Generic;
using System.Linq.Expressions;
using Mono.Reflection;

namespace DelegateDecompiler
{
    internal interface IInstructionProcessor
    {
        bool Process(Instruction instruction, Stack<Expression> stack);
    }
}