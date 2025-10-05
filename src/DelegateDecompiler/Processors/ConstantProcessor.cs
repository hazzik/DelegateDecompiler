using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class ConstantOperandProcessor : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        var processor = new ConstantOperandProcessor();
        processors.Add(OpCodes.Ldc_I4, processor);
        processors.Add(OpCodes.Ldc_I8, processor);
        processors.Add(OpCodes.Ldc_R4, processor);
        processors.Add(OpCodes.Ldc_R8, processor);
        processors.Add(OpCodes.Ldstr, processor);
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        var value = instruction.Operand;
        state.Stack.Push(Expression.Constant(value));
    }
}