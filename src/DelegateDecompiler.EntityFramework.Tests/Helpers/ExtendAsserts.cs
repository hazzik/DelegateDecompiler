// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace DelegateDecompiler.EntityFramework.Tests.Helpers
{
    internal static class ExtendAsserts
    {


        internal static void ShouldEqual(this string actualValue, string expectedValue, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, actualValue, errorMessage);
        }

        internal static void ShouldStartWith(this string actualValue, string expectedValue, string errorMessage = null)
        {
            StringAssert.StartsWith(expectedValue, actualValue, errorMessage);
        }

        internal static void ShouldEndWith(this string actualValue, string expectedValue, string errorMessage = null)
        {
            StringAssert.EndsWith(expectedValue, actualValue, errorMessage);
        }

        internal static void ShouldContain(this string actualValue, string expectedValue, string errorMessage = null)
        {
            //Regular StringAssert.Contains uses CurrentCulture, and fails in .NET 5.0
            var constraint = new SubstringConstraint(expectedValue).Using(StringComparison.Ordinal);

            Assert.That(actualValue, constraint, errorMessage);
        }

        internal static void ShouldNotEqual(this string actualValue, string expectedValue, string errorMessage = null)
        {
            Assert.True(expectedValue != actualValue, errorMessage);
        }

        internal static void ShouldEqualWithTolerance(this float actualValue, double expectedValue, double tolerance, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, actualValue, tolerance, errorMessage);
        }

        internal static void ShouldEqualWithTolerance(this long actualValue, long expectedValue, int tolerance, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, actualValue, tolerance, errorMessage);
        }

        internal static void ShouldEqualWithTolerance(this double actualValue, double expectedValue, double tolerance, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, actualValue, tolerance, errorMessage);
        }

        internal static void ShouldEqualWithTolerance(this int actualValue, int expectedValue, int tolerance, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, actualValue, tolerance, errorMessage);
        }

        internal static void ShouldEqual<T>( this T actualValue, T expectedValue, string errorMessage = null)
        {
            Assert.AreEqual(expectedValue, actualValue, errorMessage);
        }

        internal static void ShouldEqual<T>(this T actualValue, T expectedValue, IEnumerable<string> errorMessages)
        {
            Assert.AreEqual(expectedValue, actualValue,  string.Join("\n", errorMessages));
        }

        internal static void ShouldEqual<T>(this T actualValue, T expectedValue, IEnumerable<ValidationResult> validationResults)
        {
            Assert.AreEqual(expectedValue, actualValue, string.Join("\n", validationResults.Select( x => x.ErrorMessage)));
        }

        internal static void ShouldNotEqual<T>(this T actualValue, T unexpectedValue, string errorMessage = null)
        {
            Assert.AreNotEqual(unexpectedValue, actualValue);
        }

        internal static void ShouldNotEqualNull<T>(this T actualValue, string errorMessage = null) where T : class
        {
            Assert.NotNull( actualValue);
        }

        internal static void ShouldBeGreaterThan(this int actualValue, int greaterThanThis, string errorMessage = null)
        {
            Assert.Greater(actualValue, greaterThanThis);
        }

        internal static void IsA<T>(this object actualValue, string errorMessage = null) where T : class
        {
            Assert.True(actualValue.GetType() == typeof(T), "expected type {0}, but was of type {1}", typeof(T).Name, actualValue.GetType().Name);
        }
    }
}
