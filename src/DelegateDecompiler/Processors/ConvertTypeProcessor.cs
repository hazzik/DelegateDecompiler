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
            processors.Register(new ConvertTypeProcessor(), OpCodes.Castclass, OpCodes.Unbox, OpCodes.Unbox_Any);
        }
    
        public void Process(ProcessorState state, Instruction instruction)
        {
            var expression = state.Stack.Pop();
            state.Stack.Push(Expression.Convert(expression, (Type)instruction.Operand));
        }
    }
}
