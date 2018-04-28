// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup05BasicFeatures
{
    class Test05Where
    {
        private ClassEnvironment classEnv;

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public void TestWhereBoolEqualsConstant()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Where(x => x.ParentBool == true).Select( x => x.EfParentId).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Where(x => x.BoolEqualsConstant).Select(x => x.EfParentId).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        private static bool staticBool = true;

        [Test]
        public void TestWhereBoolEqualsStaticVariable()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Where(x => x.ParentBool == staticBool).Select(x => x.EfParentId).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Where(x => x.BoolEqualsStaticVariable).Select(x => x.EfParentId).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestWhereIntEqualsConstant()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Where(x => x.ParentInt == 123).Select(x => x.EfParentId).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Where(x => x.IntEqualsConstant).Select(x => x.EfParentId).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestWhereFiltersOnAbstractMembersOverTphHierarchy()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.LivingBeeing.ToList().Where(x => x.Species == "Human").Select(x => x.Id).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.LivingBeeing.Where(x => x.Species == "Human").Select(x => x.Id).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestWhereFiltersOnMultipleLevelsOfAbstractMembersOverTphHierarchy()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.LivingBeeing.OfType<Animal>().ToList().Where(p => p.Species + " : " + p.IsPet == "Apis mellifera : False").Select(x => x.Id).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.LivingBeeing.OfType<Animal>().Where(p => p.Species + " : " + p.IsPet == "Apis mellifera : False").Select(x => x.Id).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }
    }
}
