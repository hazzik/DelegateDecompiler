using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors
{
    class BinaryProcessor : IProcessor
    {
        static readonly Dictionary<OpCode, ExpressionType> ExpressionTypes = new Dictionary<OpCode, ExpressionType>()
        {
            {OpCodes.Add, ExpressionType.Add},
            {OpCodes.Add_Ovf, ExpressionType.AddChecked},
            {OpCodes.Add_Ovf_Un, ExpressionType.AddChecked},
            {OpCodes.Sub, ExpressionType.Subtract},
            {OpCodes.Sub_Ovf, ExpressionType.SubtractChecked},
            {OpCodes.Sub_Ovf_Un, ExpressionType.SubtractChecked},
            {OpCodes.Mul, ExpressionType.Multiply},
            {OpCodes.Mul_Ovf, ExpressionType.MultiplyChecked},
            {OpCodes.Mul_Ovf_Un, ExpressionType.MultiplyChecked},
            {OpCodes.Div, ExpressionType.Divide},
            {OpCodes.Div_Un, ExpressionType.Divide},
            {OpCodes.Rem, ExpressionType.Modulo},
            {OpCodes.Rem_Un, ExpressionType.Modulo},
            {OpCodes.Xor, ExpressionType.ExclusiveOr},
            {OpCodes.Shl, ExpressionType.LeftShift},
            {OpCodes.Shr, ExpressionType.RightShift},
            {OpCodes.Shr_Un, ExpressionType.RightShift},
            {OpCodes.And, ExpressionType.And},
            {OpCodes.Or, ExpressionType.Or},
            {OpCodes.Ceq, ExpressionType.Equal},
            {OpCodes.Cgt, ExpressionType.GreaterThan},
            {OpCodes.Clt, ExpressionType.LessThan},
            {OpCodes.Clt_Un, ExpressionType.LessThan},
        };

        public bool Process(ProcessorState state)
        {
            ExpressionType expressionType;
            if (state.Instruction.OpCode == OpCodes.Cgt_Un)
            {
                var right = state.Stack.Pop();
                var left = state.Stack.Pop();

                var constantExpression = right.Expression as ConstantExpression;
                if (constantExpression != null && (constantExpression.Value as int? == 0 || constantExpression.Value == null))
                {
                    //Special case.
                    state.Stack.Push(MakeBinaryExpression(left, right, ExpressionType.NotEqual));
                }
                else
                {
                    state.Stack.Push(MakeBinaryExpression(left, right, ExpressionType.GreaterThan));
                }
            }
            else if (ExpressionTypes.TryGetValue(state.Instruction.OpCode, out expressionType))
            {
                var right = state.Stack.Pop();
                var left = state.Stack.Pop();
                state.Stack.Push(MakeBinaryExpression(left, right, expressionType));
            }
            else
            {
                return false;
            }
            return true;
        }

        public static BinaryExpression MakeBinaryExpression(Address left, Address right, ExpressionType expressionType)
        {
            var rightType = right.Type;
            var leftType = left.Type;

            left = AdjustBooleanConstant(left, rightType);
            right = AdjustBooleanConstant(right, leftType);
            left = ConvertEnumExpressionToUnderlyingType(left);
            right = ConvertEnumExpressionToUnderlyingType(right);

            return Expression.MakeBinary(expressionType, left, right);
        }

        static Expression AdjustBooleanConstant(Expression expression, Type type)
        {
            if (type == typeof (bool) && expression.Type == typeof (int))
            {
                var constantExpression = expression as ConstantExpression;
                if (constantExpression != null)
                {
                    return Expression.Constant(!Equals(constantExpression.Value, 0));
                }
            }

            return expression;
        }

        static Expression ConvertEnumExpressionToUnderlyingType(Expression expression)
        {
            if (expression.Type.IsEnum)
                return Expression.Convert(expression, expression.Type.GetEnumUnderlyingType());

            return expression;
        }
    }
}