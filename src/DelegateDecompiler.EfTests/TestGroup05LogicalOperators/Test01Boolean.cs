// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using DelegateDecompiler.EfTests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EfTests.TestGroup05LogicalOperators
{
    class Test01Boolean
    {
        private ClassEnvironment classEnv;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public void TestBoolEqualsConstant()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Select(x => x.ParentBool == true).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => x.BoolEqualsConstant).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        private static bool staticBool = true;

        [Test]
        public void TestBoolEqualsStaticVariable()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Select(x => x.ParentBool == staticBool).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => x.BoolEqualsStaticVariable).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestIntEqualsConstant()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Select(x => x.ParentInt == 123).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => x.IntEqualsConstant).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }
    }
}
