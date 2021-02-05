﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;

namespace DelegateDecompiler
{
    class OptimizeExpressionVisitor : ExpressionVisitor
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

        readonly Dictionary<Expression, Expression> expressionsCache
            = new Dictionary<Expression, Expression>();

        public override Expression Visit(Expression node)
        {
            if (node == null)
                return null;
            Expression result;
            if (expressionsCache.TryGetValue(node, out result))
                return result;
            result = base.Visit(node);
            if (node != result)
                expressionsCache[node] = result;
            return result;
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            Debug.WriteLine(node);
            var test = Visit(node.Test);
            var ifTrue = Visit(node.IfTrue);
            var ifFalse = Visit(node.IfFalse);

            Expression expression;
            if (IsCoalesce(test, ifTrue, out expression))
            {
                return Expression.Coalesce(expression, ifFalse);
            }
            var ifTrueBinary = UnwrapConvertToNullable(ifTrue) as BinaryExpression;
            if (ifTrueBinary != null)
            {
                BinaryExpression result;
                if (TryConvert1(test, ifTrueBinary, out result))
                    return result;
            }
            
            var testBinary = test as BinaryExpression;
            var ifTrueConstant = ifTrue as ConstantExpression;
            var ifFalseConstant = ifFalse as ConstantExpression;

            if (testBinary != null)
            {
                BinaryExpression result;
                if (ifTrueBinary != null && TryConvert2(testBinary, ifTrueBinary, out result))
                {
                    return result;
                }
                if (TryConvert(ifTrueConstant, testBinary, ifFalse, out result, false))
                {
                    return result;
                }
                if (TryConvert(ifFalseConstant, testBinary, ifTrue, out result, true))
                {
                    return result;
                }
                if (testBinary.NodeType == ExpressionType.NotEqual)
                {
                    if (testBinary.Left == node.IfTrue)
                    {
                        if (testBinary.Right is DefaultExpression)
                        {
                            return Expression.Coalesce(testBinary.Left, ifFalse);
                        }

                        if (testBinary.Right is ConstantExpression rightConstant && rightConstant.Value == null)
                        {
                            return Expression.Coalesce(testBinary.Left, ifFalse);
                        }
                    }
                }
            }

            if (ifTrueConstant?.Value is true)
            {
                return Expression.OrElse(test, ifFalse);
            }

            if (test.NodeType == ExpressionType.Not)
            {
                var testOperand = ((UnaryExpression) test).Operand;
                if (ifTrueConstant?.Value as bool? == false)
                {
                    return Expression.AndAlso(testOperand, ifFalse);
                }

                return Visit(node.Update(testOperand, ifFalse, ifTrue));
            }

            return node.Update(test, ifTrue, ifFalse);
        }

        private static bool TryConvert(ConstantExpression constant, BinaryExpression left, Expression right, out BinaryExpression result, bool isLeft)
        {
            if (constant?.Value is bool booleanValue)
            {
                if (booleanValue)
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
            if (ExtractNullableArgument(hasValue.Left, getValueOrDefault.Left, out var left) &&
                ExtractNullableArgument(hasValue.Right, getValueOrDefault.Right, out var right))
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
                result = Expression.MakeBinary(getValueOrDefault.NodeType, ConvertToNullable(expression), ConvertToNullable(getValueOrDefault.Right));
                return true;
            }
            if (ExtractNullableArgument(hasValue, getValueOrDefault.Right, out expression))
            {
                result = Expression.MakeBinary(getValueOrDefault.NodeType, ConvertToNullable(getValueOrDefault.Left), ConvertToNullable(expression));
                return true;
            }
            result = null;
            return false;
        }

        static Expression ConvertToNullable(Expression expression)
        {
	        if (!expression.Type.IsValueType || IsNullable(expression.Type)) return expression;

	        var operand = expression.NodeType == ExpressionType.Convert
		        ? ((UnaryExpression) expression).Operand
		        : expression;

	        return Expression.Convert(operand, typeof(Nullable<>).MakeGenericType(expression.Type));
		}

		static Expression UnwrapConvertToNullable(Expression expression)
        {
            var unary = expression as UnaryExpression;
            if (unary != null && expression.NodeType == ExpressionType.Convert && IsNullable(expression.Type))
            {
                return unary.Operand;
            }
            return expression;
        }

        static bool ExtractNullableArgument(Expression hasValue, Expression getValueOrDefault, out Expression expression)
        {
            MemberExpression memberExpression;
            if (IsHasValue(hasValue, out memberExpression))
            {
	            expression = new GetValueOrDefaultRemover(memberExpression.Expression).Visit(getValueOrDefault);
                if (expression != getValueOrDefault)
                    return true;
            }
            
            expression = null;
            return false;
        }

        static bool IsCoalesce(Expression hasValue, Expression getValueOrDefault, out Expression expression)
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

	    static bool IsHasValue(Expression expression, out MemberExpression property)
        {
            property = expression as MemberExpression;
            return property != null && property.Member.Name == "HasValue" && property.Expression != null && IsNullable(property.Expression.Type);
        }

        static bool IsGetValueOrDefault(Expression expression, out MethodCallExpression method)
        {
            method = expression as MethodCallExpression;
            return method != null && IsGetValueOrDefault(method);
        }

        static bool IsGetValueOrDefault(MethodCallExpression method)
        {
            return method.Method.Name == "GetValueOrDefault" && method.Object != null && IsNullable(method.Object.Type);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            var left = Visit(node.Left);
            if (node.Right is ConstantExpression rightConstant)
            {
                if (rightConstant.Value as bool? == false)
                {
                    switch (node.NodeType)
                    {
                        case ExpressionType.Equal:
                            if (left is BinaryExpression binaryExpression && Invert(ref binaryExpression))
                                return binaryExpression;
                            return Expression.Not(left);
                        case ExpressionType.NotEqual:
                            return left;
                    }
                }

                if (rightConstant.Value as int? == 0 &&
                    left is MethodCallExpression expression &&
                    expression.Method.Name == "Compare" &&
                    expression.Method.IsStatic &&
                    expression.Method.DeclaringType == typeof(decimal))
                {
                    return Expression.MakeBinary(node.NodeType, expression.Arguments[0], expression.Arguments[1]);
                }
            }

            if (node.NodeType == ExpressionType.And)
            {
                if (ExtractNullableArgument(node.Right, left, out var result))
                {
                    return Visit(result);
                }

                if (left is BinaryExpression leftBinary && node.Right is BinaryExpression rightBinary)
                {
                    if (TryConvert2(rightBinary, leftBinary, out var binaryResult))
                    {
                        return Visit(binaryResult);
                    }
                }
            }

            return base.VisitBinary(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Not && 
                node.Operand is BinaryExpression binary &&
                Invert(ref binary))
            {
                return Visit(binary);
            }

            return base.VisitUnary(node);
        }


        static bool Invert(ref BinaryExpression expression)
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

        class GetValueOrDefaultRemover :ExpressionVisitor
        {
            readonly Expression expected;

            public GetValueOrDefaultRemover(Expression expected)
            {
                this.expected = expected;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (IsGetValueOrDefault(node) && node.Object == expected)
                {
                    return node.Object;
                }

                return base.VisitMethodCall(node);
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                var left = Visit(node.Left);
                var conversion = VisitAndConvert(node.Conversion, nameof(VisitBinary));
                var right = Visit(node.Right);

                if (left != node.Left || right != node.Right)
                {
                    left = ConvertToNullable(left);
                    right = ConvertToNullable(right);
                }

                return node.Update(left, conversion, right);
            }

	        protected override Expression VisitUnary(UnaryExpression node)
	        {
		        var before = node;
		        var operand = Visit(node.Operand);

		        if (operand != before.Operand)
		        {
			        operand = ConvertToNullable(operand);
		        }

		        return before.Update(operand);
	        }
		}

        class GetValueOrDefaultToCoalesceConverter : ExpressionVisitor
        {
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (IsGetValueOrDefault(node) &&
                    node.Type != typeof(DateTime) &&
                    node.Type != typeof(DateTimeOffset))
                {
                    var @default = node.Arguments.Count == 0 ? ExpressionHelper.Default(node.Type) : node.Arguments[0];
                    return Expression.Coalesce(node.Object, @default);
                }

                return base.VisitMethodCall(node);
            }
        }

        public static Expression Optimize(Expression expression)
        {
            return new GetValueOrDefaultToCoalesceConverter().Visit(new OptimizeExpressionVisitor().Visit(expression));
        }
    }
}
