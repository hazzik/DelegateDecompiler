// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace DelegateDecompiler.EntityFramework.Tests.Helpers
{
    internal static class ExtendAsserts
    {
        internal static void ShouldEqual(this string actualValue, string expectedValue, string errorMessage = null)
        {
            Assert.That(actualValue, Is.EqualTo(expectedValue), errorMessage);
        }

        internal static void ShouldStartWith(this string actualValue, string expectedValue, string errorMessage = null)
        {
            Assert.That(actualValue, Does.StartWith(expectedValue), errorMessage);
        }

        internal static void ShouldContain(this string actualValue, string expectedValue, string errorMessage = null)
        {
            //Regular StringAssert.Contains uses CurrentCulture, and fails in .NET 5.0
            var constraint = new SubstringConstraint(expectedValue).Using(StringComparison.Ordinal);

            Assert.That(actualValue, constraint, errorMessage);
        }

        internal static void ShouldEqual<T>(this T actualValue, T expectedValue, string errorMessage = null)
        {
            Assert.That(actualValue, Is.EqualTo(expectedValue), errorMessage);
        }

        internal static void ShouldBeGreaterThan(this int actualValue, int greaterThanThis, string errorMessage = null)
        {
            Assert.That(actualValue, Is.GreaterThan(greaterThanThis), errorMessage);
        }
    }
}
