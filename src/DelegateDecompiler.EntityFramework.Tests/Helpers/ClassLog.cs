// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DelegateDecompiler.EntityFramework.Tests.Helpers
{
    class ClassLog
    {
        private const string groupPrefix = "TestGroup";
        private const string testPrefix = "Test";

        //The unpack
        public string GroupDescription { get; private set; }

        public string TestDescription { get; private set; }

        public string FileUrlFragment { get; private set; }

        public List<MethodLog> MethodLogs { get; private set; }

        /// <summary>
        /// These are MethodLogs that we want to report (filtered out where Linq failed as not appropriate for documentation)
        /// </summary>
        public IEnumerable<MethodLog> ValidMethodLogs { get
        {
            return MethodLogs.Where(x => x.State != LogStates.EvenLinqDidNotWork);
        } }

        public string TestNameAsMarkupLinkRelativeToDocumentationDir
        {
            get {  return string.Format("[{0}]({1})", TestDescription, "../" + FileUrlFragment);}
        }

        /// <summary>
        /// This is set by MasterEnvironment so that we remember the order
        /// </summary>
        public int Order { get; set; }

        public ClassLog(string testFilePath)
        {
            DecodeIntoUsefulNames(testFilePath);
            MethodLogs = new List<MethodLog>();
        }



        //-------------------------------------------------------------------

        public StringBuilder ResultsAsMarkup(OutputVersions version)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("#### {0}:\n", TestNameAsMarkupLinkRelativeToDocumentationDir);

            var dict = MethodLogs.GroupBy(x => x.State).ToDictionary( g => g.Key, m => m.ToList());
            sb.Append(ListInGroup(LogStates.Supported, dict, version == OutputVersions.DetailWithSql));
            sb.Append(ListInGroup(LogStates.NotSupported, dict, false));
            sb.AppendLine();
            return sb;
        }

        //------------------------------------------------------------------

        private static StringBuilder ListInGroup(LogStates state, Dictionary<LogStates, List<MethodLog>> dict, bool showSql)
        {
            var sb = new StringBuilder();

            if (!dict.ContainsKey(state)) return sb;

            sb.AppendFormat("- {0}\n", state.ToString().SplitCamelCase(state != LogStates.Supported));
            foreach (var methodLog in dict[state].OrderBy(x => x.LineNumber))
            {
                sb.AppendFormat("  * {0}\n", methodLog.ResultsAsMarkup());
                if (showSql)
                {
                    sb.AppendFormat("     * T-Sql executed is\n\n```SQL\n{0}\n```\n\n",
                        string.Join("\n", methodLog.DelegateDecompilerSqlCommand));
                };
            }

            return sb;
        }


        private void DecodeIntoUsefulNames(string testFilePath)
        {
            var filename = Path.GetFileNameWithoutExtension(testFilePath);
            var pathNoFile = Path.GetDirectoryName(testFilePath);
            var directory = pathNoFile.Substring(pathNoFile.LastIndexOf('\\') + 1);

            FileUrlFragment = directory + "/" + Path.GetFileName(testFilePath);

            //now we decode the directory name to get the group of tests
            GroupDescription = DecodeNameWithPrefixAndNumber(directory, groupPrefix);
            TestDescription = DecodeNameWithPrefixAndNumber(filename, testPrefix);
        }

        private static string DecodeNameWithPrefixAndNumber(string fullname, string expectedPrefix)
        {
            if (!fullname.StartsWith(expectedPrefix))
                throw new ArgumentException("The name should start with the characters: " + expectedPrefix);

            int i;
            if (!int.TryParse(fullname.Substring(expectedPrefix.Length, 2), out i))
                throw new ArgumentException("The name must have two digits after the text " + expectedPrefix);

            return fullname.Substring(expectedPrefix.Length + 2).SplitCamelCase();
        }

    }
}
