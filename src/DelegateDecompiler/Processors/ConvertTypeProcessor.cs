using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors
{
    internal class ConvertTypeProcessor : IProcessor
    {
        public static void Register(Dictionary<OpCode, IProcessor> processors)
        {
            processors.Register(new ConvertTypeProcessor(), 
                OpCodes.Castclass, 
                OpCodes.Unbox,
                OpCodes.Unbox_Any,
                OpCodes.Constrained);
        }

        public void Process(ProcessorState state, Instruction instruction)
        {
            var expression = state.Stack.Pop();
            var targetType = (Type)instruction.Operand;
            state.Stack.Push(expression.Type != targetType
                ? Expression.Convert(expression, targetType)
                : expression);
        }
    }
}
