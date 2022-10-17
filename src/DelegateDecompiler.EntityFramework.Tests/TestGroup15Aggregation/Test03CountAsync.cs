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

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup15Aggregation
{
    class Test03CountAsync
    {
        private ClassEnvironment classEnv;

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public async Task TestCountChildrenAsync()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var linq = await env.Db.EfParents.Select(x => x.Children.Count).ToListAsync();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = await env.Db.EfParents.Select(x => x.CountChildren).DecompileAsync().ToListAsync();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public async Task TestCountChildrenWithFilterAsync()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var linq = await env.Db.EfParents.Select(x => x.Children.Count(y => y.ChildInt == 123)).ToListAsync();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = await env.Db.EfParents.Select(x => x.CountChildrenWithFilter).DecompileAsync().ToListAsync();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public async Task TestCountChildrenWithFilterByClosureAsync()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var linq = await env.Db.EfParents.Select(x => x.Children.Count(y => y.ChildInt == x.EfParentId)).ToListAsync();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = await env.Db.EfParents.Select(x => x.CountChildrenWithFilterByClosure).DecompileAsync().ToListAsync();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public async Task TestCountChildrenWithFilterByExternalClosureAsync()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var i = 123;
                var linq = await env.Db.EfParents.Select(x => x.Children.Count(y => y.ChildInt == i)).ToListAsync();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = await env.Db.EfParents.Select(x => x.GetCountChildrenWithFilterByExternalClosure(i)).DecompileAsync().ToListAsync();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public async Task TestCountChildrenWithFilterByExternalClosure2Async()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var i = 123;
                var j = 456;
                var linq = await env.Db.EfParents.Select(x => x.Children.Count(y => y.ChildInt == i && y.EfParentId == j)).ToListAsync();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = await env.Db.EfParents.Select(x => x.GetCountChildrenWithFilterByExternalClosure(i, j)).DecompileAsync().ToListAsync();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public async Task TestSingletonCountChildrenWithFilterAsync()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                var linq = await env.Db.EfParents.CountAsync(x => x.Children.Count() == 2);

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = await env.Db.EfParents.DecompileAsync().CountAsync(x => x.CountChildren == 2);

                //VERIFY
                env.CompareAndLogSingleton(linq, dd);
            }
        }

    }
}
