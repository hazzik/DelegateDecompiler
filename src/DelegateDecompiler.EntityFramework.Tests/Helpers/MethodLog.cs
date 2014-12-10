// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.Collections.Generic;
using System.Linq;

namespace DelegateDecompiler.EntityFramework.Tests.Helpers
{
    public enum LogStates { Supported, NotSupported, EvenLinqDidNotWork}

    class MethodLog
    {
        private const string testPrefix = "Test";

        public LogStates State { get; private set; }

        public string SpecificTest { get; private set; }

        public int LineNumber { get; private set; }

        public List<string> LinqSqlCommand { get; private set; }

        public List<string> DelegateDecompilerSqlCommand { get; private set; }

        public MethodLog(LogStates state, string memberName, int sourceLineNumber, IEnumerable<string> linqSql, IEnumerable<string> delegateDecompilerSql)
        {
            State = state;
            LineNumber = sourceLineNumber;
            LinqSqlCommand = FilterSqlLog(linqSql);
            DelegateDecompilerSqlCommand = FilterSqlLog(delegateDecompilerSql);

            SpecificTest = DecodeSpecificTest(memberName);
        }

        public override string ToString()
        {
            return string.Format("{0} is {1} (line {2})", SpecificTest, State.ToString().SplitCamelCase(), LineNumber);
        }

        public string ResultsAsMarkup()
        {
            return string.Format("{0} (line {1})", SpecificTest, LineNumber);
        }

        //------------------------------------------------------------------

        private List<string> FilterSqlLog(IEnumerable<string> slqLog)
        {
            return
                slqLog.Where(
                    x =>
                        !(string.IsNullOrWhiteSpace(x) || x.StartsWith("--") || x.StartsWith("Opened connection ") ||
                          x.StartsWith("Closed connection ")))
                    .Select(x => x.Replace("\r", "")).ToList();
        }

        private static string DecodeSpecificTest(string memberName)
        {
            if (!memberName.StartsWith(testPrefix))
                throw new ArgumentException("The test method must start with the characters 'Test'");
            return memberName.Substring(testPrefix.Length).SplitCamelCase();
        }
    }
}
