using System;
using System.Linq.Expressions;

namespace DelegateDecompiler
{
    // Convert nullable constructors to a Convert call instead
    public class OptimizeExpressionVisitor : ExpressionVisitor
    {
        protected override Expression VisitNew(NewExpression node)
        {
            // Test if this is a nullable type
            if (IsNullable(node.Type))
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
            var ifTrue = Visit(node.IfTrue);
            var ifFalse = Visit(node.IfFalse);
            var test = Visit(node.Test);

            var binaryExpression = test as BinaryExpression;
            var ifTrueConstant = ifTrue as ConstantExpression;
            if (binaryExpression != null)
            {
                if (ifTrueConstant != null && ifTrueConstant.Value is bool)
                {
                    if ((bool) ifTrueConstant.Value)
                    {
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

            var property = test as MemberExpression;
            var method = ifTrue as MethodCallExpression;
            if (property != null && property.Member.Name == "HasValue" &&
                method != null && method.Method.Name == "GetValueOrDefault")
            {
                var expression = property.Expression;
                if (expression == method.Object && IsNullable(expression.Type))
                {
                    return Expression.Coalesce(expression, ifFalse);
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
