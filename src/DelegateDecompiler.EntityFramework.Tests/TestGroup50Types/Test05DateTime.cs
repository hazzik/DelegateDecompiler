// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup50Types
{
    class Test05DateTime
    {
        private ClassEnvironment classEnv;

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        private static readonly DateTime dateConst = new DateTime(2000, 1, 1);

        [Test]
        public void TestDateTimeWhereCompareWithStaticVariable()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Where(x => x.StartDate > dateConst).Select(x => x.StartDate).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Where(x => x.StartDateGreaterThanStaticVar).Select(x => x.StartDate)
#if NO_AUTO_DECOMPILE
                    .Decompile()
#endif
                    .ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

    }
}
