using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler
{
    internal class BinaryInstructionProcessor : IInstructionProcessor
    {
        private static readonly IDictionary<OpCode, Func<Expression, Expression, BinaryExpression>> types = new Dictionary<OpCode, Func<Expression, Expression, BinaryExpression>>
        {
            { OpCodes.Add, Expression.Add },
            { OpCodes.Add_Ovf, Expression.AddChecked },
            { OpCodes.Add_Ovf_Un, Expression.AddChecked },
            { OpCodes.Sub, Expression.Subtract },
            { OpCodes.Sub_Ovf, Expression.SubtractChecked },
            { OpCodes.Sub_Ovf_Un, Expression.SubtractChecked },
            { OpCodes.Mul, Expression.Multiply },
            { OpCodes.Mul_Ovf, Expression.MultiplyChecked },
            { OpCodes.Mul_Ovf_Un, Expression.MultiplyChecked },
            { OpCodes.Div, Expression.Divide },
            { OpCodes.Div_Un, Expression.Divide },
            { OpCodes.Rem, Expression.Modulo },
            { OpCodes.Rem_Un, Expression.Modulo },
            { OpCodes.Xor, Expression.ExclusiveOr },
            { OpCodes.Shl, Expression.LeftShift },
            { OpCodes.Shr, Expression.RightShift },
            { OpCodes.Shr_Un, Expression.RightShift },
            { OpCodes.And, Expression.And },
            { OpCodes.Or, Expression.Or },
        };

        public bool Process(Instruction instruction, Stack<Expression> stack)
        {
            Func<Expression, Expression, BinaryExpression> expression;
            if (!types.TryGetValue(instruction.OpCode, out expression))
                return false;
            var val1 = stack.Pop();
            var val2 = stack.Pop();
            stack.Push(expression(val2, val1));
            return true;
        }
    }
}
