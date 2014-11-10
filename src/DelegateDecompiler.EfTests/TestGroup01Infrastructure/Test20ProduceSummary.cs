// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Collections.Generic;
using DelegateDecompiler.EfTests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EfTests.TestGroup01Infrastructure
{
    class Test20ProduceSummary
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
        public void TestSummaryOneGroupPartiallySupported()
        {
            //SETUP
            MasterEnvironment.ResetLogging();
            var classLog = new ClassLog(@"TestGroup01UnitTestGroup\Test01MyUnitTest");
            MasterEnvironment.AddClassLog(classLog);
            example1PartiallySupported.ForEach(classLog.MethodLogs.Add);

            //ATTEMPT
            var markup = MasterEnvironment.ResultsAsMarkup(OutputVersions.Summary);

            //VERIFY
            markup.ShouldStartWith("Summary");
            markup.ShouldContain("Group: Unit Test Group");
            markup.ShouldContain("\n- Partially Supported\n");
            markup.ShouldContain(
                "\n  * [My Unit Test](../TestGroup01UnitTestGroup/Test01MyUnitTest) (1 of 2 tests passed)");
        }

        [Test]
        public void TestSummaryOneGroupSupported()
        {
            //SETUP
            MasterEnvironment.ResetLogging();
            var classLog = new ClassLog(@"TestGroup01UnitTestGroup\Test01MyUnitTest");
            MasterEnvironment.AddClassLog(classLog);
            example2Supported.ForEach(classLog.MethodLogs.Add);

            //ATTEMPT
            var markup = MasterEnvironment.ResultsAsMarkup(OutputVersions.Summary);

            //VERIFY
            markup.ShouldStartWith("Summary");
            markup.ShouldContain("Group: Unit Test Group");
            markup.ShouldContain("\n- Supported\n");
            markup.ShouldContain(
                "\n  * [My Unit Test](../TestGroup01UnitTestGroup/Test01MyUnitTest) (2 tests)");
        }

        [Test]
        public void TestSummaryOneGroupNotSupported()
        {
            //SETUP
            MasterEnvironment.ResetLogging();
            var classLog = new ClassLog(@"TestGroup01UnitTestGroup\Test01MyUnitTest");
            MasterEnvironment.AddClassLog(classLog);
            example3NotSupported.ForEach(classLog.MethodLogs.Add);

            //ATTEMPT
            var markup = MasterEnvironment.ResultsAsMarkup(OutputVersions.Summary);

            //VERIFY
            markup.ShouldStartWith("Summary");
            markup.ShouldContain("Group: Unit Test Group");
            markup.ShouldContain("\n- Not Supported\n");
            markup.ShouldContain(
                "\n  * [My Unit Test](../TestGroup01UnitTestGroup/Test01MyUnitTest) (2 tests)");
        }

        //------------------------------

        [Test]
        public void TestSummaryOneGroupTwoClassesSupported()
        {
            //SETUP
            MasterEnvironment.ResetLogging();
            var classLog1 = new ClassLog(@"TestGroup01UnitTestGroup\Test01MyUnitTest1");
            MasterEnvironment.AddClassLog(classLog1);
            example2Supported.ForEach(classLog1.MethodLogs.Add);
            var classLog2 = new ClassLog(@"TestGroup01UnitTestGroup\Test01MyUnitTest2");
            MasterEnvironment.AddClassLog(classLog2);
            example2Supported.ForEach(classLog2.MethodLogs.Add);

            //ATTEMPT
            var markup = MasterEnvironment.ResultsAsMarkup(OutputVersions.Summary);

            //VERIFY
            markup.ShouldStartWith("Summary");
            markup.ShouldContain("Group: Unit Test Group");
            markup.ShouldContain("\n- Supported\n");
            markup.ShouldContain("\n  * [My Unit Test1]");
            markup.ShouldContain("\n  * [My Unit Test2](");
        }

        [Test]
        public void TestSummaryTwoGroupOneClassEachSupported()
        {
            //SETUP
            MasterEnvironment.ResetLogging();
            var classLog1 = new ClassLog(@"TestGroup01UnitTestGroup1\Test01MyUnitTest1");
            MasterEnvironment.AddClassLog(classLog1);
            example2Supported.ForEach(classLog1.MethodLogs.Add);
            var classLog2 = new ClassLog(@"TestGroup01UnitTestGroup2\Test01MyUnitTest2");
            MasterEnvironment.AddClassLog(classLog2);
            example2Supported.ForEach(classLog2.MethodLogs.Add);

            //ATTEMPT
            var markup = MasterEnvironment.ResultsAsMarkup(OutputVersions.Summary);

            //VERIFY
            markup.ShouldStartWith("Summary");
            markup.ShouldContain("Group: Unit Test Group1");
            markup.ShouldContain("Group: Unit Test Group2");
            markup.ShouldContain("\n  * [My Unit Test1]");
            markup.ShouldContain("\n  * [My Unit Test2](");
        }
    }
}
