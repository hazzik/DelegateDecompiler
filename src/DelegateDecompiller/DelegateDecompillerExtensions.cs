using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiller
{
    public static class DelegateDecompillerExtensions
    {
        public static LambdaExpression Decompile(this Delegate @delegate)
        {
            return Decompile(@delegate.Method);
        }

        public static LambdaExpression Decompile(this MethodBase method)
        {
            return MethodDecompiller(method);
        }

        static LambdaExpression MethodDecompiller(MethodBase method)
        {
            Expression ex = Expression.Empty();
            var stack = new Stack<Expression>();
            var locals = new Expression[16];

            return new MethodDecompiller(method).Decompile();
        }
    }
}
