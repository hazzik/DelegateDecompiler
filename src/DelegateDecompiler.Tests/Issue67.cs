using System;
using System.Linq;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    public class Issue67
    {
        [Test]
        public void Should_be_able_to_decompile_enum()
        {
            var actual = Enumerable.Range(1, 3)
                .Select(x => new Foo { Value = x })
                .AsQueryable()
                .Select(x => x.EnumName)
                .Decompile();

            Assert.That(actual.First(), Is.EqualTo("A"));
        }

        public enum FooEnum { A = 1, B = 2, C = 3 }

        public class Foo
        {
            public int Value { get; set; }

            [Computed]
            public string EnumName
            {
                get
                {
                    return Enum.GetName(typeof (FooEnum), Value);
                }
            }
        }
    }
}