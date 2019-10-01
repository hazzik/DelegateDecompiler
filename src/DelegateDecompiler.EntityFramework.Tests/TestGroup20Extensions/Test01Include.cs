using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;
#if EF_CORE
using DelegateDecompiler.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
#else
using System.Data.Entity;
#endif

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup20Extensions
{
    class Test01Include
    {
        private ClassEnvironment classEnv;

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public void TestIncludeWhereDecompile()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Include(x => x.Children).Where(x => x.ParentBool == true).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Include(x => x.Children).Where(x => x.BoolEqualsConstant).DecompileAsync().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestIncludeDecompileWhere()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Include(x => x.Children).Where(x => x.ParentBool == true).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Include(x => x.Children).DecompileAsync().Where(x => x.BoolEqualsConstant).ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestDecompileIncludeWhere()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Include(x => x.Children).Where(x => x.ParentBool == true).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.DecompileAsync().Include(x => x.Children).Where(x => x.BoolEqualsConstant).ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestWhereIncludeDecompile()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Where(x => x.ParentBool == true).Include(x => x.Children).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Where(x => x.BoolEqualsConstant).Include(x => x.Children).DecompileAsync().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestWhereDecompileInclude()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Where(x => x.ParentBool == true).Include(x => x.Children).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Where(x => x.BoolEqualsConstant).DecompileAsync().Include(x => x.Children).ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestDecompileWhereInclude()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Where(x => x.ParentBool == true).Include(x => x.Children).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.DecompileAsync().Where(x => x.BoolEqualsConstant).Include(x => x.Children).ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }
    }
}
