// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;
using DelegateDecompiler.EntityFramework.Tests.EfItems;
using System.Data.Entity;
using System.Collections.Generic;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup05BasicFeatures
{
    class Test01Include
    {
        private ClassEnvironment classEnv;

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Computed]
        private static bool ComputedSample() { return true; }

        [Test]
        public void TestInclude()
        {
            using (var db = new EfTestDbContext())
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = db.EfParents.Where(p => true).Include(p => p.Children).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Where(p => ComputedSample()).Include(p => p.Children).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

    }
}
