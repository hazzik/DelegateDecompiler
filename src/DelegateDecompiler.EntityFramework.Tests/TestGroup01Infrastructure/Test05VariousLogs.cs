// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup01Infrastructure
{
    class Test05VariousLogs
    {

        private MethodLog TestMethodLogCreate([CallerMemberName] string memberName = "",
                                       [CallerLineNumber] int lineNumber = 0)
        {
            return new MethodLog(LogStates.Supported, memberName, lineNumber, new List<string> { "LinqSql" }, new List<string> { "ddSql" });
        }

        [Test]
        public void TestMethodLogDecodeName()
        {

            //SETUP

            //ATTEMPT
            var log = TestMethodLogCreate();

            //VERIFY
            log.SpecificTest.ShouldEqual("Method Log Decode Name");
            log.LineNumber.ShouldBeGreaterThan(0);
        }

        [Test]
        public void BadDecodeNamesTest()
        {

            //SETUP

            //ATTEMPT
            var ex = Assert.Throws<ArgumentException>(() => TestMethodLogCreate());

            //VERIFY
            ex.Message.ShouldEqual("The test method must start with the characters 'Test'");
        }

        //-------------------------------------------------------------
        //classLog


        private ClassLog TestClassLogCreate([CallerFilePath] string sourceFilePath = "")
        {
            return new ClassLog(sourceFilePath);
        }

        [Test]
        public void TestClassLogDecodeName()
        {

            //SETUP

            //ATTEMPT
            var log = TestClassLogCreate();

            //VERIFY
            log.GroupDescription.ShouldEqual("Infrastructure");
            log.TestDescription.ShouldEqual("Various Logs");
            log.FileUrlFragment.ShouldEqual("TestGroup01Infrastructure/Test05VariousLogs.cs");
        }
    }
}
