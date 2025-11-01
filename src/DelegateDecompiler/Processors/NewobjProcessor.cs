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
                
                // Try to unwrap unnecessary conversions for nullable enum creation
                var simplified = TrySimplifyForNullableEnum(adjusted, innerType, enumUnderlyingType, nullableType);
                if (simplified != null)
                {
                    state.Stack.Push(simplified);
                    return;
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
    
    static Expression TrySimplifyForNullableEnum(Expression expr, Type enumType, Type enumUnderlyingType, Type nullableEnumType)
    {
        // Check if expr is Convert(x, Enum) 
        if (expr is UnaryExpression unary && unary.NodeType == ExpressionType.Convert && unary.Type == enumType)
        {
            var operand = unary.Operand;
            
            // Case 1: operand is the enum's underlying type
            if (operand.Type == enumUnderlyingType)
            {
                // Check if we can simplify further - unwrap Convert(int, long) for long enums
                if (operand is UnaryExpression underlyingUnary &&
                    underlyingUnary.NodeType == ExpressionType.Convert &&
                    underlyingUnary.Operand.Type == typeof(int) &&
                    (enumUnderlyingType == typeof(long) || enumUnderlyingType == typeof(ulong)))
                {
                    // Convert int directly to Nullable<Enum>
                    return Expression.Convert(underlyingUnary.Operand, nullableEnumType);
                }
                
                // Otherwise convert the underlying type value directly to nullable
                return Expression.Convert(operand, nullableEnumType);
            }
            
            // Case 2: operand is Convert(x, underlyingType) where x is int
            // This happens for byte/short enums: Convert(Convert(int, byte), Enum)
            // Skip to: Convert(int, Nullable<Enum>)
            if (operand is UnaryExpression innerUnary &&
                innerUnary.NodeType == ExpressionType.Convert &&
                innerUnary.Type == enumUnderlyingType &&
                innerUnary.Operand.Type == typeof(int))
            {
                return Expression.Convert(innerUnary.Operand, nullableEnumType);
            }
            
            // Case 3: operand is int and enum underlying is long/ulong - convert int directly
            if (operand.Type == typeof(int) &&
                (enumUnderlyingType == typeof(long) || enumUnderlyingType == typeof(ulong)))
            {
                return Expression.Convert(operand, nullableEnumType);
            }
            
            // Case 3b: operand is Convert(int, long) and enum underlying is long
            // This happens when int is explicitly converted to long before enum
            if (operand is UnaryExpression longUnary &&
                longUnary.NodeType == ExpressionType.Convert &&
                (longUnary.Type == typeof(long) || longUnary.Type == typeof(ulong)) &&
                longUnary.Operand.Type == typeof(int) &&
                (enumUnderlyingType == typeof(long) || enumUnderlyingType == typeof(ulong)))
            {
                // Convert int directly to Nullable<Enum>, skip the long intermediate
                return Expression.Convert(longUnary.Operand, nullableEnumType);
            }
            
            // Case 4: operand is int and enum underlying is byte/short - convert int directly
            // Expression.Convert(int, ByteEnum) is valid and handles the intermediate conversion
            // But when creating Nullable<ByteEnum>, we want Convert(int, Nullable<ByteEnum>) directly
            if (operand.Type == typeof(int) &&
                (enumUnderlyingType == typeof(byte) || enumUnderlyingType == typeof(sbyte) ||
                 enumUnderlyingType == typeof(short) || enumUnderlyingType == typeof(ushort)))
            {
                return Expression.Convert(operand, nullableEnumType);
            }
        }
        
        return null;
    }
}