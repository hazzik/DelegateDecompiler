using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class NewobjProcessor : IProcessor
{
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Register(new NewobjProcessor(), OpCodes.Newobj);
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        var constructor = (ConstructorInfo)instruction.Operand;
        
        if (constructor.DeclaringType.IsNullableType() && constructor.GetParameters().Length == 1)
        {
            // For Nullable<T> constructor, we want to simplify conversions
            // Instead of Convert(Convert(int, Enum), Nullable<Enum>), we want Convert(int, Nullable<Enum>)
            // But we need to get the argument first to apply optimizations
            var parameters = constructor.GetParameters();
            var argument = state.Stack.Pop();
            var nullableType = constructor.DeclaringType;
            var innerType = System.Nullable.GetUnderlyingType(nullableType);
            
            // If inner type is an enum, try to optimize the conversion
            if (innerType.IsEnum)
            {
                var enumUnderlyingType = innerType.GetEnumUnderlyingType();
                // Use AdjustType but intercept if it tries to create intermediate conversions
                var adjusted = Processor.AdjustType(argument, parameters[0].ParameterType);
                
                // Check if AdjustType created Convert(x, Enum) where x is int/byte/long
                if (adjusted is UnaryExpression unary &&
                    unary.NodeType == ExpressionType.Convert &&
                    unary.Type == innerType)
                {
                    var operand = unary.Operand;
                    
                    // Case 1: operand is the enum's underlying type - convert directly to nullable
                    if (operand.Type == enumUnderlyingType)
                    {
                        state.Stack.Push(Expression.Convert(operand, nullableType));
                        return;
                    }
                    
                    // Case 2: operand is Convert(x, underlyingType) where x is int
                    // Skip the intermediate conversion
                    if (operand is UnaryExpression innerUnary &&
                        innerUnary.NodeType == ExpressionType.Convert &&
                        innerUnary.Type == enumUnderlyingType &&
                        innerUnary.Operand.Type == typeof(int))
                    {
                        state.Stack.Push(Expression.Convert(innerUnary.Operand, nullableType));
                        return;
                    }
                    
                    // Case 3: operand is int and enum underlying is long - convert int directly
                    if (operand.Type == typeof(int) &&
                        (enumUnderlyingType == typeof(long) || enumUnderlyingType == typeof(ulong)))
                    {
                        state.Stack.Push(Expression.Convert(operand, nullableType));
                        return;
                    }
                }
                
                // Use the adjusted argument
                state.Stack.Push(Expression.Convert(adjusted, nullableType));
            }
            else
            {
                // Non-enum nullable, use GetArguments normally
                state.Stack.Push(argument);
                var arguments = Processor.GetArguments(state, constructor);
                state.Stack.Push(Expression.Convert(arguments[0], nullableType));
            }
        }
        else
        {
            var arguments = Processor.GetArguments(state, constructor);
            state.Stack.Push(Expression.New(constructor, arguments));
        }
    }
}