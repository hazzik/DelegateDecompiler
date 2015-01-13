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
        private class ProcessorState
        {
            public IDictionary<FieldInfo, Address> Delegates { get; private set; }
            public Stack<Address> Stack { get; private set; }
            public VariableInfo[] Locals { get; private set; }
            public IList<Address> Args { get; private set; }
            public Instruction Last { get; private set; }
            public Action RunNext { get; set; }

            public Instruction Instruction { get; set; }

            public ProcessorState(Stack<Address> stack, VariableInfo[] locals, IList<Address> args, Instruction instruction, Instruction last = null)
            {
                Delegates = new Dictionary<FieldInfo, Address>();
                Stack = stack;
                Locals = locals;
                Args = args;
                Instruction = instruction;
                Last = last;
            }

            public ProcessorState Clone(Instruction instruction, Instruction last = null)
            {
                return new ProcessorState(new Stack<Address>(Stack), Locals.ToArray(), Args.ToArray(), instruction, last);
            }

            public Expression Final()
            {
                return Stack.Count == 0
                       ? Expression.Empty()
                       : Stack.Pop();
            }
        }
  
        const string cachedAnonymousMethodDelegate = "<>9__CachedAnonymousMethodDelegate";

        static readonly MethodInfo StringConcat = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) });

        public static Expression Process(VariableInfo[] locals, IList<Address> args, Instruction instruction, Type returnType)
        {
            Processor processor = new Processor();
            processor.states.Push(new ProcessorState(new Stack<Address>(), locals, args, instruction));

            var ex = AdjustType(processor.Process(), returnType);

            if (ex.Type != returnType && returnType != typeof(void))
            {
                return Expression.Convert(ex, returnType);
            }

            return ex;
        }

        readonly Stack<ProcessorState> states = new Stack<ProcessorState>();

        Processor()
        {
        }

        Expression Process()
        {
            ProcessorState state = null;
            while(states.Count > 0)
            {
                state = states.Peek();

                if(state.RunNext != null)
                {
                    state.RunNext();
                    state.RunNext = null;
                }

                if(state.Instruction != null && state.Instruction != state.Last)
                {
                    Debug.WriteLine(state.Instruction);

                    if (state.Instruction.OpCode == OpCodes.Nop || state.Instruction.OpCode == OpCodes.Break)
                    {
                        //do nothing;
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldarg_0)
                    {
                        LdArg(state, 0);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldarg_1)
                    {
                        LdArg(state, 1);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldarg_2)
                    {
                        LdArg(state, 2);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldarg_3)
                    {
                        LdArg(state, 3);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldarg_S || state.Instruction.OpCode == OpCodes.Ldarg || state.Instruction.OpCode == OpCodes.Ldarga || state.Instruction.OpCode == OpCodes.Ldarga_S)
                    {
                        var operand = (ParameterInfo) state.Instruction.Operand;
                        state.Stack.Push(state.Args.Single(x => ((ParameterExpression) x.Expression).Name == operand.Name));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldlen)
                    {
                        var array = state.Stack.Pop();
                        state.Stack.Push(Expression.ArrayLength(array));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldelem ||
                             state.Instruction.OpCode == OpCodes.Ldelem_I ||
                             state.Instruction.OpCode == OpCodes.Ldelem_I1 ||
                             state.Instruction.OpCode == OpCodes.Ldelem_I2 ||
                             state.Instruction.OpCode == OpCodes.Ldelem_I4 ||
                             state.Instruction.OpCode == OpCodes.Ldelem_I8 ||
                             state.Instruction.OpCode == OpCodes.Ldelem_U1 ||
                             state.Instruction.OpCode == OpCodes.Ldelem_U2 ||
                             state.Instruction.OpCode == OpCodes.Ldelem_U4 ||
                             state.Instruction.OpCode == OpCodes.Ldelem_R4 ||
                             state.Instruction.OpCode == OpCodes.Ldelem_R8 ||
                             state.Instruction.OpCode == OpCodes.Ldelem_Ref)
                    {
                        var index = state.Stack.Pop();
                        var array = state.Stack.Pop();
                        state.Stack.Push(Expression.ArrayIndex(array, index));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Stloc_0)
                    {
                        StLoc(state, 0);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Stloc_1)
                    {
                        StLoc(state, 1);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Stloc_2)
                    {
                        StLoc(state, 2);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Stloc_3)
                    {
                        StLoc(state, 3);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Stloc_S ||
                             state.Instruction.OpCode == OpCodes.Stloc)
                    {
                        var operand = (LocalVariableInfo) state.Instruction.Operand;
                        StLoc(state, operand.LocalIndex);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Stelem ||
                             state.Instruction.OpCode == OpCodes.Stelem_I ||
                             state.Instruction.OpCode == OpCodes.Stelem_I1 ||
                             state.Instruction.OpCode == OpCodes.Stelem_I2 ||
                             state.Instruction.OpCode == OpCodes.Stelem_I4 ||
                             state.Instruction.OpCode == OpCodes.Stelem_I8 ||
                             state.Instruction.OpCode == OpCodes.Stelem_R4 ||
                             state.Instruction.OpCode == OpCodes.Stelem_R8 ||
                             state.Instruction.OpCode == OpCodes.Stelem_Ref)
                    {
                        StElem(state);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldnull)
                    {
                        state.Stack.Push(Expression.Constant(null));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldfld || state.Instruction.OpCode == OpCodes.Ldflda)
                    {
                        var instance = state.Stack.Pop();
                        state.Stack.Push(Expression.Field(instance, (FieldInfo) state.Instruction.Operand));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldsfld)
                    {
                        var field = (FieldInfo) state.Instruction.Operand;
                        if (IsCachedAnonymousMethodDelegate(field))
                        {
                            Address address;
                            if (state.Delegates.TryGetValue(field, out address))
                            {
                                state.Stack.Push(address);
                            }
                            else
                            {
                                state.Stack.Push(Expression.Field(null, field));
                            }
                        }
                        else
                        {
                            state.Stack.Push(Expression.Field(null, field));
                        }
                    }
                    else if (state.Instruction.OpCode == OpCodes.Stsfld)
                    {
                        var field = (FieldInfo) state.Instruction.Operand;
                        if (IsCachedAnonymousMethodDelegate(field))
                        {
                            state.Delegates[field] = state.Stack.Pop();
                        }
                        else
                        {
                            var pop = state.Stack.Pop();
                            state.Stack.Push(Expression.Assign(Expression.Field(null, field), pop));
                        }
                    }
                    else if (state.Instruction.OpCode == OpCodes.Stfld)
                    {
                        var value = state.Stack.Pop();
                        var instance = state.Stack.Pop();
                        var field = (FieldInfo)state.Instruction.Operand;
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
                    else if (state.Instruction.OpCode == OpCodes.Ldloc_0)
                    {
                        LdLoc(state, 0);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldloc_1)
                    {
                        LdLoc(state, 1);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldloc_2)
                    {
                        LdLoc(state, 2);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldloc_3)
                    {
                        LdLoc(state, 3);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldloc ||
                             state.Instruction.OpCode == OpCodes.Ldloc_S ||
                             state.Instruction.OpCode == OpCodes.Ldloca || 
                             state.Instruction.OpCode == OpCodes.Ldloca_S)
                    {
                        var operand = (LocalVariableInfo) state.Instruction.Operand;
                        LdLoc(state, operand.LocalIndex);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldstr)
                    {
                        state.Stack.Push(Expression.Constant((string) state.Instruction.Operand));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldc_I4_0)
                    {
                        LdC(state, 0);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldc_I4_1)
                    {
                        LdC(state, 1);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldc_I4_2)
                    {
                        LdC(state, 2);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldc_I4_3)
                    {
                        LdC(state, 3);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldc_I4_4)
                    {
                        LdC(state, 4);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldc_I4_5)
                    {
                        LdC(state, 5);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldc_I4_6)
                    {
                        LdC(state, 6);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldc_I4_7)
                    {
                        LdC(state, 7);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldc_I4_8)
                    {
                        LdC(state, 8);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldc_I4_S)
                    {
                        LdC(state, (sbyte)state.Instruction.Operand);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldc_I4_M1)
                    {
                        LdC(state, -1);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldc_I4)
                    {
                        LdC(state, (int)state.Instruction.Operand);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldc_I8)
                    {
                        LdC(state, (long)state.Instruction.Operand);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldc_R4)
                    {
                        LdC(state, (float)state.Instruction.Operand);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldc_R8)
                    {
                        LdC(state, (double)state.Instruction.Operand);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Br_S || state.Instruction.OpCode == OpCodes.Br)
                    {
                        state.Instruction = (Instruction) state.Instruction.Operand;
                        continue;
                    }
                    else if (state.Instruction.OpCode == OpCodes.Brfalse ||
                             state.Instruction.OpCode == OpCodes.Brfalse_S)
                    {
                        state.Instruction = ConditionalBranch(state, val => Expression.Equal(val, Default(val.Type)));
                        continue;
                    }
                    else if (state.Instruction.OpCode == OpCodes.Brtrue ||
                             state.Instruction.OpCode == OpCodes.Brtrue_S)
                    {
                        var address = state.Stack.Peek();
                        var memberExpression = address.Expression as MemberExpression;
                        if (memberExpression != null && IsCachedAnonymousMethodDelegate(memberExpression.Member as FieldInfo))
                        {
                            state.Stack.Pop();
                        }
                        else
                        {
                            state.Instruction = ConditionalBranch(state, val => Expression.NotEqual(val, Default(val.Type)));
                            continue;
                        }
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ldftn)
                    {
                        var method = (MethodInfo) state.Instruction.Operand;
                        var decompile = method.Decompile();

                        var obj = state.Stack.Pop();
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

                        state.Stack.Push(decompile);
                        state.Instruction = state.Instruction.Next;
                    }
                    else if (state.Instruction.OpCode == OpCodes.Bgt ||
                             state.Instruction.OpCode == OpCodes.Bgt_S ||
                             state.Instruction.OpCode == OpCodes.Bgt_Un ||
                             state.Instruction.OpCode == OpCodes.Bgt_Un_S)
                    {
                        var val1 = state.Stack.Pop();
                        state.Instruction = ConditionalBranch(state, val => Expression.GreaterThan(val, val1));
                        continue;
                    }
                    else if (state.Instruction.OpCode == OpCodes.Bge ||
                             state.Instruction.OpCode == OpCodes.Bge_S ||
                             state.Instruction.OpCode == OpCodes.Bge_Un ||
                             state.Instruction.OpCode == OpCodes.Bge_Un_S)
                    {
                        var val1 = state.Stack.Pop();
                        state.Instruction = ConditionalBranch(state, val => Expression.GreaterThanOrEqual(val, val1));
                        continue;
                    }
                    else if (state.Instruction.OpCode == OpCodes.Blt ||
                             state.Instruction.OpCode == OpCodes.Blt_S ||
                             state.Instruction.OpCode == OpCodes.Blt_Un ||
                             state.Instruction.OpCode == OpCodes.Blt_Un_S)
                    {
                        var val1 = state.Stack.Pop();
                        state.Instruction = ConditionalBranch(state, val => Expression.LessThan(val, val1));
                        continue;
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ble ||
                             state.Instruction.OpCode == OpCodes.Ble_S ||
                             state.Instruction.OpCode == OpCodes.Ble_Un ||
                             state.Instruction.OpCode == OpCodes.Ble_Un_S)
                    {
                        var val1 = state.Stack.Pop();
                        state.Instruction = ConditionalBranch(state, val => Expression.LessThanOrEqual(val, val1));
                        continue;
                    }
                    else if (state.Instruction.OpCode == OpCodes.Beq ||
                             state.Instruction.OpCode == OpCodes.Beq_S)
                    {
                        var val1 = state.Stack.Pop();
                        state.Instruction = ConditionalBranch(state, val => AdjustedBinaryExpression(val, val1, ExpressionType.Equal));
                        continue;
                    }
                    else if (state.Instruction.OpCode == OpCodes.Bne_Un ||
                             state.Instruction.OpCode == OpCodes.Bne_Un_S)
                    {
                        var val1 = state.Stack.Pop();
                        state.Instruction = ConditionalBranch(state, val => AdjustedBinaryExpression(val, val1, ExpressionType.NotEqual));
                        continue;
                    }
                    else if (state.Instruction.OpCode == OpCodes.Dup)
                    {
                        state.Stack.Push(state.Stack.Peek());
                    }
                    else if (state.Instruction.OpCode == OpCodes.Pop)
                    {
                        state.Stack.Pop();
                    }
                    else if (state.Instruction.OpCode == OpCodes.Add)
                    {
                        var val1 = state.Stack.Pop();
                        var val2 = state.Stack.Pop();
                        state.Stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.Add));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Add_Ovf || state.Instruction.OpCode == OpCodes.Add_Ovf_Un)
                    {
                        var val1 = state.Stack.Pop();
                        var val2 = state.Stack.Pop();
                        state.Stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.AddChecked));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Sub)
                    {
                        var val1 = state.Stack.Pop();
                        var val2 = state.Stack.Pop();
                        state.Stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.Subtract));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Sub_Ovf || state.Instruction.OpCode == OpCodes.Sub_Ovf_Un)
                    {
                        var val1 = state.Stack.Pop();
                        var val2 = state.Stack.Pop();
                        state.Stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.SubtractChecked));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Mul)
                    {
                        var val1 = state.Stack.Pop();
                        var val2 = state.Stack.Pop();
                        state.Stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.Multiply));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Mul_Ovf || state.Instruction.OpCode == OpCodes.Mul_Ovf_Un)
                    {
                        var val1 = state.Stack.Pop();
                        var val2 = state.Stack.Pop();
                        state.Stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.MultiplyChecked));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Div || state.Instruction.OpCode == OpCodes.Div_Un)
                    {
                        var val1 = state.Stack.Pop();
                        var val2 = state.Stack.Pop();
                        state.Stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.Divide));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Rem || state.Instruction.OpCode == OpCodes.Rem_Un)
                    {
                        var val1 = state.Stack.Pop();
                        var val2 = state.Stack.Pop();
                        state.Stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.Modulo));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Xor)
                    {
                        var val1 = state.Stack.Pop();
                        var val2 = state.Stack.Pop();
                        state.Stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.ExclusiveOr));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Shl)
                    {
                        var val1 = state.Stack.Pop();
                        var val2 = state.Stack.Pop();
                        state.Stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.LeftShift));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Shr || state.Instruction.OpCode == OpCodes.Shr_Un)
                    {
                        var val1 = state.Stack.Pop();
                        var val2 = state.Stack.Pop();
                        state.Stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.RightShift));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Neg)
                    {
                        var val = state.Stack.Pop();
                        state.Stack.Push(Expression.Negate(val));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Not)
                    {
                        var val = state.Stack.Pop();
                        state.Stack.Push(Expression.Not(val));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_I)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.Convert(val1, typeof (int))); // Support x64?
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_I1)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.Convert(val1, typeof (sbyte)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_I2)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.Convert(val1, typeof (short)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_I4)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.Convert(val1, typeof (int)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_I8)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.Convert(val1, typeof (long)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_U)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.Convert(val1, typeof (uint))); // Suppot x64?
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_U1)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.Convert(val1, typeof (byte)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_U2)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.Convert(val1, typeof (ushort)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_U4)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.Convert(val1, typeof (uint)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_U8)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.Convert(val1, typeof (ulong)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_Ovf_I || state.Instruction.OpCode == OpCodes.Conv_Ovf_I_Un)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.ConvertChecked(val1, typeof (int))); // Suppot x64?
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_Ovf_I1 || state.Instruction.OpCode == OpCodes.Conv_Ovf_I1_Un)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.ConvertChecked(val1, typeof (sbyte)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_Ovf_I2 || state.Instruction.OpCode == OpCodes.Conv_Ovf_I2_Un)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.ConvertChecked(val1, typeof (short)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_Ovf_I4 || state.Instruction.OpCode == OpCodes.Conv_Ovf_I4_Un)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.ConvertChecked(val1, typeof (int)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_Ovf_I8 || state.Instruction.OpCode == OpCodes.Conv_Ovf_I8_Un)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.ConvertChecked(val1, typeof (long)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_Ovf_U || state.Instruction.OpCode == OpCodes.Conv_Ovf_U_Un)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.ConvertChecked(val1, typeof (uint))); // Suppot x64?
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_Ovf_U1 || state.Instruction.OpCode == OpCodes.Conv_Ovf_U1_Un)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.ConvertChecked(val1, typeof (byte)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_Ovf_U2 || state.Instruction.OpCode == OpCodes.Conv_Ovf_U2_Un)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.ConvertChecked(val1, typeof (ushort)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_Ovf_U4 || state.Instruction.OpCode == OpCodes.Conv_Ovf_U4_Un)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.ConvertChecked(val1, typeof (uint)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_Ovf_U8 || state.Instruction.OpCode == OpCodes.Conv_Ovf_U8_Un)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.ConvertChecked(val1, typeof (ulong)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_R4 || state.Instruction.OpCode == OpCodes.Conv_R_Un)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.ConvertChecked(val1, typeof (float)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Conv_R8)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.ConvertChecked(val1, typeof (double)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Castclass)
                    {
                        var val1 = state.Stack.Pop();
                        state.Stack.Push(Expression.Convert(val1, (Type) state.Instruction.Operand));
                    }
                    else if (state.Instruction.OpCode == OpCodes.And)
                    {
                        var val1 = state.Stack.Pop();
                        var val2 = state.Stack.Pop();
                        state.Stack.Push(Expression.And(val2, val1));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Or)
                    {
                        var val1 = state.Stack.Pop();
                        var val2 = state.Stack.Pop();
                        state.Stack.Push(Expression.Or(val2, val1));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Newobj)
                    {
                        var constructor = (ConstructorInfo) state.Instruction.Operand;
                        state.Stack.Push(Expression.New(constructor, GetArguments(state, constructor)));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Initobj)
                    {
                        var address = state.Stack.Pop();
                        var type = (Type) state.Instruction.Operand;
                        address.Expression = Default(type);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Newarr)
                    {
                        var operand = (Type) state.Instruction.Operand;
                        var expression = state.Stack.Pop();
                        var size = expression.Expression as ConstantExpression;
                        if (size != null && (int) size.Value == 0) // optimization
                            state.Stack.Push(Expression.NewArrayInit(operand));
                        else
                            state.Stack.Push(Expression.NewArrayBounds(operand, expression));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Box)
                    {
                        state.Stack.Push(Box(state.Stack.Pop(), (Type) state.Instruction.Operand));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Call || state.Instruction.OpCode == OpCodes.Callvirt)
                    {
                        Call(state, (MethodInfo)state.Instruction.Operand);
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ceq)
                    {
                        var val1 = state.Stack.Pop();
                        var val2 = state.Stack.Pop();

                        state.Stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.Equal));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Cgt || state.Instruction.OpCode == OpCodes.Cgt_Un)
                    {
                        var val1 = state.Stack.Pop();
                        var val2 = state.Stack.Pop();

                        state.Stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.GreaterThan));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Clt || state.Instruction.OpCode == OpCodes.Clt_Un)
                    {
                        var val1 = state.Stack.Pop();
                        var val2 = state.Stack.Pop();

                        state.Stack.Push(AdjustedBinaryExpression(val2, AdjustType(val1, val2.Type), ExpressionType.LessThan));
                    }
                    else if (state.Instruction.OpCode == OpCodes.Ret)
                    {
                        states.Pop();
                    }
                    else
                    {
                        Debug.WriteLine("Unhandled!!!");
                    }

                    state.Instruction = state.Instruction.Next;
                }
                else
                {
                    states.Pop();
                }
            }

            return state == null ? Expression.Empty() : state.Final();
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

        Instruction ConditionalBranch(ProcessorState state, Func<Expression, BinaryExpression> condition)
        {
            var val1 = state.Stack.Pop();
            var test = condition(val1);

            var left = (Instruction) state.Instruction.Operand;
            var right = state.Instruction.Next;

            Instruction common = GetJoinPoint(left, right);

            var rightState = state.Clone(right, common);
            var leftState = state.Clone(left, common);
            states.Push(rightState);
            states.Push(leftState);

            // Run this once the conditional branches have been processed
            state.RunNext = () =>
            {
                var rightExpression = rightState.Final();
                var leftExpression = leftState.Final();
                leftExpression = AdjustType(leftExpression, rightExpression.Type);
                rightExpression = AdjustType(rightExpression, leftExpression.Type);

                var expression = Expression.Condition(test, leftExpression, rightExpression);
                state.Stack.Push(expression);
            };

            return common;
        }

        private class JoinPointState
        {
            public Instruction Left { get; set; }
            public Instruction Right { get; set; }
            public Instruction Common { get; set; }
            public Stack<Instruction> LeftInstructions { get; private set; }
            public Stack<Instruction> RightInstructions { get; private set; }
            public JoinPointState Pending { get; set; }

            public JoinPointState(Instruction left, Instruction right)
            {
                Left = left;
                Right = right;
                LeftInstructions = new Stack<Instruction>();
                RightInstructions = new Stack<Instruction>();
            }

            public Instruction Current
            {
                get { return Left != null ? Left : Right; }
                set
                {
                    if(Left != null)
                    {
                        Left = value;
                    }
                    else
                    {
                        Right = value;
                    }
                }
            }

            public Stack<Instruction> CurrentInstructions
            {
                get { return Left != null ? LeftInstructions : RightInstructions; }
            }
        }

        static Instruction GetJoinPoint(Instruction left, Instruction right)
        {
            Stack<JoinPointState> joinPointStates = new Stack<JoinPointState>();
            joinPointStates.Push(new JoinPointState(left, right));
            JoinPointState joinPointState = null;
            while(joinPointStates.Count > 0)
            {
                joinPointState = joinPointStates.Peek();
                               
                // See if both flows have been computed (right goes second) and if so, get the common join point
                if(joinPointState.Right == null)
                {
                    foreach (var leftInstruction in joinPointState.LeftInstructions)
                    {
                        if (joinPointState.RightInstructions.Count <= 0 || leftInstruction != joinPointState.RightInstructions.Pop())
                        {
                            break;
                        }
                        joinPointState.Common = leftInstruction;
                    }
                    joinPointStates.Pop();
                }
                else
                {
                    // Check if we were waiting on a pending operation
                    if (joinPointState.Pending != null)
                    {
                        joinPointState.Current = joinPointState.Pending.Common;
                        joinPointState.Pending = null;
                    }
                    else
                    {

                        joinPointState.CurrentInstructions.Push(joinPointState.Current);

                        if (joinPointState.Current.OpCode.FlowControl == FlowControl.Return)
                        {
                            joinPointState.Current = null;
                        }
                        else if (joinPointState.Current.OpCode.FlowControl == FlowControl.Branch)
                        {
                            joinPointState.Current = (Instruction)joinPointState.Current.Operand;
                        }
                        else if (joinPointState.Current.OpCode.FlowControl == FlowControl.Cond_Branch)
                        {
                            joinPointState.Pending = new JoinPointState((Instruction)joinPointState.Current.Operand, joinPointState.Current.Next);
                            joinPointStates.Push(joinPointState.Pending);
                        }
                        else
                        {
                            joinPointState.Current = joinPointState.Current.Next;
                        }
                    }
                }
            }

            return joinPointState.Common;
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

        static void StElem(ProcessorState state)
        {
            var value = state.Stack.Pop();
            var index = state.Stack.Pop();
            var array = state.Stack.Pop();

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

        static void LdC(ProcessorState state, int i)
        {
            state.Stack.Push(Expression.Constant(i));
        }

        static void LdC(ProcessorState state, long i)
        {
            state.Stack.Push(Expression.Constant(i));
        }

        static void LdC(ProcessorState state, float i)
        {
            state.Stack.Push(Expression.Constant(i));
        }

        static void LdC(ProcessorState state, double i)
        {
            state.Stack.Push(Expression.Constant(i));
        }

        static void Call(ProcessorState state, MethodInfo m)
        {
            var mArgs = GetArguments(state, m);

            var instance = m.IsStatic ? new Address() : state.Stack.Pop();
            state.Stack.Push(BuildMethodCallExpression(m, instance, mArgs));
        }

        static Expression[] GetArguments(ProcessorState state, MethodBase m)
        {
            var parameterInfos = m.GetParameters();
            var mArgs = new Expression[parameterInfos.Length];
            for (var i = parameterInfos.Length - 1; i >= 0; i--)
            {
                mArgs[i] = state.Stack.Pop();
            }
            return mArgs;
        }

        static Expression BuildMethodCallExpression(MethodInfo m, Address instance, Expression[] arguments)
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

        static void LdLoc(ProcessorState state, int index)
        {
            state.Stack.Push(state.Locals[index].Address);
        }

        static void StLoc(ProcessorState state, int index)
        {
            var info = state.Locals[index];
            info.Address = AdjustType(state.Stack.Pop(), info.Type);
        }

        static void LdArg(ProcessorState state, int index)
        {
            state.Stack.Push(state.Args[index]);
        }
    }
}
