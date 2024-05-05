// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using System.Threading.Tasks;
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
    class Test02SelectAsync
    {
        private ClassEnvironment classEnv;

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public async Task TestAsync()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = await env.Db.EfParents.ToListAsync();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = await env.Db.EfParents
#if NO_AUTO_DECOMPILE
                    .DecompileAsync()
#endif
                    .ToListAsync();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

#if !EF_CORE
        [Test]
        public async Task TestAsyncNonGeneric()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = await env.Db.EfParents.ToListAsync();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = await ((IQueryable)env.Db.EfParents).DecompileAsync().ToListAsync();

                //VERIFY
                env.CompareAndLogNonGenericList(linq, dd);
            }
        }
#endif

        [Test]
        public async Task TestBoolEqualsConstantAsync()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = await env.Db.EfParents.Select(x => x.ParentBool == true).ToListAsync();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = await env.Db.EfParents.Select(x => x.BoolEqualsConstant)
#if NO_AUTO_DECOMPILE
                    .DecompileAsync()
#endif
                    .ToListAsync();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public async Task TestDecompileUpfrontBoolEqualsConstantAsync()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = await env.Db.EfParents.Select(x => x.ParentBool == true).ToListAsync();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = await env.Db.EfParents
#if NO_AUTO_DECOMPILE
                    .DecompileAsync()
#endif
                    .Select(x => x.BoolEqualsConstant).ToListAsync();

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
                var dd = await env.Db.EfParents.Select(x => x.BoolEqualsStaticVariable)
#if NO_AUTO_DECOMPILE
                    .DecompileAsync()
#endif
                    .ToListAsync();

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
                var dd = await env.Db.EfParents.Select(x => x.IntEqualsConstant)
#if NO_AUTO_DECOMPILE
                    .DecompileAsync()
#endif
                    .ToListAsync();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

    }
}
