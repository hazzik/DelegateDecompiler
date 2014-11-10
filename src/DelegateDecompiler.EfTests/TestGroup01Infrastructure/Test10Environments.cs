// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using DelegateDecompiler.EfTests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EfTests.TestGroup01Infrastructure
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
        public void TestSupportedOk()
        {
            using (var env = new MethodEnvironment(classEnv))
            {
                //SETUP             

                //ATTEMPT
                env.CompareAndLogList(new String[] { }, new String[] { });

                //VERIFY
                classEnv.GetLastMethodLog().State.ShouldEqual(LogStates.Supported);
            }
        }

        [Test]
        public void TestNoSupportedResultsWrongOk()
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
