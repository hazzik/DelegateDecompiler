using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler
{
    internal class UnaryProcessor : IProcessor
    {
        static readonly HashSet<OpCode> NegationOpcodes = new HashSet<OpCode>
        {
            OpCodes.Neg
        };

        static readonly HashSet<OpCode> NotOpcodes = new HashSet<OpCode>
        {
            OpCodes.Not
        };

        public bool Process(ProcessorState state)
        {
            if (NegationOpcodes.Contains(state.Instruction.OpCode))
            {
                var val = state.Stack.Pop();
                state.Stack.Push(Expression.Negate(val));
                return true;
            }

            if (NotOpcodes.Contains(state.Instruction.OpCode))
            {
                var val = state.Stack.Pop();
                state.Stack.Push(Processor.MakeUnaryExpression(val, ExpressionType.Not));
                return true;
            }

            return false;
        }
    }
}