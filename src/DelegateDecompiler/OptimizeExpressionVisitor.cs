using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace DelegateDecompiler
{
    // Convert nullable constructors to a Convert call instead
    public class OptimizeExpressionVisitor : ExpressionVisitor
    {
        protected override Expression VisitNew(NewExpression node)
        {
            // Test if this is a nullable type
            if (IsNullable(node.Type) && node.Arguments.Count == 1)
            {
                return Expression.Convert(Visit(node.Arguments[0]), node.Type);
            }
            return base.VisitNew(node);
        }

        private static bool IsNullable(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            Debug.WriteLine(node);
            var ifTrue = Visit(node.IfTrue);
            var ifFalse = Visit(node.IfFalse);
            var test = Visit(node.Test);

            Expression expression;
            if (ExtractNullableArgument(test, ifTrue, out expression))
            {
                return Expression.Coalesce(expression, ifFalse);
            }
            var ifTrueBinary = UnwrapUnaryExpression(ifTrue) as BinaryExpression;
            if (ifTrueBinary != null)
            {
                if (ExtractNullableArgument(test, ifTrueBinary.Right, out expression))
                {
                    return Expression.MakeBinary(ifTrueBinary.NodeType, expression, Expression.Convert(ifTrueBinary.Left, expression.Type));
                }
                if (ExtractNullableArgument(test, ifTrueBinary.Left, out expression))
                {
                    return Expression.MakeBinary(ifTrueBinary.NodeType, Expression.Convert(ifTrueBinary.Right, expression.Type), expression);
                }
            }
            var binaryExpression = test as BinaryExpression;
            var ifTrueConstant = ifTrue as ConstantExpression;
            if (binaryExpression != null)
            {
                if (ifTrueBinary != null)
                {
                    Expression left;
                    Expression right;
                    if (binaryExpression.NodeType == ExpressionType.And &&
                        ExtractNullableArgument(binaryExpression.Left, ifTrueBinary.Left, out left) &&
                        ExtractNullableArgument(binaryExpression.Right, ifTrueBinary.Right, out right))
                    {
                        return Expression.MakeBinary(ifTrueBinary.NodeType, right, left);
                    }
                }
                if (ifTrueConstant != null && ifTrueConstant.Value is bool)
                {
                    var ifFalseBinary = ifFalse as BinaryExpression;
                    if ((bool)ifTrueConstant.Value)
                    {
                        if (ifFalseBinary != null)
                        {
                            Expression left;
                            Expression right;
                            if (ExtractNullableArgument(ifFalseBinary.Left, binaryExpression.Left, out left) &&
                                ExtractNullableArgument(ifFalseBinary.Right, binaryExpression.Right, out right))
                            {
                                return Expression.MakeBinary(ifFalseBinary.NodeType, left, right);
                            }
                        }
                        else
                        {
                            Expression left;
                            var unaryExpression = ifFalse as UnaryExpression;
                            if (unaryExpression != null && ExtractNullableArgument(unaryExpression.Operand, binaryExpression.Left, out left))
                            {
                                return Expression.MakeBinary(binaryExpression.NodeType, left, Expression.Convert(binaryExpression.Right, left.Type));
                            }
                            Expression right;
                            if (unaryExpression != null && ExtractNullableArgument(unaryExpression.Operand, binaryExpression.Right, out right))
                            {
                                return Expression.MakeBinary(binaryExpression.NodeType, Expression.Convert(binaryExpression.Left, right.Type), right);
                            }
                        }

                        if (binaryExpression.NodeType == ExpressionType.Equal ||
                            binaryExpression.NodeType == ExpressionType.NotEqual ||
                            binaryExpression.NodeType == ExpressionType.GreaterThan ||
                            binaryExpression.NodeType == ExpressionType.GreaterThanOrEqual ||
                            binaryExpression.NodeType == ExpressionType.LessThan ||
                            binaryExpression.NodeType == ExpressionType.LessThanOrEqual)
                        {
                            return Expression.OrElse(binaryExpression, ifFalse);
                        }
                    }
                    else
                    {
                        if (Invert(ref binaryExpression))
                        {
                            if (ifFalseBinary != null)
                            {
                                Expression left;
                                Expression right;
                                if (ExtractNullableArgument(ifFalseBinary.Left, binaryExpression.Left, out left) &&
                                    ExtractNullableArgument(ifFalseBinary.Right, binaryExpression.Right, out right))
                                {
                                    return Expression.MakeBinary(binaryExpression.NodeType, left, right);
                                }
                            }
                            else
                            {
                                Expression left;
                                if (ExtractNullableArgument(ifFalse, binaryExpression.Left, out left))
                                {
                                    return Expression.MakeBinary(binaryExpression.NodeType, left, Expression.Convert(binaryExpression.Right, left.Type));
                                }
                                Expression right;
                                if (ExtractNullableArgument(ifFalse, binaryExpression.Right, out right))
                                {
                                    return Expression.MakeBinary(binaryExpression.NodeType, Expression.Convert(binaryExpression.Left, right.Type), right);
                                }
                            }

                            return Expression.AndAlso(binaryExpression, ifFalse);
                        }
                    }
                }
                if (binaryExpression.NodeType == ExpressionType.NotEqual)
                {
                    if (binaryExpression.Left == node.IfTrue)
                    {
                        if (binaryExpression.Right is DefaultExpression)
                        {
                            return Expression.Coalesce(binaryExpression.Left, ifFalse);
                        }
                        var rightConstant = binaryExpression.Right as ConstantExpression;
                        if (rightConstant != null && rightConstant.Value == null)
                        {
                            return Expression.Coalesce(binaryExpression.Left, ifFalse);
                        }
                    }
                }
            }

            if (test.NodeType == ExpressionType.Not)
            {
                if (ifTrueConstant != null)
                {
                    if (ifTrueConstant.Value as bool? == false)
                    {
                        return Expression.AndAlso(((UnaryExpression) test).Operand, ifFalse);
                    }
                }
            }

            return node.Update(test, ifTrue, ifFalse);
        }

        private static Expression UnwrapUnaryExpression(Expression expression)
        {
            var unary = expression as UnaryExpression;
            if (unary != null && expression.NodeType == ExpressionType.Convert)
            {
                return unary.Operand;
            }
            return expression;
        }

        private static bool ExtractNullableArgument(Expression hasValue, Expression getValueOrDefault, out Expression expression)
        {
            MemberExpression memberExpression;
            MethodCallExpression callExpression;
            if (IsHasValue(hasValue, out memberExpression) && IsGetValueOrDefault(getValueOrDefault, out callExpression))
            {
                expression = memberExpression.Expression;
                if (expression == callExpression.Object)
                    return true;
            }
            
            expression = null;
            return false;
        }

        private static bool IsHasValue(Expression expression, out MemberExpression property)
        {
            property = expression as MemberExpression;
            return property != null && property.Member.Name == "HasValue" && property.Expression != null && IsNullable(property.Expression.Type);
        }

        private static bool IsGetValueOrDefault(Expression expression, out MethodCallExpression method)
        {
            method = expression as MethodCallExpression;
            return method != null && method.Method.Name == "GetValueOrDefault" && method.Object != null && IsNullable(method.Object.Type);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var rightConstant = node.Right as ConstantExpression;
            if (rightConstant != null)
            {
                if (rightConstant.Value as bool? == false)
                {
                    switch (node.NodeType)
                    {
                        case ExpressionType.Equal:
                            BinaryExpression binaryExpression = node.Left as BinaryExpression;
                            if (binaryExpression != null && Invert(ref binaryExpression))
                                return Visit(binaryExpression);
                            return Expression.Not(Visit(node.Left));
                        case ExpressionType.NotEqual:
                            return Visit(node.Left);
                    }
                }
            }
            return base.VisitBinary(node);
        }

        private static bool Invert(ref BinaryExpression expression)
        {
            switch (expression.NodeType)
            {
                case ExpressionType.Equal:
                {
                    expression = Expression.NotEqual(expression.Left, expression.Right);
                    return true;
                }
                case ExpressionType.NotEqual:
                {
                    expression = Expression.Equal(expression.Left, expression.Right);
                    return true;
                }
                case ExpressionType.LessThan:
                {
                    expression = Expression.GreaterThanOrEqual(expression.Left, expression.Right);
                    return true;
                }
                case ExpressionType.LessThanOrEqual:
                {
                    expression = Expression.GreaterThan(expression.Left, expression.Right);
                    return true;
                }
                case ExpressionType.GreaterThan:
                {
                    expression = Expression.LessThanOrEqual(expression.Left, expression.Right);
                    return true;
                }
                case ExpressionType.GreaterThanOrEqual:
                {
                    expression = Expression.LessThan(expression.Left, expression.Right);
                    return true;
                }
            }
            return false;
        }
    }
}
