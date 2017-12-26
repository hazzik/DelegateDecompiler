using System.Data.Entity.Core.Objects;
using System.Linq;

namespace DelegateDecompiler.EntityFramework
{
    public static class DecompileExtensions
    {
        internal static MergeOption CurrentMergeOption { get; set; }

        public static IQueryable<T> Decompile<T>(this IQueryable<T> self)
        {
            var currentMergeOption = CurrentMergeOption;
            try
            {
                if (self is AsyncDecompiledQueryable<T>)
                {
                    CurrentMergeOption = (self as AsyncDecompiledQueryable<T>).MergeOption | currentMergeOption;
                }
                var provider = new AsyncDecompiledQueryProvider(self.Provider);
                return provider.CreateQuery<T>(self.Expression);
            }
            finally
            {
                CurrentMergeOption = currentMergeOption;
            }
        }

        public static IQueryable<T> DecompileAsync<T>(this IQueryable<T> self)
        {
            return Decompile<T>(self);
        }
    }
}