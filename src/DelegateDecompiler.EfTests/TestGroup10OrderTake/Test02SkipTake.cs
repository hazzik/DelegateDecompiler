// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using DelegateDecompiler.EfTests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EfTests.TestGroup10OrderTake
{
    class Test02SkipTake
    {
        private ClassEnvironment classEnv;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public void TestOrderByChildrenCountThenTake()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var linq = env.Db.EfParents.OrderBy(x => x.Children.Count).Select( x => x.EfParentId).Take(2).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.OrderBy(x => x.CountChildren).Select( x => x.EfParentId).Take(2).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestOrderByChildrenCountThenSkipAndTake()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var linq = env.Db.EfParents.OrderBy(x => x.Children.Count).Select(x => x.EfParentId).Skip(1).Take(2).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.OrderBy(x => x.CountChildren).Select(x => x.EfParentId).Skip(1).Take(2).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestWhereAnyChildrenThenOrderByThenSkipTake()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var linq = env.Db.EfParents.Where(x => x.Children.Any()).OrderBy(x => x.Children.Count).Select(x => x.EfParentId).Skip(1).Take(1).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Where(x => x.AnyChildren).OrderBy(x => x.CountChildren).Select(x => x.EfParentId).Decompile().Skip(1).Take(1).ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }
    }
}
