// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using DelegateDecompiler.EntityFramework.Tests.EfItems.Abstracts;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;
using System.Linq;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup05BasicFeatures
{
    internal class Test01Select
    {
        private ClassEnvironment classEnv;

        [OneTimeSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public void TestBoolEqualsConstant()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Select(x => x.ParentBool == true).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => x.BoolEqualsConstant).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        private static bool staticBool = true;

        [Test]
        public void TestBoolEqualsStaticVariable()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Select(x => x.ParentBool == staticBool).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => x.BoolEqualsStaticVariable).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestIntEqualsConstant()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfParents.Select(x => x.ParentInt == 123).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfParents.Select(x => x.IntEqualsConstant).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestSelectPropertyWithoutComputedAttribute()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfPersons.Select(x => x.FirstName + " " + x.MiddleName + " " + x.LastName).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfPersons.Select(x => x.FullNameNoAttibute).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestSelectMethodWithoutComputedAttribute()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.EfPersons.Select(x => x.FirstName + " " + x.MiddleName + " " + x.LastName).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.EfPersons.Select(x => x.GetFullNameNoAttibute()).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestSelectAbstractMemberOverTphHierarchy()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.LivingBeeing.ToList().Select(l => l.Species).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var ddExpr = env.Db.LivingBeeing.Select(p => p.Species).Decompile();
                var dd = ddExpr.ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestSelectAbstractMemberOverTphHierarchyAfterRestrictingToSubtype()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.LivingBeeing.OfType<Animal>().ToList().Select(p => p.Species).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var dd = env.Db.LivingBeeing.OfType<Animal>().Select(p => p.Species).Decompile().ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestSelectMultipleLevelsOfAbstractMembersOverTphHierarchy()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linq = env.Db.LivingBeeing.OfType<Animal>().ToList().Select(p => p.Species + " : " + (p.IsPet ? "True" : "False")).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var ddExpr = env.Db.LivingBeeing.OfType<Animal>().Select(p => p.Species + " : " + (p.IsPet ? "True" : "False")).Decompile();
                var dd = ddExpr.ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestSelectWithCallToBasePropertyOverTphHierarchy()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var linqAbstract = env.Db.LivingBeeing.OfType<Feline>().ToList().Select(p => p.Species + " : " + p.Age).ToList();
                var linqConcrete = env.Db.LivingBeeing.OfType<Cat>().ToList().Select(p => p.Species + " : " + p.Age).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                var ddAbstractCallExpr = env.Db.LivingBeeing.OfType<Feline>().Select(p => p.Species + " : " + p.Age).Decompile();
                var ddAbstractCallResult = ddAbstractCallExpr.ToList();
                var ddConcreteCallExpr = env.Db.LivingBeeing.OfType<Cat>().Select(p => p.Species + " : " + p.Age).Decompile();
                var ddConcreteCallResult = ddConcreteCallExpr.ToList();

                //VERIFY
                env.CompareAndLogList(linqAbstract, ddAbstractCallResult);
                env.CompareAndLogList(linqConcrete, ddConcreteCallResult);
            }
        }

        [Test]
        public void TestSelectWithCallToBaseMethodOverTphHierarchy()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP
                var owner = env.Db.LivingBeeing.OfType<Person>();
                var linq = env.Db.LivingBeeing.OfType<Animal>().ToList().Select(p => p.IsAdoptedBy(owner)).ToList();

                //ATTEMPT
                env.AboutToUseDelegateDecompiler();
                owner = env.Db.LivingBeeing.OfType<Person>();
                var ddExpr = env.Db.LivingBeeing.OfType<Animal>().Select(p => p.IsAdoptedBy(owner)).Decompile();
                var dd = ddExpr.ToList();

                //VERIFY
                env.CompareAndLogList(linq, dd);
            }
        }

        [Test]
        public void TestCanUseLinqFunctionsInLambda()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                env.AboutToUseDelegateDecompiler();
                var persons = env.Db.Set<Person>();
                var expr = env.Db.Set<Animal>().Where(c => c.IsAdoptedBy(persons));
                expr = expr.Decompile();
                var result = expr.ToList();
            }
        }
    }
}