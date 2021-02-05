using System;
using System.Linq;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    public class Issue162
    {
        [Test]
        public void ShouldBeAbleToDecompileUsingProtectedProperty()
        {
            var actual = new[] {new Foo(42)}
                .AsQueryable()
                .Select(x => x.PublicProperty)
                .Decompile();

            Assert.That(actual.First(), Is.EqualTo(42));
        }

        public class Foo
        {
            public Foo(int value)
            {
                ProtectedProperty = value;
            }

            [Computed]
            protected virtual int ProtectedProperty { get; private set; }

            [Computed]
            public virtual int PublicProperty => ProtectedProperty;
        }
    }
}
