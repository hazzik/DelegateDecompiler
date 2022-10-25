using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.EfItems;
using DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup90AdditionalFeatures
{
    public static class EfTestDbContextExtensions
    {
        [Computed]
        public static int GetFirstChildIdByParent(this EfTestDbContext context, int pId)
        {
            return context.EfChildren.Where(a => a.EfParentId == pId).Select(b => b.EfChildId).FirstOrDefault();
        }
    }

    class Test01NestedExpressions
    {
        private ClassEnvironment classEnv;

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        class ParentIdWithFirstChildId
        {
            public int ParentId { get; set; }
            public int FirstChildId { get; set; }

            public override bool Equals(object obj)
            {
                return obj is ParentIdWithFirstChildId id && id.ParentId == ParentId && id.FirstChildId == FirstChildId;
            }

            public override int GetHashCode()
            {
                return ParentId * 131 + FirstChildId;
            }
        }

        [Test]
#if EF_CORE && !EF_CORE3 && !EF_CORE5
        [Ignore("Not natively supported in EF_CORE < 3")]
#endif
        public void TestSubqueryAsContextExtensionMethod()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Select(x => new ParentIdWithFirstChildId()
                {
                    ParentId = x.EfParentId,
                    FirstChildId = env.Db.EfChildren.Where(a => a.EfParentId == x.EfParentId).Select(b => b.EfChildId).FirstOrDefault()
                }).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var query = env.Db.EfParents.Select(x => new ParentIdWithFirstChildId()
                {
                    ParentId = x.EfParentId,
                    FirstChildId = env.Db.GetFirstChildIdByParent(x.EfParentId)
                }).Decompile();
                var dd = query.ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
#if EF_CORE && !EF_CORE3 && !EF_CORE5
        [Ignore("Not natively supported in EF_CORE < 3")]
#endif
        public void TestSubqueryAsVariableReference()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //ATTEMPT
                env.AboutToUseDelegateDecompiler();

                var referencedQuery = env.Db.Set<Animal>().Where(it => it.Species == "Canis lupus");
                var query = env.Db.Set<Person>().Where(it => it.Animals.Intersect(referencedQuery).Any()).Decompile();


                var list = query.ToList();

                //VERIFY
                Assert.AreEqual(1, list.Count());
            }
        }

        [Test]
#if EF_CORE && !EF_CORE3 && !EF_CORE5
        [Ignore("Not natively supported in EF_CORE < 3")]
#endif
        public void TestFilterWithSubqueryReference()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Any(x => env.Db.EfParents.Where(p => p.Children.Count() == 0).Contains(x));

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var referencedQuery = env.Db.EfParents.Where(p => p.CountChildren == 0);
                var dd = env.Db.EfParents.Decompile().Any(x => referencedQuery.Contains(x));

                //VERIFY
                env.CompareAndLogSingleton(linq, dd);
            }
        }
    }
}