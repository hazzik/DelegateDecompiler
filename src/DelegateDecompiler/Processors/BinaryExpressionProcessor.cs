using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class BinaryExpressionProcessor : IProcessor
{
    static readonly Dictionary<OpCode, ExpressionType> Operations = new()
    {
        // Arithmetic operations
        {OpCodes.Add, ExpressionType.Add},
        {OpCodes.Sub, ExpressionType.Subtract},
        {OpCodes.Mul, ExpressionType.Multiply},
        {OpCodes.Div, ExpressionType.Divide},
        {OpCodes.Div_Un, ExpressionType.Divide},
        {OpCodes.Rem, ExpressionType.Modulo},
        {OpCodes.Rem_Un, ExpressionType.Modulo},
            
        // Checked arithmetic operations
        {OpCodes.Add_Ovf, ExpressionType.AddChecked},
        {OpCodes.Add_Ovf_Un, ExpressionType.AddChecked},
        {OpCodes.Sub_Ovf, ExpressionType.SubtractChecked},
        {OpCodes.Sub_Ovf_Un, ExpressionType.SubtractChecked},
        {OpCodes.Mul_Ovf, ExpressionType.MultiplyChecked},
        {OpCodes.Mul_Ovf_Un, ExpressionType.MultiplyChecked},
            
        // Bitwise operations
        {OpCodes.And, ExpressionType.And},
        {OpCodes.Or, ExpressionType.Or},
        {OpCodes.Xor, ExpressionType.ExclusiveOr},
        {OpCodes.Shl, ExpressionType.LeftShift},
        {OpCodes.Shr, ExpressionType.RightShift},
        {OpCodes.Shr_Un, ExpressionType.RightShift},
            
        // Simple comparison operations
        {OpCodes.Ceq, ExpressionType.Equal},
        {OpCodes.Cgt, ExpressionType.GreaterThan},
        {OpCodes.Clt, ExpressionType.LessThan},
        {OpCodes.Clt_Un, ExpressionType.LessThan},
    };

    public bool Process(ProcessorState state, Instruction instruction)
    {
        if (!Operations.TryGetValue(instruction.OpCode, out var operation))
            return false;

        Console.WriteLine($"DEBUG: BinaryExpressionProcessor processing {instruction.OpCode}");
        var val1 = state.Stack.Pop();
        var val2 = state.Stack.Pop();
        Console.WriteLine($"DEBUG: val1: {val1} (type: {val1.Type})");
        Console.WriteLine($"DEBUG: val2: {val2} (type: {val2.Type})");
        
        var result = Processor.MakeBinaryExpression(val2, val1, operation);
        Console.WriteLine($"DEBUG: Binary result: {result} (type: {result.Type})");
        
        state.Stack.Push(result);
        return true;
    }
}