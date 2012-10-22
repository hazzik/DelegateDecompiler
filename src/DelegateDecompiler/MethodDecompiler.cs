using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;

namespace DelegateDecompiler
{
    public class MethodDecompiler
    {
        readonly IList<ParameterExpression> args;
        readonly Expression[] locals;
        readonly MethodInfo method;
        readonly Stack<Expression> stack;

        Expression ex;
        static readonly MethodInfo stringConcat = typeof (string).GetMethod("Concat", new[] { typeof (string), typeof (string) });

        public MethodDecompiler(MethodInfo method)
        {
            stack = new Stack<Expression>();
            locals = new Expression[0];
            ex = Expression.Empty();
            this.method = method;
            var parameters = method.GetParameters();
            if (method.IsStatic)
                args = parameters
                    .Select(p => Expression.Parameter(p.ParameterType, p.Name))
                    .ToList();
            else
                args = new[] { Expression.Parameter(method.DeclaringType) }.Union(
                    parameters
                        .Select(p => Expression.Parameter(p.ParameterType, p.Name)))
                                                                           .ToList();

            var body = method.GetMethodBody();
            locals = new Expression[body.LocalVariables.Count];
        }

        public LambdaExpression Decompile()
        {
            var instructions = method.GetInstructions();
            foreach (var instruction in instructions)
            {
                Console.WriteLine(instruction);

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
                    StLoc((byte)instruction.Operand);
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
                else if (instruction.OpCode == OpCodes.Br_S)
                {
                    //not implemented yet
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
                else if (instruction.OpCode == OpCodes.Ret)
                {
                    if (stack.Count == 0)
                        ex = Expression.Empty();
                    ex = stack.Pop();
                }
            }

            if (ex.Type != method.ReturnType && method.ReturnType != typeof(void))
                ex = Expression.Convert(ex, method.ReturnType);

            return Expression.Lambda(ex, args);
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
