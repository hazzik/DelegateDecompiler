// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup15Aggregation
{
    class Test01Count
    {
        private ClassEnvironment classEnv;

        [OneTimeSetUp]
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
        public void TestCountChildrenWithFilterByClosure()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var linq = env.Db.EfParents.Select(x => x.Children.Count( y => y.ChildInt == x.EfParentId)).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => x.CountChildrenWithFilterByClosure).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestCountChildrenWithFilterByExternalClosure()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var i = 123;
                var linq = env.Db.EfParents.Select(x => x.Children.Count(y => y.ChildInt == i)).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => x.GetCountChildrenWithFilterByExternalClosure(i)).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestCountChildrenWithFilterByExternalClosure2()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var i = 123;
                var j = 456;
                var linq = env.Db.EfParents.Select(x => x.Children.Count(y => y.ChildInt == i && y.EfParentId == j)).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => x.GetCountChildrenWithFilterByExternalClosure(i, j)).Decompile().ToList();

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
