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
        processors.Register(new BinaryExpressionProcessor(ExpressionType.Add), OpCodes.Add);
        processors.Register(new BinaryExpressionProcessor(ExpressionType.Subtract), OpCodes.Sub);
        processors.Register(new BinaryExpressionProcessor(ExpressionType.Multiply), OpCodes.Mul);
        processors.Register(new BinaryExpressionProcessor(ExpressionType.Divide), OpCodes.Div, OpCodes.Div_Un);
        processors.Register(new BinaryExpressionProcessor(ExpressionType.Modulo), OpCodes.Rem, OpCodes.Rem_Un);

        // Checked arithmetic operations
        processors.Register(new BinaryExpressionProcessor(ExpressionType.AddChecked), OpCodes.Add_Ovf, OpCodes.Add_Ovf_Un);
        processors.Register(new BinaryExpressionProcessor(ExpressionType.SubtractChecked), OpCodes.Sub_Ovf, OpCodes.Sub_Ovf_Un);
        processors.Register(new BinaryExpressionProcessor(ExpressionType.MultiplyChecked), OpCodes.Mul_Ovf, OpCodes.Mul_Ovf_Un);

        // Bitwise operations
        processors.Register(new BinaryExpressionProcessor(ExpressionType.And), OpCodes.And);
        processors.Register(new BinaryExpressionProcessor(ExpressionType.Or), OpCodes.Or);
        processors.Register(new BinaryExpressionProcessor(ExpressionType.ExclusiveOr), OpCodes.Xor);
        processors.Register(new BinaryExpressionProcessor(ExpressionType.LeftShift), OpCodes.Shl);
        processors.Register(new BinaryExpressionProcessor(ExpressionType.RightShift), OpCodes.Shr, OpCodes.Shr_Un);

        // Simple comparison operations
        processors.Register(new BinaryExpressionProcessor(ExpressionType.Equal), OpCodes.Ceq);
        processors.Register(new BinaryExpressionProcessor(ExpressionType.GreaterThan), OpCodes.Cgt);
        processors.Register(new BinaryExpressionProcessor(ExpressionType.LessThan), OpCodes.Clt, OpCodes.Clt_Un);

        // Branch comparison operations (conditional branch instructions)
        processors.Register(new BinaryExpressionProcessor(ExpressionType.Equal), OpCodes.Beq, OpCodes.Beq_S);
        processors.Register(new BinaryExpressionProcessor(ExpressionType.GreaterThanOrEqual), OpCodes.Bge, OpCodes.Bge_S, OpCodes.Bge_Un, OpCodes.Bge_Un_S);
        processors.Register(new BinaryExpressionProcessor(ExpressionType.GreaterThan), OpCodes.Bgt, OpCodes.Bgt_S, OpCodes.Bgt_Un, OpCodes.Bgt_Un_S);
        processors.Register(new BinaryExpressionProcessor(ExpressionType.LessThanOrEqual), OpCodes.Ble, OpCodes.Ble_S, OpCodes.Ble_Un, OpCodes.Ble_Un_S);
        processors.Register(new BinaryExpressionProcessor(ExpressionType.LessThan), OpCodes.Blt, OpCodes.Blt_S, OpCodes.Blt_Un, OpCodes.Blt_Un_S);
        processors.Register(new BinaryExpressionProcessor(ExpressionType.NotEqual), OpCodes.Bne_Un, OpCodes.Bne_Un_S);
    }


    public void Process(ProcessorState state, Instruction instruction)
    {
        var val1 = state.Stack.Pop();
        var val2 = state.Stack.Pop();
        
        state.Stack.Push(Processor.MakeBinaryExpression(val2, val1, expressionType));
    }
}