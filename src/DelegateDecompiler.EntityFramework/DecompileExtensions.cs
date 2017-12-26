using System.Linq;

namespace DelegateDecompiler.EntityFramework
{
    public static class DecompileExtensions
    {
        public static IQueryable<T> Decompile<T>(this IQueryable<T> self)
        {
            // Minor optimisation : avoid stacking multiple instances when chaining multiple
            // Decompile calls
            var provider = self.Provider;
            if (self.Provider.GetType() != typeof(AsyncDecompiledQueryProvider)) provider = new AsyncDecompiledQueryProvider(self.Provider);
            return provider.CreateQuery<T>(self.Expression);
        }

        public static IQueryable<T> DecompileAsync<T>(this IQueryable<T> self)
        {
            return Decompile<T>(self);
        }
    }
}