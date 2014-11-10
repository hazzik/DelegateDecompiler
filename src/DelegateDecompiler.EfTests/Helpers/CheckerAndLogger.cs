// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace DelegateDecompiler.EfTests.Helpers
{
    static class CheckerAndLogger
    {
        public static void CompareAndLogList<T>(this MethodEnvironment env, IList<T> linqResult, IList<T> ddResult, 
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            try
            {
                CollectionAssert.AreEqual( linqResult, ddResult);
            }
            catch (Exception)
            {
                env.LogFailer(sourceLineNumber);
                throw;
            }
            env.LogSuccess(sourceLineNumber);  
        }

        public static void CompareAndLogSingleton<T>(this MethodEnvironment env, T linqResult, T ddResult,
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (typeof(T).IsSubclassOf(typeof(List)))
                throw new ArgumentException("You should use CompareAndLogCollection for comparing lists.");

            try
            {
                Assert.AreEqual(linqResult, ddResult);
            }
            catch (Exception)
            {
                env.LogFailer(sourceLineNumber);
                throw;
            }
            env.LogSuccess(sourceLineNumber);
        }
    }
}
