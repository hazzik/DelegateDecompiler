using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class BinaryExpressionProcessor(ExpressionType expressionType) : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        // Arithmetic operations
        processors.Add(OpCodes.Add, new BinaryExpressionProcessor(ExpressionType.Add));
        processors.Add(OpCodes.Sub, new BinaryExpressionProcessor(ExpressionType.Subtract));
        processors.Add(OpCodes.Mul, new BinaryExpressionProcessor(ExpressionType.Multiply));
        processors.Add(OpCodes.Div, new BinaryExpressionProcessor(ExpressionType.Divide));
        processors.Add(OpCodes.Div_Un, new BinaryExpressionProcessor(ExpressionType.Divide));
        processors.Add(OpCodes.Rem, new BinaryExpressionProcessor(ExpressionType.Modulo));
        processors.Add(OpCodes.Rem_Un, new BinaryExpressionProcessor(ExpressionType.Modulo));

        // Checked arithmetic operations
        processors.Add(OpCodes.Add_Ovf, new BinaryExpressionProcessor(ExpressionType.AddChecked));
        processors.Add(OpCodes.Add_Ovf_Un, new BinaryExpressionProcessor(ExpressionType.AddChecked));
        processors.Add(OpCodes.Sub_Ovf, new BinaryExpressionProcessor(ExpressionType.SubtractChecked));
        processors.Add(OpCodes.Sub_Ovf_Un, new BinaryExpressionProcessor(ExpressionType.SubtractChecked));
        processors.Add(OpCodes.Mul_Ovf, new BinaryExpressionProcessor(ExpressionType.MultiplyChecked));
        processors.Add(OpCodes.Mul_Ovf_Un, new BinaryExpressionProcessor(ExpressionType.MultiplyChecked));

        // Bitwise operations
        processors.Add(OpCodes.And, new BinaryExpressionProcessor(ExpressionType.And));
        processors.Add(OpCodes.Or, new BinaryExpressionProcessor(ExpressionType.Or));
        processors.Add(OpCodes.Xor, new BinaryExpressionProcessor(ExpressionType.ExclusiveOr));
        processors.Add(OpCodes.Shl, new BinaryExpressionProcessor(ExpressionType.LeftShift));
        processors.Add(OpCodes.Shr, new BinaryExpressionProcessor(ExpressionType.RightShift));
        processors.Add(OpCodes.Shr_Un, new BinaryExpressionProcessor(ExpressionType.RightShift));

        // Simple comparison operations
        processors.Add(OpCodes.Ceq, new BinaryExpressionProcessor(ExpressionType.Equal));
        processors.Add(OpCodes.Cgt, new BinaryExpressionProcessor(ExpressionType.GreaterThan));
        processors.Add(OpCodes.Clt, new BinaryExpressionProcessor(ExpressionType.LessThan));
        processors.Add(OpCodes.Clt_Un, new BinaryExpressionProcessor(ExpressionType.LessThan));
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        var val1 = state.Stack.Pop();
        var val2 = state.Stack.Pop();
        
        state.Stack.Push(Processor.MakeBinaryExpression(val2, val1, expressionType));
    }
}