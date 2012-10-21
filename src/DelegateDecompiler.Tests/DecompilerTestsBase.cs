using System;
using System.Linq.Expressions;
using Xunit;

namespace DelegateDecompiler.Tests
{
    public class DecompilerTestsBase
    {
        protected static void Test<T>(Expression<T> expected, T compiled)
        {
            //There is no way to direct cast T to Delegate, so we are using dynamic trick
            var decompiled = Delegate.Combine(new Delegate[] { (dynamic) compiled }).Decompile();

            Assert.Equal(expected.ToString(), decompiled.ToString());
        }
    }
}
