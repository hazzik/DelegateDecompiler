// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup12QuantifierOperators
{
    class Test01Any
    {
        private ClassEnvironment classEnv;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public void TestAnyChildren()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Select(x => x.Children.Any()).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => x.AnyChildren).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestAnyChildrenWithFilter()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Select(x => x.Children.Any( y => y.ChildInt == 123)).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => x.AnyChildrenWithFilter).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        //Test removed as .Any in EF does not support finding a character in a string. 
        //[Test]
        //public void TestAnyCharInStringFilter()
        //{
        //    using (var env = new MethodEnvironment(classEnv))
        //    {
        //        //SETUP
        //        var linq = env.Db.EfParents.Where(x => x.ParentString.Any(y => y == '2')).Select(x => x.ParentString).ToList();

        //        //ATTEMPT
        //        env.AboutToUseDelegateDecompiler();
        //        var dd = env.Db.EfParents.Where(x => x.AnyCharInString).Select(x => x.ParentString).Decompile().ToList();

        //        //VERIFY
        //        env.CompareAndLogList(linq, dd);
        //    }
        //}

    }
}
