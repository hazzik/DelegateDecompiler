// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DelegateDecompiler.EfTests.EfItems;
using DelegateDecompiler.EfTests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EfTests.TestGroup05BasicFeatures
{
    class Test11SingleAsync
    {
        private ClassEnvironment classEnv;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public async Task TestSingleIntEqualsUniqueValueAsync()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = (await 
                    env.Db.EfParents.Select(x => new {x.EfParentId, x.ParentInt})
                        .SingleAsync(x => x.ParentInt == DatabaseHelpers.ParentIntUniqueValue)).EfParentId;

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = (await
                    env.Db.EfParents.Select(x => new {x.EfParentId, x.IntEqualsUniqueValue})
                        .Decompile()
                        .SingleAsync(x => x.IntEqualsUniqueValue))
                        .EfParentId;

                //VERIFY
                env.CompareAndLogSingleton(linq, dd);
            }
        }
    }
}
