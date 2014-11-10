// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using DelegateDecompiler.EfTests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EfTests.TestGroup06EqualityOperators
{
    class Test01EqualsAndNotEquals
    {
        private ClassEnvironment classEnv;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
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

        private static int staticInt = 123;
        [Test]
        public void TestIntEqualsStaticVariable()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Select(x => x.ParentInt == staticInt).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => x.IntEqualsStaticVariable).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }
        
        [Test]
        public void TestIntEqualsStringLength()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Select(x => x.ParentInt == x.ParentString.Length).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => x.IntEqualsStringLength).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }
        
        [Test]
        public void TestIntNotEqualsStringLength()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Select(x => x.ParentInt != x.ParentString.Length).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => x.IntNotEqualsStringLength).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }    }
}
