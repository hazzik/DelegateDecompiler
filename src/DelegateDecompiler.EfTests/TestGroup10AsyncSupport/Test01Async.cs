// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DelegateDecompiler.EfTests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EfTests.TestGroup10AsyncSupport
{
    class Test01Async
    {
        private ClassEnvironment classEnv;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public async Task TestBoolEqualsConstantAsync()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = await env.Db.EfParents.Select(x => x.ParentBool == true).ToListAsync();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = await env.Db.EfParents.Select(x => x.BoolEqualsConstant).Decompile().ToListAsync();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

    }
}
