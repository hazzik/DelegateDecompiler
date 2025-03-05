// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Collections.Generic;
using System.IO;
using DelegateDecompiler.EntityFramework.Tests.Helpers;
using NUnit.Framework;

namespace DelegateDecompiler.EntityFramework.Tests.TestGroup01Infrastructure
{
    class Test25ProduceDetailWithSql
    {

        private List<MethodLog> example1PartiallySupported = new List<MethodLog>
        {
            new MethodLog(LogStates.Supported, "TestGood", 1,
                new List<string>{ "LinqSql1a", "LinqSql1b"}, new List<string>{"ddSql1a", "ddSql1a"}),
            new MethodLog(LogStates.NotSupported, "TestBad", 2,
                new List<string>{ "LinqSql1a", "LinqSql1b"}, new List<string>()),
            new MethodLog(LogStates.EvenLinqDidNotWork, "TestLinqFailed", 3, new List<string>(), new List<string>())
        };



        [Test]
        public void TestDetailWithSqlOneGroupPartiallySupported()
        {
            //SETUP
            MasterEnvironment.ResetLogging();
            var classLog = new ClassLog("TestGroup01UnitTestGroup/Test01MyUnitTest");
            MasterEnvironment.AddClassLog(classLog);
            example1PartiallySupported.ForEach(classLog.MethodLogs.Add);

            //ATTEMPT
            var markup = MasterEnvironment.ResultsAsMarkup(OutputVersions.DetailWithSql);

            //VERIFY
            markup.ShouldStartWith("Detail With Sql");
            markup.ShouldContain("Group: Unit Test Group");
            markup.ShouldContain("\n#### [My Unit Test](");
            markup.ShouldContain("\n- Supported\n  * Good (line 1)\n     * T-Sql executed is\n\n```SQL");
            markup.ShouldContain("\n- **Not Supported**\n  * Bad (line 2)");
        }


    }
}
