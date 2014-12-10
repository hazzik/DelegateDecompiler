// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup01Infrastructure
{
    class Test10Environments
    {
        private ClassEnvironment classEnv;

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            classEnv = new ClassEnvironment();
        }

        [Test]
        public void TestSupportedListOk()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP             

                //ATTEMPT
                env.CompareAndLogList(new String[] { "fred" }, new String[] { "fred" });

                //VERIFY
                classEnv.GetLastMethodLog().State.ShouldEqual(LogStates.Supported);
            }
        }

        [Test]
        public void TestNoSupportedListResultsWrongOk()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP

                //ATTEMPT
                Assert.Throws<AssertionException>(() => env.CompareAndLogList(new String[] { "fred" }, new String[] { }));

                //VERIFY
                classEnv.GetLastMethodLog().State.ShouldEqual(LogStates.NotSupported);
            }
        }


        [Test]
        public void TestEmptListThrowsExceptionOk()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP             

                //ATTEMPT
                Assert.Throws<ArgumentException>( () =>  env.CompareAndLogList(new String[] { }, new String[] {}));

                //VERIFY
            }
        }

        [Test]
        public void TestSupportedSingleOk()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP             

                //ATTEMPT
                env.CompareAndLogSingleton(1, 1);

                //VERIFY
                classEnv.GetLastMethodLog().State.ShouldEqual(LogStates.Supported);
            }
        }

        [Test]
        public void TestNotSupportedSingleResultsWrongOk()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP             

                //ATTEMPT
                Assert.Throws<AssertionException>(() => env.CompareAndLogSingleton(1,2));

                //VERIFY
                classEnv.GetLastMethodLog().State.ShouldEqual(LogStates.NotSupported);
            }
        }

        [Test]
        public void TestLinqExceptionOk()
        {
            try
            {
                using (var env = new MethodEnvironment(classEnv))
                {
                    //SETUP

                    //ATTEMPT
                    throw new Exception("Failed");
                }

            }
            catch 
            {
                //VERIFY
                classEnv.GetLastMethodLog().State.ShouldEqual(LogStates.EvenLinqDidNotWork);
            }
        }

        [Test]
        public void TestDelegateDEcompilerExceptionOk()
        {
            try
            {
                using (var env = new MethodEnvironment(classEnv))
                {
                    //SETUP

                    //ATTEMPT
                    env.AboutToUseDelegateDecompiler();
                    throw new Exception("Failed");
                }

            }
            catch
            {
                //VERIFY
                classEnv.GetLastMethodLog().State.ShouldEqual(LogStates.NotSupported);
            }
        }

    }
}
