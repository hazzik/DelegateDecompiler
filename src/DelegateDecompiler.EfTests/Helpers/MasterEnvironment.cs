// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DelegateDecompiler.EfTests.Helpers
{
    public enum OutputVersions { Summary, Detail, DetailWithSql}

    static class MasterEnvironment
    {
        public const string HeaderTestFilename = "DocumentationHeaderText.md";

        //list of filenames to produce (these names must be in the same order as OutputVersions)
        public static readonly string[] NamesOfDocumentationFilesForVersions = new []
        {
            "SummaryOfSupportedCommands.md",
            "DetailedListOfSupportedCommands.md",
            "DetailedListOfSupportedCommandsWithSQL.md"
        };

        private static int OrderInc ;

        private static bool RunStartedProperly;

        private static List<ClassLog> _classLogs;

        static MasterEnvironment()
        {
            _classLogs = new List<ClassLog>();
        }

        public static void AddClassLog(ClassLog classLog)
        {
            classLog.Order = OrderInc++;
            _classLogs.Add(classLog);
        }

        public static void ResetLogging()
        {          
            _classLogs.Clear();
            OrderInc = 1;
        }

        public static void StartProperRun()
        {
            RunStartedProperly = true;
            ResetLogging();
        }

        public static string ResultsAsMarkup(OutputVersions version)
        {

            var sb = new StringBuilder();
            sb.Append(BuildHeaderAsMarkup(version));

            sb.Append( version == OutputVersions.Summary
                ? BuildSummary()
                : BuildDetailWithOptionalSql(version));

            sb.Append("\nThe End\n");

            return sb.ToString();
        }

        public static void UpdateDocumentationIfLooksLikeAFullRun()
        {
            if (!RunStartedProperly) return;

            for (var version = OutputVersions.Summary; version <= OutputVersions.DetailWithSql; version++)
            {
                MarkupFileHelpers.WriteTextToFileInMarkup(NamesOfDocumentationFilesForVersions[(int)version], ResultsAsMarkup(version));                
            }

        }

        //-------------------------------------------------
        //private helpers

        private static StringBuilder BuildSummary()
        {
            var sb = new StringBuilder();

            foreach (var groupedTests in _classLogs.GroupBy(x => x.GroupDescription))
            {
                sb.Append(BuildGroupHeaderAsMarkup(groupedTests.Key));
                //now split into three groups: supported, partially supported and not supported
                var classifiedtestsDict = groupedTests.Select(x => new ClassLogClassified(x))
                    .GroupBy(x => x.Classification).ToDictionary(k => k.Key, v => v.Select( x => x.DisplayMarkupNoPrefix));

                sb.Append(ListInGroup(ClassClassifications.Supported, classifiedtestsDict));
                sb.Append(ListInGroup(ClassClassifications.PartiallySupported, classifiedtestsDict));
                sb.Append(ListInGroup(ClassClassifications.NotSupported, classifiedtestsDict));
                sb.AppendLine();
            }

            return sb;
        }

        private static StringBuilder ListInGroup(ClassClassifications classification, Dictionary<ClassClassifications, IEnumerable<string>> dict)
        {
            var sb = new StringBuilder();

            if (!dict.ContainsKey(classification)) return sb;

            sb.AppendFormat("- {0}\n", classification.ToString().SplitCamelCase());
            foreach (var markup in dict[classification])
                sb.AppendFormat("  * {0}\n", markup);

            return sb;
        }

        private static StringBuilder BuildDetailWithOptionalSql(OutputVersions version)
        {
            var sb = new StringBuilder();
            foreach (var grouped in _classLogs.GroupBy(x => x.GroupDescription).OrderBy(x => x.Min(y => y.Order)))
            {
                sb.Append(BuildGroupHeaderAsMarkup(grouped.Key));
                foreach (var classLog in grouped.OrderBy(x => x.Order))
                {
                    sb.Append(classLog.ResultsAsMarkup(version));
                }
                sb.AppendLine(); 
            }
            return sb;
        }

        private static string BuildGroupHeaderAsMarkup(string groupName)
        {
            return string.Format("### Group: {0}\n", groupName);
        }

        private static StringBuilder BuildHeaderAsMarkup(OutputVersions version)
        {
            var sb = new StringBuilder();
            var delegateDecompilerAssembly = Assembly.GetAssembly(typeof (ComputedAttribute));

            sb.AppendFormat("{0} of supported commands\n============\n", version.ToString().SplitCamelCase());

            sb.AppendFormat("## Documentation produced for {0}, version {1} on {2:f}\n",
                delegateDecompilerAssembly.GetName().Name,
                delegateDecompilerAssembly.GetName().Version,
                DateTime.Now);

            sb.Append(MarkupFileHelpers.SearchMarkupForSingleFileReturnContent(HeaderTestFilename));
            sb.AppendLine();

            return sb;
        }
    }
}
