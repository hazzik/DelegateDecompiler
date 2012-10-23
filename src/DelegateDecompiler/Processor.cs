using System;
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
                    instruction = ConditionalBranch(instruction, val => Expression.Equal(val, val1));
                    continue;
                }
                else if (instruction.OpCode == OpCodes.Bne_Un ||
                         instruction.OpCode == OpCodes.Bne_Un_S)
                {
                    var val1 = stack.Pop();
                    instruction = ConditionalBranch(instruction, val => Expression.NotEqual(val, val1));
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
                    stack.Push(Expression.Convert(stack.Pop(), typeof (object)));
                }
                else if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt)
                {
                    Call((MethodInfo) instruction.Operand);
                }
                else if (instruction.OpCode == OpCodes.Ceq)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();

                    stack.Push(Expression.Equal(val2, AdjustType(val1, val2.Type)));
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

        static Expression Default(Type type)
        {
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

            var common = GetJoinPoint(left, right);

            var rightExpression = Clone().Process(right, common);
            var leftExpression = AdjustType(Clone().Process(left, common), rightExpression.Type);

            if (test.NodeType == ExpressionType.NotEqual)
            {
                if (val1 == leftExpression && (test.Right is DefaultExpression || (test.Right is ConstantExpression && ((ConstantExpression)test.Right).Value == null)))
                {
                    stack.Push(Expression.Coalesce(val1, rightExpression));
                    return common;
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
                            stack.Push(Expression.OrElse(test, rightExpression));
                        }
                        else
                        {
                            stack.Push(Expression.AndAlso(Expression.NotEqual(test.Left, test.Right), rightExpression));
                        }
                        return common;
                    }
                }
            }
            stack.Push(Expression.Condition(test, leftExpression, rightExpression));
            return common;
        }

        static Instruction GetJoinPoint(Instruction left, Instruction right)
        {
            var processed = new HashSet<Instruction>();
            while (left != null)
            {
                processed.Add(left);
                if (left.OpCode == OpCodes.Br_S || left.OpCode == OpCodes.Br)
                {
                    left = (Instruction) left.Operand;
                }
                else
                {
                    left = left.Next;
                }
            }
            while (right != null && !processed.Contains(right))
            {
                processed.Add(right);
                if (right.OpCode == OpCodes.Br_S || right.OpCode == OpCodes.Br)
                {
                    right = (Instruction)right.Operand;
                }
                else
                {
                    right = right.Next;
                }
            }
            return right;
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
            if (m.Name == "Concat" && m.DeclaringType == typeof(string))
            {
                var expression = arguments[0];
                for (var i = 1; i < arguments.Length; i++)
                {
                    expression = Expression.Add(expression, arguments[i], stringConcat);
                }
                return expression;
            }

            return Expression.Call(instance, m, arguments);
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