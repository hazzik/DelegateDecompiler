using System.Linq;

namespace DelegateDecompiler.EntityFrameworkCore
{
    public static class DecompileExtensions
    {
        public static IQueryable<T> DecompileAsync<T>(this IQueryable<T> self)
        {
            return new AsyncDecompiledQueryProvider(self.Provider).CreateQuery<T>(self.Expression);
        }
    }
}
