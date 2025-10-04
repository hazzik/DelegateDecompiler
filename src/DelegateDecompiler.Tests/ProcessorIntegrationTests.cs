using System;
using System.Linq.Expressions;
using System.Reflection.Emit;
using DelegateDecompiler;
using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class ProcessorIntegrationTests : DecompilerTestsBase
    {
        [Test]
        public void ProcessorChain_ShouldHandleArithmetic_Add()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x + y;
            Func<int, int, int> compiled = (x, y) => x + y;
            Test(compiled, expected);
        }

        [Test] 
        public void ProcessorChain_ShouldHandleArithmetic_Multiply()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x * y;
            Func<int, int, int> compiled = (x, y) => x * y;
            Test(compiled, expected);
        }

        [Test]
        public void ProcessorChain_ShouldHandleBitwise_Xor()
        {
            Expression<Func<int, int, int>> expected = (x, y) => x ^ y;
            Func<int, int, int> compiled = (x, y) => x ^ y;
            Test(compiled, expected);
        }

        [Test]
        public void ProcessorChain_ShouldHandleUnary_Negation()
        {
            Expression<Func<int, int>> expected = x => -x;
            Func<int, int> compiled = x => -x;
            Test(compiled, expected);
        }

        [Test]
        public void ProcessorChain_ShouldHandleConvert_ByteToInt()
        {
            Expression<Func<byte, int>> expected = x => (int)x;
            Func<byte, int> compiled = x => (int)x;
            Test(compiled, expected);
        }

        [Test]
        public void ProcessorChain_ShouldHandleComparison_Equal()
        {
            Expression<Func<int, int, bool>> expected = (x, y) => x == y;
            Func<int, int, bool> compiled = (x, y) => x == y;
            Test(compiled, expected);
        }

        [Test]
        public void ProcessorChain_ShouldHandleComparison_GreaterThan()
        {
            Expression<Func<int, int, bool>> expected = (x, y) => x > y;
            Func<int, int, bool> compiled = (x, y) => x > y;
            Test(compiled, expected);
        }
    }
}