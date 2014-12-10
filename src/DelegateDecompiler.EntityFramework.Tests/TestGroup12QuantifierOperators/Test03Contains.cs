// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup12QuantifierOperators
{
    class Test03Contains
    {
        private ClassEnvironment classEnv;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public void TestStringContainsConstantStringWithFilter()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq =
                    env.Db.EfParents.Where(x => x.ParentString.Contains("2")).Select(x => x.ParentString).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Where(x => x.StringContainsConstantString).Select(x => x.ParentString).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

    }
}
