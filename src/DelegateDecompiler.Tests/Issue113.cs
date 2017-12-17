using NUnit.Framework;
using System;
using System.Linq.Expressions;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class Issue113 : DecompilerTestsBase
    {
        private abstract class BaseClass
        {
            public int Property { get; set; }

            [Computed]
            public virtual int GetTotal()
            {
                return Property;
            }
        }

        private class ConcreteClass
            : BaseClass
        {
            public int OtherProperty { get; set; }

            [Computed]
            public override int GetTotal()
            {
                return base.GetTotal() + OtherProperty;
            }
        }

        [Test]
        public void DecompileOveriddenMemberWithBaseCall()
        {
            Expression<Func<ConcreteClass, int>> expected = x => x.Property + x.OtherProperty;
            Expression<Func<ConcreteClass, int>> compiled = x => x.GetTotal();
            var result = DecompileExpressionVisitor.Decompile(compiled);
            Assert.AreEqual(expected.ToString(), result.ToString());
            //Test(expected, compiled);
        }
    }
}