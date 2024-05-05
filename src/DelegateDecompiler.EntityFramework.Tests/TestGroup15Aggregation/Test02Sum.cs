// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup15Aggregation
{
    class Test02Sum
    {
        private ClassEnvironment classEnv;

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

#if !EF_CORE3
        [Test]
        public void TestSingletonSumChildren()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var linq = env.Db.EfParents.Sum(x => x.Children.Count());

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents
#if NO_AUTO_DECOMPILE
                    .Decompile()
#endif
                    .Sum(x => x.CountChildren);

                //VERIFY
                env.CompareAndLogSingleton(linq, dd);
            }
        }
#endif

        [Test]
        public void TestSumCountInChildrenWhereChildrenCanBeNone()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var linq = env.Db.EfParents.Select(x => x.Children.Sum(y => (int?)y.ChildInt) ?? 0).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => x.SumIntInChildrenWhereChildrenCanBeNone)
#if NO_AUTO_DECOMPILE
                    .Decompile()
#endif
                    .ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

    }
}
