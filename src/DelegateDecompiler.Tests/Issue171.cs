using NUnit.Framework;

namespace DelegateDecompiler.Tests
{
    [TestFixture]
    public class Issue171
    {
        [Test]
        public void ShouldSupportStructsWithoutInitializer()
        {
            var method = typeof(Issue171).GetMethod(nameof(X1));
            var expression = method.Decompile();

            Assert.That(expression.ToString(), Is.EqualTo("this => new DataOnStack() {StatementCount = 1}"));
        }

        [Test]
        public void ShouldSupportStructsWithInitializer()
        {
            var method = typeof(Issue171).GetMethod(nameof(X2));
            var expression = method.Decompile();

            Assert.That(expression.ToString(), Is.EqualTo("this => new DataOnStack() {StatementCount = 1}"));
        }

        [Test]
        public void ShouldSupportMethodsWithByRefArguments()
        {
            var method = typeof(Issue171).GetMethod(nameof(X3));
            var expression = method.Decompile();

            Assert.That(expression.ToString(), Is.EqualTo("this => new DataOnStack() {StatementCount = 1}"));
        }

        public DataOnStack X1()
        {
            DataOnStack x;
            x.StatementCount = 1;
            return x;
        }

        public DataOnStack X2()
        {
            return new DataOnStack {StatementCount = 1};
        }
        
        public DataOnStack X3()
        {
            DataOnStack x;
            x.StatementCount = 1;
            DataOnStack.JetBrains_Profiler_Core_Instrumentation_Begin(ref x);
            return x;
        }

        public struct DataOnStack
        {
            public uint StatementCount;

            public static void JetBrains_Profiler_Core_Instrumentation_Begin(ref DataOnStack d)
            {
            }
        }
    }
}
