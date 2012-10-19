using System.Linq;

namespace DelegateDecompiller
{
    public static class DecompileIQueryableExteions
    {
        public static IQueryable<T> DecompileExpressions<T>(this IQueryable<T> self)
        {
            var expression = new DecompileExpressionVisitor().Visit(self.Expression);
            if (expression != self.Expression)
            {
                return self.Provider.CreateQuery<T>(expression);
            }
            return self;
        }
    }
}
