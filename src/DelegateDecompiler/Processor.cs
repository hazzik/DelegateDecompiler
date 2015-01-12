using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;
using System.Runtime.CompilerServices;

namespace DelegateDecompiler
{
    class Processor
    {
        IDictionary<FieldInfo, Address> delegates = new Dictionary<FieldInfo, Address>();
  
        const string cachedAnonymousMethodDelegate = "<>9__CachedAnonymousMethodDelegate";

        public static Processor Create(VariableInfo[] locals, IList<Address> args)
        {
            return new Processor(new Stack<Address>(), locals, args);
        }

        Processor Clone()
        {
            return new Processor(new Stack<Address>(stack), locals.ToArray(), args.ToArray());
        }

        static readonly MethodInfo StringConcat = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) });

        readonly Stack<Address> stack;
        readonly VariableInfo[] locals;
        readonly IList<Address> args;

        Processor(Stack<Address> stack, VariableInfo[] locals, IList<Address> args)
        {
            this.stack = stack;
            this.locals = locals;
            this.args = args;
        }

        public Expression Process(Instruction instruction, Type returnType)
        {
            var ex = AdjustType(Process(instruction), returnType);

            if (ex.Type != returnType && returnType != typeof(void))
            {
                return Expression.Convert(ex, returnType);
            }

            return ex;
        }

        public Expression Process(Instruction instruction, Instruction last = null)
        {
            while(instruction != null && instruction != last)
            {
                Debug.WriteLine(instruction);

                if (instruction.OpCode == OpCodes.Nop || instruction.OpCode == OpCodes.Break)
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
                else if (instruction.OpCode == OpCodes.Ldarg_S || instruction.OpCode == OpCodes.Ldarg || instruction.OpCode == OpCodes.Ldarga || instruction.OpCode == OpCodes.Ldarga_S)
                {
                    var operand = (ParameterInfo) instruction.Operand;
                    stack.Push(args.Single(x => ((ParameterExpression) x.Expression).Name == operand.Name));
                }
                else if (instruction.OpCode == OpCodes.Ldlen)
                {
                    var array = stack.Pop();
                    stack.Push(Expression.ArrayLength(array));
                }
                else if (instruction.OpCode == OpCodes.Ldelem ||
                         instruction.OpCode == OpCodes.Ldelem_I ||
                         instruction.OpCode == OpCodes.Ldelem_I1 ||
                         instruction.OpCode == OpCodes.Ldelem_I2 ||
                         instruction.OpCode == OpCodes.Ldelem_I4 ||
                         instruction.OpCode == OpCodes.Ldelem_I8 ||
                         instruction.OpCode == OpCodes.Ldelem_U1 ||
                         instruction.OpCode == OpCodes.Ldelem_U2 ||
                         instruction.OpCode == OpCodes.Ldelem_U4 ||
                         instruction.OpCode == OpCodes.Ldelem_R4 ||
                         instruction.OpCode == OpCodes.Ldelem_R8 ||
                         instruction.OpCode == OpCodes.Ldelem_Ref)
                {
                    var index = stack.Pop();
                    var array = stack.Pop();
                    stack.Push(Expression.ArrayIndex(array, index));
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
                else if (instruction.OpCode == OpCodes.Stloc_S || instruction.OpCode == OpCodes.Stloc)
                {
                    StLoc(((LocalVariableInfo)instruction.Operand).LocalIndex);
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
                else if (instruction.OpCode == OpCodes.Ldnull)
                {
                    stack.Push(Expression.Constant(null));
                }
                else if (instruction.OpCode == OpCodes.Ldfld || instruction.OpCode == OpCodes.Ldflda)
                {
                    var instance = stack.Pop();
                    stack.Push(Expression.Field(instance, (FieldInfo) instruction.Operand));
                }
                else if (instruction.OpCode == OpCodes.Ldsfld)
                {
                    var field = (FieldInfo) instruction.Operand;
                    if (IsCachedAnonymousMethodDelegate(field))
                    {
                        Address address;
                        if (delegates.TryGetValue(field, out address))
                        {
                            stack.Push(address);
                        }
                        else
                        {
                            stack.Push(Expression.Field(null, field));
                        }
                    }
                    else
                    {
                        stack.Push(Expression.Field(null, field));
                    }
                }
                else if (instruction.OpCode == OpCodes.Stsfld)
                {
                    var field = (FieldInfo) instruction.Operand;
                    if (IsCachedAnonymousMethodDelegate(field))
                    {
                        delegates[field] = stack.Pop();
                    }
                    else
                    {
                        var pop = stack.Pop();
                        stack.Push(Expression.Assign(Expression.Field(null, field), pop));
                    }
                }
                else if (instruction.OpCode == OpCodes.Stfld)
                {
                    var value = stack.Pop();
                    var instance = stack.Pop();
                    var field = (FieldInfo)instruction.Operand;
                    var newExpression = instance.Expression as NewExpression;
                    if (newExpression != null)
                    {
                        instance.Expression = Expression.MemberInit(newExpression, Expression.Bind(field, value));
                    }
                    else
                    {
                        var memberInitExpression = instance.Expression as MemberInitExpression;
                        if (memberInitExpression != null)
                        {
                            var expression = memberInitExpression.NewExpression;
                            var bindings = new List<MemberBinding>(memberInitExpression.Bindings) {Expression.Bind(field, value)};
                            instance.Expression = Expression.MemberInit(expression, bindings);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }
                    }
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
                else if (instruction.OpCode == OpCodes.Brfalse ||
                         instruction.OpCode == OpCodes.Brfalse_S)
                {
                    instruction = ConditionalBranch(instruction, val => Expression.Equal(val, Default(val.Type)));
                    continue;
                }
                else if (instruction.OpCode == OpCodes.Brtrue ||
                         instruction.OpCode == OpCodes.Brtrue_S)
                {
                    var address = stack.Peek();
                    var memberExpression = address.Expression as MemberExpression;
                    if (memberExpression != null && IsCachedAnonymousMethodDelegate(memberExpression.Member as FieldInfo))
                    {
                        stack.Pop();
                    }
                    else
                    {
                        instruction = ConditionalBranch(instruction, val => Expression.NotEqual(val, Default(val.Type)));
                        continue;
                    }
                }
                else if (instruction.OpCode == OpCodes.Ldftn)
                {
                    var method = (MethodInfo) instruction.Operand;
                    var decompile = method.Decompile();

                    var obj = stack.Pop();
                    if (!method.IsStatic)
                    {
                        var expressions = new Dictionary<Expression, Expression>
                        {
                            {decompile.Parameters[0], obj}
                        };

                        var body = new ReplaceExpressionVisitor(expressions).Visit(decompile.Body);
                        body = TransparentIdentifierRemovingExpressionVisitor.RemoveTransparentIdentifiers(body);
                        decompile = Expression.Lambda(body, decompile.Parameters.Skip(1));
                    }

                    stack.Push(decompile);
                    instruction = instruction.Next;
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
                    stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.Add));
                }
                else if (instruction.OpCode == OpCodes.Add_Ovf || instruction.OpCode == OpCodes.Add_Ovf_Un)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.AddChecked));
                }
                else if (instruction.OpCode == OpCodes.Sub)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.Subtract));
                }
                else if (instruction.OpCode == OpCodes.Sub_Ovf || instruction.OpCode == OpCodes.Sub_Ovf_Un)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.SubtractChecked));
                }
                else if (instruction.OpCode == OpCodes.Mul)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.Multiply));
                }
                else if (instruction.OpCode == OpCodes.Mul_Ovf || instruction.OpCode == OpCodes.Mul_Ovf_Un)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.MultiplyChecked));
                }
                else if (instruction.OpCode == OpCodes.Div || instruction.OpCode == OpCodes.Div_Un)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.Divide));
                }
                else if (instruction.OpCode == OpCodes.Rem || instruction.OpCode == OpCodes.Rem_Un)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.Modulo));
                }
                else if (instruction.OpCode == OpCodes.Xor)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.ExclusiveOr));
                }
                else if (instruction.OpCode == OpCodes.Shl)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.LeftShift));
                }
                else if (instruction.OpCode == OpCodes.Shr || instruction.OpCode == OpCodes.Shr_Un)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();
                    stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.RightShift));
                }
                else if (instruction.OpCode == OpCodes.Neg)
                {
                    var val = stack.Pop();
                    stack.Push(Expression.Negate(val));
                }
                else if (instruction.OpCode == OpCodes.Not)
                {
                    var val = stack.Pop();
                    stack.Push(Expression.Not(val));
                }
                else if (instruction.OpCode == OpCodes.Conv_I)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.Convert(val1, typeof (int))); // Support x64?
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
                else if (instruction.OpCode == OpCodes.Castclass)
                {
                    var val1 = stack.Pop();
                    stack.Push(Expression.Convert(val1, (Type) instruction.Operand));
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
                    var constructor = (ConstructorInfo) instruction.Operand;
                    stack.Push(Expression.New(constructor, GetArguments(constructor)));
                }
                else if (instruction.OpCode == OpCodes.Initobj)
                {
                    var address = stack.Pop();
                    var type = (Type) instruction.Operand;
                    address.Expression = Default(type);
                }
                else if (instruction.OpCode == OpCodes.Newarr)
                {
                    var operand = (Type) instruction.Operand;
                    var expression = stack.Pop();
                    var size = expression.Expression as ConstantExpression;
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

                    stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.GreaterThan));
                }
                else if (instruction.OpCode == OpCodes.Clt || instruction.OpCode == OpCodes.Clt_Un)
                {
                    var val1 = stack.Pop();
                    var val2 = stack.Pop();

                    stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.LessThan));
                }
                else if (instruction.OpCode == OpCodes.Ret)
                {
                    break;
                }
                else
                {
                    Debug.WriteLine("Unhandled!!!");
                }

                instruction = instruction.Next;
            }

            return stack.Count == 0
                       ? Expression.Empty()
                       : stack.Pop();
        }

        private static bool IsCachedAnonymousMethodDelegate(FieldInfo field)
        {
            return field != null &&
                field.Name.Contains(cachedAnonymousMethodDelegate) /*&&
                Attribute.IsDefined(field, typeof (CompilerGeneratedAttribute), false)*/;
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
            if (type.IsValueType)
            {
                // LINQ to entities and possibly other providers don't support Expression.Default, so this gets the default value and then uses an Expression.Constant instead
                return Expression.Constant(Activator.CreateInstance(type), type);
            }

            return Expression.Constant(null, type);
        }

        Instruction ConditionalBranch(Instruction instruction, Func<Expression, BinaryExpression> condition)
        {
            var val1 = stack.Pop();
            var test = condition(val1);

            var left = (Instruction) instruction.Operand;
            var right = instruction.Next;

            Instruction common = GetJoinPoint(left, right);

            var rightExpression = Clone().Process(right, common);
            var leftExpression = Clone().Process(left, common);
            leftExpression = AdjustType(leftExpression, rightExpression.Type);
            rightExpression = AdjustType(rightExpression, leftExpression.Type);

            var expression = Expression.Condition(test, leftExpression, rightExpression);
            stack.Push(expression);
            return common;
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

                if (instruction.OpCode.FlowControl == FlowControl.Return)
                    break;

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
            var constantExpression = expression as ConstantExpression;
            if (constantExpression != null)
            {
                if (constantExpression.Value == null)
                {
                    return Expression.Constant(null, type);
                }
                if (expression.Type == typeof (int) && type == typeof (bool))
                {
                    return Expression.Constant(!Equals(constantExpression.Value, 0));
                }
            }
            else
            {
                if (expression.Type == typeof (int) && type == typeof (bool))
                {
                    return Expression.NotEqual(expression, Expression.Constant(0));
                }
            }
            if (!type.IsAssignableFrom(expression.Type) && expression.Type.IsEnum && expression.Type.GetEnumUnderlyingType() == type)
            {
                return Expression.Convert(expression, type);
            }
            return expression;
        }

        void StElem()
        {
            var value = stack.Pop();
            var index = stack.Pop();
            var array = stack.Pop();

            var newArray = array.Expression as NewArrayExpression;
            if (newArray != null)
            {
                var expressions = CreateArrayInitExpressions(newArray, value);
                var newArrayInit = Expression.NewArrayInit(array.Type.GetElementType(), expressions);
                array.Expression = newArrayInit;
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

            var instance = m.IsStatic ? new Address() : stack.Pop();
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

        Expression BuildMethodCallExpression(MethodInfo m, Address instance, Expression[] arguments)
        {
            if (m.Name == "Add" && instance.Expression != null && typeof(IEnumerable).IsAssignableFrom(instance.Type))
            {
                var newExpression = instance.Expression as NewExpression;
                if (newExpression != null)
                {
                    var init = Expression.ListInit(newExpression, Expression.ElementInit(m, arguments));
                    instance.Expression = init;
                    return instance;
                }
                var initExpression = instance.Expression as ListInitExpression;
                if (initExpression != null)
                {
                    var initializers = initExpression.Initializers.ToList();
                    initializers.Add(Expression.ElementInit(m, arguments));
                    var init = Expression.ListInit(initExpression.NewExpression, initializers);
                    instance.Expression = init;
                    return instance;
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
                    if (TryParseOperator(m, out type))
                    {
                        switch (arguments.Length)
                        {
                            case 1:
                                return Expression.MakeUnary(type, arguments[0], arguments[0].Type);
                            case 2:
                                return Expression.MakeBinary(type, arguments[0], arguments[1]);
                        }
                    }
                    else
                    {
                        switch (m.Name)
                        {
                            case "op_Increment":
                                return Expression.Add(arguments[0], Expression.Constant(Convert.ChangeType(1, arguments[0].Type)));
                            case "op_Decrement":
                                return Expression.Subtract(arguments[0], Expression.Constant(Convert.ChangeType(1, arguments[0].Type)));
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
                        expression = Expression.Add(expression, expressions[i], StringConcat);
                    }
                    return expression;
                }
            }
            if (m.Name == "InitializeArray" && m.DeclaringType == typeof (RuntimeHelpers))
            {
                
            }

            var parameters = m.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                var parameter = parameters[i];
                var argument = arguments[i];
                var parameterType = parameter.ParameterType;
                arguments[i] = AdjustType(argument, parameterType);
            }

            if (instance.Expression != null)
                return Expression.Call(AdjustType(instance, m.DeclaringType), m, arguments);

            return Expression.Call(null, m, arguments);
        }

        private static bool TryParseOperator(MethodInfo m, out ExpressionType type)
        {
            switch (m.Name)
            {
                case "op_Increment":
                    type = default(ExpressionType);
                    return false;

                case "op_Decrement":
                    type = default(ExpressionType);
                    return false;

                case "op_Division":
                    type = ExpressionType.Divide;
                    return true;
                
                case "op_Equality":
                    type = ExpressionType.Equal;
                    return true;

                case "op_Inequality":
                    type = ExpressionType.NotEqual;
                    return true;
            }
            return Enum.TryParse(m.Name.Substring(3), out type);
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

        void LdLoc(int index)
        {
            stack.Push(locals[index].Address);
        }

        void StLoc(int index)
        {
            var info = locals[index];
            info.Address = AdjustType(stack.Pop(), info.Type);
        }

        void LdArg(int index)
        {
            stack.Push(args[index]);
        }
    }
}