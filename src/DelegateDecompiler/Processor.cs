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

        public static Expression Process(ControlFlowGraph cfg, bool isStatic, VariableInfo[] locals, IList<Address> args, Type returnType)
        {
            Processor processor = new Processor();
            var initialState = new ProcessorState(cfg.Entry, isStatic, new Stack<Address>(), locals, args, cfg.Entry.Instructions.FirstOrDefault());
            
            processor.ProcessBlock(cfg.Entry, initialState);
            
            // Use Final() ONLY here - once per method body
            var finalResult = initialState.Final();
            finalResult = AdjustType(finalResult, returnType);

            if (!returnType.IsAssignableFrom(finalResult.Type) && returnType != typeof(void))
            {
                return Expression.Convert(finalResult, returnType);
            }

            return finalResult;
        }

        static readonly IProcessor[] processors = {
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

        void ProcessBlock(Block block, ProcessorState state)
        {
            // Process all non-branching instructions in the block
            for (var index = 0; index < block.Instructions.Count; index++)
            {
                var instruction = block.Instructions[index];
                index += ProcessInstruction(instruction, state);
            }

            // Handle block exit based on successor edges
            ProcessBlockExit(block, state);
        }

        Expression ProcessBlockExit(Block block, ProcessorState state)
        {
            if (block.Successors.Count == 0)
            {
                // End of method - don't use Final() here, let main Process method handle it
                if (state.Stack.Count > 0)
                {
                    return state.Stack.Peek();
                }
                return Expression.Default(typeof(void));
            }
            else if (block.Successors.Count == 1)
            {
                var edge = block.Successors[0];
                if (edge.Kind == EdgeKind.FallThrough || edge.Kind == EdgeKind.UnconditionalBranch)
                {
                    // Simple flow to next block
                     ProcessBlock(edge.To, state);
                     return null;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                // Conditional branch - should have exactly 2 successors
                 ProcessConditionalBranch(block, state);
                 return null;
            }
        }

        void ProcessConditionalBranch(Block block, ProcessorState state)
        {
            var trueEdge = block.Successors.FirstOrDefault(e => e.Kind == EdgeKind.ConditionalTrue);
            var falseEdge = block.Successors.FirstOrDefault(e => e.Kind == EdgeKind.ConditionalFalse);

            if (trueEdge == null || falseEdge == null)
            {
                throw new InvalidOperationException("Conditional branch must have both true and false edges");
            }

            // Find the branching instruction in this block to determine the test condition
            var branchingInstruction = block.Instructions.LastOrDefault(i => 
                i.OpCode == OpCodes.Brfalse || i.OpCode == OpCodes.Brfalse_S ||
                i.OpCode == OpCodes.Brtrue || i.OpCode == OpCodes.Brtrue_S ||
                i.OpCode == OpCodes.Bgt || i.OpCode == OpCodes.Bgt_S ||
                i.OpCode == OpCodes.Bgt_Un || i.OpCode == OpCodes.Bgt_Un_S ||
                i.OpCode == OpCodes.Bge || i.OpCode == OpCodes.Bge_S ||
                i.OpCode == OpCodes.Bge_Un || i.OpCode == OpCodes.Bge_Un_S ||
                i.OpCode == OpCodes.Blt || i.OpCode == OpCodes.Blt_S ||
                i.OpCode == OpCodes.Blt_Un || i.OpCode == OpCodes.Blt_Un_S ||
                i.OpCode == OpCodes.Ble || i.OpCode == OpCodes.Ble_S ||
                i.OpCode == OpCodes.Ble_Un || i.OpCode == OpCodes.Ble_Un_S ||
                i.OpCode == OpCodes.Beq || i.OpCode == OpCodes.Beq_S ||
                i.OpCode == OpCodes.Bne_Un || i.OpCode == OpCodes.Bne_Un_S);

            if (branchingInstruction == null)
            {
                throw new InvalidOperationException("No branching instruction found in conditional block");
            }

            // Create the test condition based on the branching instruction
            Expression test = CreateTestCondition(branchingInstruction, state);

            // Clone state for both branches
            var trueState = state.Clone(trueEdge.To.First, trueEdge.To);
            var falseState = state.Clone(falseEdge.To.First, falseEdge.To);

            var convergencePoint = FindConvergencePoint(block);
            
            Console.WriteLine($"DEBUG: Conditional branch - TrueBlock: {trueEdge.To?.First}, FalseBlock: {falseEdge.To?.First}, ConvergencePoint: {convergencePoint?.First}");
            
            if (convergencePoint != null)
            {
                Console.WriteLine($"DEBUG: Processing until convergence - TrueBlock: {trueEdge.To?.First} to {convergencePoint?.First}");
                Console.WriteLine($"DEBUG: Processing until convergence - FalseBlock: {falseEdge.To?.First} to {convergencePoint?.First}");
                
                // Both branches converge - process them until convergence
                ProcessUntilBlock(trueEdge.To, convergencePoint, trueState);
                ProcessUntilBlock(falseEdge.To, convergencePoint, falseState);
                
                // Use state.Merge() to create the conditional and push it onto the stack
                state.Merge(test, trueState, falseState);
                
                // If we have a convergence point, continue processing from there
                if (!convergencePoint.IsExit)
                {
                    ProcessBlock(convergencePoint, state);
                }
            }
            else
            {
                // No convergence - process branches independently and merge
                ProcessBlock(trueEdge.To, trueState);
                ProcessBlock(falseEdge.To, falseState);

                // Always merge states after processing branches
                state.Merge(test, trueState, falseState);
            }
        }

        Stack<Block> FollowBlockGraph(Block block)
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

        Block GetNextBlock(Block block)
        {
            return block.Successors.Count switch
            {
                0 => null,// No successors (return/exit block)
                1 => block.Successors[0].To, // Single successor (unconditional flow)
                2 => FindConvergencePoint(block), // Conditional branch - find joint point of the two branches
                _ => null
            };
        }

        Block FindConvergencePoint(Block block)
        {
            var trueBlocks = FollowBlockGraph(block.Successors[0].To);
            var falseBlocks = FollowBlockGraph(block.Successors[1].To);
            
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

        void ProcessUntilBlock(Block startBlock, Block endBlock, ProcessorState state)
        {
            Console.WriteLine($"DEBUG: ProcessUntilBlock - Start: {startBlock?.First}, End: {endBlock?.First}, Stack count: {state.Stack.Count}");
            
            if (startBlock == endBlock)
            {
                Console.WriteLine($"DEBUG: Start == End, returning stack value without processing the convergence block");
                // Don't process the convergence block - just return what's currently on the stack
                if (state.Stack.Count == 0)
                {
                    Console.WriteLine($"DEBUG: Empty stack at convergence point, returning void");
                    Expression.Default(typeof(void));
                    return;
                }
                // Don't use Final() here - just peek at the top of stack without consuming it
                var result = state.Stack.Peek();
                Console.WriteLine($"DEBUG: Convergence point result: {result} (Type: {result.Type})");
                return;
            }

            var currentBlock = startBlock;
            
            // Process blocks until we reach the end block (but don't process the end block itself)
            while (currentBlock != null && currentBlock != endBlock)
            {
                // Process all instructions in the current block
                for (var index = 0; index < currentBlock.Instructions.Count; index++)
                {
                    var instruction = currentBlock.Instructions[index];
                    index += ProcessInstruction(instruction, state);
                }

                // Move to next block
                if (currentBlock.Successors.Count == 1)
                {
                    currentBlock = currentBlock.Successors[0].To;
                }
                else if (currentBlock.Successors.Count == 2)
                {
                    // Hit another conditional branch before reaching end block
                    Console.WriteLine($"DEBUG: ProcessUntilBlock found conditional branch in block {currentBlock?.First}");
                    // Process the conditional branch directly and return the result
                    ProcessBlockExit(currentBlock, state);
                    return;
                }
                else
                {
                    // No successors or complex branching
                    break;
                }
            }
        }

        Expression CreateTestCondition(Instruction branchingInstruction, ProcessorState state)
        {
            // Handle different branching instructions
            if (branchingInstruction.OpCode == OpCodes.Brfalse || branchingInstruction.OpCode == OpCodes.Brfalse_S)
            {
                var val = state.Stack.Pop();
                return Expression.Equal(val, ExpressionHelper.Default(val.Type));
            }
            else if (branchingInstruction.OpCode == OpCodes.Brtrue || branchingInstruction.OpCode == OpCodes.Brtrue_S)
            {
                var address = state.Stack.Peek();
                if (address.Expression is MemberExpression memberExpression && 
                    IsCachedAnonymousMethodDelegate(memberExpression.Member as FieldInfo))
                {
                    state.Stack.Pop();
                    // Always false for cached delegates
                    // if (this.<delegate> != null) { this.<delegate> } else { this.<delegate> = <build> }
                    return Expression.Constant(false);
                }
                else
                {
                    var val = state.Stack.Pop();
                    return val.Type == typeof(bool) ? val : Expression.NotEqual(val, ExpressionHelper.Default(val.Type));
                }
            }
            else if (branchingInstruction.OpCode == OpCodes.Bgt || branchingInstruction.OpCode == OpCodes.Bgt_S ||
                     branchingInstruction.OpCode == OpCodes.Bgt_Un || branchingInstruction.OpCode == OpCodes.Bgt_Un_S)
            {
                var val2 = state.Stack.Pop();
                var val1 = state.Stack.Pop();
                return MakeBinaryExpression(val1, val2, ExpressionType.GreaterThan);
            }
            else if (branchingInstruction.OpCode == OpCodes.Bge || branchingInstruction.OpCode == OpCodes.Bge_S ||
                     branchingInstruction.OpCode == OpCodes.Bge_Un || branchingInstruction.OpCode == OpCodes.Bge_Un_S)
            {
                var val2 = state.Stack.Pop();
                var val1 = state.Stack.Pop();
                return MakeBinaryExpression(val1, val2, ExpressionType.GreaterThanOrEqual);
            }
            else if (branchingInstruction.OpCode == OpCodes.Blt || branchingInstruction.OpCode == OpCodes.Blt_S ||
                     branchingInstruction.OpCode == OpCodes.Blt_Un || branchingInstruction.OpCode == OpCodes.Blt_Un_S)
            {
                var val2 = state.Stack.Pop();
                var val1 = state.Stack.Pop();
                return MakeBinaryExpression(val1, val2, ExpressionType.LessThan);
            }
            else if (branchingInstruction.OpCode == OpCodes.Ble || branchingInstruction.OpCode == OpCodes.Ble_S ||
                     branchingInstruction.OpCode == OpCodes.Ble_Un || branchingInstruction.OpCode == OpCodes.Ble_Un_S)
            {
                var val2 = state.Stack.Pop();
                var val1 = state.Stack.Pop();
                return MakeBinaryExpression(val1, val2, ExpressionType.LessThanOrEqual);
            }
            else if (branchingInstruction.OpCode == OpCodes.Beq || branchingInstruction.OpCode == OpCodes.Beq_S)
            {
                var val2 = state.Stack.Pop();
                var val1 = state.Stack.Pop();
                return MakeBinaryExpression(val1, val2, ExpressionType.Equal);
            }
            else if (branchingInstruction.OpCode == OpCodes.Bne_Un || branchingInstruction.OpCode == OpCodes.Bne_Un_S)
            {
                var val2 = state.Stack.Pop();
                var val1 = state.Stack.Pop();
                return MakeBinaryExpression(val1, val2, ExpressionType.NotEqual);
            }
            else
            {
                throw new NotSupportedException($"Unsupported branching instruction: {branchingInstruction.OpCode}");
            }
        }

        static int ProcessInstruction(Instruction instruction, ProcessorState state)
        {
            Debug.WriteLine(instruction);

            if (instruction.OpCode == OpCodes.Nop || instruction.OpCode == OpCodes.Break)
            {
                // Do nothing
            }

           else  if (instruction.OpCode == OpCodes.Br_S || instruction.OpCode == OpCodes.Br ||
                instruction.OpCode == OpCodes.Brfalse || instruction.OpCode == OpCodes.Brfalse_S ||
                instruction.OpCode == OpCodes.Brtrue || instruction.OpCode == OpCodes.Brtrue_S ||
                instruction.OpCode == OpCodes.Bgt || instruction.OpCode == OpCodes.Bgt_S ||
                instruction.OpCode == OpCodes.Bgt_Un || instruction.OpCode == OpCodes.Bgt_Un_S ||
                instruction.OpCode == OpCodes.Bge || instruction.OpCode == OpCodes.Bge_S ||
                instruction.OpCode == OpCodes.Bge_Un || instruction.OpCode == OpCodes.Bge_Un_S ||
                instruction.OpCode == OpCodes.Blt || instruction.OpCode == OpCodes.Blt_S ||
                instruction.OpCode == OpCodes.Blt_Un || instruction.OpCode == OpCodes.Blt_Un_S ||
                instruction.OpCode == OpCodes.Ble || instruction.OpCode == OpCodes.Ble_S ||
                instruction.OpCode == OpCodes.Ble_Un || instruction.OpCode == OpCodes.Ble_Un_S ||
                instruction.OpCode == OpCodes.Beq || instruction.OpCode == OpCodes.Beq_S ||
                instruction.OpCode == OpCodes.Bne_Un || instruction.OpCode == OpCodes.Bne_Un_S)
            {
                // These are handled at block level, ignore during instruction processing
            }

           else if (instruction.OpCode == OpCodes.Ldftn)
            {
                var method = (MethodInfo)instruction.Operand;
                var expression = DecompileLambdaExpression(method, () => state.Stack.Pop());
                state.Stack.Push(expression);
                return 1;
            }

         else   if (instruction.OpCode == OpCodes.Newobj)
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

else            if (instruction.OpCode == OpCodes.Call || instruction.OpCode == OpCodes.Callvirt)
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
            else  if (instruction.OpCode == OpCodes.Isinst)
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
            else if (!processors.Any(processor => processor.Process(state, instruction)))
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

        static Type GetCommonType(Type type1, Type type2)
        {
            if (type1 == type2)
                return type1;

            // Check if one is assignable from the other
            if (type1.IsAssignableFrom(type2))
                return type1;
            if (type2.IsAssignableFrom(type1))
                return type2;

            // For value types that are compatible
            if (type1.IsValueType && type2.IsValueType)
            {
                // Handle nullable types
                var underlyingType1 = Nullable.GetUnderlyingType(type1) ?? type1;
                var underlyingType2 = Nullable.GetUnderlyingType(type2) ?? type2;

                if (underlyingType1 == underlyingType2)
                {
                    // If one is nullable and the other isn't, return the nullable version
                    if (Nullable.GetUnderlyingType(type1) != null || Nullable.GetUnderlyingType(type2) != null)
                    {
                        return typeof(Nullable<>).MakeGenericType(underlyingType1);
                    }
                    return underlyingType1;
                }
            }

            // As a last resort, use object
            return typeof(object);
        }
    }
}
