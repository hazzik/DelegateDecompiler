// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using DelegateDecompiler.EfTests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EfTests.TestGroup15Aggregation
{
    class Test02Sum
    {
        private ClassEnvironment classEnv;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public void TestSingletonSumChildren()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var linq = env.Db.EfParents.Sum(x => x.Children.Count());

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Decompile().Sum(x => x.CountChildren);

                //VERIFY
                env.CompareAndLogSingleton(linq, dd);
            }
        }

        [Test]
        public void TestSumCountInChildrenWhereChildrenCanBeNone()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var linq = env.Db.EfParents.Select(x => x.Children.Sum( y => (int?)y.ChildInt) ?? 0).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => x.SumIntInChildrenWhereChildrenCanBeNone).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

    }
}
