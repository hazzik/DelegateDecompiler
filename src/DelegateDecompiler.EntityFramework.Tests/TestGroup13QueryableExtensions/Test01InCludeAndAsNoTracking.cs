// Contributed by @magicmoux (GitHub)
#if !EF_CORE

using System;
using System.Data.Entity;
using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup13QueryableExtensions
{
    class Test01IncludeAndAsNoTracking
    {
        private ClassEnvironment classEnv;

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public void TestDecompileSequencingForInclude()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.LivingBeeing.Decompile().OfType<Animal>().Include(lb => lb.Owner).ToList();
                Assert.IsTrue(dd.Any(a => a.Owner != null));
            }
        }

        [Test]
        public void TestDecompileSequencingForAsNoTracking()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.LivingBeeing.Decompile().AsNoTracking().OfType<Animal>().Include(lb => lb.Owner).ToList();
                Assert.IsTrue(dd.All(a => env.Db.Entry(a).State == EntityState.Detached));
            }
        }
    }
}

#endif