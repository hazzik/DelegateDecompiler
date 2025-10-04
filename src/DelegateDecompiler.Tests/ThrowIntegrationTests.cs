using System;
using System.Linq.Expressions;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class ThrowTests : DecompilerTestsBase
    {
        [Test]
        public void SimpleThrow()
        {
            Action compiled = () => throw new ArgumentException("test");
            
            var decompiled = compiled.Decompile();
            Assert.That(decompiled, Is.Not.Null);
            Assert.That(decompiled.Body, Is.Not.Null);
            Assert.That(decompiled.Body.NodeType, Is.EqualTo(ExpressionType.Throw));
        }
    }
}