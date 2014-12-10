// Contributed by @JonPSmith (GitHub) www.thereformedprogrammer.com

using System.Linq;

namespace DelegateDecompiler.EntityFramework.Tests.Helpers
{
    public enum ClassClassifications { NoTestsFound, Supported, PartiallySupported, NotSupported}

    class ClassLogClassified
    {

        public ClassClassifications Classification { get; private set; }

        public string DisplayMarkupNoPrefix { get; private set; }

        public ClassLogClassified(ClassLog classLog)
        {
            var numTests = classLog.ValidMethodLogs.Count();
            if (numTests == 0) return;

            var numSupportedResults = classLog.ValidMethodLogs.Count(x => x.State == LogStates.Supported);
            Classification = numTests == numSupportedResults 
                ? ClassClassifications.Supported 
                : numSupportedResults == 0
                    ? ClassClassifications.NotSupported
                    : ClassClassifications.PartiallySupported;

            DisplayMarkupNoPrefix = Classification == ClassClassifications.PartiallySupported
                ? string.Format("{0} ({1} of {2} tests passed)", classLog.TestNameAsMarkupLinkRelativeToDocumentationDir, numSupportedResults, numTests)
                : string.Format("{0} ({1} tests)", classLog.TestNameAsMarkupLinkRelativeToDocumentationDir, numTests);
        }
    }
}
