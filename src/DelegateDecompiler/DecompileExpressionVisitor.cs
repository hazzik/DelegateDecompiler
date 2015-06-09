using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiler
{
    public class DecompileExpressionVisitor : ExpressionVisitor
    {
        public static Expression Decompile(Expression expression)
        {
            return new DecompileExpressionVisitor().Visit(expression);
        }

		private static readonly object NULL = new object(); // for use as a dictionary key
		private readonly Dictionary<object, Expression> visitedConstants;

		private bool hasAnyChanges = false;
		public override Expression Visit(Expression node)
		{
			var result = base.Visit(node);
			if (result != node)
				hasAnyChanges = true;
			return result;
		}

		private DecompileExpressionVisitor(Dictionary<object, Expression> sharedVisitedConstants = null)
		{
			this.visitedConstants = sharedVisitedConstants ?? new Dictionary<object, Expression>();
		}

		protected override Expression VisitConstant(ConstantExpression node)
		{
			Expression result;
			if (visitedConstants.TryGetValue(node.Value ?? NULL, out result))
			{
				return result; // avoid infinite recursion
			}

			if (typeof(IQueryable).IsAssignableFrom(node.Type))
			{
				visitedConstants.Add(node.Value ?? NULL, node);

				var value = (IQueryable)node.Value;
				var childVisitor = new DecompileExpressionVisitor(visitedConstants);
				result = childVisitor.Visit(value.Expression);

				if (childVisitor.hasAnyChanges)
				{
					result = Expression.Constant(value.Provider.CreateQuery(result), node.Type);
					visitedConstants[node.Value ?? NULL] = result;
					return result;
				}
			}

			return node;
		}

        protected override Expression VisitMember(MemberExpression node)
        {
            if (ShouldDecompile(node.Member))
            {
                var info = node.Member as PropertyInfo;
                if (info != null)
                {
                    return Decompile(info.GetGetMethod(), node.Expression, new List<Expression>());
                }
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.IsGenericMethod && node.Method.GetGenericMethodDefinition() == typeof (ComputedExtension).GetMethod("Computed", BindingFlags.Static | BindingFlags.Public))
            {
                var argument = node.Arguments.SingleOrDefault();

                var member = argument as MemberExpression;
                if (member != null)
                {
                    var info = member.Member as PropertyInfo;
                    if (info != null)
                    {
                        return Decompile(info.GetGetMethod(), member.Expression, new List<Expression>());
                    }
                }
                var methodCall = argument as MethodCallExpression;
                if (methodCall != null)
                {
                    return Decompile(methodCall.Method, methodCall.Object, methodCall.Arguments);
                }
            }

            if (ShouldDecompile(node.Method))
            {
                return Decompile(node.Method, node.Object, node.Arguments);
            }

            return base.VisitMethodCall(node);
        }

        protected virtual bool ShouldDecompile(MemberInfo methodInfo)
        {
            return Configuration.Instance.ShouldDecompile(methodInfo);
        }

        Expression Decompile(MethodInfo method, Expression instance, IList<Expression> arguments)
        {
            var expression = method.Decompile(instance?.Type);

            var expressions = new Dictionary<Expression, Expression>();
            var argIndex = 0;
            for (var index = 0; index < expression.Parameters.Count; index++)
            {
                var parameter = expression.Parameters[index];
                if (index == 0 && method.IsStatic == false)
                {
                    expressions.Add(parameter, instance);
                }
                else
                {
                    expressions.Add(parameter, arguments[argIndex++]);
                }
            }

            return Visit(new ReplaceExpressionVisitor(expressions).Visit(expression.Body));
        }
    }
}
