using System;
using System.Linq;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class UnboxTests : DecompilerTestsBase
    {
        struct Point
        {
            public int X { get; set; }
            public int Y { get; set; }
            [Computed] public int Sum => X + Y;
        }

        [Test]
        public void UnboxAnyFieldAccess()
        {
            Expression<Func<object, int>> expected = o => ((Point)o).X;
            Func<object, int> compiled = o => ((Point)o).X;
            Test(compiled, expected);
        }

        [Test]
        public void UnboxAnyPropertyAccess()
        {
            Expression<Func<object, int>> expected = o => ((Point)o).X + ((Point)o).Y;
            Func<object, int> compiled = o => ((Point)o).Sum;
            Test(compiled, expected);
        }

        [Test]
        public void GenericUnconstrainedUsage()
        {
            Expression<Func<int, int>> expected = value => value.GetHashCode();
            Func<int, int> compiled = value => value.GetHashCode();
            Test(compiled, expected);
        }

        [Test]
        public void UnboxToNullable()
        {
            Expression<Func<object, int?>> expected = o => (int?)o;
            Func<object, int?> compiled = o => (int?)o;
            Test(compiled, expected);
        }

        [Test]
        public void UnboxEnum()
        {
            Expression<Func<object, DayOfWeek>> expected = o => (DayOfWeek)o;
            Func<object, DayOfWeek> compiled = o => (DayOfWeek)o;
            Test(compiled, expected);
        }

        [Test]
        public void BoxedStructFieldAccessInQueryable()
        {
            // Simulates: qs.Select(o => ((Point)o).X).Decompile()
            var point = new Point { X = 5, Y = 3 };
            object boxedPoint = point;
            
            Expression<Func<object, int>> expected = o => ((Point)o).X;
            Func<object, int> compiled = o => ((Point)o).X;
            Test(compiled, expected);
            
            // Verify it actually works
            Assert.That(compiled(boxedPoint), Is.EqualTo(5));
        }

        [Test]
        public void GenericGetHashCodeMethod()
        {
            // Simulates: static int GetValueHash<T>(T value) => value.GetHashCode()
            // when T is a struct
            Expression<Func<DateTime, int>> expected = value => value.GetHashCode();
            Func<DateTime, int> compiled = value => value.GetHashCode();
            Test(compiled, expected);
        }
    }
}