using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler
{
    internal class BitwiseProcessor : IProcessor
    {
        static readonly Dictionary<OpCode, ExpressionType> Operations = new Dictionary<OpCode, ExpressionType>
        {
            {OpCodes.And, ExpressionType.And},
            {OpCodes.Or, ExpressionType.Or},
            {OpCodes.Xor, ExpressionType.ExclusiveOr},
            {OpCodes.Shl, ExpressionType.LeftShift},
            {OpCodes.Shr, ExpressionType.RightShift},
            {OpCodes.Shr_Un, ExpressionType.RightShift},
        };

        public bool Process(ProcessorState state)
        {
            ExpressionType operation;
            if (Operations.TryGetValue(state.Instruction.OpCode, out operation))
            {
                var val1 = state.Stack.Pop();
                var val2 = state.Stack.Pop();
                state.Stack.Push(Processor.MakeBinaryExpression(val2, val1, operation));
                return true;
            }

            return false;
        }
    }
}