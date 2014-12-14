using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiler
{
    class OptimizeExpressionVisitor : ExpressionVisitor
    {
        static readonly MethodInfo StringConcat = typeof(string).GetMethod("Concat", new[] { typeof(object), typeof(object) });

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
            var ifTrueBinary = UnwrapConvertExpression(ifTrue) as BinaryExpression;
            if (ifTrueBinary != null)
            {
                BinaryExpression result;
                if (TryConvert1(test, ifTrueBinary, out result))
                    return result;
            }
            
            var binaryExpression = test as BinaryExpression;
            var ifTrueConstant = ifTrue as ConstantExpression;
            var ifFalseConstant = ifFalse as ConstantExpression;

            if (binaryExpression != null)
            {
                BinaryExpression result;
                if (ifTrueBinary != null && TryConvert2(binaryExpression, ifTrueBinary, out result))
                {
                    return result;
                }
                if (TryConvert(ifTrueConstant, binaryExpression, ifFalse, out result, false))
                {
                    return result;
                }
                if (TryConvert(ifFalseConstant, binaryExpression, ifTrue, out result, true))
                {
                    return result;
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

        private static bool TryConvert(ConstantExpression constant, BinaryExpression left, Expression right, out BinaryExpression result, bool isLeft)
        {
            if (constant != null && constant.Value is bool)
            {
                if ((bool) constant.Value)
                {
                    if (left.NodeType == ExpressionType.Equal ||
                        left.NodeType == ExpressionType.NotEqual ||
                        left.NodeType == ExpressionType.GreaterThan ||
                        left.NodeType == ExpressionType.GreaterThanOrEqual ||
                        left.NodeType == ExpressionType.LessThan ||
                        left.NodeType == ExpressionType.LessThanOrEqual)
                    {
                        if (!isLeft || Invert(ref left))
                        {
                            //TODO: Move to Visit OrElse
                            if (TryConvertOrElse(right, left, out result))
                                return true;

                            result = Expression.OrElse(left, right);
                            return true;
                        }
                    }
                }
                else
                {
                    if (isLeft || Invert(ref left))
                    {
                        //TODO: Move to Visit AndAlso
                        if (TryConvertAndAlso(right, left, out result))
                            return true;

                        result = Expression.AndAlso(left, right);
                        return true;
                    }
                }
            }
            result = null;
            return false;
        }

        private static bool TryConvertOrElse(Expression hasValue, BinaryExpression getValueOrDefault, out BinaryExpression result)
        {
            var hasValueBinary = hasValue as BinaryExpression;
            if (hasValueBinary != null && TryConvert2(hasValueBinary, getValueOrDefault, out result))
            {
                return true;
            }
            if (hasValue.NodeType == ExpressionType.Not && TryConvert1(((UnaryExpression) hasValue).Operand, getValueOrDefault, out result))
            {
                return true;
            }
            result = null;
            return false;
        }

        private static bool TryConvertAndAlso(Expression hasValue, BinaryExpression getValueOrDefault, out BinaryExpression result)
        {
            var hasValueBinary = hasValue as BinaryExpression;
            if (hasValueBinary != null && TryConvert2(hasValueBinary, getValueOrDefault, out result))
            {
                return true;
            }
            if (TryConvert1(hasValue, getValueOrDefault, out result))
            {
                return true;
            }
            result = null;
            return false;
        }

        private static bool TryConvert2(BinaryExpression hasValue, BinaryExpression getValueOrDefault, out BinaryExpression result)
        {
            Expression left;
            Expression right;
            if (ExtractNullableArgument(hasValue.Left, getValueOrDefault.Left, out left) &&
                ExtractNullableArgument(hasValue.Right, getValueOrDefault.Right, out right))
            {
                result = Expression.MakeBinary(getValueOrDefault.NodeType, left, right);
                return true;
            }
            result = null;
            return false;
        }

        private static bool TryConvert1(Expression hasValue, BinaryExpression getValueOrDefault, out BinaryExpression result)
        {
            Expression expression;
            if (ExtractNullableArgument(hasValue, getValueOrDefault.Left, out expression))
            {
                result = Expression.MakeBinary(getValueOrDefault.NodeType, expression, Expression.Convert(getValueOrDefault.Right, expression.Type));
                return true;
            }
            if (ExtractNullableArgument(hasValue, getValueOrDefault.Right, out expression))
            {
                result = Expression.MakeBinary(getValueOrDefault.NodeType, Expression.Convert(getValueOrDefault.Left, expression.Type), expression);
                return true;
            }
            result = null;
            return false;
        }

        private static Expression UnwrapConvertExpression(Expression expression)
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
