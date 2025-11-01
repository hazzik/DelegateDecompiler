using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Mono.Reflection;

namespace DelegateDecompiler.Processors;

internal class CallProcessor : IProcessor
{
    internal static readonly MethodInfo StringConcat =
        typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) });
    
    public static void Register(Dictionary<OpCode, IProcessor> processors)
    {
        processors.Register(new CallProcessor(), OpCodes.Call, OpCodes.Callvirt);
    }

    public void Process(ProcessorState state, Instruction instruction)
    {
        switch (instruction.Operand)
        {
            case MethodInfo method:
                Call(state, method);
                break;
            case ConstructorInfo ctor:
                var address = Expression.New(ctor, Processor.GetArguments(state, ctor));
                var local = state.Stack.Pop();
                local.Expression = address;
                break;
            default:
                throw new NotSupportedException();
        }
    }

    static void Call(ProcessorState state, MethodInfo m)
    {
        var mArgs = Processor.GetArguments(state, m, out var addresses);

        var instance = m.IsStatic ? new Address() : state.Stack.Pop();
        var result = BuildMethodCallExpression(m, instance, mArgs, addresses);
        if (result.Type != typeof(void))
            state.Stack.Push(result);
    }

    static Expression BuildMethodCallExpression(MethodInfo m, Address instance, Expression[] arguments,
        Address[] addresses)
    {
        if (m.Name == "HasFlag" && m.DeclaringType == typeof(Enum))
        {
            var instanceExpr = instance.Expression;
            var argument = arguments.Length > 0 ? arguments[0] : null;
            
            // Unwrap Convert expressions on instance to get to the actual enum
            // Stop if we find an enum or if we've unwrapped to a constant (which needs to be converted to enum)
            while (instanceExpr is UnaryExpression unaryInst && 
                   unaryInst.NodeType == ExpressionType.Convert &&
                   !(unaryInst.Operand is ConstantExpression))
            {
                if (unaryInst.Operand.Type.IsEnum)
                {
                    instanceExpr = unaryInst.Operand;
                    break;
                }
                instanceExpr = unaryInst.Operand;
            }
            
            // If instance is a constant wrapped in a convert, we need to figure out what enum type it should be
            // Check the argument to infer the enum type
            Type enumType = null;
            if (instanceExpr.Type.IsEnum)
            {
                enumType = instanceExpr.Type;
            }
            else if (argument != null)
            {
                // Try to infer from argument
                var argCheck = argument;
                while (argCheck is UnaryExpression unaryCheck && unaryCheck.NodeType == ExpressionType.Convert)
                {
                    if (unaryCheck.Operand.Type.IsEnum)
                    {
                        enumType = unaryCheck.Operand.Type;
                        break;
                    }
                    argCheck = unaryCheck.Operand;
                }
                if (enumType == null && argCheck.Type.IsEnum)
                {
                    enumType = argCheck.Type;
                }
            }
            
            // Convert instance to enum type if needed
            if (enumType != null && !instanceExpr.Type.IsEnum)
            {
                if (instanceExpr is UnaryExpression unaryInst && 
                    unaryInst.NodeType == ExpressionType.Convert &&
                    unaryInst.Operand is ConstantExpression constInst)
                {
                    instanceExpr = Expression.Constant(Enum.ToObject(enumType, constInst.Value), enumType);
                }
                else if (instanceExpr is ConstantExpression constExpr)
                {
                    instanceExpr = Expression.Constant(Enum.ToObject(enumType, constExpr.Value), enumType);
                }
            }
            instance.Expression = instanceExpr;
            
            // Convert argument properly
            if (argument != null)
            {
                // If argument is already an enum, convert to System.Enum
                if (argument.Type.IsEnum)
                {
                    arguments[0] = Expression.Convert(argument, typeof(Enum));
                }
                // Otherwise, try to unwrap and convert
                else if (enumType != null)
                {
                    var unwrappedArg = argument;
                    while (unwrappedArg is UnaryExpression unaryArg &&
                           unaryArg.NodeType == ExpressionType.Convert)
                    {
                        if (unaryArg.Operand.Type.IsEnum)
                        {
                            arguments[0] = Expression.Convert(unaryArg.Operand, typeof(Enum));
                            unwrappedArg = null;
                            break;
                        }
                        unwrappedArg = unaryArg.Operand;
                    }
                    
                    if (unwrappedArg != null && !unwrappedArg.Type.IsEnum)
                    {
                        if (unwrappedArg is ConstantExpression constArg)
                        {
                            var enumConst = Expression.Constant(Enum.ToObject(enumType, constArg.Value), enumType);
                            arguments[0] = Expression.Convert(enumConst, typeof(Enum));
                        }
                    }
                }
            }
        }
        
        if (m.Name == "Add" && instance.Expression != null && typeof(IEnumerable).IsAssignableFrom(instance.Type))
        {
            switch (instance.Expression)
            {
                case NewExpression newExpression:
                {
                    var init = Expression.ListInit(newExpression, Expression.ElementInit(m, arguments));
                    instance.Expression = init;
                    return Expression.Empty();
                }
                case ListInitExpression initExpression:
                {
                    var initializers = initExpression.Initializers.ToList();
                    initializers.Add(Expression.ElementInit(m, arguments));
                    var init = Expression.ListInit(initExpression.NewExpression, initializers);
                    instance.Expression = init;
                    return Expression.Empty();
                }
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
                var expression = Processor.BuildAssignment(instance.Expression, property, value, out bool push);
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
                if (TryParseOperator(m, out var type))
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
        {
            return Expression.Call(instance.Expression, m, arguments);
        }

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
    
    internal static IList<Expression> GetExpressionsForStringConcat(Expression[] arguments)
    {
        if (arguments.Length == 1 &&
            arguments[0] is NewArrayExpression array &&
            array.NodeType == ExpressionType.NewArrayInit)
        {
            return array.Expressions;
        }

        return arguments;
    }
}