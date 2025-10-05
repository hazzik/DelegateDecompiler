using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Mono.Reflection;
using System.Runtime.CompilerServices;
using DelegateDecompiler.ControlFlow;
using DelegateDecompiler.Processors;

namespace DelegateDecompiler
{
    internal class Processor
    {
        const string cachedAnonymousMethodDelegate = "CS$<>9__CachedAnonymousMethodDelegate";
        const string cachedAnonymousMethodDelegateRoslyn = "<>9__";

        static readonly MethodInfo StringConcat = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) });

        //TODO: Move to ProcessorState??
        static readonly ConcurrentDictionary<MethodInfo, LambdaExpression> AnonymousDelegatesCache =
            new ConcurrentDictionary<MethodInfo, LambdaExpression>();

        public static Expression Process(ControlFlowGraph cfg, VariableInfo[] locals, IList<Address> args, Type returnType)
        {
            var processor = new Processor();
            var initialState = new ProcessorState(cfg.Method.IsStatic, new Stack<Address>(), locals, args);
            
            processor.ProcessBlock(initialState, cfg.Entry);
            
            var ex = initialState.Final();
            ex = AdjustType(ex, returnType);

            if (!returnType.IsAssignableFrom(ex.Type) && returnType != typeof(void))
                return Expression.Convert(ex, returnType);

            return ex;
        }

        static readonly IProcessor[] Processors =
        {
            new ConvertProcessor(),
            new BinaryExpressionProcessor(),
            new UnaryExpressionProcessor(),
            new CgtUnProcessor(),
            new LdargProcessor(),
            new StargProcessor(),
            new LdelemProcessor(),
            new LdlenProcessor(),
            new LdlocProcessor(),
            new StlocProcessor(),
            new ConstantProcessor(),
            new StackProcessor(),
            new ObjectProcessor(),
            new LdfldProcessor(),
            new StfldProcessor(),
            new StsfldProcessor(),
            new StelemProcessor(),
            // This should be last one
            new UnsupportedOpcodeProcessor()
        };

        Processor()
        {
        }

        void ProcessBlock(ProcessorState state, Block block, Block endBlock = null)
        {
            if (block == null || block == endBlock)
                return;

            for (var index = 0; index < block.Instructions.Count; index++)
            {
                var instruction = block.Instructions[index];
                index += ProcessInstruction(state, instruction);
            }

            ProcessNextBlock(state, block, endBlock);
        }

        void ProcessNextBlock(ProcessorState state, Block block, Block endBlock)
        {
            switch (block.Successors.Count)
            {
                case 0:
                    break;
                case 1:
                    ProcessBlock(state, block.Successors[0].To, endBlock);
                    break;
                case 2:
                    ProcessConditionalBranch(block, state, endBlock);
                    break;
                default:
                    throw new NotSupportedException("Switch statement is not supported");
            }
        }

        void ProcessConditionalBranch(Block block, ProcessorState state, Block endBlock)
        {
            var trueEdge = block.Successors.FirstOrDefault(e => e.Kind == EdgeKind.ConditionalTrue);
            var falseEdge = block.Successors.FirstOrDefault(e => e.Kind == EdgeKind.ConditionalFalse);

            if (trueEdge == null || falseEdge == null)
                throw new InvalidOperationException("Conditional branch must have both true and false edges");

            var test = state.Stack.Pop();

            // Clone state for both branches
            var trueState = state.Clone();
            var falseState = state.Clone();

            var joint = FindJointPoint(block);
            if (joint == null)
                throw new InvalidOperationException($"No convergence point found for conditional branch at {block.First}. This indicates malformed IL or a bug in the convergence detection algorithm.");
                
            ProcessBlock(trueState, trueEdge.To, joint);
            ProcessBlock(falseState, falseEdge.To, joint);
            
            state.Merge(test, trueState, falseState);

            ProcessBlock(state, joint, endBlock);
        }

        static int ProcessInstruction(ProcessorState state, Instruction instruction)
        {
            Debug.WriteLine(instruction);

            if (instruction.OpCode == OpCodes.Nop || 
                instruction.OpCode == OpCodes.Break ||
                instruction.OpCode.FlowControl == FlowControl.Branch)
            {
                // Do nothing
            }
            else if (instruction.OpCode == OpCodes.Brfalse || 
                     instruction.OpCode == OpCodes.Brfalse_S)
            {
                var val = state.Stack.Pop();
                var condition = Expression.Equal(val, ExpressionHelper.Default(val.Type));
                state.Stack.Push(condition);
            }
            else if (instruction.OpCode == OpCodes.Brtrue ||
                     instruction.OpCode == OpCodes.Brtrue_S)
            {
                var address = state.Stack.Peek();
                if (address.Expression is MemberExpression memberExpression && 
                    IsCachedAnonymousMethodDelegate(memberExpression.Member as FieldInfo))
                {
                    state.Stack.Pop();
                    // Always false for cached delegates
                    // if (this.<delegate> != null) { this.<delegate> } else { this.<delegate> = <build> }
                    var condition = Expression.Constant(false);
                    state.Stack.Push(condition);
                }
                else
                {
                    var val = state.Stack.Pop();
                    var condition = val.Type == typeof(bool) ? val : Expression.NotEqual(val, ExpressionHelper.Default(val.Type));
                    state.Stack.Push(condition);
                }
            }
            else if (instruction.OpCode == OpCodes.Bgt ||
                     instruction.OpCode == OpCodes.Bgt_S ||
                     instruction.OpCode == OpCodes.Bgt_Un ||
                     instruction.OpCode == OpCodes.Bgt_Un_S)
            {
                var val2 = state.Stack.Pop();
                var val1 = state.Stack.Pop();
                var condition = MakeBinaryExpression(val1, val2, ExpressionType.GreaterThan);
                state.Stack.Push(condition);
            }
            else if (instruction.OpCode == OpCodes.Bge || 
                     instruction.OpCode == OpCodes.Bge_S ||
                     instruction.OpCode == OpCodes.Bge_Un ||
                     instruction.OpCode == OpCodes.Bge_Un_S)
            {
                var val2 = state.Stack.Pop();
                var val1 = state.Stack.Pop();
                var condition = MakeBinaryExpression(val1, val2, ExpressionType.GreaterThanOrEqual);
                state.Stack.Push(condition);
            }
            else if (instruction.OpCode == OpCodes.Blt || 
                     instruction.OpCode == OpCodes.Blt_S ||
                     instruction.OpCode == OpCodes.Blt_Un ||
                     instruction.OpCode == OpCodes.Blt_Un_S)
            {
                var val2 = state.Stack.Pop();
                var val1 = state.Stack.Pop();
                var condition = MakeBinaryExpression(val1, val2, ExpressionType.LessThan);
                state.Stack.Push(condition);
            }
            else if (instruction.OpCode == OpCodes.Ble || 
                     instruction.OpCode == OpCodes.Ble_S ||
                     instruction.OpCode == OpCodes.Ble_Un || 
                     instruction.OpCode == OpCodes.Ble_Un_S)
            {
                var val2 = state.Stack.Pop();
                var val1 = state.Stack.Pop();
                var condition = MakeBinaryExpression(val1, val2, ExpressionType.LessThanOrEqual);
                state.Stack.Push(condition);
            }
            else if (instruction.OpCode == OpCodes.Beq ||
                     instruction.OpCode == OpCodes.Beq_S)
            {
                var val2 = state.Stack.Pop();
                var val1 = state.Stack.Pop();
                var condition = MakeBinaryExpression(val1, val2, ExpressionType.Equal);
                state.Stack.Push(condition);
            }
            else if (instruction.OpCode == OpCodes.Bne_Un || 
                     instruction.OpCode == OpCodes.Bne_Un_S)
            {
                var val2 = state.Stack.Pop();
                var val1 = state.Stack.Pop();
                var condition = MakeBinaryExpression(val1, val2, ExpressionType.NotEqual);
                state.Stack.Push(condition);
            }
            else if (instruction.OpCode == OpCodes.Ldftn)
            {
                var method = (MethodInfo)instruction.Operand;
                var expression = DecompileLambdaExpression(method, () => state.Stack.Pop());
                state.Stack.Push(expression);
                return 1;
            }
            else if (instruction.OpCode == OpCodes.Newobj)
            {
                var constructor = (ConstructorInfo)instruction.Operand;
                var arguments = GetArguments(state, constructor);
                if (constructor.DeclaringType.IsNullableType() && constructor.GetParameters().Length == 1)
                {
                    state.Stack.Push(Expression.Convert(arguments[0], constructor.DeclaringType));
                }
                else
                {
                    state.Stack.Push(Expression.New(constructor, arguments));
                }
            }

            else if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt)
            {
                var method = instruction.Operand as MethodInfo;
                var constructor = instruction.Operand as ConstructorInfo;
                if (method != null)
                {
                    Call(state, method);
                }
                else if (constructor != null)
                {
                    var address = Expression.New(constructor, GetArguments(state, constructor));
                    var local = state.Stack.Pop();
                    local.Expression = address;
                }
                else
                {
                    throw new NotSupportedException();
                }

            }
            else if (instruction.OpCode == OpCodes.Isinst)
            {
                var val = state.Stack.Pop();
                if (instruction.Next != null && instruction.Next.OpCode == OpCodes.Ldnull &&
                    instruction.Next.Next != null && instruction.Next.Next.OpCode == OpCodes.Cgt_Un)
                {
                    state.Stack.Push(Expression.TypeIs(val, (Type)instruction.Operand));
                    // Skip the next two instructions as they're part of this pattern
                    return 2;
                }

                state.Stack.Push(Expression.TypeAs(val, (Type)instruction.Operand));
            }
            else if (instruction.OpCode == OpCodes.Ret)
            {
                // Return instruction - signal early return
            }
            else if (!Processors.Any(processor => processor.Process(state, instruction)))
            {
                // This should never happen since UnsupportedOpcodeProcessor is the last processor
                throw new InvalidOperationException("No processor handled the instruction, including the fallback processor.");
            }

            return 0;
        }

        static LambdaExpression DecompileLambdaExpression(MethodInfo method, Func<Expression> @this)
        {
            if (method.IsStatic)
            {
                return AnonymousDelegatesCache.GetOrAdd(method, m => m.Decompile());
            }

            //Should always call.
            var expression = @this();
            return AnonymousDelegatesCache.GetOrAdd(method, m =>
            {
                var decompiled = m.Decompile();

                var expressions = new Dictionary<Expression, Expression>
                {
                    {decompiled.Parameters[0], expression}
                };

                var delegateType = Expression.GetDelegateType(GetDelegateParameterTypes(m));
                var body = new ReplaceExpressionVisitor(expressions).Visit(decompiled.Body);
                body = TransparentIdentifierRemovingExpressionVisitor.RemoveTransparentIdentifiers(body);
                return Expression.Lambda(delegateType, body, decompiled.Parameters.Skip(1));
            });
        }

        static Type[] GetDelegateParameterTypes(MethodInfo m)
        {
            var parameters = m.GetParameters();

            var result = new Type[parameters.Length + 1];
            var index = 0;
            while (index < parameters.Length)
            {
                result[index] = parameters[index].ParameterType;
                index++;
            }

            result[index] = m.ReturnType;

            return result;
        }

        internal static bool IsCachedAnonymousMethodDelegate(FieldInfo field)
        {
            if (field == null) return false;
            return field.Name.StartsWith(cachedAnonymousMethodDelegate) && Attribute.IsDefined(field, typeof(CompilerGeneratedAttribute), false) ||
                   field.Name.StartsWith(cachedAnonymousMethodDelegateRoslyn) && field.DeclaringType != null && Attribute.IsDefined(field.DeclaringType, typeof(CompilerGeneratedAttribute), false);
        }

        internal static BinaryExpression MakeBinaryExpression(Address left, Address right, ExpressionType expressionType)
        {
            var rightType = right.Type;
            var leftType = left.Type;

            left = AdjustBooleanConstant(left, rightType);
            right = AdjustBooleanConstant(right, leftType);
            left = ConvertEnumExpressionToUnderlyingType(left);
            right = ConvertEnumExpressionToUnderlyingType(right);

            return Expression.MakeBinary(expressionType, left, right);
        }

        internal static UnaryExpression MakeUnaryExpression(Expression operand, ExpressionType expressionType)
        {
            operand = ConvertEnumExpressionToUnderlyingType(operand);

            return Expression.MakeUnary(expressionType, operand, operand.Type);
        }

        internal static Expression Box(Expression expression, Type type)
        {
            if (expression.Type == type)
                return expression;

            var constantExpression = expression as ConstantExpression;
            if (constantExpression != null)
            {
                if (type.IsEnum)
                    return Expression.Constant(Enum.ToObject(type, constantExpression.Value));
            }

            if (expression.Type.IsEnum)
                return Expression.Convert(expression, type);

            return expression;
        }

        static Expression ConvertEnumExpressionToUnderlyingType(Expression expression)
        {
            if (expression.Type.IsEnum)
                return Expression.Convert(expression, expression.Type.GetEnumUnderlyingType());

            return expression;
        }

        internal static Expression AdjustType(Expression expression, Type type)
        {
            if (expression.Type == type)
            {
                return expression;
            }

            if (expression is ConstantExpression constant)
            {
                if (constant.Value == null)
                {
                    return Expression.Constant(null, type);
                }

                if (constant.Type == typeof(int))
                {
                    if (type.IsEnum)
                    {
                        return Expression.Constant(Enum.ToObject(type, constant.Value));
                    }

                    if (type == typeof(bool))
                    {
                        return Expression.Constant(Convert.ToBoolean(constant.Value));
                    }

                    if (type == typeof(byte))
                    {
                        return Expression.Constant(Convert.ToByte(constant.Value));
                    }

                    if (type == typeof(sbyte))
                    {
                        return Expression.Constant(Convert.ToSByte(constant.Value));
                    }

                    if (type == typeof(short))
                    {
                        return Expression.Constant(Convert.ToInt16(constant.Value));
                    }

                    if (type == typeof(ushort))
                    {
                        return Expression.Constant(Convert.ToUInt16(constant.Value));
                    }

                    if (type == typeof(uint))
                    {
                        return Expression.Constant(Convert.ToUInt32(constant.Value));
                    }
                }
            }
            else
            {
                if (expression.Type == typeof(int) && type == typeof(bool))
                {
                    return Expression.NotEqual(expression, Expression.Constant(0));
                }
            }

            if (!type.IsAssignableFrom(expression.Type) && expression.Type.IsEnum && expression.Type.GetEnumUnderlyingType() == type)
            {
                return Expression.Convert(expression, type);
            }

            if (type.IsValueType != expression.Type.IsValueType)
            {
                return Expression.Convert(expression, type);
            }

            return expression;
        }

        static Expression AdjustBooleanConstant(Expression expression, Type type)
        {
            if (type == typeof(bool) && expression.Type == typeof(int))
            {
                var constantExpression = expression as ConstantExpression;
                if (constantExpression != null)
                {
                    return Expression.Constant(!Equals(constantExpression.Value, 0));
                }
            }

            return expression;
        }

        static void Call(ProcessorState state, MethodInfo m)
        {
            var mArgs = GetArguments(state, m, out var addresses);

            var instance = m.IsStatic ? new Address() : state.Stack.Pop();
            var result = BuildMethodCallExpression(m, instance, mArgs, addresses);
            if (result.Type != typeof(void))
                state.Stack.Push(result);
        }

        internal static Expression BuildAssignment(Expression instance, MemberInfo member, Expression value, out bool push)
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

            if (instance.Type.IsValueType &&
                (instance.NodeType == ExpressionType.Parameter || instance.NodeType == ExpressionType.Constant))
            {
                push = false;
                return Expression.MemberInit(Expression.New(instance.Type), Expression.Bind(member, adjustedValue));
            }

            push = true;
            return Expression.Assign(Expression.MakeMemberAccess(instance, member), adjustedValue);
        }

        static Expression[] GetArguments(ProcessorState state, MethodBase m)
        {
            return GetArguments(state, m, out _);
        }

        static Expression[] GetArguments(ProcessorState state, MethodBase m, out Address[] addresses)
        {
            var parameterInfos = m.GetParameters();
            addresses = new Address[parameterInfos.Length];
            var args = new Expression[parameterInfos.Length];
            for (var i = parameterInfos.Length - 1; i >= 0; i--)
            {
                var argument = state.Stack.Pop();
                var parameter = parameterInfos[i];
                var parameterType = parameter.ParameterType;
                args[i] = AdjustType(argument, parameterType.IsByRef ? parameterType.GetElementType() : parameterType);
                addresses[i] = argument;
            }

            return args;
        }

        static Expression BuildMethodCallExpression(MethodInfo m, Address instance, Expression[] arguments, Address[] addresses)
        {
            if (m.Name == "Add" && instance.Expression != null && typeof(IEnumerable).IsAssignableFrom(instance.Type))
            {
                var newExpression = instance.Expression as NewExpression;
                if (newExpression != null)
                {
                    var init = Expression.ListInit(newExpression, Expression.ElementInit(m, arguments));
                    instance.Expression = init;
                    return Expression.Empty();
                }
                var initExpression = instance.Expression as ListInitExpression;
                if (initExpression != null)
                {
                    var initializers = initExpression.Initializers.ToList();
                    initializers.Add(Expression.ElementInit(m, arguments));
                    var init = Expression.ListInit(initExpression.NewExpression, initializers);
                    instance.Expression = init;
                    return Expression.Empty();
                }
            }
            if (m.IsSpecialName)
            {
                if (m.Name.StartsWith("get_") && arguments.Length == 0)
                {
                    return Expression.Property(instance, m);
                }

                if (m.Name.StartsWith("set_") && arguments.Length == 1)
                {
                    var value = arguments.Single();
                    var property = Expression.Property(instance, m).Member;
                    var expression = BuildAssignment(instance.Expression, property, value, out bool push);
                    if (push)
                    {
                        return expression;
                    }
                    else
                    {
                        instance.Expression = expression;
                        return Expression.Empty();
                    }
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
                            case "op_Implicit":
                                return Expression.Convert(arguments[0], m.ReturnType, m);
                        }
                    }
                }
            }
            if (m.IsStatic && m.DeclaringType == typeof(decimal))
            {
                switch (m.Name)
                {
                    case "Add":
                        return Expression.MakeBinary(ExpressionType.Add, arguments[0], arguments[1]);
                    case "Subtract":
                        return Expression.MakeBinary(ExpressionType.Subtract, arguments[0], arguments[1]);
                    case "Multiply":
                        return Expression.MakeBinary(ExpressionType.Multiply, arguments[0], arguments[1]);
                    case "Divide":
                        return Expression.MakeBinary(ExpressionType.Divide, arguments[0], arguments[1]);
                }
            }
            if (m.Name == "Concat" && m.DeclaringType == typeof(string))
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

            if (m.Name == "InitializeArray" && m.DeclaringType == typeof(RuntimeHelpers))
            {
                var arrayGetter = (Func<Array>)Expression.Lambda(arguments[0]).Compile();
                var fieldGetter = (Func<RuntimeFieldHandle>)Expression.Lambda(arguments[1]).Compile();
                var array = arrayGetter();
                RuntimeHelpers.InitializeArray(array, fieldGetter());

                IEnumerable<Expression> initializers = array.Cast<object>().Select(Expression.Constant);

                addresses[0].Expression = Expression.NewArrayInit(arguments[0].Type.GetElementType(), initializers);
                return Expression.Empty();
            }

            if (instance.Expression != null)
                return Expression.Call(instance, m, arguments);

            return Expression.Call(null, m, arguments);
        }

        static bool TryParseOperator(MethodInfo m, out ExpressionType type)
        {
            switch (m.Name)
            {
                /* The complete set of binary operator function names used is as follows: 
                 * op_Addition, op_Subtraction, op_Multiply, op_Division, op_Modulus, 
                 * op_BitwiseAnd, op_BitwiseOr, op_ExclusiveOr, op_LeftShift, op_RightShift, 
                 * op_Equality, op_Inequality, op_LessThan, op_LessThanOrEqual, op_GreaterThan, 
                 * and op_GreaterThanOrEqual.
                 */
                case "op_Addition":
                    type = ExpressionType.Add;
                    return true;

                case "op_Subtraction":
                    type = ExpressionType.Subtract;
                    return true;

                case "op_Multiply":
                    type = ExpressionType.Multiply;
                    return true;

                case "op_Division":
                    type = ExpressionType.Divide;
                    return true;

                case "op_Modulus":
                    type = ExpressionType.Modulo;
                    return true;

                case "op_BitwiseAnd":
                    type = ExpressionType.And;
                    return true;

                case "op_BitwiseOr":
                    type = ExpressionType.Or;
                    return true;

                case "op_ExclusiveOr":
                    type = ExpressionType.ExclusiveOr;
                    return true;

                case "op_LeftShift":
                    type = ExpressionType.LeftShift;
                    return true;

                case "op_RightShift":
                    type = ExpressionType.RightShift;
                    return true;

                case "op_Equality":
                    type = ExpressionType.Equal;
                    return true;

                case "op_Inequality":
                    type = ExpressionType.NotEqual;
                    return true;

                case "op_LessThan":
                    type = ExpressionType.LessThan;
                    return true;

                case "op_LessThanOrEqual":
                    type = ExpressionType.LessThanOrEqual;
                    return true;

                case "op_GreaterThan":
                    type = ExpressionType.GreaterThan;
                    return true;

                case "op_GreaterThanOrEqual":
                    type = ExpressionType.GreaterThanOrEqual;
                    return true;

                /*
                 * The complete set of unary operator function names used is as follows: 
                 * op_UnaryPlus, op_UnaryNegation, op_LogicalNot, op_OnesComplement, op_Increment, op_Decrement, op_True, and op_False.
                 */
                case "op_UnaryPlus":
                    type = ExpressionType.UnaryPlus;
                    return true;

                case "op_UnaryNegation":
                    type = ExpressionType.Negate;
                    return true;

                case "op_LogicalNot":
                    type = ExpressionType.Not;
                    return true;

                case "op_OnesComplement":
                    type = ExpressionType.OnesComplement;
                    return true;

                case "op_Increment":
                    type = default(ExpressionType);
                    return false;

                case "op_Decrement":
                    type = default(ExpressionType);
                    return false;

                case "op_True":
                    type = default(ExpressionType);
                    return false;

                case "op_False":
                    type = default(ExpressionType);
                    return false;
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
 
        Stack<Block> FollowGraph(Block block)
        {
            var blocks = new Stack<Block>();
            var visited = new HashSet<Block>(); // Prevent infinite loops
            
            while (block != null && !visited.Contains(block))
            {
                blocks.Push(block);
                visited.Add(block);
                block = GetNextBlock(block);
            }
            return blocks;
        }

        Block FindJointPoint(Block block)
        {
            var trueBlocks = FollowGraph(block.Successors[0].To);
            var falseBlocks = FollowGraph(block.Successors[1].To);
            
            Block common = null;
            foreach (var b in falseBlocks)
            {
                if (trueBlocks.Count > 0 && b == trueBlocks.Pop())
                    common = b;
                else
                    break;
            }

            return common;
        }
       
        Block GetNextBlock(Block block)
        {
            return block.Successors.Count switch
            {
                0 => null,// No successors (return/exit block)
                1 => block.Successors[0].To, // Single successor (unconditional flow)
                2 => FindJointPoint(block), // Conditional branch - find joint point of the two branches
                _ => null
            };
        }
    }
}
