// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using DelegateDecompiler.EfTests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EfTests.TestGroup05BasicFeatures
{
    class Test02SelectAsync
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

        private static bool staticBool = true;

        [Test]
        public async Task TestBoolEqualsStaticVariableToArrayAsync()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = await env.Db.EfParents.Select(x => x.ParentBool == staticBool).ToListAsync();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = await env.Db.EfParents.Select(x => x.BoolEqualsStaticVariable).Decompile().ToListAsync();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public async Task TestIntEqualsConstant()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = await env.Db.EfParents.Select(x => x.ParentInt == 123).ToListAsync();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = await env.Db.EfParents.Select(x => x.IntEqualsConstant).Decompile().ToListAsync();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

    }
}
