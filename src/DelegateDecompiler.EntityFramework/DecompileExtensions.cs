﻿using System.Linq;

namespace DelegateDecompiler.EntityFramework
{
    public static class DecompileExtensions
    {
        public static IQueryable<T> Decompile<T>(this IQueryable<T> self)
        {
            var provider = new AsyncDecompiledQueryProvider(self.Provider);
            return provider.CreateQuery<T>(self.Expression);
        }

        public static IQueryable<T> DecompileAsync<T>(this IQueryable<T> self)
        {
            return new AsyncDecompiledQueryProvider(self.Provider).CreateQuery<T>(self.Expression);
        }
    }
}