using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors
{
    internal class IsinstProcessor : IProcessor
    {
        public static void Register(Dictionary<OpCode, IProcessor> processors)
        {
            processors.Register(new IsinstProcessor(), OpCodes.Isinst);
        }
    
        public void Process(ProcessorState state, Instruction instruction)
        {
            var val = state.Stack.Pop();
            state.Stack.Push(Expression.TypeAs(val, (Type)instruction.Operand));
        }
    }
}