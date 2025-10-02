using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler
{
    internal class ComparisonProcessor : IProcessor
    {
        static readonly Dictionary<OpCode, ExpressionType> SimpleComparisons = new Dictionary<OpCode, ExpressionType>
        {
            {OpCodes.Ceq, ExpressionType.Equal},
            {OpCodes.Cgt, ExpressionType.GreaterThan},
            {OpCodes.Clt, ExpressionType.LessThan},
            {OpCodes.Clt_Un, ExpressionType.LessThan},
        };

        public bool Process(ProcessorState state)
        {
            ExpressionType comparison;
            if (SimpleComparisons.TryGetValue(state.Instruction.OpCode, out comparison))
            {
                var val1 = state.Stack.Pop();
                var val2 = state.Stack.Pop();
                state.Stack.Push(Processor.MakeBinaryExpression(val2, val1, comparison));
                return true;
            }

            // Special handling for Cgt_Un which has special logic for null/zero comparison
            if (state.Instruction.OpCode == OpCodes.Cgt_Un)
            {
                var val1 = state.Stack.Pop();
                var val2 = state.Stack.Pop();

                var constantExpression = val1.Expression as ConstantExpression;
                if (constantExpression != null && (constantExpression.Value as int? == 0 || constantExpression.Value == null))
                {
                    // Special case for comparison with null/zero
                    state.Stack.Push(Processor.MakeBinaryExpression(val2, val1, ExpressionType.NotEqual));
                }
                else
                {
                    state.Stack.Push(Processor.MakeBinaryExpression(val2, val1, ExpressionType.GreaterThan));
                }
                return true;
            }

            return false;
        }
    }
}