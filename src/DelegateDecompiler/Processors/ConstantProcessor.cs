using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class ConstantOperandProcessor : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Register(new ConstantOperandProcessor(),
            OpCodes.Ldc_I4,
            OpCodes.Ldc_I8,
            OpCodes.Ldc_R4,
            OpCodes.Ldc_R8,
            OpCodes.Ldstr
        );
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        var value = instruction.Operand;
        state.Stack.Push(Expression.Constant(value));
    }
}