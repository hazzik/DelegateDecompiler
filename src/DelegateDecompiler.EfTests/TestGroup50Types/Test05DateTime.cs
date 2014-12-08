// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.Linq;
using DelegateDecompiler.EfTests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EfTests.TestGroup50Types
{
    class Test05DateTime
    {
        private ClassEnvironment classEnv;

        [TestFixtureSetUp]
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
                var linq = env.Db.EfParents.Where(x => x.StartDate > dateConst).Select( x => x.StartDate).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Where(x => x.StartDateGreaterThanStaticVar).Select( x => x.StartDate).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

    }
}
