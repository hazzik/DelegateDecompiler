using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace DelegateDecompiler.Processors;

internal class FieldProcessor : IProcessor
{
    const string cachedAnonymousMethodDelegate = "CS$<>9__CachedAnonymousMethodDelegate";
    const string cachedAnonymousMethodDelegateRoslyn = "CS$<>9__";

    static readonly HashSet<OpCode> StelemOpcodes = new()
    {
        OpCodes.Stelem,
        OpCodes.Stelem_I,
        OpCodes.Stelem_I1,
        OpCodes.Stelem_I2,
        OpCodes.Stelem_I4,
        OpCodes.Stelem_I8,
        OpCodes.Stelem_R4,
        OpCodes.Stelem_R8,
        OpCodes.Stelem_Ref
    };

    public bool Process(ProcessorState state)
    {
        if (StelemOpcodes.Contains(state.Instruction.OpCode))
        {
            StElem(state);
            return true;
        }

        if (state.Instruction.OpCode == OpCodes.Ldfld || state.Instruction.OpCode == OpCodes.Ldflda)
        {
            LdFld(state, state.Stack.Pop());
            return true;
        }
        
        if (state.Instruction.OpCode == OpCodes.Ldsfld)
        {
            LdFld(state, null);
            return true;
        }
        
        if (state.Instruction.OpCode == OpCodes.Stfld)
        {
            var value = state.Stack.Pop();
            var instance = state.Stack.Pop();
            var field = (FieldInfo)state.Instruction.Operand;
            if (IsCachedAnonymousMethodDelegate(field))
            {
                state.Delegates[Tuple.Create(instance, field)] = value;
            }
            else
            {
                var expression = BuildAssignment(instance.Expression, field, value, out var push);
                if (push)
                    state.Stack.Push(expression);
                else
                    instance.Expression = expression;
            }
            return true;
        }
        
        if (state.Instruction.OpCode == OpCodes.Stsfld)
        {
            var sfValue = state.Stack.Pop();
            var sfField = (FieldInfo)state.Instruction.Operand;
            if (IsCachedAnonymousMethodDelegate(sfField))
            {
                state.Delegates[Tuple.Create(default(Address), sfField)] = sfValue;
            }
            else
            {
                state.Stack.Push(Expression.Assign(Expression.Field(null, sfField), sfValue));
            }
            return true;
        }

        return false;
    }

    static void StElem(ProcessorState state)
    {
        var value = state.Stack.Pop();
        var index = state.Stack.Pop();
        var array = state.Stack.Pop();

        if (array.Expression is NewArrayExpression newArray)
        {
            var initializers = newArray.Expressions.ToArray();
            if (index.Expression is ConstantExpression ce && ce.Value is int i && i < initializers.Length)
            {
                initializers[i] = value;
                array.Expression = Expression.NewArrayInit(newArray.Type.GetElementType(), initializers);
                return;
            }
        }

        state.Stack.Push(Expression.ArrayAccess(array, index));
    }

    static void LdFld(ProcessorState state, Address instance)
    {
        var field = (FieldInfo)state.Instruction.Operand;
        if (IsCachedAnonymousMethodDelegate(field) &&
            state.Delegates.TryGetValue(Tuple.Create(instance, field), out var address))
        {
            state.Stack.Push(address);
        }
        else
        {
            state.Stack.Push(Expression.Field(instance?.Expression, field));
        }
    }

    static bool IsCachedAnonymousMethodDelegate(FieldInfo field)
    {
        if (field == null) return false;
        return field.Name.StartsWith(cachedAnonymousMethodDelegate) && Attribute.IsDefined(field, typeof(CompilerGeneratedAttribute), false) ||
               field.Name.StartsWith(cachedAnonymousMethodDelegateRoslyn) && field.DeclaringType != null && Attribute.IsDefined(field.DeclaringType, typeof(CompilerGeneratedAttribute), false);
    }

    static Expression BuildAssignment(Expression instance, MemberInfo member, Expression value, out bool push)
    {
        var adjustedValue = AdjustType(value, member.FieldOrPropertyType());

        if (instance.NodeType == ExpressionType.New)
        {
            push = false;
            return Expression.MemberInit((NewExpression)instance, Expression.Bind(member, adjustedValue));
        }

        if (instance.NodeType == ExpressionType.MemberInit)
        {
            var memberInitExpression = (MemberInitExpression)instance;
            push = false;
            return Expression.MemberInit(
                memberInitExpression.NewExpression,
                new List<MemberBinding>(memberInitExpression.Bindings)
                {
                    Expression.Bind(member, adjustedValue)
                });
        }

        push = true;
        return Expression.Assign(Expression.MakeMemberAccess(instance, member), adjustedValue);
    }

    static Expression AdjustType(Expression expression, Type type)
    {
        if (expression.Type == type)
            return expression;

        if (expression is ConstantExpression constantExpression)
        {
            if (type.IsEnum)
                return Expression.Constant(Enum.ToObject(type, constantExpression.Value));
        }

        if (expression.Type == typeof(int) && type == typeof(bool))
        {
            return Expression.NotEqual(expression, Expression.Constant(0));
        }

        return Expression.Convert(expression, type);
    }
}

static class MemberInfoExtensions
{
    public static Type FieldOrPropertyType(this MemberInfo member)
    {
        return member switch
        {
            FieldInfo field => field.FieldType,
            PropertyInfo property => property.PropertyType,
            _ => throw new ArgumentException("Member must be either FieldInfo or PropertyInfo", nameof(member))
        };
    }
}