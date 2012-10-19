using System;
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
            return new MethodDecompiller(method).Decompile();
        }
    }
}
