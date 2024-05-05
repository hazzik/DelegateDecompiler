// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup10OrderTake
{
    class Test02SkipTake
    {
        private ClassEnvironment classEnv;

        [OneTimeSetUp]
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

                var linq = env.Db.EfParents.OrderBy(x => x.Children.Count).Select(x => x.EfParentId).Take(2).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.OrderBy(x => x.CountChildren).Select(x => x.EfParentId).Take(2)
#if NO_AUTO_DECOMPILE
                    .Decompile()
#endif
                    .ToList();

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
                var dd = env.Db.EfParents.OrderBy(x => x.CountChildren).Select(x => x.EfParentId).Skip(1).Take(2)
#if NO_AUTO_DECOMPILE
                    .Decompile()
#endif
                    .ToList();

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
                var dd = env.Db.EfParents.Where(x => x.AnyChildren).OrderBy(x => x.CountChildren).Select(x => x.EfParentId)
#if NO_AUTO_DECOMPILE
                    .Decompile()
#endif
                    .Skip(1).Take(1).ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }
    }
}
