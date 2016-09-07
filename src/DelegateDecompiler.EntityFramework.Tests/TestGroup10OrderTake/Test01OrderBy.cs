// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup10OrderTake
{
    class Test01OrderBy
    {
        private ClassEnvironment classEnv;

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public void TestOrderByChildrenCount()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var linq = env.Db.EfParents.OrderBy(x => x.Children.Count).Select( x => x.EfParentId).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.OrderBy(x => x.CountChildren).Select( x => x.EfParentId).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestOrderByChildrenCountThenByStringLength()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var linq = env.Db.EfParents.OrderBy(x => x.Children.Count).ThenBy( x => x.ParentString.Length).Select(x => x.EfParentId).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.OrderBy(x => x.CountChildren).ThenBy( x => x.GetStringLength).Select(x => x.EfParentId).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestWhereAnyChildrenThenOrderByChildrenCount()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var linq = env.Db.EfParents.Where(x => x.Children.Any()).OrderBy(x => x.Children.Count).Select(x => x.EfParentId).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Where(x => x.AnyChildren).OrderBy(x => x.CountChildren).Select(x => x.EfParentId).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

    }
}
