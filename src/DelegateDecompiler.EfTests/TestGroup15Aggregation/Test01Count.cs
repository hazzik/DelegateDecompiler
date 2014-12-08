// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using DelegateDecompiler.EfTests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EfTests.TestGroup15Aggregation
{
    class Test01Count
    {
        private ClassEnvironment classEnv;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public void TestCountChildren()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var linq = env.Db.EfParents.Select(x => x.Children.Count).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => x.CountChildren).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestCountChildrenWithFilter()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var linq = env.Db.EfParents.Select(x => x.Children.Count( y => y.ChildInt == 123)).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => x.CountChildrenWithFilter).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestSingletonCountChildrenWithFilter()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var linq = env.Db.EfParents.Count(x => x.Children.Count() == 2);

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Decompile().Count(x => x.CountChildren == 2);

                //VERIFY
                env.CompareAndLogSingleton(linq, dd);
            }
        }

    }
}
