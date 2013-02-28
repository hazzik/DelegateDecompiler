﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler
{
    public class Processor 
    {
        public static Processor Create(Expression[] locals, IList<ParameterExpression> args)
        {
            return new Processor(new Stack<Expression>(), locals, args);
        }

        Processor Clone()
        {
            return new Processor(new Stack<Expression>(stack), locals.ToArray(), args.ToArray());
        }

        static readonly MethodInfo stringConcat = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) });

        readonly Stack<Expression> stack;
        readonly Expression[] locals;
        readonly IList<ParameterExpression> args;

        Processor(Stack<Expression> stack, Expression[] locals, IList<ParameterExpression> args)
        {
            this.stack = stack;
            this.locals = locals;
            this.args = args;
        }

        public Expression Process(Instruction instruction, Instruction last = null)
        {
            while(instruction != null && instruction != last)
            {
                Debug.WriteLine(instruction);

                if (instruction.OpCode == OpCodes.Nop)
                {
                    //do nothing;
                }
                else if (instruction.OpCode == OpCodes.Ldarg_0)
                {
                    LdArg(0);
                }
                else if (instruction.OpCode == OpCodes.Ldarg_1)
                {
                    LdArg(1);
                }
                else if (instruction.OpCode == OpCodes.Ldarg_2)
                {
                    LdArg(2);
                }
                else if (instruction.OpCode == OpCodes.Ldarg_3)
                {
                    LdArg(3);
                }
                else if (instruction.OpCode == OpCodes.Ldarg_S)
                {
                    LdArg((short) instruction.Operand);
                }
                else if (instruction.OpCode == OpCodes.Ldarg)
                {
                    LdArg((int) instruction.Operand);
                }
                else if (instruction.OpCode == OpCodes.Ldarga_S || instruction.OpCode == OpCodes.Ldarga)
                {
                    var operand = (ParameterInfo) instruction.Operand;
                    stack.Push(args.Single(x => x.Name == operand.Name));
                }
                else if (instruction.OpCode == OpCodes.Stloc_0)
                {
                    StLoc(0);
                }
                else if (instruction.OpCode == OpCodes.Stloc_1)
                {
                    StLoc(1);
                }
                else if (instruction.OpCode == OpCodes.Stloc_2)
                {
                    StLoc(2);
                }
                else if (instruction.OpCode == OpCodes.Stloc_3)
                {
                    StLoc(3);
                }
                else if (instruction.OpCode == OpCodes.Stloc_S)
                {
                    StLoc((byte) instruction.Operand);
                }
                else if (instruction.OpCode == OpCodes.Stloc)
                {
                    StLoc((int) instruction.Operand);
                }
                else if (instruction.OpCode == OpCodes.Stelem ||
                         instruction.OpCode == OpCodes.Stelem_I ||
                         instruction.OpCode == OpCodes.Stelem_I1 ||
                         instruction.OpCode == OpCodes.Stelem_I2 ||
                         instruction.OpCode == OpCodes.Stelem_I4 ||
                         instruction.OpCode == OpCodes.Stelem_I8 ||
                         instruction.OpCode == OpCodes.Stelem_R4 ||
                         instruction.OpCode == OpCodes.Stelem_R8 ||
                         instruction.OpCode == OpCodes.Stelem_Ref)
                {
                    StElem();
                }
                else if (instruction.OpCode == OpCodes.Ldfld)
                {
                    var instance = stack.Pop();
                    stack.Push(Expression.Field(instance, (FieldInfo) instruction.Operand));
                }
                else if (instruction.OpCode == OpCodes.Ldsfld)
                {
                    stack.Push(Expression.Field(null, (FieldInfo) instruction.Operand));
                }
                else if (instruction.OpCode == OpCodes.Ldloc_0)
                {
                    LdLoc(0);
                }
                else if (instruction.OpCode == OpCodes.Ldloc_1)
                {
                    LdLoc(1);
                }
                else if (instruction.OpCode == OpCodes.Ldloc_2)
                {
                    LdLoc(2);
                }
                else if (instruction.OpCode == OpCodes.Ldloc_3)
                {
                    LdLoc(3);
                }
                else if (instruction.OpCode == OpCodes.Ldloc_S)
                {
                    LdLoc((byte) instruction.Operand);
                }
                else if (instruction.OpCode == OpCodes.Ldloc)
                {
                    LdLoc((int) instruction.Operand);
                }
                else if (instruction.OpCode == OpCodes.Ldloca || instruction.OpCode == OpCodes.Ldloca_S)
                {
                    var operand = (LocalVariableInfo) instruction.Operand;
                    LdLoc(operand.LocalIndex);
                }
                else if (instruction.OpCode == OpCodes.Ldstr)
                {
                    stack.Push(Expression.Constant((string) instruction.Operand));
                }
                else if (instruction.OpCode == OpCodes.Ldc_I4_0)
                {
                    LdC(0);
                }
                else if (instruction.OpCode == OpCodes.Ldc_I4_1)
                {
                    LdC(1);
                }
                else if (instruction.OpCode == OpCodes.Ldc_I4_2)
                {
                    LdC(2);
                }
                else if (instruction.OpCode == OpCodes.Ldc_I4_3)
                {
                    LdC(3);
                }
                else if (instruction.OpCode == OpCodes.Ldc_I4_4)
                {
                    LdC(4);
                }
                else if (instruction.OpCode == OpCodes.Ldc_I4_5)
                {
                    LdC(5);
                }
                else if (instruction.OpCode == OpCodes.Ldc_I4_6)
                {
                    LdC(6);
                }
                else if (instruction.OpCode == OpCodes.Ldc_I4_7)
                {
                    LdC(7);
                }
                else if (instruction.OpCode == OpCodes.Ldc_I4_8)
                {
                    LdC(8);
                }
                else if (instruction.OpCode == OpCodes.Ldc_I4_S)
                {
                    LdC((sbyte) instruction.Operand);
                }
                else if (instruction.OpCode == OpCodes.Ldc_I4_M1)
                {
                    LdC(-1);
                }
                else if (instruction.OpCode == OpCodes.Ldc_I4)
                {
                    LdC((int) instruction.Operand);
                }
                else if (instruction.OpCode == OpCodes.Ldc_I8)
                {
                    LdC((long) instruction.Operand);
                }
                else if (instruction.OpCode == OpCodes.Ldc_R4)
                {
                    LdC((float) instruction.Operand);
                }
                else if (instruction.OpCode == OpCodes.Ldc_R8)
                {
                    LdC((double) instruction.Operand);
                }
                else if (instruction.OpCode == OpCodes.Br_S || instruction.OpCode == OpCodes.Br)
                {
                    instruction = (Instruction) instruction.Operand;
                    continue;
                }
                else if (instruction.OpCode == OpCodes.Brfalse_S || instruction.OpCode == OpCodes.Brfalse)
                {
                    instruction = ConditionalBranch(instruction, val => Expression.Equal(val, Default(val.Type)));
                    continue;
                }
                else if (instruction.OpCode == OpCodes.Brtrue || instruction.OpCode == OpCodes.Brtrue_S)
                {
                    instruction = ConditionalBranch(instruction, val => Expression.NotEqual(val, Default(val.Type)));
                    continue;
                }
                else if (instruction.OpCode == OpCodes.Bgt ||
                         instruction.OpCode == OpCodes.Bgt_S ||
                         instruction.OpCode == OpCodes.Bgt_Un ||
                         instruction.OpCode == OpCodes.Bgt_Un_S)
                {
                    var val1 = stack.Pop();
                    instruction = ConditionalBranch(instruction, val => Expression.GreaterThan(val, val1));
                    continue;
                }
                else if (instruction.OpCode == OpCodes.Bge ||
                         instruction.OpCode == OpCodes.Bge_S ||
                         instruction.OpCode == OpCodes.Bge_Un ||
                         instruction.OpCode == OpCodes.Bge_Un_S)
                {
                    var val1 = stack.Pop();
                    instruction = ConditionalBranch(instruction, val => Expression.GreaterThanOrEqual(val, val1));
                    continue;
                }
                else if (instruction.OpCode == OpCodes.Blt ||
                         instruction.OpCode == OpCodes.Blt_S ||
                         instruction.OpCode == OpCodes.Blt_Un ||
                         instruction.OpCode == OpCodes.Blt_Un_S)
                {
                    var val1 = stack.Pop();
                    instruction = ConditionalBranch(instruction, val => Expression.LessThan(val, val1));
                    continue;
                }
                else if (instruction.OpCode == OpCodes.Ble ||
                         instruction.OpCode == OpCodes.Ble_S ||
                         instruction.OpCode == OpCodes.Ble_Un ||
                         instruction.OpCode == OpCodes.Ble_Un_S)
                {
                    var val1 = stack.Pop();
                    instruction = ConditionalBranch(instruction, val => Expression.LessThanOrEqual(val, val1));
                    continue;
                }
                else if (instruction.OpCode == OpCodes.Beq ||
                         instruction.OpCode == OpCodes.Beq_S)
                {
                    var val1 = stack.Pop();
                    instruction = ConditionalBranch(instruction, val => AdjustedBinaryExpression(val, val1, ExpressionType.Equal));
                    continue;
                }
                else if (instruction.OpCode == OpCodes.Bne_Un ||
                         instruction.OpCode == OpCodes.Bne_Un_S)
                {
                    var val1 = stack.Pop();
                    instruction = ConditionalBranch(instruction, val => AdjustedBinaryExpression(val, val1, ExpressionType.NotEqual));
                    continue;
                }
                else if (instruction.OpCode == OpCodes.Dup)
                {
                    stack.Push(stack.Peek());
                }
                else if (instruction.OpCode == OpCodes.Pop)
                {
                    stack.Pop();
                }
                else if (instruction.OpCode == OpCodes.Add)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(Expression.Add(val2, val1));
                }
                else if (instruction.OpCode == OpCodes.Add_Ovf || instruction.OpCode == OpCodes.Add_Ovf_Un)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(Expression.AddChecked(val2, val1));
                }
                else if (instruction.OpCode == OpCodes.Sub)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(Expression.Subtract(val2, val1));
                }
                else if (instruction.OpCode == OpCodes.Sub_Ovf || instruction.OpCode == OpCodes.Sub_Ovf_Un)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(Expression.SubtractChecked(val2, val1));
                }
                else if (instruction.OpCode == OpCodes.Mul)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(Expression.Multiply(val2, val1));
                }
                else if (instruction.OpCode == OpCodes.Mul_Ovf || instruction.OpCode == OpCodes.Mul_Ovf_Un)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(Expression.MultiplyChecked(val2, val1));
                }
                else if (instruction.OpCode == OpCodes.Div || instruction.OpCode == OpCodes.Div_Un)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(Expression.Divide(val2, val1));
                }
                else if (instruction.OpCode == OpCodes.Conv_I)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.Convert(val1, typeof (int))); // Suppot x64?
                }
                else if (instruction.OpCode == OpCodes.Conv_I1)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.Convert(val1, typeof (sbyte)));
                }
                else if (instruction.OpCode == OpCodes.Conv_I2)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.Convert(val1, typeof (short)));
                }
                else if (instruction.OpCode == OpCodes.Conv_I4)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.Convert(val1, typeof (int)));
                }
                else if (instruction.OpCode == OpCodes.Conv_I8)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.Convert(val1, typeof (long)));
                }
                else if (instruction.OpCode == OpCodes.Conv_U)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.Convert(val1, typeof (uint))); // Suppot x64?
                }
                else if (instruction.OpCode == OpCodes.Conv_U1)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.Convert(val1, typeof (byte)));
                }
                else if (instruction.OpCode == OpCodes.Conv_U2)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.Convert(val1, typeof (ushort)));
                }
                else if (instruction.OpCode == OpCodes.Conv_U4)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.Convert(val1, typeof (uint)));
                }
                else if (instruction.OpCode == OpCodes.Conv_U8)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.Convert(val1, typeof (ulong)));
                }
                else if (instruction.OpCode == OpCodes.Conv_Ovf_I || instruction.OpCode == OpCodes.Conv_Ovf_I_Un)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.ConvertChecked(val1, typeof (int))); // Suppot x64?
                }
                else if (instruction.OpCode == OpCodes.Conv_Ovf_I1 || instruction.OpCode == OpCodes.Conv_Ovf_I1_Un)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.ConvertChecked(val1, typeof (sbyte)));
                }
                else if (instruction.OpCode == OpCodes.Conv_Ovf_I2 || instruction.OpCode == OpCodes.Conv_Ovf_I2_Un)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.ConvertChecked(val1, typeof (short)));
                }
                else if (instruction.OpCode == OpCodes.Conv_Ovf_I4 || instruction.OpCode == OpCodes.Conv_Ovf_I4_Un)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.ConvertChecked(val1, typeof (int)));
                }
                else if (instruction.OpCode == OpCodes.Conv_Ovf_I8 || instruction.OpCode == OpCodes.Conv_Ovf_I8_Un)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.ConvertChecked(val1, typeof (long)));
                }
                else if (instruction.OpCode == OpCodes.Conv_Ovf_U || instruction.OpCode == OpCodes.Conv_Ovf_U_Un)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.ConvertChecked(val1, typeof (uint))); // Suppot x64?
                }
                else if (instruction.OpCode == OpCodes.Conv_Ovf_U1 || instruction.OpCode == OpCodes.Conv_Ovf_U1_Un)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.ConvertChecked(val1, typeof (byte)));
                }
                else if (instruction.OpCode == OpCodes.Conv_Ovf_U2 || instruction.OpCode == OpCodes.Conv_Ovf_U2_Un)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.ConvertChecked(val1, typeof (ushort)));
                }
                else if (instruction.OpCode == OpCodes.Conv_Ovf_U4 || instruction.OpCode == OpCodes.Conv_Ovf_U4_Un)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.ConvertChecked(val1, typeof (uint)));
                }
                else if (instruction.OpCode == OpCodes.Conv_Ovf_U8 || instruction.OpCode == OpCodes.Conv_Ovf_U8_Un)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.ConvertChecked(val1, typeof (ulong)));
                }
                else if (instruction.OpCode == OpCodes.Conv_R4 || instruction.OpCode == OpCodes.Conv_R_Un)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.ConvertChecked(val1, typeof (float)));
                }
                else if (instruction.OpCode == OpCodes.Conv_R8)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.ConvertChecked(val1, typeof (double)));
                }
                else if (instruction.OpCode == OpCodes.And)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(Expression.And(val2, val1));
                }
                else if (instruction.OpCode == OpCodes.Or)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(Expression.Or(val2, val1));
                }
                else if (instruction.OpCode == OpCodes.Newobj)
                {
                    var operand = (ConstructorInfo) instruction.Operand;
                    stack.Push(Expression.New(operand, GetArguments(operand)));
                }
                else if (instruction.OpCode == OpCodes.Newarr)
                {
                    var operand = (Type) instruction.Operand;
                    var expression = stack.Pop();
                    var size = expression as ConstantExpression;
                    if (size != null && (int) size.Value == 0) // optimization
                        stack.Push(Expression.NewArrayInit(operand));
                    else
                        stack.Push(Expression.NewArrayBounds(operand, expression));
                }
                else if (instruction.OpCode == OpCodes.Box)
                {
                    stack.Push(Box(stack.Pop(), (Type) instruction.Operand));
                }
                else if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt)
                {
                    Call((MethodInfo) instruction.Operand);
                }
                else if (instruction.OpCode == OpCodes.Ceq)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();

                    stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.Equal));
                }
                else if (instruction.OpCode == OpCodes.Cgt || instruction.OpCode == OpCodes.Cgt_Un)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();

                    stack.Push(Expression.GreaterThan(val2, AdjustType(val1, val2.Type)));
                }
                else if (instruction.OpCode == OpCodes.Clt || instruction.OpCode == OpCodes.Clt_Un)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();

                    stack.Push(Expression.LessThan(val2, AdjustType(val1, val2.Type)));
                }
                else if (instruction.OpCode == OpCodes.Ret)
                {
                    break;
                }

                instruction = instruction.Next;
            }

            return stack.Count == 0
                       ? Expression.Empty()
                       : stack.Pop();
        }

        private static BinaryExpression AdjustedBinaryExpression(Expression left, Expression right, ExpressionType expressionType)
        {
            left = ConvertEnumExpressionToUnderlyingType(left);
            right = ConvertEnumExpressionToUnderlyingType(right);

            return Expression.MakeBinary(expressionType, left, right);
        }

        private static Expression Box(Expression expression, Type type)
        {
            var constantExpression = expression as ConstantExpression;
            if (constantExpression != null)
            {
                if (type.IsEnum)
                    return Expression.Convert(Expression.Constant(Enum.ToObject(type, constantExpression.Value)), typeof (Enum));
            }

            if (expression.Type.IsEnum)
                return Expression.Convert(expression, typeof (Enum));

            return Expression.Convert(expression, typeof (object));
        }

        private static Expression ConvertEnumExpressionToUnderlyingType(Expression expression)
        {
            if (expression.Type.IsEnum)
                return Expression.Convert(expression, expression.Type.GetEnumUnderlyingType());

            return expression;
        }

        static Expression Default(Type type)
        {
            if (type == typeof (bool))
                return Expression.Constant(false);
            if (type.IsValueType)
                return Expression.Default(type);
            return Expression.Constant(null, type);
        }

        Instruction ConditionalBranch(Instruction instruction, Func<Expression, BinaryExpression> condition)
        {
            var val1 = stack.Pop();
            var test = condition(val1);

            var left = (Instruction)instruction.Operand;
            var right = instruction.Next;

            Instruction common = GetJoinPoint(left, right);

            var rightExpression = Clone().Process(right, common);
            var leftExpression = AdjustType(Clone().Process(left, common), rightExpression.Type);

            var expression = BuildConditionalBranch(test, val1, leftExpression, rightExpression);
            stack.Push(expression);
            return common;
        }

        private static Expression BuildConditionalBranch(BinaryExpression test, Expression val1, Expression leftExpression, Expression rightExpression)
        {
            if (test.NodeType == ExpressionType.NotEqual)
            {
                if (val1 == leftExpression && (test.Right is DefaultExpression || (test.Right is ConstantExpression && ((ConstantExpression) test.Right).Value == null)))
                {
                    return Expression.Coalesce(val1, rightExpression);
                }
            }
            else if (test.NodeType == ExpressionType.Equal)
            {
                var leftConstant = leftExpression as ConstantExpression;
                if (leftConstant != null)
                {
                    if (leftConstant.Value is bool)
                    {
                        if ((bool) leftConstant.Value)
                        {
                            return Expression.OrElse(test, rightExpression);
                        }
                        var constantExpression = test.Right as ConstantExpression;
                        if (constantExpression != null && constantExpression.Value is bool && !((bool) constantExpression.Value))
                        {
                            return Expression.AndAlso(test.Left, rightExpression);
                        }
                        else
                        {
                            return Expression.AndAlso(Expression.NotEqual(test.Left, test.Right), rightExpression);
                        }
                    }
                }
            }
            return Expression.Condition(test, leftExpression, rightExpression);
        }

        static Instruction GetJoinPoint(Instruction left, Instruction right)
        {
            return GetCommon(GetFlow(left), GetFlow(right));
        }

        static Instruction GetCommon(IEnumerable<Instruction> leftFlow, Stack<Instruction> rightFlow)
        {
            Instruction instruction = null;
            foreach (var left in leftFlow)
            {
                if (rightFlow.Count <= 0 || left != rightFlow.Pop())
                    break;
                
                instruction = left;
            }
            return instruction;
        }

        static Stack<Instruction> GetFlow(Instruction instruction)
        {
            var instructions = new Stack<Instruction>();
            while (instruction != null)
            {
                instructions.Push(instruction);
                if (instruction.OpCode.FlowControl == FlowControl.Branch)
                {
                    instruction = (Instruction) instruction.Operand;
                }
                else if (instruction.OpCode.FlowControl == FlowControl.Cond_Branch)
                {
                    instruction = GetJoinPoint((Instruction) instruction.Operand, instruction.Next);
                }
                else
                {
                    instruction = instruction.Next;
                }
            }
            return instructions;
        }

        static Expression AdjustType(Expression expression, Type type)
        {
            if (expression.Type == typeof(int) && type == typeof(bool))
            {
                var constant = expression as ConstantExpression;
                if (constant != null)
                {
                    return Expression.Constant(!Equals(constant.Value , 0));
                }
                return Expression.NotEqual(expression, Expression.Constant(0));
            }
            return expression;
        }

        void StElem()
        {
            var value = stack.Pop();
            var index = stack.Pop();
            var array = stack.Pop();

            var newArray = array as NewArrayExpression;
            if (newArray != null)
            {
                var expressions = CreateArrayInitExpressions(newArray, value);
                UpdateLocals(newArray, Expression.NewArrayInit(array.Type.GetElementType(), expressions));
                return;
            }

            throw new NotImplementedException();
        }

        static IEnumerable<Expression> CreateArrayInitExpressions(NewArrayExpression newArray, Expression value)
        {
            if (newArray.NodeType == ExpressionType.NewArrayInit)
            {
                var expressions = newArray.Expressions.ToList();
                expressions.Add(value);
                return expressions;
            }
            return new[] { value };
        }

        void LdC(int i)
        {
            stack.Push(Expression.Constant(i));
        }

        void LdC(long i)
        {
            stack.Push(Expression.Constant(i));
        }

        void LdC(float i)
        {
            stack.Push(Expression.Constant(i));
        }

        void LdC(double i)
        {
            stack.Push(Expression.Constant(i));
        }

        void Call(MethodInfo m)
        {
            var mArgs = GetArguments(m);

            var instance = m.IsStatic ? null : stack.Pop();
            stack.Push(BuildMethodCallExpression(m, instance, mArgs));
        }

        Expression[] GetArguments(MethodBase m)
        {
            var parameterInfos = m.GetParameters();
            var mArgs = new Expression[parameterInfos.Length];
            for (var i = parameterInfos.Length - 1; i >= 0; i--)
            {
                mArgs[i] = stack.Pop();
            }
            return mArgs;
        }

        Expression BuildMethodCallExpression(MethodInfo m, Expression instance, Expression[] arguments)
        {
            if (m.Name == "Add" && instance != null && typeof(IEnumerable).IsAssignableFrom(instance.Type))
            {
                var newExpression = instance as NewExpression;
                if (newExpression != null)
                {
                    var init = Expression.ListInit(newExpression, Expression.ElementInit(m, arguments));
                    UpdateLocals(newExpression, init);
                    return init;
                }
                var initExpression = instance as ListInitExpression;
                if (initExpression != null)
                {
                    var initializers = initExpression.Initializers.ToList();
                    initializers.Add(Expression.ElementInit(m, arguments));
                    var init = Expression.ListInit(initExpression.NewExpression, initializers);
                    UpdateLocals(initExpression, init);
                    return init;
                }
            }
            if (m.IsSpecialName && m.IsHideBySig)
            {
                if (m.Name.StartsWith("get_"))
                {
                    return Expression.Property(instance, m);
                }
                if (m.Name.StartsWith("op_"))
                {
                    ExpressionType type;
                    if (Enum.TryParse(m.Name.Substring(3), out type))
                    {
                        switch (arguments.Length)
                        {
                            case 1:
                                return Expression.MakeUnary(type, arguments[0], arguments[0].Type);
                            case 2:
                                return Expression.MakeBinary(type, arguments[0], arguments[1]);
                        }
                    }
                }
            }
            if (m.Name == "Concat" && m.DeclaringType == typeof (string))
            {
                var expressions = GetExpressionsForStringConcat(arguments);
                if (expressions.Count > 1)
                {
                    var expression = expressions[0];
                    for (var i = 1; i < expressions.Count; i++)
                    {
                        expression = Expression.Add(expression, expressions[i], stringConcat);
                    }
                    return expression;
                }
            }

            var parameters = m.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var argument = arguments[i];
                var parameterType = parameter.ParameterType;
                var argumentType = argument.Type;
                if (!parameterType.IsAssignableFrom(argumentType))
                {
                    if (argumentType.IsEnum && argumentType.GetEnumUnderlyingType() == parameterType)
                    {
                        arguments[i] = Expression.Convert(argument, parameterType);
                    }
                }
            }

            return Expression.Call(instance, m, arguments);
        }

        static IList<Expression> GetExpressionsForStringConcat(Expression[] arguments)
        {
            if (arguments.Length == 1)
            {
                var array = arguments[0] as NewArrayExpression;
                if (array != null)
                {
                    if (array.NodeType == ExpressionType.NewArrayInit)
                    {
                        return array.Expressions;
                    }
                }
            }
            return arguments;
        }

        void UpdateLocals(Expression oldExpression, Expression newExpression)
        {
            for (var i = 0; i < locals.Length; i++)
            {
                var local = locals[i];
                if (local == oldExpression)
                {
                    locals[i] = newExpression;
                }
            }
        }

        void LdLoc(int index)
        {
            stack.Push(locals[index]);
        }

        void StLoc(int index)
        {
            locals[index] = stack.Pop();
        }

        void LdArg(int index)
        {
            stack.Push(args[index]);
        }
    }
}