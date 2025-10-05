using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors
{
    internal class StsfldProcessor : IProcessor
    {
        public static void Register(Dictionary<OpCode, IProcessor> processors)
        {
            processors.Add(OpCodes.Stsfld, new StsfldProcessor());
        }

        public void Process(ProcessorState state, Instruction instruction)
        {
            var value = state.Stack.Pop();
            var field = (FieldInfo)instruction.Operand;
            if (Processor.IsCachedAnonymousMethodDelegate(field))
            {
                state.Delegates[Tuple.Create(default(Address), field)] = value;
            }
            else
            {
                state.Stack.Push(Expression.Assign(Expression.Field(null, field), value));
            }
        }
    }
}
