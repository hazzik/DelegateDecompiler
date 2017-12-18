﻿using NUnit.Framework;
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

        private class ConcreteClass1
            : BaseClass
        {
            public int OtherProperty { get; set; }

            [Computed]
            public override int GetTotal()
            {
                return base.GetTotal() + OtherProperty;
            }
        }

        private class ConcreteClass2
            : ConcreteClass1
        {
            [Computed]
            public override int GetTotal()
            {
                return base.GetTotal() + 5;
            }
        }

        private class ConcreteClass3_NotOverridden
            : BaseClass
        {
        }

        [Test]
        public void DecompileOveriddenMemberWithBaseCall()
        {
            Expression<Func<ConcreteClass1, int>> expected = x => x is ConcreteClass2 ? (x as ConcreteClass2).Property + (x as ConcreteClass2).OtherProperty + 5 : x.Property + x.OtherProperty;
            Expression<Func<ConcreteClass1, int>> compiled = x => x.GetTotal();
            var result = new OptimizeExpressionVisitor().Visit(DecompileExpressionVisitor.Decompile(compiled));
            Assert.AreEqual(expected.ToString(), result.ToString());
        }

        [Test]
        public void DecompileOveriddenMemberWithFullTypeHierarchy()
        {
            Expression<Func<BaseClass, int>> expected = x => x is ConcreteClass2 ? (x as ConcreteClass2).Property + (x as ConcreteClass2).OtherProperty + 5 : x is ConcreteClass1 ? (x as ConcreteClass1).Property + (x as ConcreteClass1).OtherProperty : x.Property;
            Expression<Func<BaseClass, int>> compiled = x => x.GetTotal();
            var result = new OptimizeExpressionVisitor().Visit(DecompileExpressionVisitor.Decompile(compiled));
            Assert.AreEqual(expected.ToString(), result.ToString());
        }
    }
}