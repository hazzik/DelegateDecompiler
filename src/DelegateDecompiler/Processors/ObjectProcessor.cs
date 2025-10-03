using System;
using System.Linq.Expressions;
using System.Reflection.Emit;

namespace DelegateDecompiler.Processors;

internal class ObjectProcessor : IProcessor
{
    public bool Process(ProcessorState state)
    {
        if (state.Instruction.OpCode == OpCodes.Initobj)
        {
            var address = state.Stack.Pop();
            var type = (Type)state.Instruction.Operand;
            address.Expression = ExpressionHelper.Default(type);
            return true;
        }

        if (state.Instruction.OpCode == OpCodes.Newarr)
        {
            var operand = (Type)state.Instruction.Operand;
            var expression = state.Stack.Pop();
            if (expression.Expression is ConstantExpression size && (int)size.Value == 0) // optimization
                state.Stack.Push(Expression.NewArrayInit(operand));
            else
                state.Stack.Push(Expression.NewArrayBounds(operand, expression));
            return true;
        }

        if (state.Instruction.OpCode == OpCodes.Box)
        {
            state.Stack.Push(Processor.Box(state.Stack.Pop(), (Type)state.Instruction.Operand));
            return true;
        }

        if (state.Instruction.OpCode == OpCodes.Unbox)
        {
            var expression = state.Stack.Pop();
            var type = (Type)state.Instruction.Operand;
            // Unbox returns a pointer to the value, but in expression trees we represent this as a conversion
            state.Stack.Push(Expression.Convert(expression, type));
            return true;
        }

        if (state.Instruction.OpCode == OpCodes.Unbox_Any)
        {
            var expression = state.Stack.Pop();
            var type = (Type)state.Instruction.Operand;
            // Unbox_Any converts boxed value types to their unboxed form or leaves reference types unchanged
            state.Stack.Push(Expression.Convert(expression, type));
            return true;
        }

        return false;
    }
}