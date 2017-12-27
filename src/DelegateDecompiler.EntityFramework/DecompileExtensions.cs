using System.Linq;

namespace DelegateDecompiler.EntityFramework
{
    public static class DecompileExtensions
    {
        public static IQueryable<T> Decompile<T>(this IQueryable<T> self)
        {
            // Minor optimisation : avoid stacking multiple instances when chaining multiple calls to Decompile
            var provider = self.Provider;
            if (provider.GetType() != typeof(AsyncDecompiledQueryProvider)) provider = new AsyncDecompiledQueryProvider(self.Provider);
            AsyncDecompiledQueryable<T> decompiled = (AsyncDecompiledQueryable<T>)provider.CreateQuery<T>(self.Expression);
            if (self is AsyncDecompiledQueryable<T>) decompiled.MergeOption = ((AsyncDecompiledQueryable<T>)self).MergeOption;
            return decompiled;
        }

        public static IQueryable<T> DecompileAsync<T>(this IQueryable<T> self)
        {
            return Decompile<T>(self);
        }
    }
}