// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using System.Threading.Tasks;
using DelegateDecompiler.EntityFramework.Tests.EfItems;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;
#if EF_CORE
using DelegateDecompiler.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
#else
using System.Data.Entity;
#endif

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup05BasicFeatures
{
    class Test11SingleAsync
    {
        private ClassEnvironment classEnv;

        [OneTimeSetUp]
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
                        .DecompileAsync()
                        .SingleAsync(x => x.IntEqualsUniqueValue))
                        .EfParentId;

                //VERIFY
                env.CompareAndLogSingleton(linq, dd);
            }
        }
    }
}
