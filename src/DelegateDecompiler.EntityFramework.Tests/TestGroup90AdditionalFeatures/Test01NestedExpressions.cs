using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.EfItems;
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
                })
#if !EF_CORE
                .Decompile()
#endif
                    ;
                var dd = query.ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }
    }
}