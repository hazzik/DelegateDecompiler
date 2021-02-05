// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup50Types
{
    class Test01Strings
    {
        private ClassEnvironment classEnv;

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public void TestConcatenatePersonNotHandleNull()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfPersons.Select(x => x.FirstName + " " + x.MiddleName + " " + x.LastName).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfPersons.Select(x => x.FullNameNoNull).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestConcatenatePersonHandleNull()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfPersons.Select(x => x.FirstName + (x.MiddleName == null ? "" : " ") + x.MiddleName + " " + x.LastName).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfPersons.Select(x => x.FullNameHandleNull).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestConcatenatePersonHandleNameOrder()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfPersons.Select(x => x.NameOrder 
                    ? x.LastName +", " + x.FirstName + (x.MiddleName == null ? "" : " ")
                    : x.FirstName + (x.MiddleName == null ? "" : " ") + x.MiddleName + " " + x.LastName).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfPersons.Select(x => x.UseOrderToFormatNameStyle).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestGenericMethodPersonHandle()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfPersons.Select(x => x.FirstName + (x.MiddleName == null ? "" : " ") + x.MiddleName + " " + x.LastName).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = GetGenericPersonHandle(env.Db.EfPersons).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        public IQueryable<string> GetGenericPersonHandle<T>(IQueryable<T> people)
            where T : class, EfItems.Abstracts.IPerson
        {
            return people.Select(x => x.FullNameHandleNull);
        }
    }
}
