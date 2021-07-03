// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.Helpers
{
    static class CheckerAndLogger
    {
        public static void CompareAndLogList<T>(this MethodEnvironment env, IList<T> linqResult, IList<T> ddResult, 
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (!linqResult.Any())
                throw new ArgumentException("The linq result was empty, so this was not a fair test.");

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

        public static void CompareAndLogNonGenericList(this MethodEnvironment env, IList linqResult, IList ddResult,
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            if (linqResult.Count == 0)
                throw new ArgumentException("The linq result was empty, so this was not a fair test.");

            try
            {
                CollectionAssert.AreEqual(linqResult, ddResult);
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
