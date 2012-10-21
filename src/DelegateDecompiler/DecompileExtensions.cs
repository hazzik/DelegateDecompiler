using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiler
{
    public static class DecompileExtensions
    {
        public static LambdaExpression Decompile(this Delegate @delegate)
        {
            return Decompile(@delegate.Method);
        }

        public static LambdaExpression Decompile(this MethodInfo method)
        {
            return new MethodDecompiler(method).Decompile();
        }

        public static IQueryable<T> Decompile<T>(this IQueryable<T> self)
        {
            var expression = DecompileExpressionVisitor.Decompile(self.Expression);
            if (expression != self.Expression)
            {
                return self.Provider.CreateQuery<T>(expression);
            }
            return self;
        }
    }
}
