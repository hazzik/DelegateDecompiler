using System.Linq;

namespace DelegateDecompiler.EntityFramework
{
    public static class DecompileExtensions
    {
        public static IQueryable<T> DecompileAsync<T>(this IQueryable<T> self)
        {
            return new AsyncDecompiledQueryProvider(self.Provider).CreateQuery<T>(self.Expression);
        }
    }
}
