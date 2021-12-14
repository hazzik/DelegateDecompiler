using System;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    public class GenericTypesTest
    {
        class A<T1, T2>
        {
        }

        class B<T1, T2> : A<T2, T1>
        {
        }

        [Test]
        public void CanConstructClosedBFromA()
        {
            var a = typeof(A<int, string>);
            var b = typeof(B<,>).MakeGenericTypeFromClosedParentArguments(a.GetGenericArguments());
            Assert.That(b, Is.EqualTo(typeof(B<string, int>)));
        }
    }
}
