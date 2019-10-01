using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.EfItems.Concretes;
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
    class Test02AsNoTracking
    {
        private ClassEnvironment classEnv;
        readonly DelegateComparer<EfParent> comparer = new DelegateComparer<EfParent>((x, y) => x.EfParentId - y.EfParentId);

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public void TestAsNoTrackingWhereDecompile()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.AsNoTracking().Where(x => x.ParentBool == true).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.AsNoTracking().Where(x => x.BoolEqualsConstant).DecompileAsync().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd, comparer);
                Assert.IsEmpty(env.Db.ChangeTracker.Entries());
            }
        }

        [Test]
        public void TestAsNoTrackingDecompileWhere()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var efParents = env.Db.EfParents.AsNoTracking().Where(x => x.ParentBool == true);
                var linq = efParents.ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var queryable = env.Db.EfParents.AsNoTracking().DecompileAsync().Where(x => x.BoolEqualsConstant);
                var dd = queryable.ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd, comparer);
                Assert.IsEmpty(env.Db.ChangeTracker.Entries());
            }
        }

        [Test]
        public void TestDecompileAsNoTrackingWhere()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.AsNoTracking().Where(x => x.ParentBool == true).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.DecompileAsync().AsNoTracking().Where(x => x.BoolEqualsConstant).ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd, comparer);
                Assert.IsEmpty(env.Db.ChangeTracker.Entries());
            }
        }

        [Test]
        public void TestWhereAsNoTrackingDecompile()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Where(x => x.ParentBool == true).AsNoTracking().ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Where(x => x.BoolEqualsConstant).AsNoTracking().DecompileAsync().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd, comparer);
                Assert.IsEmpty(env.Db.ChangeTracker.Entries());
            }
        }

        [Test]
        public void TestWhereDecompileAsNoTracking()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Where(x => x.ParentBool == true).AsNoTracking().ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Where(x => x.BoolEqualsConstant).DecompileAsync().AsNoTracking().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd, comparer);
                Assert.IsEmpty(env.Db.ChangeTracker.Entries());
            }
        }

        [Test]
        public void TestDecompileWhereAsNoTracking()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Where(x => x.ParentBool == true).AsNoTracking().ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.DecompileAsync().Where(x => x.BoolEqualsConstant).AsNoTracking().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd, comparer);
                Assert.IsEmpty(env.Db.ChangeTracker.Entries());
            }
        }
    }
}
