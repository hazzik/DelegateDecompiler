// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.EfItems;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup05BasicFeatures
{
    class Test10Single
    {
        private ClassEnvironment classEnv;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }


        [Test]
        public void TestSingleIntEqualsUniqueValue()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq =
                    env.Db.EfParents.Select(x => new {x.EfParentId, x.ParentInt})
                        .Single(x => x.ParentInt == DatabaseHelpers.ParentIntUniqueValue).EfParentId;

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd =
                    env.Db.EfParents.Select(x => new {x.EfParentId, x.IntEqualsUniqueValue})
                        .Decompile()
                        .Single(x => x.IntEqualsUniqueValue)
                        .EfParentId;

                //VERIFY
                env.CompareAndLogSingleton(linq, dd);
            }
        }
    }
}
