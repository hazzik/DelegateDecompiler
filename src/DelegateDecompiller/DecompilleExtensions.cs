using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DelegateDecompiller
{
    public static class DecompilleExtensions
    {
        public static LambdaExpression Decompile(this Delegate @delegate)
        {
            return Decompile(@delegate.Method);
        }

        public static LambdaExpression Decompile(this MethodBase method)
        {
            return new MethodDecompiller(method).Decompile();
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
