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

        static readonly Dictionary<OpCode, IProcessor> processors;

        static Processor()
        {
            processors = new Dictionary<OpCode, IProcessor>();
            BrfalseProcessor.Register(processors);
            BrtrueProcessor.Register(processors);
            BinaryExpressionProcessor.Register(processors);
            BoxProcessor.Register(processors);
            CallProcessor.Register(processors);
            CgtUnProcessor.Register(processors);
            ConstantValueProcessor.Register(processors);
            ConstantOperandProcessor.Register(processors);
            ConvertCheckedProcessor.Register(processors);
            ConvertProcessor.Register(processors);
            ConvertTypeProcessor.Register(processors);
            DupProcessor.Register(processors);
            InitObjProcessor.Register(processors);
            IsinstProcessor.Register(processors);
            LdargConstantProcessor.Register(processors);
            LdargParameterProcessor.Register(processors);
            LdcI4SProcessor.Register(processors);
            LdelemProcessor.Register(processors);
            LdfldProcessor.Register(processors);
            LdlenProcessor.Register(processors);
            LdlocConstantProcessor.Register(processors);
            LdlocVariableProcessor.Register(processors);
            LdtokenProcessor.Register(processors);
            NewArrProcessor.Register(processors);
            NewobjProcessor.Register(processors);
            PopProcessor.Register(processors);
            StargProcessor.Register(processors);
            StelemProcessor.Register(processors);
            StfldProcessor.Register(processors);
            StlocConstantProcessor.Register(processors);
            StlocVariableProcessor.Register(processors);
            StsfldProcessor.Register(processors);
            UnaryExpressionProcessor.Register(processors);
        }

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
            var successors = block.Successors.Where(b => !b.IsException).ToList();
            switch (successors.Count)
            {
                case 0:
                    break;
                case 1:
                    ProcessBlock(state, successors[0].To, endBlock);
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

            var joint = FindJointPoint(block.Successors.Where(b => !b.IsException).ToList());
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
                instruction.OpCode == OpCodes.Ret ||
                instruction.OpCode.FlowControl == FlowControl.Branch)
            {
                // Do nothing
            }
            else if (instruction.OpCode == OpCodes.Ldftn)
            {
                var method = (MethodInfo)instruction.Operand;
                var expression = DecompileLambdaExpression(method, () => state.Stack.Pop());
                state.Stack.Push(expression);
                return 1;
            }
            else if (processors.TryGetValue(instruction.OpCode, out var processor))
            {
                processor.Process(state, instruction);
            }
            else
            {
                throw new NotSupportedException(
                    $"The IL opcode '{instruction.OpCode}' is not supported by DelegateDecompiler. " +
                    $"This opcode cannot be decompiled into an expression tree. " +
                    $"Consider simplifying the method or using a different approach.");
            }

            return 0;
        }

        static LambdaExpression DecompileLambdaExpression(MethodInfo method, Func<Expression> @this)
        {
            if (method.IsStatic)
                return AnonymousDelegatesCache.GetOrAdd(method, m => m.Decompile());

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
            
            // For comparison operations, convert enums directly to int to avoid type mismatches
            // This handles cases where an enum with a non-int underlying type (e.g., byte, long) is compared
            // with an int constant loaded from IL
            if (IsComparisonOperation(expressionType))
            {
                left = ConvertEnumExpressionToInt(left);
                right = ConvertEnumExpressionToInt(right);
            }
            else
            {
                left = ConvertEnumExpressionToUnderlyingType(left);
                right = ConvertEnumExpressionToUnderlyingType(right);
            }

            return Expression.MakeBinary(expressionType, left, right);
        }

        static bool IsComparisonOperation(ExpressionType expressionType)
        {
            return expressionType == ExpressionType.Equal ||
                   expressionType == ExpressionType.NotEqual ||
                   expressionType == ExpressionType.LessThan ||
                   expressionType == ExpressionType.LessThanOrEqual ||
                   expressionType == ExpressionType.GreaterThan ||
                   expressionType == ExpressionType.GreaterThanOrEqual;
        }

        internal static Expression ConvertEnumExpressionToInt(Expression expression)
        {
            // If the expression is already an int, return as-is
            if (expression.Type == typeof(int))
                return expression;
            
            if (expression.Type.IsEnum)
                return Expression.Convert(expression, typeof(int));

            // If the expression is a Convert from an enum to its underlying type,
            // replace it with a direct conversion to int to avoid double conversion
            if (expression is UnaryExpression unary && unary.NodeType == ExpressionType.Convert)
            {
                // Check if we're converting an enum to its underlying type
                var operand = unary.Operand;
                
                if (operand.Type.IsEnum && operand.Type.GetEnumUnderlyingType() == expression.Type)
                {
                    // Replace Convert(enumValue, underlyingType) with Convert(enumValue, int)
                    return Expression.Convert(operand, typeof(int));
                }
            }

            return expression;
        }

        internal static Expression ConvertEnumExpressionToUnderlyingType(Expression expression)
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

        internal static Expression[] GetArguments(ProcessorState state, MethodBase m)
        {
            return GetArguments(state, m, out _);
        }

        internal static Expression[] GetArguments(ProcessorState state, MethodBase m, out Address[] addresses)
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

        Block FindJointPoint(List<BlockEdge> successors)
        {
            var trueBlocks = FollowGraph(successors[0].To);
            var falseBlocks = FollowGraph(successors[1].To);
            
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
            var successors = block.Successors.Where(b => !b.IsException).ToList();
            return successors.Count switch
            {
                0 => null,// No successors (return/exit block)
                1 => successors[0].To, // Single successor (unconditional flow)
                2 => FindJointPoint(block.Successors.Where(b => !b.IsException).ToList()), // Conditional branch - find joint point of the two branches
                _ => null
            };
        }
    }
}
