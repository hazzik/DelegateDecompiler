using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler
{
    internal class ArithmeticProcessor : IProcessor
    {
        static readonly Dictionary<OpCode, ExpressionType> Operations = new Dictionary<OpCode, ExpressionType>
        {
            {OpCodes.Add, ExpressionType.Add},
            {OpCodes.Sub, ExpressionType.Subtract},
            {OpCodes.Mul, ExpressionType.Multiply},
            {OpCodes.Div, ExpressionType.Divide},
            {OpCodes.Div_Un, ExpressionType.Divide},
            {OpCodes.Rem, ExpressionType.Modulo},
            {OpCodes.Rem_Un, ExpressionType.Modulo},
        };

        static readonly Dictionary<OpCode, ExpressionType> CheckedOperations = new Dictionary<OpCode, ExpressionType>
        {
            {OpCodes.Add_Ovf, ExpressionType.AddChecked},
            {OpCodes.Add_Ovf_Un, ExpressionType.AddChecked},
            {OpCodes.Sub_Ovf, ExpressionType.SubtractChecked},
            {OpCodes.Sub_Ovf_Un, ExpressionType.SubtractChecked},
            {OpCodes.Mul_Ovf, ExpressionType.MultiplyChecked},
            {OpCodes.Mul_Ovf_Un, ExpressionType.MultiplyChecked},
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

            if (CheckedOperations.TryGetValue(state.Instruction.OpCode, out operation))
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