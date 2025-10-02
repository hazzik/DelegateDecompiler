// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Collections.Generic;
using System.IO;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup01Infrastructure
{
    class Test22ProduceDetail
    {

        private List<MethodLog> example1PartiallySupported = new List<MethodLog>
        {
            new MethodLog(LogStates.Supported, "TestGood", 1, new List<string>{ "LinqSql"}, new List<string>{"ddSql"}),
            new MethodLog(LogStates.NotSupported, "TestBad", 2, new List<string>{ "LinqSql"}, new List<string>{"ddSql"}),
            new MethodLog(LogStates.EvenLinqDidNotWork, "TestLinqFailed", 3, new List<string>{ "LinqSql"}, new List<string>{"ddSql"})
        };
        private List<MethodLog> example2Supported = new List<MethodLog>
        {
            new MethodLog(LogStates.Supported, "TestGood1", 1, new List<string>{ "LinqSql"}, new List<string>{"ddSql"}),
            new MethodLog(LogStates.Supported, "TestGood2", 2, new List<string>{ "LinqSql"}, new List<string>{"ddSql"})
        };
        private List<MethodLog> example3NotSupported = new List<MethodLog>
        {
            new MethodLog(LogStates.NotSupported, "TestBad1", 1, new List<string>{ "LinqSql"}, new List<string>{"ddSql"}),
            new MethodLog(LogStates.NotSupported, "TestBad2", 2, new List<string>{ "LinqSql"}, new List<string>{"ddSql"})
        };


        [Test]
        public void TestDetailOneGroupPartiallySupported()
        {
            //SETUP
            MasterEnvironment.ResetLogging();
            var classLog = new ClassLog("TestGroup01UnitTestGroup/Test01MyUnitTest");
            MasterEnvironment.AddClassLog(classLog);
            example1PartiallySupported.ForEach(classLog.MethodLogs.Add);

            //ATTEMPT
            var markup = MasterEnvironment.ResultsAsMarkup(OutputVersions.Detail);

            //VERIFY
            markup.ShouldStartWith("Detail");
            markup.ShouldContain("Group: Unit Test Group");
            markup.ShouldContain("\n#### [My Unit Test](");
            markup.ShouldContain("\n- Supported\n  * Good (line 1)");
            markup.ShouldContain("\n- **Not Supported**\n  * Bad (line 2)");
        }

        [Test]
        public void TestDetailOneGroupSupported()
        {
            //SETUP
            MasterEnvironment.ResetLogging();
            var classLog = new ClassLog("TestGroup01UnitTestGroup/Test01MyUnitTest");
            MasterEnvironment.AddClassLog(classLog);
            example2Supported.ForEach(classLog.MethodLogs.Add);

            //ATTEMPT
            var markup = MasterEnvironment.ResultsAsMarkup(OutputVersions.Detail);

            //VERIFY
            markup.ShouldStartWith("Detail");
            markup.ShouldContain("Group: Unit Test Group");
            markup.ShouldContain("\n#### [My Unit Test](");
            markup.ShouldContain("\n- Supported\n  * Good1 (line 1)\n  * Good2 (line 2)");
        }

        //------------------------------

        [Test]
        public void TestDetailOneGroupTwoClassesSupported()
        {
            //SETUP
            MasterEnvironment.ResetLogging();
            var classLog1 = new ClassLog("TestGroup01UnitTestGroup/Test01MyUnitTest1");
            MasterEnvironment.AddClassLog(classLog1);
            example2Supported.ForEach(classLog1.MethodLogs.Add);
            var classLog2 = new ClassLog("TestGroup01UnitTestGroup/Test01MyUnitTest2");
            MasterEnvironment.AddClassLog(classLog2);
            example2Supported.ForEach(classLog2.MethodLogs.Add);

            //ATTEMPT
            var markup = MasterEnvironment.ResultsAsMarkup(OutputVersions.Detail);

            //VERIFY
            markup.ShouldStartWith("Detail");
            markup.ShouldContain("Group: Unit Test Group");
            markup.ShouldContain("\n#### [My Unit Test1](");
            markup.ShouldContain("\n#### [My Unit Test2](");
            markup.ShouldContain("\n- Supported\n  * Good1 (line 1)\n  * Good2 (line 2)");
        }

        [Test]
        public void TestDetailTwoGroupOneClassEachSupported()
        {
            //SETUP
            MasterEnvironment.ResetLogging();
            var classLog1 = new ClassLog("TestGroup01UnitTestGroup1/Test01MyUnitTest1");
            MasterEnvironment.AddClassLog(classLog1);
            example2Supported.ForEach(classLog1.MethodLogs.Add);
            var classLog2 = new ClassLog("TestGroup01UnitTestGroup2/Test01MyUnitTest2");
            MasterEnvironment.AddClassLog(classLog2);
            example2Supported.ForEach(classLog2.MethodLogs.Add);

            //ATTEMPT
            var markup = MasterEnvironment.ResultsAsMarkup(OutputVersions.Detail);

            //VERIFY
            markup.ShouldStartWith("Detail");
            markup.ShouldContain("Group: Unit Test Group1");
            markup.ShouldContain("Group: Unit Test Group2");
            markup.ShouldContain("\n#### [My Unit Test1](");
            markup.ShouldContain("\n#### [My Unit Test2](");
            markup.ShouldContain("\n- Supported\n  * Good1 (line 1)\n  * Good2 (line 2)");
        }
    }
}
