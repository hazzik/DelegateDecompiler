using System.Linq.Expressions;

namespace DelegateDecompiller
{
    public static class DelegateDecompillerExtensions
    {
        public static LambdaExpression Decompile<TDelegate>(this TDelegate @delegate)
        {
            var parameter = Expression.Parameter(typeof (object), "o");
            return Expression.Lambda(parameter, parameter);
        }
    }
}
