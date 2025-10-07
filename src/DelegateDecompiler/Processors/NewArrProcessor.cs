using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors
{
    internal class NewArrProcessor : IProcessor
    {
        public static void Register(Dictionary<OpCode, IProcessor> processors)
        {
            processors.Register(new NewArrProcessor(), OpCodes.Newarr);
        }

        public void Process(ProcessorState state, Instruction instruction)
        {
            var operand = (Type)instruction.Operand;
            var expression = state.Stack.Pop();
            if (expression.Expression is ConstantExpression size && (int)size.Value == 0) // optimization
                state.Stack.Push(Expression.NewArrayInit(operand));
            else
                state.Stack.Push(Expression.NewArrayBounds(operand, expression));
        }

    }
}
